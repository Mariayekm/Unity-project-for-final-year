using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private static int _nextLevelIndex = 3;//3
    int _shuffleIndex = 1;//1
    public static LevelController instance;
    Scene _previousScene;
    public static bool is_LogicGame = true;
    public static bool is_SensoryGame = false;
    public static bool is_MotorGame = false;
    //bool sensory = false;
    private static GameState gameState = GameState.None;
    private const int MAX_LV_LOGIC = 8;//11//8
    private const int MAX_LV_SENSORY = 9;//7//9

    public enum LoadType
    {
        None = 0,
        Consent = 1,
        LogicWelcome = 2,
        LogicPractice = 3,
        LogicLevel = 4,
        LogicEnd = 5,
        SensoryWelcome = 6,
        SensoryPractice = 7,
        SensoryLevel = 8,
        SensoryEnd = 9,
        MotorWelcome = 10,
        MotorPractice = 11,
        MotorLevel = 12,
        MotorEnd = 13,
        QuestionaireLv = 14,
        End = 15
    }

    public enum GameState
    {
        None = 0,
        ConsentForm = 1,
        Questionaire = 2,
        LogicGame = 3,
        SensoryGame = 4,
        MotorGame = 5,
        End = 6,
    }

    void Awake()
    {
        instance = this;
    }
    
    public void LoadNextLogicLevel()
    {
        string sceneName = _previousScene.name;


        if (_nextLevelIndex >= MAX_LV_LOGIC )
        {
            //SceneManager.LoadScene("LogicGameEnd");
            //Debug.Log("MaxLv");
            sceneName = "LogicEndScreen";
            _nextLevelIndex = -1;
        }
        Debug.Log("LvC-LogicLoad: SceneName: [" + sceneName + "], nextLvIndex: " + _nextLevelIndex);
        
        switch (sceneName)
        {
            case "LogicGameWelcome":
                //Debug.Log("LSwitch-LogicGameWelcome");
                SceneManager.LoadScene("LogicGamePractice");
                gameState = GameState.LogicGame;
                _nextLevelIndex = 0;
                break;
            case "LogicGamePractice":
                //Debug.Log("LSwitch-LogicGamePractic");
                SceneManager.LoadScene("LogicGameLevel1");
                gameState = GameState.LogicGame;
                _nextLevelIndex = 1;
                break;
            case "LogicEndScreen":
                //Debug.Log("LSwitch-LogicEndScreen");
                //Debug.Break();
                gameState = GameState.LogicGame;
                SceneManager.LoadScene("LogicGameEnd");
                break;
            case "LogicGameEnd":
                //Debug.Log("LSwitch-LogicGameEnd");
                //Debug.Break();
                is_LogicGame = false;
                SceneManager.LoadScene("SensoryGameWelcome");
                _nextLevelIndex = 1;
                gameState = GameState.SensoryGame;
                break;
            default:
                //Debug.Log("LSwitch-Defualt");
                _nextLevelIndex++;
                string nextLevelName = "LogicGameLevel" + _nextLevelIndex;
                gameState = GameState.LogicGame;
                SceneManager.LoadScene(nextLevelName);
                break;

            
    }
    }
    public void LoadNextSensoryLevel()
    {
        string sceneName = _previousScene.name;
        
        if (_nextLevelIndex >= MAX_LV_SENSORY && gameState == GameState.SensoryGame)
        {
            sceneName = "SensoryEndScreen";
        }else if (gameState == GameState.MotorGame)
        {
            sceneName = "SensoryGameEnd";
        }
        Debug.Log("LvC-SensoryLoad: SceneName: [" + sceneName + "], nextLvIndex: " + _nextLevelIndex);

        switch (sceneName)
        {
            case "SensoryGameWelcome":
                //Debug.Log("SSwitch-SensoryGameWelcome");
                SceneManager.LoadScene("SensoryGamePractice");
                _nextLevelIndex = 0;
                gameState = GameState.SensoryGame;
                //sensory true
                break;/*
            case "SensoryGamePractice":
                Debug.Log("SSwitch-SensoryGamePractice");
                SceneManager.LoadScene("SensoryGameLevel1");
                gameState = GameState.SensoryGame;
                _nextLevelIndex = 1;
                break;*/
            case "SensoryEndScreen":
                //Debug.Log("SSwitch-SensoryEndScreen");
                SceneManager.LoadScene("SensoryGameEnd");
                gameState = GameState.MotorGame;
                break;
            case "SensoryGameEnd":
                //Debug.Log("SSwitch-SensoryGameEnd");
                Motor_Loadlevel(LoadType.MotorWelcome);
                gameState = GameState.MotorGame;
                is_SensoryGame = false;
                break;
            default:
                //Debug.Log("SSwitch-Defualt");
                _nextLevelIndex++;
                string nextLevelName = "SensoryGameLevel" + _nextLevelIndex;
                gameState = GameState.SensoryGame;
                SceneManager.LoadScene(nextLevelName);
                break;
        }
    }

    public void Motor_Loadlevel(LoadType type = LoadType.MotorLevel, int lvCurr = 0)
    {
        is_LogicGame = false;
        is_SensoryGame = false;
        is_MotorGame = true;
        Debug.Log("LoadingLv: " + type);
        switch (type)
        {
            case LoadType.MotorWelcome:
                gameState = GameState.MotorGame;
                SceneManager.LoadScene("GameMotorIntroScreen");
                break;
            case LoadType.MotorLevel:
                gameState = GameState.MotorGame;
                Debug.Log("Placeholder lv load: MotorLevel");
                break;
            case LoadType.MotorEnd:
                gameState = GameState.End;
                Debug.Log("Placeholder lv load: MotorEnd");
                break;
        }
    }


    public void getSceneName()
    {
        _previousScene = SceneManager.GetActiveScene();
    }

    public bool IsLogicGame()
    {
/*        Debug.Log("LvC-isLogicGame: nextLvIndex: " + _nextLevelIndex + ", isLogicGame: " + is_LogicGame);
        if (_nextLevelIndex >= MAX_LV_LOGIC && is_LogicGame)
        {
            is_LogicGame = false;
            _nextLevelIndex = 0;
            gameState = GameState.SensoryGame;
        }*/


        return is_LogicGame;
    }

    public int GetShuffleIndex()
    {
        _shuffleIndex = _nextLevelIndex;
        //Debug.Log("Shuffle Index: " + _shuffleIndex);
        return _shuffleIndex;
    }

    public int GetGridIndex()
    {
        return _nextLevelIndex;
    }

    public GameState GetGameState()
    {
        return gameState;
    }


    //Sets the game state and relevent flags from previous system
    //Used to set state start of other games for Debug, so questionaire will load back into game being tested arther than Logic LV4
    public void SetGameState(GameState state)
    {
        is_LogicGame = false;
        is_SensoryGame = false;
        is_MotorGame = false;
        gameState = state;
        switch (state)
        {
            case GameState.LogicGame:
                is_LogicGame = true;
                break;
            case GameState.SensoryGame:
                is_SensoryGame = true;

                break;
            case GameState.MotorGame:
                is_MotorGame = true;
                break;
            case GameState.End:
                gameState = state;
                break;
            default:
                Debug.LogError("Unknown GameState set: " + state);
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
