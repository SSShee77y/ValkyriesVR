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
            Gun,
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

    void Start()
    {
        SortWeaponry();
        EnableMainGun(false);
    }

    void FixedUpdate()
    {
        if (_mainGun.emission.enabled == true && GetList(WeaponryList.Type.Gun).TotalCount() > 0)
            _aircraftWeaponsList[GetListIndex(WeaponryList.Type.Gun)].list[0].count -= _mainGun.emission.rateOverTimeMultiplier * Time.fixedDeltaTime;
        else if (_mainGun.emission.enabled == true)
            EnableMainGun(false);
    }

    void SortWeaponry()
    {
        foreach (WeaponryList weaponsList in _aircraftWeaponsList)
        {
            if (weaponsList.type == WeaponryList.Type.Gun)
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

    public void FireWeapon(bool toFire)
    {
        if (_aircraftWeaponsList[_currentIndex].type == WeaponryList.Type.Gun)
            EnableMainGun(toFire);
        else
            EnableMainGun(false);
    }

    public void EnableMainGun(bool isEnabled)
    {
        if (_mainGun != null)
        {
            var particleEmission = _mainGun.emission;
            particleEmission.enabled = isEnabled;
            if (GetList(WeaponryList.Type.Gun).TotalCount() <= 0)
                particleEmission.enabled = false;
        }
    }

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
