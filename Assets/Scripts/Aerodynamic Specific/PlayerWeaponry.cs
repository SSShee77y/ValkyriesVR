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
    private List<WeaponryList> _aircraftWeaponsList = new List<WeaponryList>();

    private int _currentIndex = 0;
    private ParticleSystem _mainGun;
    private Rigidbody _rb;

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
                if (weapon.count > 0 && weapon.gameObject != null && weapon.gameObject.GetComponent<MissileController>() != null)
                {
                    weapon.gameObject.GetComponent<MissileController>().ActivateMissile();
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
