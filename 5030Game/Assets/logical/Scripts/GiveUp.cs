using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GiveUp : MonoBehaviour
{
    public Button yourButton;
    public static GiveUp instance;
    private const float GIVEUP_TIME = 50f;//60f
    private CSVHandler csvHandler;

    [SerializeField] private GameObject CSVHandlerGObj;

    void Awake()
    {
        instance = this;
        CSVHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
    }

    void Start()
    {
        yourButton = GetComponent<Button>();
        yourButton.onClick.AddListener(TaskOnClick);
        yourButton.enabled = false;
        GetComponentInChildren<Text>().enabled = false;
        GetComponentInChildren<Image>().enabled = false;
        StartCoroutine(GiveUpAppear());
    }

    // Update is called once per frame
    void TaskOnClick()
    {
        int s = LevelController.instance.GetShuffleIndex();
        Puzzle.instance.AddToBuffer();
        csvHandler.WriteLogicBuffer(s);
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        LevelController.instance.getSceneName();
        if (sceneName == "LogicGamePractice") 
        {
            LevelController.instance.LoadNextLogicLevel();
        }
        else 
        {
           SceneManager.LoadScene("Questionnaire");
        }
    }

    private IEnumerator GiveUpAppear()
    {
        while (Timer.instance.GetCurrentTime_float() < GIVEUP_TIME)
        {
            yield return null;
        }

        yourButton.enabled = true;
        GetComponentInChildren<Text>().enabled = true;
        GetComponentInChildren<Image>().enabled = true;
    }
}
