using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    [SerializeField]
    private GameObject _OVRRig;
    [SerializeField]
    private Transform _rigSeat;

    private AerodynamicController _ac;
    private bool _isBraking;

    void Start()
    {
        _ac = gameObject.GetComponent<AerodynamicController>();

        _OVRRig.transform.parent = null;

        foreach (Collider c1 in GetComponentsInChildren<Collider>())
        {
            if (gameObject.GetComponent<Collider>() != null)
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), c1, true);
                
            foreach (Collider c2 in GetComponentsInChildren<Collider>())
            {
                if (c2 != c1) Physics.IgnoreCollision(c2, c1, true);
            }
        }

        _OVRRig.transform.SetParent(_rigSeat, true);
    }

    void Update() {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>())
        {
            if (_isBraking == false && _ac.CurrentEngineSpeed > 1f)
            {
                if (_ac.CurrentEngineSpeed >= 20)
                {
                    w.motorTorque = 1_000f;
                }
                else
                {
                    w.motorTorque = _ac.CurrentEngineSpeed * 50;
                }
            }
            else 
            {
                w.motorTorque = 0f;
            }
        }
    }

    public bool AnyWheelIsColldingWithGround()
    {
        foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>())
        {
            if (w.isGrounded)            
            {
                return true;
            }
        }
        return false;
    }

    public void SetIsBraking(bool isBraking)
    {
        _isBraking = isBraking;
    }
}
