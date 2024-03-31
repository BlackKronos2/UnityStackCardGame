using UnityEngine;
using UnityEngine.UI;

public class UIFillBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    private float _value;
    private float _maxValue;

	public float Value
	{
		get { return _value; }
		set
		{
			_value = Mathf.Clamp(value, 0, _maxValue);
			BarValueUpdate();
		}
	}

	private void Start()
	{
		BarReset(100);
	}

	private void BarValueUpdate()
	{
		_slider.value = _value / _maxValue;
	}

	public void BarReset(float maxValue)
	{
		_maxValue = maxValue;
		_value = 0;

		BarValueUpdate();
	}
}
