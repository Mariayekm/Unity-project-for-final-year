using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Option3 : MonoBehaviour
{
	public Button yourButton;
	public Sprite Image1;
	public Sprite Image2;

	bool is_pressed;
	public static Option3 instance;
	Navigation disabledNav = new Navigation();


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		yourButton = GetComponent<Button>();
		yourButton.onClick.AddListener(TaskOnClick);
	}

	public void TaskOnClick()
	{
		is_pressed = !is_pressed;
		ChangeImage();
	}

	public bool IsPressed()
	{
		return is_pressed;
	}

	void ChangeImage()
	{
		if (is_pressed)
		{
			yourButton.GetComponent<Image>().sprite = Image2;
		}
		else
		{
			yourButton.GetComponent<Image>().sprite = Image1;
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public void DisableButton()
	{
		yourButton.interactable = false;
		is_pressed = false;
		disabledNav.mode = Navigation.Mode.None;
		yourButton.navigation = disabledNav;
	}

	public void EnableButton()
	{
		yourButton.interactable = true;
	}
}