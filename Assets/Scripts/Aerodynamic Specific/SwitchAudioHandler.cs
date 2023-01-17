using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAudioHandler : AudioManager
{
    [SerializeField]
    private float _rangeDeviation = 0.01f;
    
    public bool _inRange = false;

    private PositionLockConstrainer _plc;

    void Start()
    {
        _plc = GetComponent<PositionLockConstrainer>();
    }

    void Update()
    {
        CheckPositions();
    }

    void CheckPositions()
    {
        foreach (float point in _plc.ZPosition.lockPoints)
        {
            if (gameObject.transform.localPosition.z >= point - _rangeDeviation && gameObject.transform.localPosition.z <= point + _rangeDeviation)
            {
                if (_inRange == false)
                {
                    _inRange = true;
                    Play(Sounds[0].name);
                }
                return;
            }
        }
        _inRange = false;
    }
}
