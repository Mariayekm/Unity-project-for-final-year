using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConsentFormCont : MonoBehaviour
{
    [SerializeField] private GameObject scrollViewGObj;
    [SerializeField] private GameObject btnConsentGObj;
    private Scrollbar scrollBar;
    private Button btnConsent;
    private bool isFormRead;


    private const float MIN_SCROLL_VAL = 0.1f;//0 = bottom, 1 = top
    // Start is called before the first frame update
    void Start()
    {
        scrollViewGObj.GetComponentInChildren<Mask>().enabled = true;
        scrollBar = scrollViewGObj.GetComponentInChildren<Scrollbar>();
        btnConsent = btnConsentGObj.GetComponent<Button>();
        SetButtonEnable(false);
        isFormRead = false;
    }
        
    //Checks if the player has been to the bottom of 
    public void CheckAtBottomScroll()
    {
        if (scrollBar.value <= MIN_SCROLL_VAL) { isFormRead = true; SetButtonEnable(true); }
    }

    private void SetButtonEnable(bool enable)
    {
        TMPro.TextMeshProUGUI txt_btn = btnConsentGObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (enable)
        {
            txt_btn.text = "I Consent";
            txt_btn.fontSize = 50;
        }
        else
        {
            txt_btn.text = "Read form to progress";
            txt_btn.fontSize = 36;
        }
        btnConsent.enabled = enable;
        btnConsent.interactable = enable;
        
    }

    public void LoadIntroQuestionnaire()
    {
        SceneManager.LoadScene("IntroQuestionaire");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
