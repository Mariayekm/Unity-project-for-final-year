using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PressPlay : MonoBehaviour
{
    public Button playButton;
    bool is_pressed;
    private const float MIN_WAIT_TIME = 10f;//10f
    public Text Countdown;
    int _count = 10;
    // Start is called before the first frame update
    void Start()
    {
        playButton = GetComponent<Button>();
        playButton.onClick.AddListener(TaskOnClick);
        playButton.interactable = false;
        Countdown.text = "Play in 10s";
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    public void TaskOnClick()
    {
        LevelController.instance.getSceneName();
        LevelController.instance.LoadNextLogicLevel();
    }

    private IEnumerator Wait()
    {
        //yield return new WaitForSeconds(MIN_WAIT_TIME);
        while (_count > 0)
        {
            yield return new WaitForSeconds(1);
            _count--;
            if (_count > 0)
            {
                string str = "Play in " + _count.ToString() + "s";
                Countdown.text = str;
            }
        }
        string new_str = "Play";
        Countdown.text = new_str;       
        playButton.interactable = true;
    }

}


