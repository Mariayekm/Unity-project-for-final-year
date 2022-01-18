using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroQuiz_DropDown : MonoBehaviour
{
    [SerializeField] private GameObject txtQuestionGObj;
    [SerializeField] private GameObject dropDownGObj;
    private TMPro.TMP_Dropdown dropDown;
    private bool isValid;
    private string selectedStr;
    private int selectedIndex;

    private void Start()
    {
        dropDown = dropDownGObj.GetComponent<TMP_Dropdown>();
        isValid = false;
        selectedStr = "";
        selectedIndex = 0;
    }
    public void SetQText(string txt) { txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>().text = txt; }
    public TMPro.TextMeshProUGUI GetQTextRef() { return txtQuestionGObj.GetComponent<TMPro.TextMeshProUGUI>(); }

    public bool GetIsValid() { return isValid; }
    public int GetSelectedIndex() { return selectedIndex; }
    public string GetSelectedStr() { return selectedStr; }
    public void SetSelectedItem()
    {
        
        if (dropDown.value != 0)
        {
            isValid = true;
            selectedStr = dropDown.options[dropDown.value].text;
            selectedIndex = dropDown.value;
        }
        else
        {
            isValid = false;
            selectedStr = "";
            selectedIndex = 0;
        }
    }


   



    

}
