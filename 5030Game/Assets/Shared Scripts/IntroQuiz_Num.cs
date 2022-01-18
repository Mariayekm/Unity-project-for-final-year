using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroQuiz_Num : MonoBehaviour
{
    [SerializeField] private GameObject txtQuestionGObj;
    [SerializeField] private GameObject textBoxGObj;
    private TMP_InputField inputField;
    private int inputNum;
    private bool isValid;
    

    // Start is called before the first frame update
    private void Start()
    {
        inputField = textBoxGObj.GetComponent<TMP_InputField>();
        isValid = false;
        inputNum = -1;
    }
    public TMPro.TextMeshProUGUI GetQTextRef() { return txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>(); }
    public void SetQText(string txt) { txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>().text = txt; }
    public bool GetIsValid() { return isValid; }
    public int GetInputNum() { return inputNum; }

    public void SetInputNum()
    {
        int tempNum = System.Int32.Parse(inputField.text);
        if (inputField.text.Length > 0)
        {
            isValid = true;
            inputNum = tempNum;
        }
    }


}
