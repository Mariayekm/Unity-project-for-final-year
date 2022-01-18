using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LeveIndicator : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject textBackground;
    [SerializeField] private GameObject imgGObj;
    [SerializeField] private GameObject txt_TitleGOobj;
    [SerializeField] private GameObject txt_MsgGOobj;
    [SerializeField] private GameObject txt_PrizeMsgGObj;
    [SerializeField] private GameObject btn_continueGOobj;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject gameMotorHandlerGObj;
    [Header("Images")]
    [SerializeField] private Sprite imgSpeed;
    [SerializeField] private Sprite imgPrecision;
    [SerializeField] private Sprite imgPractice;
     

    private Image imgIcon;
    private TMPro.TextMeshProUGUI txt_Title, txt_Msg, txt_PMsg,txtbtn_continue;
    private GameMotorHandler gameMotorHandler;

    private readonly string PRACTICE_TITLE = "Practice Level";
    private readonly string PRACTICE_MSG = "Click pause in the top right to see the rules";
    private readonly string SPEED_TITLE = "Speed Level";
    //private readonly string SPEED_MSG = "Aim for the fastest time" + System.Environment.NewLine + " on these Levels";
    private readonly string SPEED_MSG = "Aim for the fastest time possible on these levels";
    private readonly string SPEED_PRIZEMSG = "£20 voucher to overall fastest time";
    private readonly string PRECISION_TITLE = "Precision Level";
   // private readonly string PRECISION_MSG = "Aim to not hit any walls" + System.Environment.NewLine + "on these Levels";
    private readonly string PRECISION_MSG = "Aim to not hit any walls on these levels";
    private readonly string PRECISION_PRIZEMSG = "£20 voucher to least total collisions";
    private bool isShowing;
    
    private void Awake()    
    {
        imgIcon = imgGObj.GetComponent<Image>();
        txt_Title = txt_TitleGOobj.GetComponent<TMPro.TextMeshProUGUI>();
        txt_Msg = txt_MsgGOobj.GetComponent<TMPro.TextMeshProUGUI>();
        txt_PMsg = txt_PrizeMsgGObj.GetComponent<TMPro.TextMeshProUGUI>();
        txtbtn_continue = btn_continueGOobj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        gameMotorHandler = gameMotorHandlerGObj.GetComponent<GameMotorHandler>();
    }

    private void SetSpeedLv()
    {
        txt_Title.text = SPEED_TITLE;
        txt_Title.fontSize = 70f;
        txt_Msg.text = SPEED_MSG;
        txt_PMsg.fontSize = 30f;
        txt_PMsg.text = SPEED_PRIZEMSG;
        imgIcon.sprite = imgSpeed;
        
    }
    private void SetPrecisionLv()
    {
        txt_Title.text = PRECISION_TITLE;
        txt_Title.fontSize = 55f;
        txt_Msg.text = PRECISION_MSG;
        txt_PMsg.fontSize = 30f;
        txt_PMsg.text = PRECISION_PRIZEMSG;
        imgIcon.sprite = imgPrecision;
    }

    private void SetPracticeLv()
    {
        txt_Title.text = PRACTICE_TITLE;
        txt_Title.fontSize = 50f;
        txt_Msg.text = PRACTICE_MSG;
        txt_PMsg.text = "";
        imgIcon.sprite = imgPractice;
    }


    public void On(int type)
    {
        if(type == 1)
        {
            SetPracticeLv();
        }
        else if (type == 2) 
        { 
            SetSpeedLv(); 
        } 
        else if(type == 3) { 
            SetPrecisionLv(); 
        }
        if (type != 0) {
            background.SetActive(true);
            gameObject.SetActive(true);
            isShowing = true;
            Time.timeScale = 0f;
        }
        else
        {
            gameMotorHandler.CloseLvIndicator();
        }
    }

    public void Off()
    {
        background.SetActive(false);
        gameObject.SetActive(false);
        gameMotorHandler.ClearCollisionCrosses();
        isShowing = false;
        Time.timeScale = 1f;
    }

    public bool GetIsShowing() { return isShowing; }
}
