using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSensoryGame : MonoBehaviour
{
    public Button playButton;
    bool is_pressed;
    // Start is called before the first frame update
    void Start()
    {
        playButton = GetComponent<Button>();
        playButton.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    public void TaskOnClick()
    {
        LevelController.instance.getSceneName();
        LevelController.instance.LoadNextSensoryLevel();
    }
}
