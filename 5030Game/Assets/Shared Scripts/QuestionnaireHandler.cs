using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class QuestionnaireHandler : MonoBehaviour
{
    public static QuestionnaireHandler instance;
    public static bool popup = true;
    public Button[] Buttons;
    public EventSystem eventSystem;
    public Image[] redbox;
    public bool is_pressed = false;
    public TextMeshProUGUI redText;
    Navigation disabledNav = new Navigation();
    Navigation enabledNav = new Navigation();
    Button selectedButton;

    private GameObject csvHandlerGObj;
    private CSVHandler csvHandler;
    

    string button_name;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Q-Start");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Questionnaire"));
        button_name = null;
        is_pressed = false;
        disabledNav.mode = Navigation.Mode.None;
        enabledNav.mode = Navigation.Mode.Automatic;
        
        csvHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = csvHandlerGObj.GetComponent<CSVHandler>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Submit.instance.IsPressed())
        {
            SubmitData();
            //Debug.Break();
        }
        /*else if (is_pressed == true)
        {
            //Debug.Log("Q-is_pressed");
            OneButtonPressed();
            DisableAllOtherButtons();
            Submit.instance.EnableButton();
            //Debug.Break();
        }
        else 
        {
            EnableAllButtons();
            Submit.instance.DisableButton();
        }*/
        
    }

    public void OneButtonPressed()
    {
        //Debug.Log("Q-OneButtonPressed");
        if (eventSystem.currentSelectedGameObject != null) { button_name = EventSystem.current.currentSelectedGameObject.name; }     
       
    }

    public void ChangeBool()
    {
        //Debug.Log("Q-ChangeBool");
        is_pressed = !is_pressed;
        if (is_pressed == true) 
        {
            OneButtonPressed();
            DisableAllOtherButtons();
            Submit.instance.EnableButton();
            
            //change text to red if popup = true, then set popup to false
        }
        else 
        {
            Submit.instance.DisableButton();
            EnableAllButtons(); 
        }

        if (popup == true) 
        {
            foreach (Image line in redbox) { line.GetComponent<Image>().color = new Color(1, 0, 0, 1); }
            redText.characterSpacing = -5;
            redText.fontStyle = FontStyles.Bold;
            StartCoroutine(MakeBoxRed());
            popup = false;
        }
    }

    public void DisableAllOtherButtons()
    {
        //Debug.Log("Q-DisableAllOtherButtons");
        for (int i = 0; i < 7; i++)
        {
            if (Buttons[i].name != button_name)
            {
                Buttons[i].navigation = disabledNav;
                Buttons[i].interactable = false;
            }
        }

    }

    public void EnableAllButtons()
    {
        //Debug.Log("Q-EnableAllButtons");
        
        for (int i = 0; i < 7; i++)
        {
                Buttons[i].interactable = true;
                Buttons[i].navigation = enabledNav;
        }
    }

    private string getButtonPressed()
    {
        //Debug.Log("Q-GetButtonPressed");
        string button;
        switch (button_name)
        {
            case "Option (1)":
                button = "Extremely easy";
                break;
            case "Option (2)":
                button = "Easy";
                break;
            case "Option (3)":
                button = "Moderately easy";
                break;
            case "Option (4)":
                button = "Neither easy nor difficult";
                break;
            case "Option (5)":
                button = "Moderately difficult";
                break;
            case "Option (6)":
                button = "Difficult";
                break;
            case "Option (7)":
                button = "Extremely difficult";
                break;
            default:
                button = "N/A";
                break;
        }
        return button;
    }

    private int BtnNameToNum()
    {
        //Debug.Log("Q-BtnNameToNum");
        int output;
        switch (button_name)
        {
            case "Option (1)":
                output = 1;
                break;
            case "Option (2)":
                output = 2;
                break;
            case "Option (3)":
                output = 3;
                break;
            case "Option (4)":
                output = 4;
                break;
            case "Option (5)":
                output = 5;
                break;
            case "Option (6)":
                output = 6;
                break;
            case "Option (7)":
                output = 7;
                break;
            default:
                Debug.LogError("Unknown Button Name");
                output = -1;
                break;
        }
        return output;
    }


    public void Debug_skip()
    {
        //Debug.Log("Q-Debug skip");
        SubmitData();
        
    }

    public void SubmitData()
    {
        string answer = getButtonPressed();
        WriteData();
        
        if (LevelController.instance.IsLogicGame())
        {
            //Debug.Log("Q-Islogic");
            LevelController.instance.LoadNextLogicLevel();
        }
        else
        {
            //Debug.Log("Q-notLogic");
            if (LevelController.instance.GetGameState() == LevelController.GameState.SensoryGame)
            {
                LevelController.instance.LoadNextSensoryLevel();
            }
            else if (LevelController.instance.GetGameState() == LevelController.GameState.MotorGame) //Return to motor game?
            {
                //Debug.Log("Q-MotorGame");
                Camera.main.GetComponent<AudioListener>().enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
                SceneManager.SetActiveScene( SceneManager.GetSceneByName("GameMotorScene") );
                GameObject.Find("GameMotorHandler").GetComponent<GameMotorHandler>().SetCompoentsForQuestionnaire(true);
                SceneManager.UnloadSceneAsync("Questionnaire");
            }
            else if(LevelController.instance.GetGameState() == LevelController.GameState.End){
                GameObject tempGMH = GameObject.Find("GameMotorHandler");
                tempGMH.GetComponent<GameMotorHandler>().ZipAndEmailData();
                SceneManager.LoadScene("GameMotorEndScreen");
                SceneManager.UnloadSceneAsync("GameMotorScene");
                
            }
        }
    }

    private IEnumerator MakeBoxRed()
    {
        yield return new WaitForSeconds(2);
        foreach (Image line in redbox) { line.GetComponent<Image>().color = new Color(0, 0, 0, 1); }
        redText.characterSpacing = 0;
        redText.fontStyle = FontStyles.Normal;
        //popup = false;
    }

        //Writes the Data from the Difficulty Quiz to a CSV File
        private void WriteData()
    {
        DataSet_DiffQuiz dataSet = new DataSet_DiffQuiz();          //Create DataSet object
        dataSet.Reset();                                            //Resets internal variables to defualt values, not nessesary but done to be certain
        dataSet.ans = BtnNameToNum();                               //Add data to DataSet object
        csvHandler.AddtoDiffQuizBuffer(dataSet);                    //Add to CSV buffer
        csvHandler.WriteDiffQuizBuffer();                           //Write Buffer to CSV file,  
                                                                    //Generally you want to load up as much data as you can before writing to text file but this is all the data there is in this case so we just write it straight away

    }



}
