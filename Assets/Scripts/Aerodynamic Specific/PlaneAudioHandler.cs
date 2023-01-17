using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAudioHandler : AudioManager
{
    private AerodynamicController _ac;
    private Rigidbody _rb;

    void Start()
    {
        _ac = gameObject.GetComponent<AerodynamicController>();
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Play("Wind");
        SetVolume("Wind", _rb.velocity.magnitude / 100f);

        if (_ac.CurrentEngineSpeed > 1)
        {
            Play("Jet Engine");
            SetVolume("Jet Engine", Mathf.Sqrt(_ac.CurrentEngineSpeed - 1) / 10f);
        }
        else
        {
            Stop("Jet Engine");
        }

        if (_ac.CurrentEngineSpeed > 101)
        {
            Play("Afterburner");
            SetVolume("Afterburner", Mathf.Sqrt((_ac.CurrentEngineSpeed - 101) * 6) / 10f);
        }
        else
        {
            Stop("Afterburner");
        }
    }

    
}
