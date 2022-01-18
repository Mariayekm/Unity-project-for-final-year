using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroQuiz_Text : MonoBehaviour
{
    [SerializeField] private GameObject txtQuestionGObj;
    [SerializeField] private GameObject textBoxGObj;
    private TMP_InputField inputField;
    private string inputText;
    private bool isValid;
    private const int MIN_STRLEN = 1;

    // Start is called before the first frame update
    void Start()
    {
        inputField = textBoxGObj.GetComponent<TMP_InputField>();
        isValid = false;
        inputText = "";

    }
    public TMPro.TextMeshProUGUI GetQTextRef() { return txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>(); }
    public void SetQText(string txt) { txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>().text = txt; }
    public bool GetIsValid() { return isValid; }
    public string GetInputText() { return inputText; }

    public void SetInputText()
    {
        string tempStr = inputField.text;
        if (tempStr.Length >= MIN_STRLEN)
        {
            isValid = true;
            inputText = tempStr;
        }
        else
        {
            isValid = false;
            inputText = "";
        }
    }

}
