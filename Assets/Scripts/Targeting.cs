using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Targeting : MonoBehaviour
{
    [SerializeField]
    private PlayerWeaponry _playerWeaponry;
    [SerializeField]
    private GameObject _gunReticle;
    [SerializeField]
    private float _optimalGunRange = 1200f;
    [SerializeField]
    private GameObject _targetLockBox;
    [SerializeField]
    private GameObject _referenceTargetBox;
    [SerializeField]
    private Transform _hudGlassLocation;
    
    private List<GameObject> _targetBoxList = new List<GameObject>();
    
    void Update()
    {
        ManageTargetBoxAmount();
        UpdateTargetBox();
        UpdateTargetLockBox();
        UpdateGunReticle();
    }

    void UpdateGunReticle()
    {
        int lockedTargetIndex = _playerWeaponry.GetIndexOfCurrentEnemy();
        if (_playerWeaponry.GetCurrentWeaponGroup().type != PlayerWeaponry.WeaponryList.Type.GUN || lockedTargetIndex < 0)
        {
            _gunReticle.SetActive(false);
            return;
        }

        var enemy = _playerWeaponry.EnemiesList[lockedTargetIndex].gameObject;
        float distance = Vector3.Distance(enemy.transform.position, _hudGlassLocation.position);
        if (distance >= _optimalGunRange)
        {
            _gunReticle.SetActive(false);
            return;
        }

        _gunReticle.SetActive(true);

        Vector3 predictedTargetLocation = PredictedTargetPosition(enemy.transform, _hudGlassLocation);
        distance = Vector3.Distance(predictedTargetLocation, _hudGlassLocation.position);

        float timeFromTarget = distance / _playerWeaponry.mainGun.main.startSpeedMultiplier;
        float gravityDisplacement = 9.81f * timeFromTarget;
        predictedTargetLocation += new Vector3(0, gravityDisplacement, 0);

        Vector3 positionAhead = transform.TransformPoint(Vector3.forward * distance);

        Vector3 displacementOfEnemy = (predictedTargetLocation - enemy.transform.position);
        Vector3 displacementToEnemy = (positionAhead - displacementOfEnemy) - _hudGlassLocation.position;

        _gunReticle.GetComponent<RectTransform>().anchoredPosition = DisplacementToAnchorPosition(displacementToEnemy);

    }

    Vector3 PredictedTargetPosition(Transform target, Transform player)
    {
        float gunSpeed = _playerWeaponry.mainGun.main.startSpeedMultiplier;
        float timeFromTarget = Vector3.Distance(player.position, target.position) / gunSpeed;

        Vector3 predictedPosition = target.position;

        if (target.GetComponent<Rigidbody>() != null)
        {
            for (int i = 0; i < 3; i++)
            {
                timeFromTarget = Vector3.Distance(player.position, predictedPosition) / gunSpeed;

                predictedPosition = target.position + (timeFromTarget * target.GetComponent<Rigidbody>().velocity);
            }
        }

        return predictedPosition;
    }
    Vector2 DisplacementToAnchorPosition(Vector3 displacement)
    {
        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, _playerWeaponry.transform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, _playerWeaponry.transform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, _playerWeaponry.transform.right);

        float horizontalAngle = Mathf.Atan(relativeDisplacement.x / relativeDisplacement.z);
        float verticalAngle = Mathf.Atan(relativeDisplacement.y / relativeDisplacement.z);

        Vector2 newAnchorPosition = new Vector2(40f * Mathf.Tan(horizontalAngle), 40f * Mathf.Tan(verticalAngle));
        return newAnchorPosition;
    }

    void UpdateTargetLockBox()
    {
        int lockedTargetIndex = _playerWeaponry.GetIndexOfCurrentEnemy();
        if (lockedTargetIndex < 0)
        {
            _targetLockBox.SetActive(false);
            return;
        }

        _targetLockBox.SetActive(true);

        var targetDetails = _targetLockBox.GetComponentInChildren<TextMeshProUGUI>();
        if (targetDetails != null)
        {
            var enemy = _playerWeaponry.EnemiesList[lockedTargetIndex];
            targetDetails.text = string.Format("{0}\n{1:0.000} km", enemy.gameObject.name, enemy.distanceToPlayer / 1000f);
        }

        _targetLockBox.GetComponent<RectTransform>().anchoredPosition = _targetBoxList[lockedTargetIndex].GetComponent<RectTransform>().anchoredPosition;
    }

    void UpdateTargetBox()
    {
        List<PlayerWeaponry.Enemy> enemies = _playerWeaponry.EnemiesList;

        for (int i = 0; i < _targetBoxList.Count && i < enemies.Count; i++)
        {
            GameObject enemy = enemies[i].gameObject;
            GameObject targetBox = _targetBoxList[i];

            Vector3 displacement = (enemy.transform.position - _hudGlassLocation.position);
            targetBox.GetComponent<RectTransform>().anchoredPosition = DisplacementToAnchorPosition(displacement);
        }
    }

    void ManageTargetBoxAmount()
    {
        List<PlayerWeaponry.Enemy> enemies = _playerWeaponry.EnemiesList;

        if (_targetBoxList.Count > enemies.Count)
        {
            int lastIndex = _targetBoxList.Count - 1;
            Destroy(_targetBoxList[lastIndex].gameObject);
            _targetBoxList.Remove(_targetBoxList[lastIndex]);
        }
        else if (_targetBoxList.Count < enemies.Count)
        {
            var newTargetBox = Instantiate(_referenceTargetBox, transform.position, transform.rotation);
            newTargetBox.transform.SetParent(gameObject.transform);
            _targetBoxList.Add(newTargetBox);
        }
    }
}
