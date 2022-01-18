using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*Comments
    Gates will not render in game when z is less than 350 in the inspector (mainCam z = -10), gates will render fine when set z=0 programatically however 
    
    scale y = 10 is the height of camera
    cale z ~= 17.7 is width of camera

*/
public class GateClass : MonoBehaviour
{
    [SerializeField] public int Id;
    [SerializeField] private GameObject top, mid, bot;
    private Camera camMain;
    private GameObject wrapperGO;
    [SerializeField] private Vector2 midPos;
    [SerializeField] private float gateHeight;
    [SerializeField] private bool usePreset;
    [SerializeField] private float defualtWallThickness;

    private const float MINGATERROR = 0.2f;
    public bool complete;
    private bool isEnabled;

    public GateClass(Vector2 pos, float holeHeight, int ID)
    {
        Id = ID;
        SetGate(pos, holeHeight);
    }
   

    private void Awake()
    {
        camMain = Camera.main;                      //reference to main camera
        transform.localScale = new Vector3(1,1,1);
        complete = false;
        isEnabled = false;

    }

    private void Start()
    {
/*
        if (usePreset)
        {
            SetGate(midPos, gateHeight);
        }
*/
    }

    public void SetId(int i) { Id = i; }
    public int GetId() { return Id; }
    public Vector2 GetMidPos() { return midPos; }
    public float GetGateHeight() { return gateHeight; }

    public float[] GetGateDataArr() { return new float[] { midPos.x, midPos.y, gateHeight, (float)Id }; }
    public string GetGateDataStr()
    {
        string str = "pos (" + midPos.x + ", " + midPos.y + "), H: " + gateHeight + ", ID: " + Id;
        return str;
    }

    public Vector2 GetPos() { return new Vector2(transform.localPosition.x, transform.localPosition.y); }

    //Sets the gate position 
    public void SetGate(Vector2 pos, float holeHeight)
    {
        midPos = pos;
        gateHeight = holeHeight;
        transform.position = new Vector3(pos.x,pos.y, 0);
        holeHeight = CheckGateHeight(holeHeight);
        SetMid(pos, holeHeight);
        SetTop();
        SetBot();
        CheckGateY();
        complete = false;
        isEnabled = false;
        SetEnabled(isEnabled);
        //Debug.Log("GC-"+Id + "-parent: " + transform.position + ", Mid: " + mid.transform.localPosition + ", Top: " + top.transform.localPosition + ", Bot: " + bot.transform.localPosition + ", gate Height: " +  mid.transform.localScale.y);
    }

    //Set mid block trigger position
    private void SetMid(Vector2 pos, float Height)
    {
        mid.transform.localScale = new Vector3(0.2f, Height, 1f);
        mid.transform.localPosition = new Vector3(0, 0, 0);

    }

    //Set top block position
    private void SetTop()
    {
        float camScaleHeight = 2f * camMain.orthographicSize;
        Vector3 scale = new Vector3(defualtWallThickness, camScaleHeight,0);
        top.transform.localScale = scale;
        Vector3 pos = new Vector3(mid.transform.localPosition.x, (mid.transform.localPosition.y + mid.transform.localScale.y/2 + scale.y/2), 0);
        top.transform.localPosition = pos;
    }

    private void SetBot()
    {
        float camScaleHeight = 2f * camMain.orthographicSize;
        Vector3 scale = new Vector3(defualtWallThickness, camScaleHeight, 0);
        bot.transform.localScale = scale;
        Vector3 pos = new Vector3(mid.transform.localPosition.x, -top.transform.localPosition.y, 0);
        bot.transform.localPosition = pos;
    }

    //Checks gate size is within range and retuns a value within range if not
    public float CheckGateHeight(float height)
    {
        float ph = GameObject.Find("Player").transform.localScale.y;    //size of player, player is circle so only 1 dimsention needed
        float minSize = ph + MINGATERROR;                               //size of player + error margin
        float maxSize = camMain.orthographicSize * 2f;                  //camera height
        float output;
        if (height >= minSize)
        {
            output = height;
            if (height > maxSize) { output = maxSize; }

        } else { output = minSize; }; 

        return output;
    }

    //Checks that the entirety of the middle section is on screen
    public bool CheckGateY()
    {
        bool pass;
        float camTop = Camera.main.orthographicSize; 
        
        BoxCollider2D collider = mid.GetComponent<BoxCollider2D>();
        float midTop = mid.transform.position.y + mid.transform.localScale.y/2;
        float midBot = mid.transform.position.y - mid.transform.localScale.y/2;
        if (midTop >= camTop || midBot <= -camTop)
        {
            pass = false;
            Debug.LogError("GateMid out of bounds");
        }
        else
        {
            pass = true;
        }

        return pass;
    }
    
    
    public void SetGateOnOff(bool On)
    {
        //Sets mid to be invisible and disables the collider
        SpriteRenderer sp = mid.GetComponent<SpriteRenderer>();
        Color tempColor = sp.color;
        if (On)
        {
            tempColor.a = 1f;
            sp.color = tempColor;
            mid.GetComponent<Collider2D>().enabled = true;
            complete = false;
        }
        else
        {
            tempColor.a = 0f;
            sp.color = tempColor;
            mid.GetComponent<Collider2D>().enabled = false;
            complete = true;
        }

    }

    public bool GetIsEnabled() { return isEnabled; }

    public void SetEnabled(bool enable)
    {
        Collider2D collider = mid.GetComponent<Collider2D>();
        isEnabled = enable;
        collider.enabled = enable;
    }

    public GameObject GetGateMidGObj() { return mid; }
    public GameObject GetGateTopGObj() { return top; }
    public GameObject GetGateBotGObj() { return bot; }
    public Vector2 GetMidTopBotPos(bool top)
    {
        float Y ;

        if (top) { Y = mid.transform.position.y + mid.transform.localScale.y / 2; }
        else     { Y = mid.transform.position.y - mid.transform.localScale.y / 2; }

        return new Vector2(midPos.x, Y);
    }
    public Vector3 GetMidScale() { return mid.transform.localScale; }

}
