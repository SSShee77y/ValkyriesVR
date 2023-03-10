using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponry : MonoBehaviour
{
    [Serializable]
    public class WeaponryList
    {
        public string name;

        public enum Type
        {
            GUN,
            AIM9,
            AIM120,
            AGM,
            GB,
            UGB
        }
        public Type type;

        [Serializable]
        public class Weaponry
        {
            public GameObject gameObject;
            public float count = 1;
        }

        public List<Weaponry> list = new List<Weaponry>();

        public int TotalCount()
        {
            int total = 0;
            foreach (Weaponry weapon in list)
            {
                total += (int) weapon.count;
            }
            return total;
        }
    }
    [SerializeField]
    private float radarScanAngle;
    [SerializeField]
    private float radarScanRange;

    [SerializeField]
    private int maxEnemiesToSwitch;

    [SerializeField]
    private List<WeaponryList> _aircraftWeaponsList = new List<WeaponryList>();

    private int _currentIndex = 0;
    private ParticleSystem _mainGun;
    public ParticleSystem mainGun => _mainGun;
    private Rigidbody _rb;

    public class Enemy
    {
        public GameObject gameObject;
        public float distanceToPlayer;
        public float angleToPlayer;

        public Enemy(GameObject go, float d, float a)
        {
            gameObject = go;
            distanceToPlayer = d;
            angleToPlayer = a;
        }
    }

    private List<Enemy> _enemiesList = new List<Enemy>();
    public List<Enemy> EnemiesList => _enemiesList;

    private GameObject _currentEnemy;
    public GameObject CurrentEnemy => _currentEnemy;

    void Start()
    {
        SortWeaponry();
        EnableMainGun(false);
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var mainGun = _mainGun.main;
        mainGun.startSpeedMultiplier = GetFowardVelocity() + 1000f;
        
        if (_mainGun.emission.enabled == true && GetList(WeaponryList.Type.GUN).TotalCount() > 0)
            _aircraftWeaponsList[GetListIndex(WeaponryList.Type.GUN)].list[0].count -= _mainGun.emission.rateOverTimeMultiplier * Time.fixedDeltaTime;
        else if (_mainGun.emission.enabled == true)
            EnableMainGun(false);
    }

    void FixedUpdate()
    {
        CheckWeaponryStatus();
        if (_currentEnemy == null || GetIndexOfCurrentEnemy() < 0)
        {
            SetNextEnemy();
        }
    }

    void LateUpdate()
    {
        UpdateEnemiesList();
    }

    float GetFowardVelocity()
    {
        return Vector3.Dot(_rb.velocity, transform.forward);
    }

    void SortWeaponry()
    {
        foreach (WeaponryList weaponsList in _aircraftWeaponsList)
        {
            if (weaponsList.type == WeaponryList.Type.GUN)
            {
                _mainGun = weaponsList.list[0].gameObject.GetComponent<ParticleSystem>();
                var particleEmission = _mainGun.emission;
                particleEmission.enabled = false;
            }
        }
    }

    void CheckWeaponryStatus()
    {
        WeaponryList currentList = _aircraftWeaponsList[_currentIndex];

        if (currentList.type != WeaponryList.Type.GUN)
        {
            foreach (WeaponryList.Weaponry weapon in currentList.list)
            {
                if (weapon.count > 0 && weapon.gameObject == null)
                {
                    weapon.count = 0;
                }
            }
        }
    }

    WeaponryList GetList(WeaponryList.Type type)
    {
        foreach (WeaponryList weaponsList in _aircraftWeaponsList)
        {
            if (weaponsList.type == type)
            {
                return weaponsList;
            }
        }
        return null;
    }

    int GetListIndex(WeaponryList.Type type)
    {
        int iterations = 0;
        foreach (WeaponryList weaponsList in _aircraftWeaponsList)
        {
            if (weaponsList.type == type)
            {
                return iterations;
            }
            iterations++;
        }
        iterations = -1;
        return iterations;
    }    

    void UpdateEnemiesList()
    {
        _enemiesList.Clear();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject go in gos)
        {
            if (DistanceToPlayer(go.transform) <= Mathf.Abs(radarScanRange) && AngleToPlayer(go.transform) <= radarScanAngle / 2f)
            {
                HealthManager hpManager = go.gameObject.GetComponent<HealthManager>();
                if (hpManager == null)
                    _enemiesList.Add(new Enemy(go, DistanceToPlayer(go.transform), AngleToPlayer(go.transform)));
                else if (hpManager.GetHealth() > 0)
                    _enemiesList.Add(new Enemy(go, DistanceToPlayer(go.transform), AngleToPlayer(go.transform)));
            }
        }

        _enemiesList.Sort(delegate(Enemy x, Enemy y)
        {
            return x.angleToPlayer.CompareTo(y.angleToPlayer);
        } );
    }

    float AngleToPlayer(Transform enemyTransform)
    {
        Vector3 angleToPlayer = (enemyTransform.position - _rb.transform.position).normalized;
        return Vector3.Angle(angleToPlayer, _rb.transform.forward);
    }

    float DistanceToPlayer(Transform enemyTransform)
    {
        return Vector3.Distance(enemyTransform.position, _rb.transform.position);
    }

    GameObject GetNextEnemy()
    {
        if (_enemiesList.Count == 0)
            return null;

        if (_currentEnemy == null)
            return _enemiesList[0].gameObject;

        int indexOfCurrentEnemy = GetIndexOfCurrentEnemy();
        if (indexOfCurrentEnemy >= maxEnemiesToSwitch - 1 || indexOfCurrentEnemy < 0)
            return _enemiesList[0].gameObject;
        
        if (_enemiesList.Count > indexOfCurrentEnemy + 1)
            return _enemiesList[indexOfCurrentEnemy + 1].gameObject;
        
        return _enemiesList[0].gameObject;
    }

    public int GetIndexOfCurrentEnemy()
    {
        if (_currentEnemy == null)
            return -1;

        for (int i = 0; i < _enemiesList.Count; i++)
        {
            if (_currentEnemy == _enemiesList[i].gameObject)
                return i;
        }
        return -1;
    }

    public void SetNextEnemy()
    {
        _currentEnemy = GetNextEnemy();
    }

    [ContextMenu("FireWeapon")]
    public void FireWeapon()
    {
        FireWeapon(true);
    }

    [ContextMenu("StopFireWeapon")]
    public void StopFireWeapon()
    {
        FireWeapon(false);
    }

    public void FireWeapon(bool toFire)
    {
        WeaponryList currentList = _aircraftWeaponsList[_currentIndex];

        if (currentList.type == WeaponryList.Type.GUN)
            EnableMainGun(toFire);
        else
            EnableMainGun(false);

        if (toFire == false) return;

        if (currentList.type != WeaponryList.Type.GUN)
        {
            foreach (WeaponryList.Weaponry weapon in currentList.list)
            {
                if (weapon.count > 0 && weapon.gameObject == null)
                {
                    weapon.count = 0;
                }
                if (weapon.count > 0 && weapon.gameObject != null && weapon.gameObject.GetComponent<MissileController>() != null)
                {
                    weapon.gameObject.GetComponent<MissileController>().ActivateMissile();
                    if (_currentEnemy != null) weapon.gameObject.GetComponent<MissileController>().SetTarget(_currentEnemy);
                    weapon.gameObject = null;
                    weapon.count--;
                    return;
                }
            }
        }
    }

    public void EnableMainGun(bool isEnabled)
    {
        if (_mainGun != null)
        {
            var particleEmission = _mainGun.emission;
            particleEmission.enabled = isEnabled;
            if (GetList(WeaponryList.Type.GUN).TotalCount() <= 0)
                particleEmission.enabled = false;
        }
    }

    [ContextMenu("NextWeaponGroup")]
    public void NextWeaponGroup()
    {
        _currentIndex++;
        if(_currentIndex >= _aircraftWeaponsList.Count)
            _currentIndex = 0;
    }

    public WeaponryList GetCurrentWeaponGroup()
    {
        return _aircraftWeaponsList[_currentIndex];
    }
    
}
