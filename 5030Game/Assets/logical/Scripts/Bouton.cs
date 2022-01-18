using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Bouton : MonoBehaviour
{
	public Button yourButton;
	bool is_pressed;
	int moves = 0;
	public static Bouton instance;
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
		moves++;
	}

	public bool IsPressed()
    {
		return is_pressed;
    }

	public int GetMovesTaken()
	{
		return moves;
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
