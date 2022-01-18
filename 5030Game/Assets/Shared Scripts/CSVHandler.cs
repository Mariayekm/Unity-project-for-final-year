using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;

//Can get mouse speed on windows using https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.systeminformation.mousespeed?view=net-5.0
//name space: System.Windows.Forms, class: SystemInformation.MouseSpeed, outputs the windows mouse speed 1-20
//dll to add assembly not found in package manager, dll exists in multiple places across system but causes error when using any of them


public class CSVHandler : MonoBehaviour
{

    private List<string> startFp = new List<string>();
    [SerializeField] private int playerID;
    [SerializeField] private string playerName;
    [SerializeField] private GameObject GameMotorHandlerGObj;
    private GameMotorHandler motorHandler;
    private static volatile string playerIDstr;
    private const string PLAYERIDSTRLEN = "D8";         //used to set the lendth of player ID folder strings, and proceeding zeros
    private const string ATTEMPTIDSTRLEN = "D8";       //used to set the lendth of attempt folder name strings, and proceeding zeros
    private const string PLAYERIDSTR_FORMAT = "yyyyMMddHHmmssfff";


    //Directories path strings
    private volatile string fp_app, fp_dataCSV, fp_player;                           //Application, DataCSV, player
    private volatile string fp_motor, fp_sensory, fp_logic, fp_quiz, fp_other;       //Game data folders
    private volatile string fp_attempt0, fp_attemptCur;                              //file paths for attempts, can be switched to any game attempt
    private volatile string fp_zip;

    public const string FNAME_motor = "Motor", FNAME_sensory = "Sensory", FNAME_logic = "Logic", FNAME_other = "Other", FNAME_dataCSV = "DataCSV";//folder names for games constants 
    public const string FNAME_diffQuiz = "DiffQuiz.txt", FNAME_introQuiz = "IntroQuiz.txt", FNAME_playerID = "playerID.txt", FNAME_motorScore = "MotorScore.txt";

    private readonly string[] HEADERS_motor = new string[] { "TimeT", "TimeS", "Mx", "My", "Px", "Py", "Cx", "Cy", "Cio", "CtimeT", "CtimeS", "Gx", "Gy", "Gnum", "PMDist" };
    private readonly string[] HEADERS_motorScore = new string[] { "TimeS",  "Collisions"};
    private readonly string[] HEADERS_sensory = new string[] { "NumCorrectSq", "MinDist", "MinMag", "AvgDist", "AvgMag"};
    private readonly string[] HEADERS_logic = new string[] { "TotalMoves", "NumCorrectTiles", "TotalTime", "MoveTimeTaken" };
    private readonly string[] HEADERS_introQuiz = new string[] { "Age", "Gender", "GamingExp", "Mouse/Trackpad" };
    private readonly string[] HEADERS_diffQuiz = new string[] {"Diff"};

    private const int NUMLV_LOGIC = 8;
    private const int NUMLV_SENSORY = 9;
    private const int NUMLV_MOTOR = 10;

    private const int MOTOR_BUFFER_SIZE = 1000;
    private const int LOGIC_BUFFER_SIZE = 100;
    private const int SENSORY_BUFFER_SIZE = 100;
    private const int INTROQUIZ_BUFFER_SIZE = 10;
    private const int DIFFQUIZ_BUFFER_SIZE = 10;
    private volatile static DataSet_Motor[] motorBuffer;
    private volatile static String[][] motorBufferStr;
    private volatile static string[] logicBuffer, sensoryBuffer, introQuizBuffer, diffQuizBuffer;
    private volatile static int motorBufferCount, logicBufferCount, sensoryBufferCount, introQuizBufferCount, diffQuizBufferCount;
    private bool isDoNotDestroyOnLoad;

    public enum Directories
    {
        None = 0,               //Nothing
        App = 1,                //Assests dirextory/application folder
        DataCSV = 2,            //DataCSV directory
        Player = 3,             //Player directory
        Logic = 4,              //Player's Logic game folder
        Sensory = 5,            //Player's sensory game folder
        Motor = 6,              //Player's motor game folder
        Other = 7,              //player's other folder, contains intro quiz data and diff quiz data
        Attempt0 = 8,           //Gets attempt0
        AttemptCurrent = 9,     //Filepath of current attempt in game
        End = 10,               //Signifies end of enum
    }
    public enum GameNames
    {
        None = 0,
        Logic = 1,
        Sensory = 2,
        Motor = 3,
        IntroQuiz = 4,
        DiffQuiz = 5,
        Other = 6,
        MotorScore = 8,
        End = 9,
    }

    /*
     *Filestructure for data collection storage
     *txt files are csvformatted files
    ================  Filestructure ================
    Gamefolder
        Assests
            DataCSV
                PlayerID (player folder)
                    Logic
                        Attempt
                            Lv1.txt
                            Lv2.txt
                    Sensory
                        Attempt
                            Lv1.txt
                            Lv2.txt
                    Motor
                        Attempt
                            Lv1.txt
                            Lv2.txt
                    Other
                         Attempt
                            IntroQuizData.txt
                            DiffQuizData.txt

    */
    private void Awake()
    {
        if (GameMotorHandlerGObj != null)
        {
            motorHandler = GameMotorHandlerGObj.GetComponent<GameMotorHandler>();
        }
        else
        {
            //Debug.Log("GameMotorHandlerGObj is null");
        }
        isDoNotDestroyOnLoad = false;
        //SetFP();
        InitFiles();
        //GetAllAttemptDir(FNAME_motor);
        ResetBuffers();
        SetDoNotDestoryOnLoad();
    }

    private bool CSVHandlerExist()
    {
        bool CSVHandlerExists = false;
        GameObject tempGObj;
        CSVHandler tempCSVHandler;
        try
        {
            //tempGObj = GameObject.Find("CSVHandler");
            //tempCSVHandler = tempGObj.GetComponent();
        }
        catch
        {

        }

        return CSVHandlerExists;
    }

    private void SetDoNotDestoryOnLoad()
    {
        if (!isDoNotDestroyOnLoad) { DontDestroyOnLoad(gameObject); DontDestroyOnLoad(this); isDoNotDestroyOnLoad = true; }
    }

    public void ResetBuffers()
    {
        motorBuffer = new DataSet_Motor[MOTOR_BUFFER_SIZE];
        motorBufferStr = new string[MOTOR_BUFFER_SIZE][];
        logicBuffer = new string[LOGIC_BUFFER_SIZE];
        sensoryBuffer = new string[SENSORY_BUFFER_SIZE];
        diffQuizBuffer = new string[DIFFQUIZ_BUFFER_SIZE];
        introQuizBuffer = new string[INTROQUIZ_BUFFER_SIZE];
        motorBufferCount = 0;
        sensoryBufferCount = 0;
        logicBufferCount = 0;
        introQuizBufferCount = 0;
        diffQuizBufferCount = 0;
    }

    //Reinstantiates the file paths and buffers, passing CSV Handler between scenes causes them to get cleared occasionally
    public void ResetHandler()
    {
        ResetBuffers();
        SetFP();
    }

    private void CSVReadWriteTest(bool writeTest = true, bool readTest = true, Int64 numLines = 1000, bool countIE = false)
    {
        //tests reading and writing functionality of CSV handler
        //write test - override and write test file
        //read test - run read test
        //numLines - number of lines written in write test, min is headers and 1 defualt dataset
        //countIE - iterate of IEnumerable<String> read function to count number of elements
        SetFP();
        CheckAndMakeDir();
        string fp_temp = System.IO.Path.Combine(fp_attempt0, "Lv-1.txt");
        int lv = -1;
        TimeSpan ts_temp = new TimeSpan();
        System.DateTime dt1, dt2;
        DataSet_Motor dataMotorTest = new DataSet_Motor();
        if (writeTest)
        {
            //write headers
            WriteCSVDataSet(fp_temp, HEADERS_motor, false);
            //write defualt line
            WriteLine_Motor(dataMotorTest, lv, 0);
            //Populate dataset
            ts_temp = System.DateTime.Now.TimeOfDay;
            dataMotorTest.timeT = ts_temp;
            dataMotorTest.timeS = (float)ts_temp.TotalMilliseconds;
            dataMotorTest.Mx = 1f;
            dataMotorTest.My = 2f;
            dataMotorTest.Px = 3f;
            dataMotorTest.Py = 4f;
            dataMotorTest.Cx = 5f;
            dataMotorTest.Cy = 6f;
            //Timespan days, hours, minutes, seconds, miliseconds
            ts_temp = new TimeSpan(0, 0, 0, 10, 120);
            dataMotorTest.Cdt = DataSet_Motor.CollisionDataType.gateEnter;
            dataMotorTest.CtimeT = ts_temp;
            dataMotorTest.CtimeS = (float)ts_temp.TotalMilliseconds;
            dataMotorTest.Gx = 8f;
            dataMotorTest.Gy = 9f;
            dataMotorTest.Gnum = 10;
            WriteLine_Motor(dataMotorTest, lv, 0);
            //write many lines lines
            Debug.Log("Writing " + numLines + " test lines");
            dt1 = System.DateTime.Now;
            Debug.Log("Start: " + dt1.ToLongTimeString());
            for (Int64 i = 0; i < numLines; i++)
            {
                dataMotorTest.timeT = System.DateTime.Now.TimeOfDay;
                WriteLine_Motor(dataMotorTest, lv, 0);
            }
            dt2 = System.DateTime.Now;
            Debug.Log("End: " + dt2.ToLongTimeString());
            ts_temp = (dt2 - dt1);
            Debug.Log("Time taken = " + ts_temp.ToString());
            Debug.Log("Finished writing");
    }
        //reading test
        if (readTest)
        {
            string str_all = ReadAllToStr(fp_temp);
            string[] strAr_all = ReadAllToStrAr(fp_temp);
            System.Collections.Generic.IEnumerable<String> IE_all = ReadAllToIE(fp_temp);

            Debug.Log(str_all);
            Debug.Log("Num Ar lines read: " + strAr_all.Length);
            //cannot print size of ReadAllToIE as by definition it cant be done without iterating, over all so this section of the test is made optional by countIE
            if (countIE)
            {
                Int64 count = 0;
                foreach (string i in IE_all)
                {
                    count++;
                }
                Debug.Log("Num IE items read: " + count);
            }
        }

    }
    #region Var GetSet playerData
    //public void setPlayerID(int id) { playerID = id; CheckAndMakeDir(); }
    public int GetPlayerID() { return playerID; }
    public string GetPlayerIdStr() { return playerIDstr; }
    private void SetPlayerName(string name) { playerName = name; }
    private string GetPlayerName() { return playerName; }

    #endregion

    private void InitFiles()
    {
        //Make File Paths
        playerIDstr = playerID.ToString(PLAYERIDSTRLEN);
        fp_app = Application.dataPath;
        fp_dataCSV = System.IO.Path.Combine(fp_app, FNAME_dataCSV);
        fp_player = System.IO.Path.Combine(fp_dataCSV, playerIDstr);
        fp_motor = System.IO.Path.Combine(fp_player, FNAME_motor);
        fp_sensory = System.IO.Path.Combine(fp_player, FNAME_sensory);
        fp_logic = System.IO.Path.Combine(fp_player, FNAME_logic);
        fp_other = System.IO.Path.Combine(fp_player, FNAME_other);
        fp_attempt0 = System.IO.Path.Combine(fp_motor, 0.ToString(PLAYERIDSTRLEN));
        fp_attemptCur = fp_attempt0;
        CheckAndMakeDir();
        playerIDstr = MakePlayerID();
        playerIDstr = ReadPlayerIDStrFromFile();
    }



    //sets the nessesary local file paths
    public void SetFP()
    {
        Debug.Log("SetFP");
        #region Path fomatting concerns 
        //UnityEngine.Application.dataPath returns: C:/Users/Barna/OneDrive/Documents/Uni/TP/Games/Test2/Assets
        //System.IO.Path.Combine(fp_dataCSV, playerIDstr) return: C:/Users/Barna/OneDrive/Documents/Uni/TP/Games/Test2/Assets\DataCSV\00000001
        //UnityEngine returns " \ " as the deliminator, System.IO returns " / " as deliminator
        //May cause problems in future but currenlty does not cause problems
        //does not cause problems as \ is alt separator char
        #endregion
        #region testing comments
        //useful char = Path.DirectorySeparatorChar and str = str.Replace("\", "/")

        /*        Debug.Log("fp_app:" + fp_app);
                Debug.Log("exists: " + System.IO.Directory.Exists(fp_app));
                Debug.Log("fp_dataCSV: " + fp_dataCSV);
                Debug.Log("exists: " + System.IO.Directory.Exists(fp_dataCSV));
                Debug.Log("fp_player:" + fp_player);
                Debug.Log("exists: " + System.IO.Directory.Exists(fp_player));
                Debug.Log("fp_motor:" + fp_motor);
                Debug.Log("exists: " + System.IO.Directory.Exists(fp_motor));
                Debug.Log("fp_attempt0" + fp_attempt0);
                Debug.Log("exists: " + System.IO.Directory.Exists(fp_attempt0));*/
        #endregion

        playerIDstr = playerID.ToString(PLAYERIDSTRLEN);//converting playerID num to formatted string
        //setting file paths
        fp_app = Application.dataPath;                                              
        fp_dataCSV = System.IO.Path.Combine(fp_app, FNAME_dataCSV);                 
        fp_player = System.IO.Path.Combine(fp_dataCSV, playerIDstr);
        fp_motor = System.IO.Path.Combine(fp_player, FNAME_motor); 
        fp_sensory = System.IO.Path.Combine(fp_player, FNAME_sensory); 
        fp_logic = System.IO.Path.Combine(fp_player, FNAME_logic);
        fp_other = System.IO.Path.Combine(fp_player, FNAME_other);
        fp_attempt0 = System.IO.Path.Combine(fp_motor, 0.ToString(PLAYERIDSTRLEN));
        fp_attemptCur = fp_attempt0;
        playerIDstr = ReadPlayerIDStrFromFile();
        fp_zip = System.IO.Path.Combine(fp_dataCSV, FNAME_dataCSV + "_" + playerIDstr + ".zip");
    }

    private string MakePlayerID()
    {
        System.DateTime tempDT = System.DateTime.Now;
        string playerID = tempDT.ToString(PLAYERIDSTR_FORMAT);
        Debug.Log("New player Id: " + playerID);
        WritePlayerIdStrToFile(playerID);
        return tempDT.ToString(PLAYERIDSTR_FORMAT);
    }

    //checks if nessesary Directories exist
    public List<string> CheckAllDirExist()
    {
        //Debug.Log("Checking Directories");

        //checks bottom of file tree, as if bottom exist then parent folders must also exist
        //File creator will create parenrt files specified in path if they do not exist
        //so only children need be checked
        List<string> fp_failed = new List<string>();
        string[] fp_games = { FNAME_motor, FNAME_sensory, FNAME_logic, FNAME_other};
        fp_attemptCur = System.IO.Path.Combine(fp_motor, 0.ToString(PLAYERIDSTRLEN));

        //check attempt 0 for each game exists, and add those not found to a list
        for (int i = 0; i < fp_games.Length ; i++)
        {
            fp_attemptCur = GetAttemptFp(fp_games[i]);
            
            if (Directory.Exists(fp_attemptCur) == false)
            { 
               fp_failed.Add(fp_attemptCur); 
            }
        }
        #region DebugStuff
        /*
                //Debug: prints directories not found
                if (fp_failed.Count != 0 && fp_failed.Contains(null) == false)
                {
                    Debug.Log("Directories not found: ");
                    for (int i = 0; i < fp_failed.Count; i++)
                    {
                        Debug.Log(fp_failed[i]);
                    }
                }
        */
        #endregion
        return fp_failed;
    }

    //Gets new Atttempt File path for the games
    public string GetAttemptFp(string FNAME_game, int attempt = 0)
    {
        string fp;
        string temp;
        string fname_attempt = attempt.ToString(ATTEMPTIDSTRLEN);
        switch (FNAME_game)
        {
            case FNAME_motor:
                temp = System.IO.Path.Combine(fp_player, FNAME_motor);
                fp = System.IO.Path.Combine(temp, fname_attempt);
                break;
            case FNAME_sensory:
                temp = System.IO.Path.Combine(fp_player, FNAME_sensory);
                fp = System.IO.Path.Combine(temp, fname_attempt);
                break;
            case FNAME_logic:
                temp = System.IO.Path.Combine(fp_player, FNAME_logic);
                fp = System.IO.Path.Combine(temp, fname_attempt);
                break;
            case FNAME_other:
                temp = System.IO.Path.Combine(fp_player, FNAME_other);
                fp = System.IO.Path.Combine(temp, fname_attempt);
                break;
            case FNAME_motorScore:
                temp = System.IO.Path.Combine(fp_player, FNAME_motor);
                fp = System.IO.Path.Combine(temp, fname_attempt);
                break;
            default:
                fp = null;
                Debug.LogError("Unknown game directory atttempted: " + FNAME_game);
                System.Console.WriteLine("Unknown game directory atttempted: " + FNAME_game);
                break;

        }
        return fp;
    }

    //Checks Directory name of game directory is correct 
    public bool CheckGameStr(string FNAME_game)
    {
        if (FNAME_game == FNAME_motor || FNAME_game == FNAME_logic || FNAME_game == FNAME_sensory || FNAME_game == FNAME_other){ return true; }
        else { return false; }
    }


    //checks if nessesayr Directories exist and then makes any missing directories
    public void CheckAndMakeDir()
    {
        //get list of missing directories, and remake them
        List<string> toMake = new List<string>(); 
        toMake = CheckAllDirExist();
        if (toMake.Count != 0 && toMake != null )
        {
            for (int i = 0; i < toMake.Count; i++) {
                Debug.Log("Creating Directory: " + toMake[i]);
                System.IO.Directory.CreateDirectory(toMake[i]);
            }
        }
    }

    //returns the file paths of all attempt directories for a game
    public string[] GetAllAttemptDir(string FNAME_game)
    {
        //Debug.Log("GetAllAttemptDir");
        string[] dir = null;
        if (CheckGameStr(FNAME_game))
        {
            fp_attemptCur = System.IO.Path.Combine(fp_player, FNAME_game);
            dir = Directory.GetDirectories(fp_attemptCur, "*", SearchOption.TopDirectoryOnly);
            //Debug.Log("num: " + dir.Length);
            if (dir != null && dir.Length != 0)
            {
                for (int i = 0; i < dir.Length; i++)
                {
                    //Debug.Log(dir[i]);
                }
            }
            else { Debug.LogError("No attempt directories found"); }
        } else {Debug.LogError("Invalid Game name"); }

        return dir;
    }

    #region Writing functions
    //writes a line of data to a CSV
    //Using field automatically destroys the stream writer on finish and implements another trycatch
    public void WriteCSVDataSet(string fp, string[] dataSet, bool append = true)
    {
        string line;
        line = SetToStr(dataSet);
        try
        {
            using (System.IO.StreamWriter sWriter = new System.IO.StreamWriter(fp, append))
            {
                sWriter.WriteLine(line);
            }

        }
        catch(Exception ex)
        {
            throw new ApplicationException("Failed write to CSV."+System.Environment.NewLine + " Filepath: " +fp+ System.Environment.NewLine+ ", data: " + line + System.Environment.NewLine + ", append: " + append + System.Environment.NewLine + ", exception: " + ex);
        }

    }

    public void WriteStr(string fp, string str, bool append)
    {
        try
        {
            using (System.IO.StreamWriter sWriter = new System.IO.StreamWriter(fp, append))
            {
                sWriter.WriteLine(str);
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to write to file." + System.Environment.NewLine + " Filepath: " + fp + System.Environment.NewLine + ", data: " + str + System.Environment.NewLine + ", append: " + append + System.Environment.NewLine + ", exception: " + ex);
        }
    }

    private void WritePlayerIdStrToFile(string playerID)
    {
        string fp = System.IO.Path.Combine(fp_dataCSV, FNAME_playerID);
        Debug.Log("WPSF - Writing ID: " + playerID + ", fp: " + fp);
        WriteStr(fp, playerID, false);
    }

    private string ReadPlayerIDStrFromFile()
    {
        string fp = System.IO.Path.Combine(fp_dataCSV, FNAME_playerID);
        string temp = ReadAllToStr(fp);
        string playerID = "";
        //Reads first 17 characters, strips off last two control characters, ASCII: 13, 10
        for (int i = 0; i < PLAYERIDSTR_FORMAT.Length; i++)
        {
            playerID += temp[i];
        }
        //Debug.Log("RPS - Read: " + playerID + ", from: " + fp);
        return playerID;
    }

    //Writes one line of CSV formatted data from the Motor game
    public void WriteLine_Motor(DataSet_Motor dataSet, int lv, int attempt = 0)
    {
        //set file paths
        fp_attemptCur = GetAttemptFp(FNAME_motor, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv + ".txt");
        //Debug.Log("WritingStr: " + dataSet.ToStrCSV());
        WriteCSVDataSet(fp_file, dataSet.ToStrArray(), true);
    }

    public void WriteWrapper(DataSet_Motor dataSet, int lv, int attempt = 0)
    {
        WriteLine_Motor(dataSet, lv, attempt);
    }


    public void AddToMotorBuffer(DataSet_Motor dataSet)
    {
        AddToMotorBuffer_strArr(dataSet);
    }



    private void AddToMotorBuffer_strArr(DataSet_Motor dataSet)
    {
        string[] tempArr = dataSet.ToStrArray();
        motorBufferStr[motorBufferCount] = tempArr;
        motorBufferCount++;
        if (motorBufferCount >= MOTOR_BUFFER_SIZE)
        {
            WriteMotorBuffer();
        }
    }

    //change over to motorBufferStr
    public void WriteMotorBuffer()
    {
        Debug.Log("Writing MotorBuffer: num lines - " + motorBufferCount);
        WriteMotorBuffer_strArr();
    }
    

    private void WriteMotorBuffer_dataMotor()
    {
        for (int i = 0; i < motorBufferCount; i++)
        {
            WriteLine_Motor(motorBuffer[i], motorHandler.GetCurentLv());
        }
        motorBuffer = new DataSet_Motor[MOTOR_BUFFER_SIZE];
        motorBufferCount = 0;
    }

    private void AddToMotorBuffer_dataMotor(DataSet_Motor dataSet)
    {
        //Does not work, need to make DeepCoppy for this to work, using string array buffer as more space efficient
        motorBuffer[motorBufferCount] = dataSet;
        motorBufferCount++;
        if (motorBufferCount >= MOTOR_BUFFER_SIZE)
        {
            WriteMotorBuffer();
        }
    }

    public void WriteLine_MotorLine(string[] dataSet, int lv, int attempt = 0)
    {
        //set file paths
        fp_attemptCur = GetAttemptFp(FNAME_motor, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv + ".txt");
        WriteCSVDataSet(fp_file, dataSet, true);
    }

    private void WriteMotorBuffer_strArr(int lv = 0)
    {
        for (int i = 0; i < motorBufferCount; i++)
        {
            WriteLine_MotorLine(motorBufferStr[i], motorHandler.GetCurentLv());
        }
        motorBuffer = new DataSet_Motor[MOTOR_BUFFER_SIZE];
        motorBufferCount = 0;
    }

    //skeleton code for Logic and sensory buffers
    public void AddToLogicBuffer(DataSet_Logic dataSet, int lv = 0, int attempt = 0)
    {
        //Debug.Log("ALB-Adding to logic buffer, dataset:"+ dataSet.ToStrCSV() +", lv: " + lv  + ", attempt: " + attempt);
        logicBuffer[logicBufferCount] = dataSet.ToStrCSV();
        logicBufferCount++;
        if (logicBufferCount>=LOGIC_BUFFER_SIZE)
        { 
            WriteLogicBuffer(lv, attempt);
        }
    }

    public void WriteLogicBuffer(int Lv, int attempt = 0)
    {
        Debug.Log("WLB-Writeing num lines: " + logicBufferCount);
        fp_attemptCur = GetAttemptFp(FNAME_logic, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, "Lv" + Lv.ToString() + ".txt");
        for (int i = 0; i < logicBufferCount; i++)
        {
            WriteStr(fp_file, logicBuffer[i], true);
        }
        logicBuffer = new string[LOGIC_BUFFER_SIZE];
        logicBufferCount = 0;
    }

    public void AddToSensoryBuffer(DataSet_Sensory dataSet, int lv = 0, int attempt = 0)
    {
        sensoryBuffer[sensoryBufferCount] = dataSet.ToStrCSV();
        sensoryBufferCount++;
        if (sensoryBufferCount >= SENSORY_BUFFER_SIZE)
        {
            WriteSensoryBuffer(lv, attempt);
        }
    }

    public void WriteSensoryBuffer(int Lv, int attempt = 0)
    {
        Debug.Log("WSB-Writeing num lines: " + sensoryBufferCount);
        fp_attemptCur = GetAttemptFp(FNAME_sensory, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, "Lv" + Lv.ToString() + ".txt");
        for (int i = 0; i < sensoryBufferCount; i++)
        {
            WriteStr(fp_file, sensoryBuffer[i], true);
        }
        sensoryBuffer = new string[SENSORY_BUFFER_SIZE];
        sensoryBufferCount = 0;
    }

    public void AddtoIntroBuffer(DataSet_IntroQuiz dataSet)
    {
        introQuizBuffer[introQuizBufferCount] = dataSet.ToStrCSV();
        introQuizBufferCount++;
        if (introQuizBufferCount >= INTROQUIZ_BUFFER_SIZE)
        {
            WriteIntroQuizBuffer();
        }
    }

    public void WriteIntroQuizBuffer()
    {
        fp_attemptCur = GetAttemptFp(FNAME_other);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, FNAME_introQuiz);
        for (int i = 0; i < introQuizBufferCount; i++)
        {
            WriteStr(fp_file, introQuizBuffer[i], true);
        }
        introQuizBuffer = new string[INTROQUIZ_BUFFER_SIZE];
        introQuizBufferCount = 0;
    }

    public void AddtoDiffQuizBuffer(DataSet_DiffQuiz dataSet)
    {
        diffQuizBuffer[introQuizBufferCount] = dataSet.ToStrCSV();
        diffQuizBufferCount++;
        if (diffQuizBufferCount >= DIFFQUIZ_BUFFER_SIZE)
        {
            WriteDiffQuizBuffer();
        }
    }

    public void WriteDiffQuizBuffer(int lv = 0, int attempt = 0)
    {
        fp_attemptCur = GetAttemptFp(FNAME_other, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, FNAME_diffQuiz);
        for (int i = 0; i < diffQuizBufferCount; i++)
        {
            WriteStr(fp_file, diffQuizBuffer[i], true);
        }
        diffQuizBuffer = new string[DIFFQUIZ_BUFFER_SIZE];
        diffQuizBufferCount = 0;
    }

    public void WriteMotorScore(string[] score)
    {
        foreach (string line in score)
        {
            WriteStr(GetDataFilePath(GameNames.MotorScore, 0, 0), line, true);
        }
    }


    public void ResetCSV(string fp, string[] header)
    {
            WriteCSVDataSet(fp, header, false);
    }

    public void ResetLv(GameNames FNAME_game, int lv, int attempt = 0)
    {
        string fp;
        string[] header;
        fp_attemptCur = GetAttemptFp(GetFName(FNAME_game), attempt);
        switch (FNAME_game)
        {
            case GameNames.Logic:
                header = HEADERS_logic;
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.Sensory:
                header = HEADERS_sensory;
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.Motor:
                header = HEADERS_motor;
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.IntroQuiz:
                header = HEADERS_introQuiz;
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_introQuiz);
                break;
            case GameNames.DiffQuiz:
                header = HEADERS_diffQuiz;
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_diffQuiz);
                break;
            case GameNames.MotorScore:
                header = HEADERS_motorScore;
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_motorScore);
                break;
            default:
                header = new string[] {""};
                fp = "";
                Debug.LogError("Unknown GameNames: " + FNAME_game);
                break;
        }
        ResetCSV(fp, header);
    }
    public string GetDataFilePath(GameNames FNAME_game, int lv, int attempt = 0)
    {
        string fp;
        fp_attemptCur = GetAttemptFp(GetFName(FNAME_game), attempt);
        switch (FNAME_game)
        {
            case GameNames.Logic:
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.Sensory:
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.Motor:
                fp = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv.ToString() + ".txt");
                break;
            case GameNames.IntroQuiz:
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_introQuiz);
                break;
            case GameNames.DiffQuiz:
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_diffQuiz);
                break;
            case GameNames.MotorScore:
                fp = System.IO.Path.Combine(fp_attemptCur, FNAME_motorScore);
                break;
            default:
                fp = "";
                Debug.LogError("Unknown GameNames: " + FNAME_game);
                break;
        }
        return fp;
    }

    public void ResetSavedData(int attempt = 0)
    {
        ResetLv(GameNames.IntroQuiz, 0, attempt);
        ResetLv(GameNames.DiffQuiz, 0 , attempt);
        ResetLv(GameNames.MotorScore, 0, attempt);
        for (int i = 0; i < NUMLV_LOGIC; i++)
        {
            ResetLv(GameNames.Logic, i, attempt);
        }
        for (int i = 0; i < NUMLV_SENSORY   ; i++)
        {
            ResetLv(GameNames.Sensory, i, attempt);
        }
        for (int i = 0; i < NUMLV_MOTOR; i++)
        {
            ResetLv(GameNames.Motor, i, attempt);
        }
    }

    //Puts DataCSV Data into a zip file, coppies temporary direcory to avoid sending unity MetaData files
    //Setting fp_zip and playerIDstr will not persist outside of this function for unknown resons
    public string ZipData()
    {
        //string IDStr = MakePlayerID();
        //playerIDstr = IDStr;
        //string outputFilePath = System.IO.Path.Combine(fp_dataCSV, FNAME_dataCSV +"_"+ playerIDstr + ".zip");
        
        string outputFilePath = fp_zip;
        string tempDirStr = System.IO.Path.Combine(fp_dataCSV, "tempDir");
        string tempCopyDest = "";
        string tempFileSource = "";
        string fileToZip = tempDirStr;

        //create temporary directories and coppy txt files to temp directories
        System.IO.Directory.CreateDirectory(tempDirStr);
        //Logic Game Files
        tempDirStr = System.IO.Path.Combine(fileToZip, FNAME_logic);
        System.IO.Directory.CreateDirectory(tempDirStr);
        for (int i = 0; i <= NUMLV_LOGIC; i++)
        {
            tempFileSource = GetDataFilePath(GameNames.Logic, i);
            tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
            File.Copy(tempFileSource, tempCopyDest);
        }
        //Sensory Game Files
        tempDirStr = System.IO.Path.Combine(fileToZip, FNAME_sensory);
        System.IO.Directory.CreateDirectory(tempDirStr);
        for (int i = 0; i <= NUMLV_SENSORY; i++)
        {
            tempFileSource = GetDataFilePath(GameNames.Sensory, i);
            tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
            File.Copy(tempFileSource, tempCopyDest);
        }
        //Motor Game Files
        tempDirStr = System.IO.Path.Combine(fileToZip, FNAME_motor);
        System.IO.Directory.CreateDirectory(tempDirStr);
        for (int i = 0; i <= NUMLV_MOTOR; i++)
        {
            tempFileSource = GetDataFilePath(GameNames.Motor, i);
            tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
            File.Copy(tempFileSource, tempCopyDest);
        }
        tempFileSource = GetDataFilePath(GameNames.MotorScore, 0);
        tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
        File.Copy(tempFileSource, tempCopyDest);

        //Diff Quiz Files
        tempDirStr = System.IO.Path.Combine(fileToZip, FNAME_other);
        System.IO.Directory.CreateDirectory(tempDirStr);
        tempFileSource = GetDataFilePath(GameNames.DiffQuiz, 0);
        tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
        File.Copy(tempFileSource, tempCopyDest);
        //IntroQuiz Files
        tempFileSource = GetDataFilePath(GameNames.IntroQuiz, 0);
        tempCopyDest = System.IO.Path.Combine(tempDirStr, Path.GetFileName(tempFileSource));
        File.Copy(tempFileSource, tempCopyDest);


        Debug.Log("outputFile: " + outputFilePath);
        Debug.Log("fileToZip: " + fileToZip);
        if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
        ZipFile.CreateFromDirectory(fileToZip, outputFilePath);
        Directory.Delete(fileToZip, true); //Delete temporary file
        Debug.Log("ZD-fp_zip: " + fp_zip);
        return outputFilePath;
    }

    #endregion 

    #region String formatting
    //converts a string array to a CSV fomrmatted string
    public string SetToStr(string[] set) {
        string output = null;
        if (set.Length > 1)
        {
            output = set[0];
            for (int i = 1; i < set.Length; i++)
            {
                output += "," + set[i];
            }
        }
        else if (set.Length == 1)
        {
            output = set[0];
        }
        else { Debug.LogError("Set empty"); }
        return output;
    }
    //convers a string of a CSV line to an array
    public string[] CSVLineToArray(string str)
    {
        return str.Split(',');
    }
    #endregion region

    #region CSV reading funcs
    //Multiple methods for reading CSV due to worries of reaching mac data limit in a variable 
    //Reads all data from CSV and returns a single string
    public string ReadAllToStr(string path) {
        bool Fexists = File.Exists(path);
        string data = "";
        if (Fexists)
        {
            try
            {
                data = File.ReadAllText(path);
            }catch(Exception ex) 
            {
                throw new ApplicationException("Failed to read CSV: ", ex); 
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + path);
        }
        return data;
    }

    //reads all text in CSV and returns each line as an item in the array
    public string[] ReadAllToStrAr(string path)
    {
        bool Fexists = File.Exists(path);
        string[] data = null;
        if (Fexists)
        {
            try
            {
                data = File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to read CSV: ", ex);
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + path);
        }
        return data;
    }
    //Returns All lines of CSV as IEnumerable<string>
    public System.Collections.Generic.IEnumerable<string> ReadAllToIE(string path)
    {
        System.Collections.Generic.IEnumerable<string> data = null;
        bool Fexists = File.Exists(path);
        if (Fexists)
        {
            try
            {
                data = File.ReadLines(path);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to read CSV: ", ex);
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + path);
        }
        return data;
    }
    #endregion
    
    public string GetDirectoriesFP(Directories fp)
    {
        string dirFp = "";
        switch (fp)
        {
            case Directories.None:
                Debug.Log("Directory none requested");
                break;
            case Directories.App  :
                dirFp = fp_app;
                break;
            case Directories.DataCSV:
                dirFp = fp_dataCSV;
                break;
            case Directories.Player:
                dirFp = fp_player;
                break;
            case Directories.Logic:
                dirFp = fp_logic;
                break;
            case Directories.Sensory:
                dirFp = fp_sensory;
                break;
            case Directories.Motor:
                dirFp = fp_motor;
                break;
            case Directories.Other:
                dirFp = fp_other;
                break;
            case Directories.Attempt0:
                dirFp = fp_attempt0;
                break;
            case Directories.AttemptCurrent:
                dirFp = fp_attemptCur;
                break;
            default:
                Debug.LogError("Unknown Filepath requested");
                break;
        }
        return dirFp;
    }

    public string GetFName(GameNames FNAME_game)
    {
        string output = "";
        switch (FNAME_game)
        {
            case GameNames.Logic:
                output = FNAME_logic;
                break;
            case GameNames.Sensory:
                output = FNAME_sensory;
                break;
            case GameNames.Motor:
                output = FNAME_motor;
                break;
            case GameNames.IntroQuiz:
                output = FNAME_other;
                break;
            case GameNames.DiffQuiz:
                output = FNAME_other;
                break;
            case GameNames.Other:
                output = FNAME_other;
                break;
            case GameNames.MotorScore:
                output = FNAME_motorScore;
                break;
            default:
                Debug.LogError("Unknown GameFolderNames: " + FNAME_game);
                break;
        }

        return output;
    }


    public bool CheckFileExist(string fp)
    {
        return File.Exists(fp);
    }

    public void SetGameMotorHandlerRef(GameObject GMH_GObj)
    {
        GameMotorHandlerGObj = GMH_GObj;
        motorHandler = GMH_GObj.GetComponent<GameMotorHandler>();
    }

    public string GetZipFp() { Debug.Log("GetZipFp: " + fp_zip); Debug.Break(); return fp_zip; }

    public void BtnTesting()
    {
         System.DateTime tempDT = System.DateTime.Now;
         Debug.Log("Y:" + tempDT.Year + ", M:" + tempDT.Month + ", D:" + tempDT.Day + ", H:" + tempDT.Hour + ", M:" + tempDT.Minute + ", ms:" + tempDT.Millisecond);
         Debug.Log( tempDT.ToString(PLAYERIDSTR_FORMAT));
    }




}
#region struct dataSet format Legacy code
//Struct for data set lines for motor game  
/*    public struct DataMotor {
        public enum CollisionDataType
        {
            None = 0,
            start = 1,
            end = 2
        }
        public System.TimeSpan timeT;
        public float timeS;
        public float Mx;
        public float My;
        public float Px;
        public float Py;
        public float Cx;
        public float Cy;
        public CollisionDataType Cio;
        public TimeSpan CtimeT;
        public float CtimeS;
        public float Gx;
        public float Gy;
        public float Gnum;
    }

    //struct for logic game data entries
    public struct DataLogic
    {
        public TimeSpan timeT;
        public float timeS;
        public int Diff;
        public bool pass;
    }

    public struct DataSensory
    {
        public TimeSpan timeT;
        public float timeS;
        public int Diff;
        public bool pass;
        public int posSq1x;
        //poss store location of squares that turned blue
    }*/
//sets a DataMotor struct to its defualt values;
/*    public DataMotor SetDataMotorDefualt(DataMotor data)
    {
        data.timeT = new System.TimeSpan();
        data.timeS = 0f;
        data.Mx = 0f;
        data.My = 0f;
        data.Px = 0f;
        data.Py = 0f;
        data.Cx = 0f;
        data.Cy = 0f;
        data.Cio = DataMotor.CollisionDataType.None;
        data.CtimeT = new System.TimeSpan();
        data.CtimeS = 0f;
        data.Gx = 0f;
        data.Gy = 0f;
        data.Gnum = 0f;
        return data;
    }*/

//
/*    public DataMotor CreateDataSetMotor(TimeSpan timeT = new System.TimeSpan(), float timeS = 0f, float Mx = 0f, float My = 0f, float Px = 0f, float Py = 0f, float Cx = 0f, float Cy = 0f, DataMotor.CollisionDataType Cio = DataMotor.CollisionDataType.None, TimeSpan CtimeT = new System.TimeSpan(), float CtimeS = 0f,float Gx = 0f, float Gy = 0f,float Gnum = 0f)
    {
        CSVHandler.DataMotor data = new CSVHandler.DataMotor();
        data = SetDataMotorDefualt(data);
        data.timeT = timeT;
        data.timeS = timeS;
        data.Mx = Mx;
        data.My = My;
        data.Px = Px;
        data.Py = Py;
        data.Cx = Cx;
        data.Cy = Cy;
        data.Cio = Cio;
        data.CtimeT = CtimeT;
        data.CtimeS = CtimeS;
        data.Gx = Gx;
        data.Gy = Gy;
        data.Gnum = Gnum;
        return data;
    }*/
//converts Datamotor type to a string array
/*    public string[] DataMotorToSet(DataMotor data)
    {
        return new string[] { data.timeT.ToString("mm':'ss'.'fff"), data.timeS.ToString(), data.Mx.ToString(), data.My.ToString(), data.Px.ToString(), data.Py.ToString(), data.Cx.ToString(), data.Cy.ToString(), data.Cio.ToString(), data.CtimeT.ToString("mm':'ss'.'fff"), data.CtimeS.ToString(), data.Gx.ToString(), data.Gy.ToString(), data.Gnum.ToString() };
    }*/

//writes a DataMotor type to a CSV
/*    public void WriteLine_Motor(DataMotor dataSet, int lv, int attempt = 0)
    {
        fp_attemptCur = GetAttemptFp(FNAME_motor, attempt);
        string fp_file = System.IO.Path.Combine(fp_attemptCur, "Lv" + lv + ".txt");
        string[] data = DataMotorToSet(dataSet);
        WriteCSVLine(fp_file, data, true);
    }*/
#endregion
