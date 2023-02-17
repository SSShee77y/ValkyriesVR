using UnityEngine;
using UnityEngine.Events;

public class EventToggler : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _whenToggledOn;
    [SerializeField]
    private UnityEvent _whenToggledOff;

    #region EventProperties

        public UnityEvent WhenToggledOn => _whenToggledOn;
        public UnityEvent WhenToggledOff => _whenToggledOff;

    #endregion

    private bool onToggle;
    private int togglerCooldown;
    private static readonly int MAX_COOLDOWN = 5;

    void FixedUpdate()
    {
        if (togglerCooldown < MAX_COOLDOWN) togglerCooldown++;
    }

    void SwitchToggle() 
    {
        onToggle = !onToggle;
    }

    public void InvokeToggle()
    {
        if(togglerCooldown == MAX_COOLDOWN)
        {
            SwitchToggle();
            if (onToggle)
                _whenToggledOn.Invoke();
            else
                _whenToggledOff.Invoke();
        }
        else
        {
            togglerCooldown = 0;
        }
    }
}
