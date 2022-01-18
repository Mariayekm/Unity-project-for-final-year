using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SquaresHappy : MonoBehaviour
{
	public Button yourButton;
	bool is_pressed = false;
	public static SquaresHappy instance;
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		yourButton = GetComponent<Button>();
		yourButton.onClick.AddListener(TaskOnClick);
		yourButton.interactable = false;
	}

	public void TaskOnClick()
	{
		is_pressed = !is_pressed;
		yourButton.interactable = false;
	}

	public bool IsPressed()
	{
		return is_pressed;
	}

	public void DisableButton()
	{
		yourButton.interactable = false;
		is_pressed = false;
	}

	public void EnableButton()
	{
		yourButton.interactable = true;
	}
}
