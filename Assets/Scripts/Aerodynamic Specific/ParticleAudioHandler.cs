using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioHandler  : AudioManager
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    void Start()
    {
        
    }

    void Update()
    {
        if (_particleSystem.emission.enabled)
            Play(Sounds[0].name);
        else
            Stop(Sounds[0].name);
    }
}
