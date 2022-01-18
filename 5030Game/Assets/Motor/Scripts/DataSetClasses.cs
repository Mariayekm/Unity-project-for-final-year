using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DataSet
{
    public DataSet() { }//Initialise values
    private const string TIMESPANFORMATSTR = "mm':'ss'.'ffff";
    public abstract void Reset();//reset values
    public abstract string[] ToStrArray();//convert data to string array
    public abstract string ToStrCSV();//convert data to CSV formatted string
    public abstract string ToJson();//convert data to Json formatted string

}

[System.Serializable]
public class DataSet_Motor :DataSet
{
    private const string TIMESPANFORMATSTR = "mm':'ss'.'ffff";
    public enum CollisionDataType
    {
        Error = -1,
        None = 0,
        gateEnter = 1,
        gateExit = 2,
        wallEnter = 3,
        wallExit = 4,
        triggerEnter = 5,
        triggerExit = 6
    }


    //Timespan variables cannot be serialized but can be reconstructed from seconds float count
    public System.TimeSpan timeT;   
    public float timeS;
    public float Mx;
    public float My;
    public float Px;
    public float Py;
    public float Cx;
    public float Cy;
    public CollisionDataType Cdt;
    public System.TimeSpan CtimeT;
    public float CtimeS;
    public float Gx;
    public float Gy;
    public float Gnum;
    public float PMDist;

    public DataSet_Motor()
    {
        timeT = new System.TimeSpan();      // Total time
        timeS = 0f;                         // Total time in seconds
        Mx = 0f;                            // Mouse X coord
        My = 0f;                            // Mouse Y coord
        Px = 0f;                            // Player X coord
        Py = 0f;                            // Player Y coord 
        Cx = 0f;                            // Collsion X coord
        Cy = 0f;                            // Collsion Y coord
        Cdt = CollisionDataType.None;       // Collsioin enter/exit/none
        CtimeT = new System.TimeSpan();     // Total time of collsion
        CtimeS = 0f;                        // Total time of collsion in seconds
        Gx = 0f;                            // Gate trigger x coord
        Gy = 0f;                            // Gate trigger y coord
        Gnum = 0f;                          // Gate number
        PMDist = 0f;                        // Player Mouse Distance
    }

    public override void Reset()
    {
        timeT = new System.TimeSpan();
        timeS = 0f;
        Mx = 0f;
        My = 0f;
        Px = 0f;
        Py = 0f;
        Cx = 0f;
        Cy = 0f;
        Cdt = CollisionDataType.None;
        CtimeT = new System.TimeSpan();
        CtimeS = 0f;
        Gx = 0f;
        Gy = 0f;
        Gnum = 0f;
    }

    public override string[] ToStrArray()
    {
        return new string[] { timeT.ToString(TIMESPANFORMATSTR), timeS.ToString(), Mx.ToString(), My.ToString(), Px.ToString(), Py.ToString(), Cx.ToString(), Cy.ToString(), Cdt.ToString(), CtimeT.ToString(TIMESPANFORMATSTR), CtimeS.ToString(), Gx.ToString(), Gy.ToString(), Gnum.ToString(), PMDist.ToString() };
    }

    public override string ToStrCSV()
    {
    string str;
    string[] set = ToStrArray();
    str = set[0];
    for (int i = 1; i < set.Length; i++)
    {
        str += "," + set[i];
    }
    return str;
    }
    
    public override string ToJson()
    {
    return JsonUtility.ToJson(this);
    }

    public string GetTIMESPANFORMATSTR()
    {
        return TIMESPANFORMATSTR;
    }

    //Converts the float times in secondes (timeS, CtimeS) to TimeSpan and writes them to the timespan variables (timeT, CtimeT)
    public void UpdateTs()
    {
        timeT = System.TimeSpan.FromSeconds(timeS);
        CtimeT = System.TimeSpan.FromSeconds(CtimeS);
    }
    public void UpdatePMDist()
    {
        Vector2 mPos = new Vector2(Mx, My);
        Vector2 pPos = new Vector2(Px, Py);
        PMDist = Vector2.Distance(mPos, pPos);
    }

    private string MakeDebugPrintStr()
    {
        string dp = "n3";
        return timeT.ToString(TIMESPANFORMATSTR) + ", " + timeS.ToString(dp) + ", Mpos(" + Mx.ToString(dp) + "," + My.ToString(dp) + "), Ppos(" + Px.ToString(dp) + "," + Py.ToString(dp) + ")" + ", Cpos(" + Cx.ToString(dp) + "," + Cy.ToString(dp) + ")" + ", Cdt:" + Cdt.ToString() + ", " + "Ctime: " + CtimeT.ToString(TIMESPANFORMATSTR) + ", " + CtimeS.ToString() + ", Gpos(" + Gx.ToString(dp) + "," + Gy.ToString(dp) + "), Gnum:" + Gnum.ToString() + ", PMDist: " + PMDist.ToString(dp);
    }
    public void DebugPrint()
    {
        string str = MakeDebugPrintStr();
        Debug.Log(str);
    }

    public string GetDebugPrintStr()
    {
        return MakeDebugPrintStr();
    }
}

[System.Serializable]
public class DataSet_Logic : DataSet
{
    public string temp;
    public int moves; 
    public int correct_tiles;
    public float time;
    public float split_time; //array

    //Initialise values
    public DataSet_Logic()
    {
        moves = 0;
        correct_tiles = 0;
        time = 0f;
        split_time = 0f;
    }
    public override void Reset()
    {
        moves = 0;
        correct_tiles = 0;
        time = 0f;
        split_time = 0f;
    }
    public override string[] ToStrArray()
    {
        return new string[] {  moves.ToString(), correct_tiles.ToString(), time.ToString(), split_time.ToString() };
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson()
    {
        string str = "";
        return str;
    }
}

[System.Serializable]
public class DataSet_Sensory : DataSet
{
    public string temp;
    public int correct_squares;
    public Vector2 min_dist; //array
    public float min_mag; //array
    //Initialise values
    public DataSet_Sensory()
    {
        correct_squares = 0;
        min_dist = new Vector2(0, 0);
        min_mag = 0f;
    }
    public override void Reset()
    {
        correct_squares = 0;
        min_dist = new Vector2(0, 0);
        min_mag = 0f;
    }
    public override string[] ToStrArray()
    {
        return new string[] { correct_squares.ToString(), min_dist.ToString(), min_mag.ToString() };
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson()
    {
        string str = "";
        return str;
    }
}

[System.Serializable]
public class DataSet_IntroQuiz : DataSet
{
    public int age;
    public string gender;
    public string gExp;
    public string mHardware;

    //Initialise values
    public DataSet_IntroQuiz()
    {
        age = 0;
        gender = "";
        gExp = "";
        mHardware = "";
    }
    public override void Reset()
    {
        age = 0;
        gender = "";
        gExp = "";
        mHardware = "";
    }
    public override string[] ToStrArray()
    {
        return new string[] { age.ToString(), gender, gExp, mHardware };
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson()
    {
        string str = "";
        return str;
    }
}

[System.Serializable]
public class DataSet_DiffQuiz : DataSet
{
    public int ans;

    //Initialise values
    public DataSet_DiffQuiz()
    {
        ans = -1;
    }
    public override void Reset()
    {
        ans = -1;
    }
    public override string[] ToStrArray()
    {
        return new string[] {ans.ToString()};
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson()
    {
        string str = "";
        return str;
    }
}
#region Logic and sensory skeleton dataset classes
/*[System.Serializable]
public class DataSet_Logic:DataSet
{
    public string temp;

    //Initialise values
    public DataSet_Logic() { 
    
    }
    public override void Reset() { 

    }
    public override string[] ToStrArray()
    {
        return new string[3];
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson() {
        string str = "";
        return str;
    }
}*/


/*[System.Serializable]
public class DataSet_Sensory : DataSet
{
    public string temp;

    //Initialise values
    public DataSet_Sensory ()
    {

    }
    public override void Reset()
    {

    }
    public override string[] ToStrArray()
    {
        return new string[3];
    }
    public override string ToStrCSV()
    {
        string str;
        string[] set = ToStrArray();
        str = set[0];
        for (int i = 1; i < set.Length; i++)
        {
            str += "," + set[i];
        }
        return str;
    }
    public override string ToJson()
    {
        string str = "";
        return str;
    }
}
*/
#endregion