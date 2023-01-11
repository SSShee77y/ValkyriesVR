using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private AerodynamicController _ac;

    void Start() {
        _ac = gameObject.GetComponent<AerodynamicController>();
    }

    void Update() {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>()) {
            if (_ac.CurrentEngineSpeed > 0.5f)
            {
                w.motorTorque = 0.00001f;
            }
            else 
            {
                w.motorTorque = 0f;
            }
        }
    }

    public bool AnyWheelIsColldingWithGround()
    {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>()) {
            if (w.isGrounded)            
            {
                return true;
            }
        }
        return false;
    } 
}
