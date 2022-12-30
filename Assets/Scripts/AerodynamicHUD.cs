using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicHUD : MonoBehaviour
{
    [SerializeField]
    private GameObject flightPathMarker;

    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateFlightPathMarker();
    }

    void UpdateFlightPathMarker()
    {
        flightPathMarker.transform.eulerAngles =
            new Vector3(-Mathf.Atan2(rb.velocity.y, Mathf.Sqrt(Mathf.Pow(rb.velocity.z,2) + Mathf.Pow(rb.velocity.x,2))) * Mathf.Rad2Deg,
                        Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg,
                        rb.transform.eulerAngles.z);
    }
}
