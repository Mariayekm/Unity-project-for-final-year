using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//By Barnaby Ayriss
//Class Stores and generates Levels for GameMotorHandler
public class Levels
{
    public enum LevelType{
        Error = -1,
        None = 0,
        Practice = 1,
        Speed = 2,
        Precision = 3,
    }
    private Vector2 DEFULAT_PLAYERSTARTPOS = new Vector2(-8f, 0); 
    
    //list of gate data, last item in list is type of level and player staring position
    public List<float[]> GetLevelData(int lv)
    {
        Debug.Log("Loading Level: "+lv);
        List<float[]> lvData = new List<float[]>();
        float[] lvTypeData = new float[] { (float)LevelType.None, DEFULAT_PLAYERSTARTPOS.x, DEFULAT_PLAYERSTARTPOS.y, -1 };      //Array added holding level data, {Level Type, PLayer starting x, player starting y, impossible gate ID}
        switch (lv) {
            case 0:
                lvData = Lv0();                             // Run level function to genreate array of gate data
                lvTypeData[0] = (float)LevelType.Practice;  // convert interger level type enum to float
                break;
            case 1:
                lvData = Lv1();
                lvTypeData[0] = (float)LevelType.Precision;
                break;
            case 2:
                lvData = Lv2();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 3:
                lvData = Lv3();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 4:
                lvData = Lv4();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 5:
                lvData = Lv5();
                lvTypeData[0] = (float)LevelType.None;
                //lvTypeData[1] = -lvTypeData[1];             // Setting player staring on right side of map
                break;
            case 6:
                lvData = Lv1();
                lvTypeData[0] = (float)LevelType.Speed;
                break;
            case 7:
                lvData = Lv2();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 8:
                lvData = Lv3();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 9:
                lvData = Lv4();
                lvTypeData[0] = (float)LevelType.None;
                break;
            case 10:
                lvData = Lv5();
                lvTypeData[0] = (float)LevelType.None;
                break;
            default:
                Debug.LogError("Unknown Level Requested");
                lvData = null;
                break;
        }
        lvData.Add(lvTypeData); //Add level data to list
        return lvData;
    }

    // Converts a float to level type
    // Used for consistent float to integer conversion
    public LevelType ConvertToLevelType(float data)
    {
        LevelType lt;
        int d = (int)System.Math.Floor(data);
        switch (d)
        {
            case -1:
                lt = LevelType.Error;
                break;
            case 0:
                lt = LevelType.None;
                break;
            case 1:
                lt = LevelType.Practice;
                break;
            case 2:
                lt = LevelType.Speed;
                break;
            case 3:
                lt = LevelType.Precision;
                break;
            default:
                lt = LevelType.Error;
                Debug.LogError("Unknown Level Type");
                break;
        }

        return lt;
    }

    //Converts a 2D float array to a lst of float arrays
    private List<float[]> ArToList(float[][] dataAr)
    {
        List<float[]> lvData = new List<float[]>();

        foreach(float[] gate in dataAr)
        {
            lvData.Add(gate);
        }

        return lvData;
    }

    /*
     * Rules to level Data
     * X: -7.385 -> 7.385
     * Y: -4.8 - > 4.8
     * Gate Height: 0.7 -> 9.6
     * Separation Min: 1.4
     * 
     * {X, Y, GateHeight, ID}
     * ID must be in accending order 0->n equal to its index in the array
     * Then choose if level should be speed preision practice
     * Choose player start position, defualt (-8f, 0f)
    */
    //Level function - generates a list of gate data
    //Gate data is a float array of length 4, {X, Y, GateHeight, ID}
    private List<float[]> Lv0()
    {
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
            new float[]{-7f, 1f, 3.5f, 0f },
            new float[]{-2.5f, 3f, 1f, 1f },
            new float[]{2.5f, 0f , 2.4f ,2f },
            new float[]{7f, -1f, 2f, 3f },

        };
        lvData = ArToList(temp);
        return lvData;
    }

    private List<float[]> Lv1()
    {
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
            
            new float[]{ -7f, 1f, 4f, 0f },
            new float[]{ -5f, -1f, 4f, 1f },
            new float[]{ -3f, -1f, 5f, 2f },
            new float[]{ -1f, -0f, 2f, 3f },
            new float[]{ 1f, 1.5f, 3f, 4f },
            new float[]{ 3.5f, 1f, 2f, 5f },
            new float[]{ 5.4f, 0f, 4f, 6f },
            new float[]{ 7.385f, 1.5f, 3f, 7f }, 

        };
        lvData = ArToList(temp);
        return lvData;
    }

    private List<float[]> Lv2()
    {
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
            
            new float[]{ -7f, 2f, 4f, 0f },
            new float[]{ -5f, -2f, 4f, 1f },
            new float[]{ -3f, -2f, 5f, 2f },
            new float[]{ -1f, -0f, 2f, 3f },
            new float[]{ 1f, 3f, 3f, 4f },
            new float[]{ 3.5f, 2f, 2f, 5f },
            new float[]{ 5.4f, 0f, 4f, 6f },
            new float[]{ 7.385f, 3f, 3f, 7f },

        };
        lvData = ArToList(temp);
        return lvData;
    }

    private List<float[]> Lv3()
    {
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
            
            new float[]{ -7f, 3f, 2f, 0f },
            new float[]{ -5f, -3f, 2f, 1f },
            new float[]{ -3f, -3f, 2.5f, 2f },
            new float[]{ -1f, -2f, 1f, 3f },
            new float[]{ 1f, 3.5f, 1.5f, 4f },
            new float[]{ 3.5f, 3f, 1f, 5f },
            new float[]{ 5.4f, 0f, 2f, 6f }, 
            new float[]{ 7.385f, 3.5f, 1.5f, 7f },

        };
        lvData = ArToList(temp);
        return lvData;
    }

    //level That is an iteration of Lv3, all gates are shifted to the right by 0.1
    private List<float[]> Lv4()
    {
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
   
            new float[]{ -5.25f, 3f, 2f, 0f },
            new float[]{ -3.75f, -3f, 2f, 1f },
            new float[]{ -2.25f, -3f, 2.5f, 2f },
            new float[]{ -0.75f, -2f, 1f, 3f },
            new float[]{ 0.75f, 3.5f, 1.5f, 4f },
            new float[]{ 2.63f, 3f, 1f, 5f },
            new float[]{ 4.05f, 0f, 2f, 6f }, 
            new float[]{ 5.54f, 3.5f, 1.5f, 7f },

        };
        lvData = ArToList(temp);
        return lvData;
    }

    private List<float[]> Lv5()
    {
        //level testing starting on right side of map
        List<float[]> lvData = new List<float[]>();
        float[][] temp = new float[][]
        {
          
            new float[]{ -5.25f, 3f, 0.75f, 0f },
            new float[]{ -3.75f, -3f, 0.75f, 1f },
            new float[]{ -2.25f, -3f, 1.88f, 2f },
            new float[]{ -0.75f, -2f, 0.75f, 3f },
            new float[]{ 0.75f, 3.5f, 1.13f, 4f },
            new float[]{ 2.63f, 3f, 0.75f, 5f },
            new float[]{ 4.05f, 0f, 1.5f, 6f }, 
            new float[]{ 5.54f, 3.5f, 1.13f, 7f },

        };
        lvData = ArToList(temp);
        return lvData;
    }
    private List<float[]> Lv6()
    {
        List<float[]> lvData = new List<float[]>();



        return lvData;
    }
    private List<float[]> Lv7()
    {
        List<float[]> lvData = new List<float[]>();



        return lvData;
    }
    private List<float[]> Lv8()
    {
        List<float[]> lvData = new List<float[]>();



        return lvData;
    }
    private List<float[]> Lv9()
    {
        List<float[]> lvData = new List<float[]>();



        return lvData;
    }


    //skeleton level
    private List<float[]> Lv10()
    {
        List<float[]> lvData = new List<float[]>();
/*        float[][] temp = new float[][]
        {
            new float[]{ f, f, f, 0f },
            new float[]{ f, f, f, 1f },
            new float[]{ f, f, f, 2f },
            new float[]{ f, f, f, 3f },
            new float[]{ f, f, f, 4f },
            new float[]{ f, f, f, 5f },
            new float[]{ f, f, f, 6f },
            new float[]{ f, f, f, 7f },

        };
        lvData = ArToList(temp);*/
        return lvData;
    }
}
