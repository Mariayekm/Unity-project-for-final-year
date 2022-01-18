using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SQuitPractice : MonoBehaviour
{
	public Button yourButton;
	bool is_pressed = false;
	public static SQuitPractice instance;
	private const float MIN_QUIT_TIME = 10f;//10
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
		yield return new WaitForSeconds(MIN_QUIT_TIME);
		yourButton.enabled = true;
	}
}
	