using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMotorHandler : MonoBehaviour
{
    [SerializeField] private GameObject timerGObj;
    private Timer timerGlobal;
    List<GateClass> gates;
    [SerializeField] private Spawner spawner;
    [SerializeField] private GameObject CSVHandlerGObj;
    [SerializeField] private GameObject rulesMenuGObj;
    [SerializeField] private GameObject levelIndicatorGObj;
    [SerializeField] private GameObject gateHandlerGObj;
    [SerializeField] private GameObject playerGObj;
    [SerializeField] private GameObject collisionCrossesParentGObj;
    [SerializeField] private GameObject eventsSystemGObj;
    [SerializeField] private GameObject emailerGObj;
    private CSVHandler csvHandler;
    //private List<GateClass> gateList;
    private Camera mainCam, uiCam;
    private RulesMenu rulesMenu;
    private LeveIndicator levelIndicator;
    private GatesHandler gateHandler;
    private PlayerCollisionManager playerColMan;
    private MouseFollow_circ playerFollow;
    private PlayerPosLogger posLogger;
    private Emailer emailer;
    public List<GameObject> crossGObjList;

    private bool showLvIndicator;
    private bool showRules;
    private bool playerCanFollow;
    private bool timerOn;
    private bool levelFinished;

    private int lvCurrent;
    private int lvTypeCurr;
    private Levels levelsDB;
    private List<float[]> lvDataCurr;
    private int nextGateId;
    private Vector3 playerStartingPos;
    private const int MAX_LV = 10;       //Set max number of levels, used for debug 
    private const int START_LV = 0;     //Set starting level, used for debug
    private ScoreLog[] scoreLog;
    public struct ScoreLog
    {
        public float timeSec;
        public int numCollisions;
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);//Sets self to not be destroyed when loading, so that loading questionaire does not destory game state
        mainCam = Camera.main;
        uiCam = GameObject.Find("UI Camera").GetComponent<Camera>();
        rulesMenu = rulesMenuGObj.GetComponent<RulesMenu>();
        showRules = false;
        levelIndicator = levelIndicatorGObj.GetComponent<LeveIndicator>();
        gateHandler = gateHandlerGObj.GetComponent<GatesHandler>();
        lvDataCurr = new List<float[]>();
        levelsDB = new Levels();
        playerColMan = playerGObj.GetComponent<PlayerCollisionManager>();
        playerFollow = playerGObj.GetComponent<MouseFollow_circ>();
        posLogger = playerGObj.GetComponent<PlayerPosLogger>();
        playerStartingPos = new Vector3(-8.5f, 0, 0);
        timerGlobal = timerGObj.GetComponent<Timer>();
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
        scoreLog = new ScoreLog[MAX_LV+1];
        crossGObjList = new List<GameObject>();
        LevelController.instance.SetGameState(LevelController.GameState.MotorGame);
        csvHandler.SetGameMotorHandlerRef(gameObject);
        emailer = emailerGObj.GetComponent<Emailer>();
    }

    private void Start()
    {
        lvCurrent = START_LV;
        levelIndicatorGObj.SetActive(true);
        rulesMenuGObj.SetActive(true);
        levelIndicator.Off();
        rulesMenu.RulesOff();
        timerGlobal.TimerReset();
        LoadLevel(START_LV);
        Cursor.lockState = CursorLockMode.Confined;
        csvHandler.ResetHandler();

    }
    private void Update()
    {
        //DebugLvSwitch();
    }

    //Loads levels 0-9 by pressing corresponding number keys (not numpad) on keyboard
    public void DebugLvSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LoadLevel(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLevel(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LoadLevel(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadLevel(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LoadLevel(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LoadLevel(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LoadLevel(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LoadLevel(9);
        }
    }

    public int GetCurentLv()
    {
        return lvCurrent;
    }

    //Loads a new level and sets variaous flads and UI elements
    public void LoadLevel(int lv)
    {
        lvCurrent = lv;
        float[] LvTypeData;
        //Load Level Data
        lvDataCurr = levelsDB.GetLevelData(lv);
        LvTypeData = lvDataCurr[lvDataCurr.Count - 1];
        lvTypeCurr = (int)levelsDB.ConvertToLevelType(LvTypeData[0]);
        playerStartingPos = new Vector3(LvTypeData[1], LvTypeData[2], 0);
        lvDataCurr.RemoveAt(lvDataCurr.Count - 1);

        //Remove and Spawn game  gameobjects
        ClearCollisionCrosses();
        gateHandler.SpawnLevel(lvDataCurr);
        playerGObj.transform.position = playerStartingPos;
        
        //Set flags and UI
        showLvIndicator = true;
        playerFollow.SetCanFollow(false);
        levelIndicator.On(lvTypeCurr);
        nextGateId = 0;
        levelFinished = false;
        timerOn = false;
        gateHandler.gateList[0].SetEnabled(true);
        timerGlobal.TimerReset();
        playerColMan.countCollisions = true;
        playerColMan.ResetCollisionCount();
    }

    public void ShowLvIndicator(int type) {
        levelIndicator.On(type);
    }

    public void ShowRules(bool show)
    {
        if (show)
        {
            if (showLvIndicator)
            {
                levelIndicator.Off();
            }
            timerGlobal.TimerPause();
            rulesMenu.RulesOn();
        }
        else
        {
            rulesMenu.RulesOff();
            if (showLvIndicator)
            {
                levelIndicator.On(lvTypeCurr);
            }
            if (timerOn)
            {
                timerGlobal.TimerResume();
            }


        }
        
    }

    public void ToggleRulesShowing() {
        ShowRules(!rulesMenu.GetIsShowing());
    }

    public void CloseLvIndicator()
    {
        showLvIndicator = false;
        levelIndicator.Off();
        playerFollow.SetCanFollow(true);
        levelFinished = false;
    }

    public void UpdateGatePassed(int id)
    {
        if (id == gateHandler.gateList.Count - 1)
        {
            playerColMan.countCollisions = false;
            timerGlobal.TimerPause();
            levelFinished = true;
            timerOn = false;
            SetFollowing(false);
            posLogger.isLogging = false;
            Debug.Log("Lv finished");
            ScoreLog tempSL;
            tempSL.timeSec = timerGlobal.GetCurrentTime_float();
            tempSL.numCollisions = playerColMan.totalCollisionsLv;
            scoreLog[lvCurrent] = tempSL;
            playerColMan.totalCollisionsLv = 0;
            LoadNextLevel();
            LoadQuestionare();
            ClearCollisionCrosses();
        }
        else if (id == nextGateId)
        {
            gateHandler.gateList[nextGateId].SetEnabled(false);
            nextGateId++;
            gateHandler.gateList[nextGateId].SetEnabled(true);
        }

    }

    public void SetFollowing(bool following)
    {
        if (following)
        {
            timerOn = true;
        }
        else
        {
            timerOn = false;
            playerCanFollow = false;
            playerFollow.StopFollow();
            timerGlobal.TimerPause();
        }
    }

    public void ClearCollisionCrosses()
    {
        #region Alt Method Destory Crosses
        /* foreach (Transform child in collisionCrossesParentGObj.transform)
                {
                    Destroy(child.gameObject);
                }*/
        #endregion
        foreach (GameObject cross in crossGObjList)
        {
            Destroy(cross);
        }
        crossGObjList.Clear();
        foreach (Transform child in collisionCrossesParentGObj.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void LoadNextLevel(){
        if(lvCurrent < MAX_LV)
        {
            csvHandler.WriteMotorBuffer();
            lvCurrent++;
            LoadLevel(lvCurrent);
        }else if (lvCurrent >= MAX_LV)
        {
            SetEnd();
        }
    }

    public void LoadQuestionare()
    {
        Cursor.lockState = CursorLockMode.None;
        //mainCam.GetComponent<AudioListener>().enabled = false;
        //eventsSystemGObj.SetActive(false);
        SetCompoentsForQuestionnaire(false);
        SceneManager.LoadScene("Questionnaire", LoadSceneMode.Additive);

    }

    public void SetCompoentsForQuestionnaire(bool on)
    {
        mainCam.GetComponent<AudioListener>().enabled = on;
        eventsSystemGObj.SetActive(on);
    }

    public int GetNextGateId() { return nextGateId; }
    public Vector2 GetNextGatePos() { return gateHandler.gateList[nextGateId].GetMidPos(); }
    public ScoreLog[] GetScoreLog() { return scoreLog; }

    private void SetEnd()
    {
        csvHandler.WriteMotorBuffer();
        csvHandler.WriteMotorScore(ScoreLogToAr());
        LevelController.instance.SetGameState(LevelController.GameState.End);
    }
    private string[] ScoreLogToAr()
    {
        string[] tempAr = new string[scoreLog.Length];
        string tempLine = "";
        int i = 0;
        foreach (ScoreLog sl in scoreLog)
        {
            tempLine = sl.timeSec.ToString() + "," + sl.numCollisions.ToString();
            tempAr[i] = tempLine;
            i++;
        }
        return tempAr;
    }

    public void ZipAndEmailData()
    {
        PrintScoreLog();
        string fp_zip = csvHandler.ZipData();
        Debug.Log("GMH-fp_zip: " + fp_zip);
        emailer.EmailData(fp_zip);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("GMH-Application quitting");
        csvHandler.WriteMotorBuffer();
        //consolodate files
        //Send Files
    }

    private void PrintScoreLog()
    {
        string msg = "ScoreLog: " + System.Environment.NewLine;
        foreach (ScoreLog sl in scoreLog)
        {
            msg += "timeSec: " + sl.timeSec + ", hits: " + sl.numCollisions + System.Environment.NewLine;
        }
        Debug.Log(msg);
    }
}
