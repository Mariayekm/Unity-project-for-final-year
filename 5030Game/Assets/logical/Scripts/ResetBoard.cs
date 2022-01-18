using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetBoard : MonoBehaviour
{
	public static ResetBoard instance;
	public Button resetButton;
	bool is_pressed;
	int _reset = 0;
	
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		resetButton = GetComponent<Button>();
		resetButton.onClick.AddListener(TaskOnClick);
	}

	public void TaskOnClick()
	{
		is_pressed = !is_pressed;
		_reset++;
	}

	public bool IsPressed()
	{
		return is_pressed;
	}

	public int GetResetNumber()
	{
		return _reset;
	}

	public void ResetReset()
    {
		is_pressed = false;
	}

	public void DisableButton()
	{
		resetButton.interactable = false;
		is_pressed = false;
	}

	public void EnableButton()
	{
		resetButton.interactable = true;
	}
}
