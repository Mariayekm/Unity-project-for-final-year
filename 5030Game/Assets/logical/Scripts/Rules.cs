using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Rules : MonoBehaviour
{
	public Button yourButton;
	bool is_pressed = false;
	public static Rules instance;
	public Text buttonText;

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
		ChangeText();
		ChangeColour();
	}

	public bool IsPressed()
	{
		return is_pressed;
	}
	void ChangeText()
	{
		string txt;
		if (is_pressed) { txt = "Okay"; }
		else { txt = "Read Rules"; }

		buttonText.text = txt;
	}
	void ChangeColour()
	{
		ColorBlock colors = yourButton.colors;
		if (is_pressed)
		{
			colors.normalColor = new Color32(0, 207, 86, 255);
			colors.highlightedColor = new Color32(0, 180, 70, 255);
			yourButton.colors = colors;
		}
		else 
		{
			colors.normalColor = new Color32(197, 200, 174, 255);
			colors.highlightedColor = new Color32(160, 160, 160, 255);
			yourButton.colors = colors;
		}
	}
}