using UnityEngine;

public class GrabTransformResetter : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;

    void Start()
    {
        defaultPosition = gameObject.transform.localPosition;
        defaultRotation = gameObject.transform.localEulerAngles;
    }
    
    public void InvokeReset()
    {
        gameObject.transform.localPosition = defaultPosition;
        gameObject.transform.localEulerAngles = defaultRotation;
    }

}
