using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAudioHandler : AudioManager
{
    private AerodynamicController _ac;

    void Start()
    {
        _ac = gameObject.GetComponent<AerodynamicController>();
    }

    void Update()
    {
        if (_ac.CurrentEngineSpeed > 1)
        {
            Play("Jet Engine");
            SetVolume("Jet Engine", Mathf.Sqrt(_ac.CurrentEngineSpeed - 1) / 400f);
        }
        else
        {
            Stop("Jet Engine");
        }

        if (_ac.CurrentEngineSpeed > 110)
        {
            //Play("Afterburner");
            SetVolume("Afterburner", Mathf.Sqrt((_ac.CurrentEngineSpeed - 100) * 5) / 400f);
        }
        else
        {
            Stop("Afterburner");
        }
    }

    
}
