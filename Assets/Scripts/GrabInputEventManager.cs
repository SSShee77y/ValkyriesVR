using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction.HandGrab;

public class GrabInputEventManager : MonoBehaviour
{
    private static HandGrabInteractor[] _grabInteractors = {null, null};

    [Serializable]
    public class InputEventManager
    {
        public OVRInput.Button SelectedButton;

        public bool useSingleCall;
        private bool _caller;

        [SerializeField]
        private UnityEvent _whenPressed;
        [SerializeField]
        private UnityEvent _whenReleased;

        public UnityEvent WhenPressed => _whenPressed;
        public UnityEvent WhenReleased => _whenReleased;

        public void InvokeInputEvent(OVRInput.Controller controller)
        {
            if (OVRInput.GetDown(SelectedButton, controller))
                InvokePressedEvent();
            else if (OVRInput.GetUp(SelectedButton, controller))
                InvokeReleasedEvent();
        }

        public void InvokePressedEvent()
        {
            if (_whenPressed == null) return;
            if (useSingleCall && _caller == true)
                return;
            _whenPressed.Invoke();
            if (useSingleCall)
                _caller = true;
        }

        public void InvokeReleasedEvent()
        {
            if (_whenReleased == null) return;
            if (useSingleCall && _caller == false)
                return;
            _whenReleased.Invoke();
            if (useSingleCall)
                _caller = false;
        }
    }

    [SerializeField]
    private List<InputEventManager> _inputEventManagerList = new List<InputEventManager>();

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }

    void Awake()
    {
        GetGrabInteractors();
    }

    void Update()
    {
        if (_isSelected)
        {
            foreach (InputEventManager eventManager in _inputEventManagerList)
                eventManager.InvokeInputEvent(GetActiveController(GetComponent<HandGrabInteractable>()));
        }
    }

    void GetGrabInteractors()
    {
        HandGrabInteractor[] grabInteractorsArray = GameObject.FindObjectsOfType<HandGrabInteractor>();
        foreach (HandGrabInteractor grabInteractor in grabInteractorsArray)
        {
            HandGrabDefiner handDefiner = grabInteractor.GetComponent<HandGrabDefiner>();
            if (handDefiner != null)
            {
                if (handDefiner.WhichHand == HandGrabDefiner.Hand.Left)
                {
                    _grabInteractors[0] = grabInteractor;
                }
                else if (handDefiner.WhichHand == HandGrabDefiner.Hand.Right)
                {
                    _grabInteractors[1] = grabInteractor;
                }
            }
        }
    }

    OVRInput.Controller GetActiveController(HandGrabInteractable interactable)
    {
        if (_grabInteractors[0].SelectedInteractable == interactable)
            return OVRInput.Controller.LTouch;
        else if (_grabInteractors[1].SelectedInteractable == interactable)
            return OVRInput.Controller.RTouch;
        return OVRInput.Controller.None;
    }

    public InputEventManager GetInputEventManager(int elementIndex) {
        if (elementIndex >= _inputEventManagerList.Count || elementIndex < 0)
            return null;
        else
            return _inputEventManagerList[elementIndex];
    }
}
