using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSAM : MonoBehaviour
{
    [SerializeField]
    public bool IsEnabled = true;

    [Header("Turret Variables")]
    [SerializeField] [Tooltip("Thing that rotates to look at target")]
    private Transform turretHead;
    [SerializeField]
    private float _turretSpeed = 10f;

    [Header("Combat Variables")]
    // private serialize variables
    [SerializeField]
    public bool _engageCombat = true;
    [SerializeField]
    private float _engagementDistance = 12000f;
    [SerializeField]
    private bool _isWeaponMissile = true;
    [SerializeField]
    private float _weaponCooldown = 2f;
    [SerializeField]
    private float _weaponReloadTime = 2f;
    [SerializeField]
    private float _weaponFireAngle = 10f;
    [SerializeField]
    private float _weaponLeadMultiplier = 1.1f;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private Transform _missileHardpoint;
    [SerializeField]
    private Transform _target;

    // private variables
    private PlayerWeaponry _weaponsControl;
    private Transform _currentMissile;
    private float _missileShootTimer;

    // colors
    private Color fadedRed = new Color(1,0,0,0.5f);
    private Color fadedWhite = new Color(1,1,1,0.5f);
    private Color fadedBlue = new Color(0,0,1,0.5f); 

    void OnDrawGizmos()
    {
        if (IsEnabled)
        {
            Gizmos.color = fadedWhite;
            if (_engageCombat)
            {
                if (gameObject.tag.Equals("Ally"))
                {
                    Gizmos.color = fadedBlue;
                }
                else if (gameObject.tag.Equals("Enemy"))
                {
                    Gizmos.color = fadedRed;
                }
            }

            if (_target != null)
            {
                Gizmos.DrawLine(transform.position, _target.position);
                Gizmos.DrawLine(transform.position, PredictedTargetPosition(_target, 1000f, true));
            }
        }
    }

    void Start()
    {
        _weaponsControl = GetComponent<PlayerWeaponry>();
    }

    void Update()
    {
        if (IsEnabled)
        {
            TurnTurretTowardsTarget();
            FireControl();
        }
    }

    void TurnTurretTowardsTarget()
    {
        if (_target == null)
        {
            if (_weaponsControl.EnemiesList.Count > 0)
            {
                int randomIndex = Random.Range(0, _weaponsControl.EnemiesList.Count);
                _weaponsControl.SetNextEnemy(randomIndex);
                _target = _weaponsControl.EnemiesList[randomIndex].gameObject.transform;
            }
            return;
        }
        else
        {
            Vector3 targetPosition = PredictedTargetPosition(_target, 1000f, true);
            Vector3 directionToTarget = targetPosition - turretHead.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, _turretSpeed * Time.deltaTime);
        }
    }

    void ReloadWeapon()
    {
        if (_isWeaponMissile)
        {
            GameObject newMissile = Instantiate(_missilePrefab, _missileHardpoint.position, _missileHardpoint.rotation);
            newMissile.transform.SetParent(_missileHardpoint);
            newMissile.GetComponent<Rigidbody>().useGravity = false;
            _weaponsControl.WeaponsList[0].list[0].gameObject = newMissile;
            _weaponsControl.WeaponsList[0].list[0].count = 1;
        }
        else
        {
            _weaponsControl.WeaponsList[0].list[0].count = 5000;
        }
    }

    void FireControl()
    {
        _weaponsControl.FireWeapon(false, 0);

        if (_engageCombat == false)
        {
            return;
        }

        // Take target from weapons control radar
        if (_target == null || _weaponsControl.WeaponsList[0].list[0].count == 0)
        {
            _missileShootTimer = _weaponCooldown;
            return;
        }

        // If target is dead, remove target from nav
        HealthManager targetHealth = _target.GetComponent<HealthManager>();
        if (targetHealth != null && targetHealth.IsDead)
        {
            _target = null;
            return;
        }

        // Get distance to target and attack angle to target
        Vector3 targetPosition = PredictedTargetPosition(_target, 1000f, true);
        float distanceToTarget = Vector3.Distance(targetPosition, transform.position);
        float angleBetween = Vector3.Angle(transform.forward, (targetPosition - transform.position).normalized);

        // If no target targeted,
        if (distanceToTarget >= _engagementDistance || angleBetween >= _weaponFireAngle)
        {
            if (_missileShootTimer < _weaponCooldown)
                _missileShootTimer += Time.deltaTime;
            else
                _missileShootTimer = _weaponCooldown;
            return;
        }
        
        // If missile has not been fired, fire it
        if (_currentMissile == null && _missileShootTimer <= 0)
        {
            _missileShootTimer = _weaponCooldown;
            _currentMissile = _weaponsControl.FireWeapon(true, 0);
            if (_isWeaponMissile || _weaponsControl.WeaponsList[0].list[0].count <= 0)
            {
                _currentMissile.GetComponent<MissileController>().SetTarget(_target.gameObject);
                Invoke("ReloadWeapon", _weaponReloadTime);
            }
            if (!_isWeaponMissile)
            {
                var gunEmitter = _weaponsControl.WeaponsList[0].list[0].gameObject.GetComponent<ParticleSystem>().main;
                gunEmitter.startLifetimeMultiplier = distanceToTarget / 1000f + 0.01f;
            }
            return;
        }
        else if (_currentMissile == null && _missileShootTimer > 0)
        {
            _missileShootTimer -= Time.deltaTime;
        }
        
    }

    Vector3 PredictedTargetPosition(Transform target, float towardTargetSpeed, bool useBulletDrop = false)
    {
        float timeFromTarget = Vector3.Distance(_missileHardpoint.position, target.position) / towardTargetSpeed;
        Vector3 predictedPosition = target.position;

        if (target.GetComponent<Rigidbody>() != null)
        {
            Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;
            for (int i = 0; i < 3; i++)
            {
                timeFromTarget = Vector3.Distance(_missileHardpoint.position, predictedPosition) / towardTargetSpeed;
                predictedPosition = target.position + (timeFromTarget * targetVelocity * _weaponLeadMultiplier);
            }
        }

        if (useBulletDrop)
        {
            float gravityDisplacement = 9.81f * timeFromTarget;
            predictedPosition += new Vector3(0, gravityDisplacement, 0);
        }

        return predictedPosition;
    }
}
