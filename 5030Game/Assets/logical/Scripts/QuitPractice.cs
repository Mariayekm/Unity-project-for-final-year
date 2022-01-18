using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuitPractice : MonoBehaviour
{
	public Button yourButton;
	bool is_pressed = false;
	public Text _continue;
	public static QuitPractice instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		yourButton = GetComponent<Button>();
		yourButton.onClick.AddListener(TaskOnClick);
		yourButton.enabled = false;
		StartCoroutine(Wait());

	}

	public void TaskOnClick()
	{
		is_pressed = !is_pressed;
	}

	public bool IsPressed()
	{
		return is_pressed;
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(10);
		yourButton.enabled = true;
	}
}
