using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AerodynamicHUD : MonoBehaviour
{
    [SerializeField]
    private Transform flightPathMarker;
    [SerializeField]
    private RectTransform pitchIndicator;
    [SerializeField]
    private GameObject climbTemplate;
    [SerializeField]
    private GameObject descendTemplate;
    [SerializeField]
    private TextMeshProUGUI speedometer;
    [SerializeField]
    private TextMeshProUGUI altimeter;
    [SerializeField]
    private TextMeshProUGUI heading;
    [SerializeField]
    private TextMeshProUGUI AoaMachG;
    [SerializeField]
    private TextMeshProUGUI weaponry;

    private Rigidbody rb;
    private Transform pitchIndicatorChildTransform;
    private float[] lastG = new float[2];
    private Vector3 previousVelocity;
    private PlayerWeaponry _pw;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        _pw = gameObject.GetComponent<PlayerWeaponry>();
        pitchIndicatorChildTransform = pitchIndicator.GetChild(0);
        ConstructPitchLadder();
    }

    void Update()
    {
        UpdateFlightPathMarker();
        UpdatePitchLadder();
        UpdateTextIndicators();
    }

    void UpdateFlightPathMarker()
    {
        flightPathMarker.eulerAngles =
            new Vector3(-Mathf.Atan2(rb.velocity.y, Mathf.Sqrt(Mathf.Pow(rb.velocity.z,2) + Mathf.Pow(rb.velocity.x,2))) * Mathf.Rad2Deg,
                        Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg,
                        rb.transform.eulerAngles.z);
    }

    void ConstructPitchLadder()
    {
        for (int i = 10; i <= 90; i += 5)
		{
			GameObject climbObject = Object.Instantiate(climbTemplate, pitchIndicatorChildTransform);
			climbObject.transform.localPosition = new Vector3(0, i / (10f / 7f), 0);
			foreach (TextMeshProUGUI textGUI in climbObject.GetComponentsInChildren<TextMeshProUGUI>())
            {
                textGUI.text = i.ToString(); 
            }
		}
		for (int j = 10; j <= 90; j += 5)
		{
			GameObject descendObject = Object.Instantiate(descendTemplate, pitchIndicatorChildTransform);
			descendObject.transform.localPosition = new Vector3(0, j / (-10f / 7f), 0);
			foreach (TextMeshProUGUI textGUI in descendObject.GetComponentsInChildren<TextMeshProUGUI>())
            {
                textGUI.text = j.ToString(); 
            }
		}
    }

    void UpdatePitchLadder()
    {
        float xAngle = rb.transform.eulerAngles.x;
        if (xAngle > 180)
            xAngle -= 360;
        if (Mathf.Abs(xAngle) > 90)
            xAngle -= (xAngle > 0) ? 2 * (xAngle - 90) : 2 * (xAngle + 90);

        pitchIndicatorChildTransform.localPosition = new Vector3(0, xAngle / (5.0f / 3.5f), 0);
        
        pitchIndicator.localEulerAngles = new Vector3(pitchIndicator.localEulerAngles.x, pitchIndicator.localEulerAngles.y, -transform.localEulerAngles.z);
    }

    void UpdateTextIndicators()
    {
        speedometer.text = string.Format("{0:#,0.} kt", rb.velocity.magnitude * 1.94384f);
        altimeter.text = string.Format("{0:#,0.} ft", rb.transform.position.y * 3.28084f);
        heading.text = string.Format("{0:#,0.}", rb.transform.eulerAngles.y);
        AoaMachG.text = string.Format("{0:#,0.0}\n{1:#,0.00}\n{2:#,0.0}", GetAngleOfAttack(), rb.velocity.magnitude / 343f, GetGForces());
        weaponry.text = string.Format("{0}\n{1:#,0.}", _pw.GetCurrentWeaponGroup().name, _pw.GetCurrentWeaponGroup().TotalCount());
    }

    float GetAngleOfAttack()
    {
        float angleOfAttack = Vector3.Angle(transform.forward, Vector3.Normalize(rb.velocity));
        return angleOfAttack;
    }

    float GetGForces()
    {
        float xG = Mathf.Abs((rb.velocity.x - previousVelocity.x) / Time.deltaTime / Physics.gravity.magnitude);
        float yG = 1 + Mathf.Abs((rb.velocity.y - previousVelocity.y) / Time.deltaTime / Physics.gravity.magnitude);
        float zG = Mathf.Abs((rb.velocity.z - previousVelocity.z) / Time.deltaTime / Physics.gravity.magnitude);

        Vector3 linearG = new Vector3(xG, yG, zG);
        previousVelocity = rb.velocity;

        float averageG = (lastG[0] + lastG[1] + linearG.magnitude) / 3f;

        lastG[1] = lastG[0];
        lastG[0] = linearG.magnitude;

        return averageG;
    }
}
