using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class QuesionaireHandler2 : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject btn0;
    [SerializeField] private GameObject btn1;
    [SerializeField] private GameObject btn2;
    [SerializeField] private GameObject btn3;
    [SerializeField] private GameObject btn4;
    [SerializeField] private GameObject btn5;
    [SerializeField] private GameObject btn6;

    [Header("Text")]
    [SerializeField] private GameObject txt0;
    [SerializeField] private GameObject txt1;
    [SerializeField] private GameObject txt2;
    [SerializeField] private GameObject txt3;
    [SerializeField] private GameObject txt4;
    [SerializeField] private GameObject txt5;
    [SerializeField] private GameObject txt6;
    [SerializeField] private GameObject txtQuestion;

    private GameObject[] btnList;
    private GameObject[] txtList;
    private int currentBtnSelected;
    private void Awake()
    {
        btnList = new GameObject[] { btn0, btn1, btn2, btn3, btn4, btn5, btn6 };
        txtList = new GameObject[] { txt0, txt1, txt2, txt3, txt4, txt5, txt6 };
    }
    private void Start()
    {
        foreach (GameObject btn in btnList)
        {
            btn.SetActive(true);
        }
    }

    private void EnableButtons()
    {
        
    }
}
