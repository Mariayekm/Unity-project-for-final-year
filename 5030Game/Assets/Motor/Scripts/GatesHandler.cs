using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatesHandler : MonoBehaviour
{
    public int lvCurrent;
    public List<GateClass> gateList { get; set; }
    private GameObject playerGObj;
    [SerializeField] private GameObject spawnerGObj;
    private Spawner spawner;
    private Camera mainCam;
    private List<float[]> LevelTest;
    private List<int> gateIDs;
    private float ERROR_MARGIN;//0.2
    private float X_MIN;//-7
    private float X_MAX;//7.8
    private float Y_MIN;//5
    private float Y_MAX;//-5
    private float HOLE_MIN;//0.7
    private float HOLE_MAX;//10
    private float SEPARATION_MIN;//0.7

    public enum GateFailCode
    {
        None = 0,
        Formatting = 1,
        Xmin = 2,
        Xmax = 3,
        Ymin = 4,
        Ymax = 5,
        HoleMin = 6,
        HoleMax = 7,
        Separation_min = 8,
        GateCheckHeight = 9,
        GateCheckMidYOnScreen = 10,
        IdNotUnique = 11,
        IdDoesNotMatchPos = 12
    }


    private void Awake()
    {
        LevelTest = new List<float[]>();
        lvCurrent = 0;
        gateList = new List<GateClass>();
        gateIDs = new List<int>();
        playerGObj = GameObject.Find("Player");
        spawner = spawnerGObj.GetComponent<Spawner>();
        mainCam = Camera.main;
        float worldHeight = mainCam.orthographicSize * 2f;//10
        float worldWidth = worldHeight * mainCam.aspect; //17.88
        ERROR_MARGIN = 0.2f;
        X_MIN = -worldWidth/2 + playerGObj.transform.localScale.x*3; //-7.385
        X_MAX = worldWidth/2 - playerGObj.transform.localScale.x*3; //7.385
        Y_MAX = worldHeight/2 - ERROR_MARGIN;//
        Y_MIN = -worldHeight/2 + ERROR_MARGIN;
        HOLE_MIN = playerGObj.transform.localScale.y + ERROR_MARGIN;
        HOLE_MAX = worldHeight - 2*ERROR_MARGIN; 
        SEPARATION_MIN = playerGObj.transform.localScale.x*2 + ERROR_MARGIN*2;
    }

    public void PrintGateConstrines()
    {
        Debug.Log("================Constraints===============");
        Debug.Log("Wolrd Height: " + mainCam.orthographicSize * 2f);
        Debug.Log("WorldWidth: " + mainCam.orthographicSize * 2f * mainCam.aspect);
        Debug.Log("ErrorMargin: " + ERROR_MARGIN);
        Debug.Log("PlayerDiameter: " + playerGObj.transform.localScale.x);
        Debug.Log("X: " + X_MIN + ", " + X_MAX);
        Debug.Log("Y: " + Y_MIN + ", " + Y_MAX);
        Debug.Log("H: " + HOLE_MIN + ", " + HOLE_MAX);
        Debug.Log("SeparationMin: " + SEPARATION_MIN);
        Debug.Log("==========================================");
    }

    public void ClearGates()
    {
        if (gateList.Count == 0)
        {
            GameObject[] gatesAr = GameObject.FindGameObjectsWithTag("Gate");
            foreach (GameObject g in gatesAr )
            {
                Destroy(g);
            }
        }
        else
        {
            
            foreach (GateClass g in gateList)
            {
                Destroy(g.gameObject);
            }
        }
        gateList = new List<GateClass>();
        gateIDs = new List<int>();
        spawner.SetGateList(new List<GateClass>());
    }


    // [X, Y, Height, ID]
    private GateFailCode CheckGateData(float[] gateData)
    {
        GateFailCode state = GateFailCode.None;
        bool checkFail;
        string failMsg = "Gate failed, code: ";
        if (gateData.Length != 4)
        {
            state = GateFailCode.Formatting;
            failMsg += state + ", incorrect gateData not 4 float Array";
        }else if (gateData[0] < X_MIN)
        {
            state = GateFailCode.Xmin;
            failMsg += state + ", Gate X [" + gateData[1] + "] too small, min size [" + X_MIN + "]";
        }
        else if (gateData[0] > X_MAX)
        {
            state = GateFailCode.Xmax;
            failMsg += state + ", Gate X [" + gateData[1] + "] too big, max size [" + X_MAX + "]";
        }
        else if (gateData[1] < Y_MIN)
        {
            state = GateFailCode.Ymax;
            failMsg += state + "Gate Y [" + gateData[1] + "] too small, min size [" + Y_MIN + "]";
        }
        else if (gateData[1] > Y_MAX)
        {
            state = GateFailCode.Ymax;
            failMsg += state + ", Gate Y [" + gateData[1] + "] too big, max size [" + Y_MAX + "]";
        }
        else if (gateData[2] < HOLE_MIN)
        {
            state = GateFailCode.HoleMin;
            failMsg += state + ", Gate Height [" + gateData[2] + "] too small, min size [" + HOLE_MIN + "]";
        }
        else if (gateData[2] > HOLE_MAX)
        {
            state = GateFailCode.HoleMax;
            failMsg += state + ", Gate Height ["+ gateData[2] + "] too big, max size [" + HOLE_MAX + "]" ;
        }
        else if (!CheckGateSeparation(gateData))
        {
            state = GateFailCode.Separation_min;
            failMsg += state + ", Gate too close to other gates";
        }
        else if (!CheckUniqueId(gateData[3]))
        {
            state = GateFailCode.IdNotUnique;
            failMsg += state + ", Id number ["+ FloatIdToInt(gateData[3]) + "] already exists";
        }

        checkFail = (state != GateFailCode.None);
        if (checkFail)
        {
            Debug.LogError(failMsg);
        }

        return state;
    }


    private GateFailCode CheckGateAfterSpawn(GateClass gate, float[] gateData) 
    {
        GateFailCode state = GateFailCode.None;
        int intID = FloatIdToInt(gateData[3]);
        if (!gate.CheckGateY())
        {
            state = GateFailCode.GateCheckMidYOnScreen;
            Debug.LogError("Gate failed code: " + state);
        }
        else if (gate.Id != intID)
        {
            state = GateFailCode.IdDoesNotMatchPos;
            Debug.LogError("Gate failed code: " + state + ", ID load:" + intID +", Given: " + gate.Id);
        }

        return state;
    }

    private bool CheckUniqueId(float idCurr)
    {
        bool pass = true;
        int id = FloatIdToInt(idCurr);
        if (gateList.Count > 0 && gateIDs.Count > 0)
        {
            if (gateIDs.Contains(id))
            {
                pass = false;
                Debug.LogError("Gate failed: Gate ID number [ " + idCurr + " ] already exists");
            }
        }


        return pass;
    }

    private bool CheckGateSeparation(float[] gateData)
    {
        bool pass = true;
        bool farEnoughApart;
        float dist = 0;
        Vector2 p1 = new Vector2(0,0);
        Vector2 p2 = new Vector2(0, 0);
        if (gateList.Count > 0)
        {
            foreach (GateClass gateC in gateList)
            {
                p1.x = gateData[0];
                p2.x = gateC.GetMidPos().x;
                dist = Vector2.Distance(p1, p2);
                dist = (float)System.Math.Round(dist, 3);
                farEnoughApart = dist >= SEPARATION_MIN;
                if (!farEnoughApart)
                {
                    pass = false;
                    Debug.LogError("Gate failed: Gates " + gateData[3] + " and " + gateC.Id + " are too close must be " + SEPARATION_MIN  + " appart, X coords " + p1.x + " & " + p2.x + ", distance: " + dist);
                }
            }
        }
        return pass;
    }

    public GameObject SpawnGate(float[] gateData)
    {
        GameObject tempGObj = null;
        GateClass tempGC = null;
        if(CheckGateData(gateData) == GateFailCode.None){
            tempGObj = spawner.SpawnGate(new Vector2(gateData[0],gateData[1]), gateData[2]);
            tempGC = tempGObj.GetComponent<GateClass>();

            if (CheckGateAfterSpawn(tempGC, gateData) == GateFailCode.None)
            {
                gateList.Add(tempGC);
                gateIDs.Add(tempGC.Id); 
            }
            else
            {
                spawner.gateList.Remove(tempGC);
                Destroy(tempGObj);
                tempGObj = null;
                tempGC = null;
                
            }
        }
        return tempGObj;
    }

    public void SpawnLevel(List<float[]> lvData)
    {
        int count = 0;
        GameObject tempGObj;
        ClearGates();
        //PrintLvData(lvData);
        if (lvData.Count> 0)
        {
            foreach (float[] gateData in lvData)
            {
                tempGObj = SpawnGate(gateData);
                
                if (tempGObj == null)
                {
                    Debug.LogError("Gate count " + count + " failed");
                }
                else
                {
                    count++;
                }

            }
        }
        else
        {
            Debug.LogError("Level Data is empty");
        }
    }

    //used to have consistent conversition from float storage to int storage
    private int FloatIdToInt(float id)
    {
        return (int)System.Math.Floor(id);
    }

    public string GateDataToStr(float[] gateData)
    {
        string str = "pos (" + gateData[0] + "," + gateData[1] + "), H: " + gateData[2] + ", ID: " + FloatIdToInt(gateData[3]);
        return str;
    }

    public void PrintLvData(List<float[]> lvData)
    {
        foreach (float[] gateData in lvData)
        {
            Debug.Log(GateDataToStr(gateData));
        }
    }


    public List<float[]> GetTestLv()
    {
        List<float[]> lvData = new List<float[]>();
        float[] gateData = new float[] { 0, 0, 0, 0 };
        gateData = new float[] {-7, 0, 2, 0};
        lvData.Add(gateData);
        gateData = new float[] { -6, 2.5f, 1.5f, 1 };
        lvData.Add(gateData);
        gateData = new float[] { -4, -2.5f, 3f, 2 };
        lvData.Add(gateData);
        gateData = new float[] { -3, 0f, 4f, 3 };
        lvData.Add(gateData);

        return lvData;
    }

}
