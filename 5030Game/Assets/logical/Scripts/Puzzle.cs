using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Puzzle : MonoBehaviour
{
    public NumberBox boxPrefab;
    public static Puzzle instance;
    public static bool popup2 = true;

    public NumberBox[,] boxes = new NumberBox[3, 3];
    public NumberBox[,] initial_boxes = new NumberBox[3, 3];
    public TimeIntervalLogger Prefab;
    public Sprite[] sprites;

    public Text Countdown;
    public Text ready;
    public GameObject myPanel;

    public int index = 0;
    public int X = 0;
    public int Y = 0;
    public int DX = 0;
    public int DY = 0;
    public int moves_taken;
    public Text movesText;

    [SerializeField] private GameObject CSVHandlerGObj;
    private CSVHandler csvHandler;
    private TimeIntervalLogger timeIntervalLogger;
    private const float COUNTDOWN_STEPTIME = 0.75f;//1
    private const float DELAY_AFTER_COUNTDOWN = 0f;//1

    int _count = 4;
    int s = 0;
    ArrayList time_between_moves = new ArrayList();
    float previousTime = 0;
    int a = 0;
    bool _start = true;
    bool _rules = false;

    void Awake()
    {
        instance = this;
        CSVHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
        timeIntervalLogger = Instantiate(Prefab);
    }

    void Start()
    {
        Init();
        csvHandler.ResetHandler();
        if (GameObject.Find("red square popup") != null) { myPanel.SetActive(false); }
        //set click func to null
        _start = true;
        s = LevelController.instance.GetShuffleIndex();
        int c = s;
        if (s == 0) { c = 3; }
        for (int i = 0; i < c; i++)
            Shuffle();

        StartCoroutine(WaitStart());
        movesText.text = "0";
        moves_taken = 0;
        previousTime = 0;
        time_between_moves.Clear();
        time_between_moves.Add(0);

    }

    void Init()
    {
        int n = 0;
        for (int y = 2; y >= 0; y--)
            for (int x = 0; x < 3; x++)
            {
                NumberBox box = Instantiate(boxPrefab, new Vector2(x, y), Quaternion.identity);
                box.Init(x, y, n + 1, sprites[n], ClickToSwap);
                boxes[x, y] = box;
                initial_boxes[x, y] = box;
                n++;
            }
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "LogicGamePractice")
        {
            if (QuitPractice.instance.IsPressed() == true) 
            {
                LevelController.instance.getSceneName();
                LevelController.instance.LoadNextLogicLevel(); 
            }
        }

        if (CheckWin() == 9)
        {
            Win();
        }
        else if (Rules.instance.IsPressed())
        {
            DisplayRules();
        }
        else if (!Rules.instance.IsPressed() && !Timer.instance.GetTimerStatus())//Causing error by calling unknown func from Timer, cannot find original function. See timer class for details
        {
            HideRules();
        }
        /*
        if (ResetBoard.instance.IsPressed())
        {
            reset_board();  ///get rid of this
        }*/
        if (SquaresBlue() == 1)
        {
            bool SwapConfirmed = Bouton.instance.IsPressed();
            if (SwapConfirmed)
            {
                Swap(X, Y, DX, DY);
                //if (GameObject.Find("red square popup") != null) { redLogicText.enabled = false; }
                getTimeBetweenMoves();
            }
        }


    }

    int SquaresBlue()
    {
        int colour = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boxes[i, j].colour == true)
                {
                    colour++;
                }
            }
        }
        return colour;
    }

    void ClickToSwap(int x, int y)
    {

        int dx = getDx(x, y);
        int dy = getDy(x, y);
        
        if (_start == true || _rules == true)
        {
            //dnt let them click!!
        }
        else if (SquaresBlue() == 0)
        {
             if (popup2 == true)
            {
                myPanel.SetActive(true);
                popup2 = false;
            }
            TurnBlue(x, y, dx, dy);
            index = boxes[x, y].GetIndex();
            DX = dx;  //updates which square is blue
            DY = dy;
            X = x;
            Y = y;           
        }
        else if (SquaresBlue() == 1 && index == boxes[x, y].GetIndex())
        {
            Revert(x, y, dx, dy);
            if (GameObject.Find("red square popup") != null) { myPanel.SetActive(false); }
        }

    }

    void Swap(int x, int y, int dx, int dy)
    {
        Revert(x, y, dx, dy);
        var from = boxes[x, y];
        var target = boxes[x + dx, y + dy];

        // swap these 2 boxes
        boxes[x, y] = target;
        boxes[x + dx, y + dy] = from;

        moves_taken = Bouton.instance.GetMovesTaken();
        string str = moves_taken.ToString();
        movesText.text = str;
        // update pos 2 boxes
        from.UpdatePos(x + dx, y + dy);
        target.UpdatePos(x, y);

    }

    int getDx(int x, int y)
    {
        // is right empty
        if (x < 2 && boxes[x + 1, y].IsFive())
            return 1;

        // is left empty
        if (x > 0 && boxes[x - 1, y].IsFive())
            return -1;

        return 0;
    }

    int getDy(int x, int y)
    {
        // is top empty
        if (y < 2 && boxes[x, y + 1].IsFive())
            return 1;

        // is bottom empty
        if (y > 0 && boxes[x, y - 1].IsFive())
            return -1;

        return 0;
    }

    void Shuffle()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boxes[i, j].IsFive())
                {
                    //Debug.Log(i);
                    //Debug.Log(j);
                    Vector2 pos = getValidMove(i, j);
                    Swap(i, j, (int)pos.x, (int)pos.y);
                    return;
                }
            }
        }
    }

    private Vector2 previousMove;

    Vector2 getValidMove(int x, int y)
    {
        Vector2 pos = new Vector2();
        do
        {
            int n = UnityEngine.Random.Range(0, 4);
            if (n == 0)
                pos = Vector2.left;
            else if (n == 1)
                pos = Vector2.right;
            else if (n == 2)
                pos = Vector2.up;
            else
                pos = Vector2.down;
        } while (!(isValidRange(x + (int)pos.x) && isValidRange(y + (int)pos.y)) || isRepeatMove(pos));

        previousMove = pos;
        //Debug.Log(pos);
        return pos;
    }

    bool isValidRange(int n)
    {
        return n >= 0 && n <= 2;
    }

    bool isRepeatMove(Vector2 pos)
    {
        return pos * -1 == previousMove;

    }

    void TurnBlue(int x, int y, int dx, int dy)
    {
        //true when a button next to five is pressed, false when five is pressed or a far away square
        if (!(dx == 0 && dy == 0) && !boxes[x, y].IsFive())
        {
            boxes[x, y].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            boxes[x, y].colour = !boxes[x, y].colour;
            if (GameObject.Find("red square popup") != null)
            {
                if (!myPanel.activeInHierarchy) { Bouton.instance.EnableButton(); }
            }
            else { Bouton.instance.EnableButton(); }
                
        }
    }

    void Revert(int x, int y, int dx, int dy)
    {
        int index1 = boxes[x, y].GetIndex(); ;
        float g = 0;
        if (!(dx == 0 && dy == 0) && !boxes[x, y].IsFive())
        {
            g = 0.4f + index1 * 0.06f;
            boxes[x, y].GetComponent<SpriteRenderer>().color = new Color(0.4f, g, 0.4f, 1);
            boxes[x, y].colour = !boxes[x, y].colour;
            Bouton.instance.DisableButton();
        }
    }

    int CheckWin()
    {
        int check_win = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boxes[i, j] == initial_boxes[i, j])
                {
                    check_win++;
                }
            }
        }
        return check_win;
    }

    void Win()
    {
        Timer.instance.TimerEnd();
        ResetBoard.instance.DisableButton();
        Bouton.instance.DisableButton();
        Congratulations.instance.EnableText();
        StartCoroutine(WinMessage());
        //disable everything
        //display win message
        //start coroutine that waits for 3s before next ready 123 starts
        //LevelController.instance.LoadNextLevel();
    }

    private IEnumerator WinMessage()
    {
        
        yield return new WaitForSeconds(2);
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "LogicGamePractice") {  /*do nothing*/ }
        else 
        {
            LevelController.instance.getSceneName();
            AddToBuffer(); // also in Give up script
            csvHandler.WriteLogicBuffer(s); //s is the shuffle index = level index
            SceneManager.LoadScene("Questionnaire"); 
        }
    }

    private IEnumerator WaitStart()
    {
        //altered coutdoen timer to be faster


        while (_count > 0)
        {
            yield return new WaitUntil(() => Rules.instance.IsPressed() == false);
            yield return new WaitForSeconds(COUNTDOWN_STEPTIME); //Time taken for intial countdown
            _count--;
            if (_count > 0)
            {
                string str = _count.ToString();
                Countdown.text = str;
            }
        }
        Countdown.enabled = false;
        ready.enabled = false;
        yield return new WaitUntil(() => Rules.instance.IsPressed() == false);
        yield return new WaitForSeconds(DELAY_AFTER_COUNTDOWN); //wait after countdown is done
        _start = false;
        Timer.instance.TimerBegin();
    }

        void DisplayRules()
    {
        //Revert(X, Y, DX, DY);
        Timer.instance.TimerPause();
        ResetBoard.instance.DisableButton();
        RulesText.instance.EnableRules();
        _rules = true;
    }

    void HideRules()
    {
        if (_start == false) { Timer.instance.TimerResume(); }
        ResetBoard.instance.EnableButton();
        RulesText.instance.DisableRules();
        _rules = false;
    }

    void reset_board()
    {
        Revert(X, Y, DX, DY);
        for (int y = 2; y >= 0; y--)
            for (int x = 0; x < 3; x++)
            {
                var target = initial_boxes[x, y];
                boxes[x, y] = target;
                target.UpdatePos(x, y);
            }
        ResetBoard.instance.ResetReset();
        for (int i = 0; i < s; i++) { Shuffle(); }
    }

    void getTimeBetweenMoves()
    {
        float time = Timer.instance.GetCurrentTime_float();
        float duration = time - previousTime;
        double dbl_duration = Math.Round((double)duration, 2);
        time_between_moves.Add(dbl_duration);
        previousTime = time;
        //Debug.Log("a:, " + a + ", timeBetween moves: " + time_between_moves[a] + ", a+1: " + time_between_moves[a + 1]);
        timeIntervalLogger.AddTimeInterval((float)dbl_duration);
        a++;
    }

    public void AddToBuffer()
    {
        DataSet_Logic dataSet = new DataSet_Logic();
        dataSet.Reset();
        dataSet.moves = moves_taken;
        dataSet.correct_tiles = CheckWin();
        dataSet.time = previousTime;
        // dataSet.split_time = 0f; - array, do as time interval logger

        csvHandler.AddToLogicBuffer(dataSet, s);
    }

    public void ExitPopup()
    {
        myPanel.SetActive(false);
        if (SquaresBlue() == 1) { Bouton.instance.EnableButton(); }
        //make it soo you can click everything again
    }

}
