using UnityEngine;

public class HandGrabDefiner : MonoBehaviour
{
    public enum Hand
    {
        None,
        Left,
        Right
    }

    [SerializeField]
    public Hand WhichHand;
}
