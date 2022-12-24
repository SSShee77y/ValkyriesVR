using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIController : MonoBehaviour
{
    private float _defaultSliderValue;

    private float _sliderValue;
    
    public float SliderValue
    {
        get => _sliderValue;
        set => _sliderValue = value;
    }

    [SerializeField]
    private float _slideFactor = 1f;
    
    void Start()
    {
        _defaultSliderValue = GetComponent<Slider>().value;
    }

    void Update()
    {
        GetComponent<Slider>().value = _defaultSliderValue + (_sliderValue / _slideFactor);
    }

}
