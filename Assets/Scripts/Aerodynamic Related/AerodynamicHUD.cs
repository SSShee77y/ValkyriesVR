using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AerodynamicHUD : MonoBehaviour
{
    [SerializeField]
    private GameObject flightPathMarker;
    [SerializeField]
    private TextMeshProUGUI speedometer;
    [SerializeField]
    private TextMeshProUGUI altimeter;

    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateFlightPathMarker();
        UpdateTextIndicators();
    }

    void UpdateFlightPathMarker()
    {
        flightPathMarker.transform.eulerAngles =
            new Vector3(-Mathf.Atan2(rb.velocity.y, Mathf.Sqrt(Mathf.Pow(rb.velocity.z,2) + Mathf.Pow(rb.velocity.x,2))) * Mathf.Rad2Deg,
                        Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg,
                        rb.transform.eulerAngles.z);
    }

    void UpdateTextIndicators()
    {
        speedometer.text = string.Format("{0:#,0.} kt", rb.velocity.magnitude * 1.94384f);
        altimeter.text = string.Format("{0:#,0.} ft", rb.transform.position.y * 3.28084f);
    }
}
