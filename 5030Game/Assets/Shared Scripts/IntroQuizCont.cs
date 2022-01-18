using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net.Mail;

public class IntroQuizCont : MonoBehaviour
{
    [SerializeField] private GameObject q_usernameGObj;
    [SerializeField] private GameObject q_emailGObj;
    [SerializeField] private GameObject q_ageGObj;
    [SerializeField] private GameObject q_genderGObj;
    [SerializeField] private GameObject q_gamingExpGObj;
    [SerializeField] private GameObject q_mouseHardwareGObj;
    [SerializeField] private GameObject txtAnsErrorGObj;
    [SerializeField] private GameObject googlFormHandlerGObj;
    private GameObject csvHandlerGObj;
    private CSVHandler csvhandler;
    private GoogleFormHandler googleFormHandler;
    private TMPro.TextMeshProUGUI txtAnsError;
    private List<bool> isValid;
    private List<string> errorMsgs;
    private bool emailEmpty;
    

    private const int MIN_AGE = 18;

    void Start()
    {
        isValid = new List<bool>();
        errorMsgs = new List<string>();
        txtAnsError = txtAnsErrorGObj.GetComponent<TMPro.TextMeshProUGUI>();
        txtAnsError.enabled = false;
        csvHandlerGObj = GameObject.Find("CSVHandler");
        csvhandler = csvHandlerGObj.GetComponent<CSVHandler>();
        csvhandler.ResetSavedData();
        googleFormHandler = googlFormHandlerGObj.GetComponent<GoogleFormHandler>();
        emailEmpty = true;

        

    }

    public void CheckInputs()
    {
        if (CheckAllInputsExist())
        {
            if (RunAnswerChecks()) { DisplayErrors(); }
            else
            {
                string emailStr = "";
                DataSet_IntroQuiz dataSet = new DataSet_IntroQuiz();
                dataSet.Reset();
                dataSet.age = q_ageGObj.GetComponent<IntroQuiz_Num>().GetInputNum();
                dataSet.gender = q_genderGObj.GetComponent<IntroQuiz_DropDown>().GetSelectedStr();
                dataSet.gExp = q_gamingExpGObj.GetComponent<IntroQuiz_DropDown>().GetSelectedStr();
                dataSet.mHardware = q_mouseHardwareGObj.GetComponent<IntroQuiz_DropDown>().GetSelectedStr();
                csvhandler.AddtoIntroBuffer(dataSet);
                csvhandler.WriteIntroQuizBuffer();

                if (!emailEmpty) { emailStr = q_emailGObj.GetComponent<IntroQuiz_Text>().GetInputText();  }
                googleFormHandler.PostAns(q_usernameGObj.GetComponent<IntroQuiz_Text>().GetInputText(), csvhandler.GetPlayerIdStr(), !emailEmpty, emailStr);

                LoadLogicWelcome();
            }
        }
        else
        {
            errorMsgs.Add("Answer all questions");
            DisplayErrors();
        }
    }
    private bool CheckAllInputsExist()
    {
        bool pass = true;
        isValid.Clear();
        isValid.Add(q_usernameGObj.GetComponent<IntroQuiz_Text>().GetIsValid());
        isValid.Add(true);                                                   //IntroQuiz_Text isValid func checks if length is non zero, but email can be zero
        isValid.Add(q_ageGObj.GetComponent<IntroQuiz_Num>().GetIsValid());
        isValid.Add(q_genderGObj.GetComponent<IntroQuiz_DropDown>().GetIsValid());
        isValid.Add(q_gamingExpGObj.GetComponent<IntroQuiz_DropDown>().GetIsValid());
        isValid.Add(q_mouseHardwareGObj.GetComponent<IntroQuiz_DropDown>().GetIsValid());

        pass = !isValid.Contains(false);

        return pass;
    }
    private bool RunAnswerChecks()
    {
        List<bool> checks = new List<bool>();
        checks.Add(CheckUsername(q_usernameGObj.GetComponent<IntroQuiz_Text>().GetInputText()));
        checks.Add(CheckEmailAddress(q_emailGObj.GetComponent<IntroQuiz_Text>().GetInputText()));
        checks.Add(CheckAgeAns(q_ageGObj.GetComponent<IntroQuiz_Num>().GetInputNum()));

        return checks.Contains(false);
    }

    // Checks if the player is over 18
    private bool CheckAgeAns(int age)
    {
        bool pass = age >= MIN_AGE;
        if (!pass) { errorMsgs.Add("Age too young"); }

        return pass;
    }

    // Checks gender text question answer mees criteria: length
    // Not used anymore as gender has been changed to a drop down question
    private bool CheckGenderAns(string gender)
    {
        bool lengthCheck = gender.Length >= 1;
        if (!lengthCheck) { errorMsgs.Add("Gender legth too short"); }
        return lengthCheck;
    }

    // checks username text question answer meets criteria: length
    // Other checks are done in the editor by setting the InputField to content type: Alphanumeric
    private bool CheckUsername(string uname)
    {
        bool lengthCheck = uname.Length >= 7;
        if (!lengthCheck) { errorMsgs.Add("Username minimum 7 characters"); }
        return lengthCheck;
    }

    // Checks that if there is no email or a valid email
    //email check taken from https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
    //email format checker needs work, meme@meme is allowed.
    //Disabled email format validation on google form as invalid emails that are allowed through in game prevent entire submission if they fail format testing on the google form
    private bool CheckEmailAddress(string email)
    {
        #region unused variables for email syntax validation
        /*
          int nameMaxLen = 64;
        int nameMinLen = 1;
        int DomainMaxLen = 253;
        int DomainMinLen = 1;
        int TopLvDomainMinLen = 2;
        bool CheckOneAt = false;
        bool checkDoubleDot = false;
        bool checkNameMinlen = false;
        bool checkNameMaxlen = false;
        bool checkDomainLenMin = false;
        bool checkDomainMaxLen = false;
        bool checkNameFirstChar = false;
        bool checkNameLastChar = false;
       */
        #endregion

        bool pass = false;
        bool empty = email == "" || email == null;
        
        if (empty)
        {
            emailEmpty = true;
            return true;
        }
        else
        {
            emailEmpty = false;
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                pass = addr.Address == email;
            }
            catch
            {
                pass = false;
            }

            if (!pass)
            {
                errorMsgs.Add("Invalid email");
            }
        }
        return pass;
    }

    private void DisplayErrors()
    {
        //Could swap out for a pop up instead of Text at bottom
        string errorStr = "";
        foreach (string str in errorMsgs) { errorStr += str + ", "; }
        txtAnsError.text = "Answer Errors: " + errorStr;
        errorMsgs.Clear();
        txtAnsError.enabled = true;
    }

    private void LoadLogicWelcome()
    {
        SceneManager.LoadScene("LogicGameWelcome");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
