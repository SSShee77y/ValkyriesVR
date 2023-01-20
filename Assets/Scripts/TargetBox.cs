using UnityEngine;
using TMPro;

public class TargetBox : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI targetDetails;

    public void SetTargetDetails(string details)
    {
        targetDetails.text = details;
    }
}
