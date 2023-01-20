using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    [SerializeField]
    private PlayerWeaponry _playerWeaponry;
    [SerializeField]
    private GameObject _targetLockBox;
    [SerializeField]
    private GameObject _referenceTargetBox;
    [SerializeField]
    private Transform _hudGlassLocation;
    
    private List<TargetBox> _targetBoxList = new List<TargetBox>();
    
    void Update()
    {
        ManageTargetBoxAmount();
        UpdateTargetBox();
        UpdateTargetLockBox();
    }

    void UpdateTargetBox()
    {
        List<PlayerWeaponry.Enemy> enemies = _playerWeaponry.EnemiesList;

        for (int i = 0; i < _targetBoxList.Count && i < enemies.Count; i++)
        {
            GameObject enemy =  enemies[i].gameObject;
            TargetBox targetBox = _targetBoxList[i];

            string details = string.Format("{0}\n{1:0.000} NM", enemy.name, enemies[i].distanceToPlayer / 1852f);
            targetBox.SetTargetDetails(details);
            
            Vector3 displacement = (enemy.transform.position - _hudGlassLocation.transform.position);

            Vector3 relativeDisplacement = new Vector3();
            relativeDisplacement.z = Vector3.Dot(displacement, _playerWeaponry.transform.forward);
            relativeDisplacement.y = Vector3.Dot(displacement, _playerWeaponry.transform.up);
            relativeDisplacement.x = Vector3.Dot(displacement, _playerWeaponry.transform.right);

            float horizontalAngle = Mathf.Tan(relativeDisplacement.x / relativeDisplacement.z);
            float verticalAngle = Mathf.Tan(relativeDisplacement.y / relativeDisplacement.z);

            Vector2 newBoxLocation = new Vector2(40f * Mathf.Tan(horizontalAngle), 40f * Mathf.Tan(verticalAngle));

            targetBox.GetComponent<RectTransform>().anchoredPosition = newBoxLocation;
        }
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
        _targetLockBox.GetComponent<RectTransform>().anchoredPosition = _targetBoxList[lockedTargetIndex].GetComponent<RectTransform>().anchoredPosition;
    }

    void ManageTargetBoxAmount()
    {
        List<PlayerWeaponry.Enemy> enemies = _playerWeaponry.EnemiesList;

        if (_targetBoxList.Count > enemies.Count)
        {
            int lastIndex = _targetBoxList.Count - 1;
            Destroy(_targetBoxList[lastIndex].gameObject);
            _targetBoxList.Remove(_targetBoxList[lastIndex].GetComponent<TargetBox>());
        }
        else if (_targetBoxList.Count < enemies.Count)
        {
            var newTargetBox = Instantiate(_referenceTargetBox, transform.position, transform.rotation);
            newTargetBox.transform.SetParent(gameObject.transform);
            _targetBoxList.Add(newTargetBox.GetComponent<TargetBox>());
        }
    }
}
