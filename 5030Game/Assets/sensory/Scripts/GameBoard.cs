using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GameBoard : MonoBehaviour
{
    public static GameBoard instance;
    public GridTile tilePrefab;
    public MinimumDistances Prefab;
    public GridTile[,] tiles = new GridTile[4, 4];
    public Sprite[] sprites;
    public Text Countdown;
    public Sprite Square1;
    public Text ready;
    public Text congrats;
    public GameObject Grid_background;

    private const float COUNTDOWN_STEPTIME = 0.75f;//1
    private const float DELAY_AFTER_COUNTDOWN = 0.1f;//1 
    private const float DELAY_SHOW_ANSWER = 2f;//5
    private const float FADEOUT_TIME = 1f;//1
    [SerializeField] private GameObject CSVHandlerGObj;
    private CSVHandler csvHandler;
    private GameObject minDistObj;
    private MinimumDistances minimumDistances;

    Vector2[] correct_squares = new Vector2[5];
    ArrayList selected_squares = new ArrayList();
    Vector2[] min_distance = new Vector2[5];
    float[] min_magnitude = new float[5];


    Camera MainCamera;
    int _count = 4;
    bool isValid = true;
    int blue_squares = 0;
    int p = 0;
    float oldscale = 0.248f;
    int JustOnce;
    int lvl;
    int hasWritten = 0;

    void Awake()
    {
        CSVHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
        minimumDistances = Instantiate(Prefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        float pixelRatio = (MainCamera.orthographicSize * 2) / MainCamera.pixelHeight;
        Grid_background = GameObject.Find("Grid Background");
        Grid_background.gameObject.transform.localScale = new Vector2(pixelRatio * 330, pixelRatio * 330);
        //Debug.Log(MainCamera.pixelHeight);
        csvHandler.ResetHandler();
        lvl = LevelController.instance.GetGridIndex();
        Init(pixelRatio);
        Countdown.text = "4";
        blue_squares = 5;
        StartCoroutine(WaitStart());
        JustOnce = 0;
        congrats.enabled = false;
        //confirm button disabled 
    }

    void Init(float pixRatio)
    {
        p = 3 + LevelController.instance.GetGridIndex();
        if (lvl == 0) { p = 4; }
        int n = 0;
        float pixelRatio = (MainCamera.orthographicSize * 2) / MainCamera.pixelHeight;
        float move = 0.99f / (float)p;
        float scale = (move-0.01f) /*/8f*/;
        float offset = 0.365f + (oldscale - scale)/2f;
        tiles = new GridTile[p, p];
        for (int y = p - 1; y >= 0; y--)
            for (int x = 0; x < p; x++)
            {
                GridTile tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity);
                tile.Init(x, y, n + 1, sprites[n], ClickToSelect);
                tiles[x, y] = tile;
                 //tiles[x, y].gameObject.transform.localScale = new Vector2(0.25f, 0.25f);
                //tiles[x, y].gameObject.transform.localScale = new Vector2(0.25f, 0.25f);
                tiles[x, y].gameObject.transform.localPosition = new Vector2((move * x)-offset, (move * y)-offset);
                tiles[x, y].gameObject.transform.localScale = new Vector2(scale, scale);
                //tiles[x, y].gameObject.transform.localPosition = new Vector2((2f * x) , (2f * y) );
                tiles[x, y].GetComponent<BoxCollider2D>().size = new Vector2(1,1);                                  //size of box colliders are set to be (1.666,1.666) by defualt for unknown reasons, this resets them
                n++;
            }
        oldscale = scale;
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "SensoryGamePractice")
        {
            if(SQuitPractice.instance.IsPressed() == true) 
            {
                LevelController.instance.getSceneName();
                LevelController.instance.LoadNextSensoryLevel(); 
            }
        }
            if (SquaresHappy.instance.IsPressed()) //condition will be confirm button
        {
            EndOfGame();            
        }
        
    } 

    void ClickToSelect(int x, int y)
    {
        if (SRulesText.instance.RulesEnabled() == false)
        {
            Vector2 square = new Vector2();
            if (tiles[x, y].IsBlue())
            {
                tiles[x, y].GetComponent<SpriteRenderer>().color = new Color(0.215f, 0.23f, 0.24f, 1);
                blue_squares--;
                square.x = x;
                square.y = y;
                selected_squares.Remove(square);
                SquaresHappy.instance.DisableButton();
                //remove that tile for the array
            }
            else if (blue_squares < 5)
            {
                tiles[x, y].GetComponent<SpriteRenderer>().color = new Color(0.008f, 0.53f, 0.82f, 1);
                blue_squares++;
                square.x = x;
                square.y = y;
                selected_squares.Add(square);

                if (blue_squares == 5)
                    SquaresHappy.instance.EnableButton();
                //append to array of positions
            }
        }
        //Debug.Log(x);
       // Debug.Log(y);
    }

    void Flash()
    {
        Vector2 pos = new Vector2();
        for (int i = 0; i < 5; i++)
        {
            pos = getValidSquare(i);
            StartCoroutine(Fade(pos));
        }
    }

    Vector2 getValidSquare(int j)
    {
        Vector2 pos = new Vector2();
        do
        {
            pos.x = UnityEngine.Random.Range(0, p);
            pos.y = UnityEngine.Random.Range(0, p);
            isValidRange(pos);

        } while (isValid == false);

        correct_squares[j] = pos; //array of correct squares
        return pos;

    }

    void isValidRange(Vector2 _pos)
    {
        isValid = true;
        int j = correct_squares.Length;
        for (int i = 0; i < j; i++)
        {
            if (_pos == correct_squares[i])
            {
                isValid = false;
            }
            if (_pos == (correct_squares[i] + Vector2.left))
            {
                isValid = false;
            }
            if (_pos == (correct_squares[i] + Vector2.right))
            {
                isValid = false;
            }
            if (_pos == (correct_squares[i] + Vector2.up)) 
            {
                isValid = false;
            }
            if (_pos == (correct_squares[i] + Vector2.down)) 
            {
                isValid = false;
            }
        }

    }

    private IEnumerator WaitStart()
    {
        while (_count > 0)
        {
            yield return new WaitUntil(() => ReadRules.instance.IsPressed() == false);
            yield return new WaitForSeconds(COUNTDOWN_STEPTIME);
            _count--;
            if (_count > 0)
            {
                string str = _count.ToString();
                Countdown.text = str;
            }
        }
        Countdown.enabled = false;
        ready.enabled = false;
        yield return new WaitUntil(() => ReadRules.instance.IsPressed() == false);
        yield return new WaitForSeconds(DELAY_AFTER_COUNTDOWN);
        ReadRules.instance.DisableButton();
        Flash();
        ReadRules.instance.EnableButton();
    }

    private IEnumerator Fade(Vector2 _pos)
    {
        float t = FADEOUT_TIME;//1f
        //t = FADEOUT_TIME - (float)LevelController.instance.GetGridIndex()/8f;
        //Debug.Log(t);
        for (float i = t; i >= 0; i -= Time.deltaTime) //2 seconds because that's how long it is in the french game for the easiest level
        {
            float r = 0.215f - (i * (0.207f / t)); //0.138 with t = 1.5, 0.104 with t = 2
            float g = 0.23f + (i * (0.3f / t)); //0.2f, 0.15
            float b = 0.24f + (i * (0.58f / t)); //0.38f, 0.29
            // set color with i as alpha
            tiles[(int)_pos.x, (int)_pos.y].GetComponent<SpriteRenderer>().color = new Color(r, g, b, 1); //full blue colour = 0.008f, 0.53f, 0.82f
            yield return null;
        }

        foreach (Vector2 i in correct_squares)
            tiles[(int)i.x, (int)i.y].GetComponent<SpriteRenderer>().color = new Color(0.215f, 0.23f, 0.24f, 1);

        blue_squares = 0;
    }

    private IEnumerator WaitEnd()
    {
        yield return new WaitForSeconds(DELAY_SHOW_ANSWER); //wait time at end after submission of answers
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "SensoryGamePractice") { /*do nothing*/ }
        else 
        {
            LevelController.instance.getSceneName();
            SceneManager.LoadScene("Questionnaire"); 
        }
    }
    
    void EndOfGame()
    {
        int match = 0;
        blue_squares = 5;
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
            {
                if (correct_squares[i].Equals(selected_squares[j]))
                {
                    match++;
                }
            }

        if (JustOnce == 0)
        {
            HighlightSquares();           
            JustOnce++;
        }
        
        for (int y = p - 1; y >= 0; y--)
            for (int x = 0; x < p; x++)
            {
                tiles[x, y].setClickToNull(); //might have to set clickfunc not to null when starting another level
            }
        //display correct squares
        if (match == 5)
        {
            Win();
        }

        if (hasWritten == 0)
        {
            CalcMinDistance();
            AddToBuffer(match);
            csvHandler.WriteSensoryBuffer(lvl);
            hasWritten = 1;
        }

        StartCoroutine(WaitEnd());
        
    }

    void CalcMinDistance()
    {
        Vector2[] ss_array = selected_squares.OfType<Vector2>().ToArray();
        for (int i = 0; i < 5; i++)
        {
            Vector2[] distances = new Vector2[5];
            float[] magnitudes = new float[5];
            float min_mag = 0;

            for (int j = 0; j < 5; j++)
            {
                distances[j] = correct_squares[j] - ss_array[i];
                magnitudes[j] = distances[j].magnitude;
            }

            min_mag = Mathf.Min(magnitudes);
            int index = Array.IndexOf(magnitudes, min_mag);  //find which index has the lowest magnitude 
            min_magnitude[i] = magnitudes[index];  //add the magnitude with this index the min-magnitudes list
            min_distance[i] = distances[index];  //add the distance with this index the min - distances list  
            minimumDistances.AddMininumDistances(min_magnitude[i], min_distance[i]);//adds to buffer

            //Debug.Log(min_distance[i]);
        }
    }

    void HighlightSquares()
    {
        int a = 0;
        int b = 0;
        GameObject go = new GameObject("New Sprite");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = Square1;
        renderer.sortingOrder = 11;
        a = (int)correct_squares[0].x;
        b = (int)correct_squares[0].y;
        go.transform.parent = tiles[a, b].GetComponent<SpriteRenderer>().transform;
        go.gameObject.transform.localScale = new Vector2(3, 3);
        go.gameObject.transform.localPosition = new Vector2(0, 0);
        go.GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.8f, 0.27f, 1);

        GameObject go1 = new GameObject("New Sprite");
        SpriteRenderer renderer1 = go1.AddComponent<SpriteRenderer>();
        renderer1.sprite = Square1;
        renderer1.sortingOrder = 11;
        a = (int)correct_squares[1].x;
        b = (int)correct_squares[1].y;
        go1.transform.parent = tiles[a, b].GetComponent<SpriteRenderer>().transform;
        go1.gameObject.transform.localScale = new Vector2(3, 3);
        go1.gameObject.transform.localPosition = new Vector2(0, 0);
        go1.GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.8f, 0.27f, 1);

        GameObject go2 = new GameObject("New Sprite");
        SpriteRenderer renderer2 = go2.AddComponent<SpriteRenderer>();
        renderer2.sprite = Square1;
        renderer2.sortingOrder = 11;
        a = (int)correct_squares[2].x;
        b = (int)correct_squares[2].y;
        go2.transform.parent = tiles[a, b].GetComponent<SpriteRenderer>().transform;
        go2.gameObject.transform.localScale = new Vector2(3, 3);
        go2.gameObject.transform.localPosition = new Vector2(0, 0);
        go2.GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.8f, 0.27f, 1);

        GameObject go3 = new GameObject("New Sprite");
        SpriteRenderer renderer3 = go3.AddComponent<SpriteRenderer>();
        renderer3.sprite = Square1;
        renderer3.sortingOrder = 11;
        a = (int)correct_squares[3].x;
        b = (int)correct_squares[3].y;
        go3.transform.parent = tiles[a, b].GetComponent<SpriteRenderer>().transform;
        go3.gameObject.transform.localScale = new Vector2(3, 3);
        go3.gameObject.transform.localPosition = new Vector2(0, 0);
        go3.GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.8f, 0.27f, 1);

        GameObject go4 = new GameObject("New Sprite");
        SpriteRenderer renderer4 = go4.AddComponent<SpriteRenderer>();
        renderer4.sprite = Square1;
        renderer4.sortingOrder = 11;
        a = (int)correct_squares[4].x;
        b = (int)correct_squares[4].y;
        go4.transform.parent = tiles[a, b].GetComponent<SpriteRenderer>().transform;
        go4.gameObject.transform.localScale = new Vector2(3, 3);
        go4.gameObject.transform.localPosition = new Vector2(0, 0);
        go4.GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.8f, 0.27f, 1);
    }

    void Win()
    {
        congrats.enabled = true;
    }

    public void AddToBuffer(int squares)
    {
        DataSet_Sensory dataSet = new DataSet_Sensory();
        dataSet.Reset();
        dataSet.correct_squares = squares;
        /*dataSet.min_dist = new Vector2(0, 0);
        dataSet.min_mag = 0f;*/

        csvHandler.AddToSensoryBuffer(dataSet, lvl);
    }
}
    