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
            public int count = 1;
        }

        public List<Weaponry> list = new List<Weaponry>();
    }
    [SerializeField]
    private List<WeaponryList> _aircraftWeaponsList = new List<WeaponryList>();

    private ParticleSystem _mainGun;

    void Start()
    {
        SortWeaponry();
        EnableMainGun(false);
    }

    void Update()
    {
        
    }

    void SortWeaponry()
    {
        foreach (WeaponryList weaponsList in _aircraftWeaponsList)
        {
            if (weaponsList.type == WeaponryList.Type.Gun)
            {
                _mainGun = weaponsList.list[0].gameObject.GetComponent<ParticleSystem>();
            }
        }
    }

    public void EnableMainGun(bool isEnabled)
    {
        if (_mainGun != null)
        {
            var particleEmission = _mainGun.emission;
            particleEmission.enabled = isEnabled;
        }
    }
}
