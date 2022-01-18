using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleFormHandler : MonoBehaviour
{
    private string googleFormURL = "https://docs.google.com/forms/u/3/d/e/1FAIpQLSdkDyPwWxVJIfRswVGv0B9PgVvZz9S4JRIzvU3exWa8gvLlBg/formResponse";
    //Entry codes, question Id numbers
    private string ec_idName = "entry.407085484";
    private string ec_num = "entry.1330918453";
    //private string ec_enterPrizeD = "entry.1699647149_sentinel";
    private string ec_enterPrizeD = "entry.1699647149";
    private string ec_email = "entry.484739465";

    public void TestPost()
    {
        StartCoroutine(PostToForm("Testing", "00000000000000000", true, "testing@gmail.com"));
    }

    public void PostAns(string idName, string idNum, bool enterPrizeD, string email = "")
    {
        StartCoroutine(PostToForm(idName, idNum, enterPrizeD, email));
    }

    IEnumerator PostToForm(string idName, string idNum,bool enterPrizeD, string email = "")
    {
        Debug.Log("Posting Form - Uname: " + idName + ", id: " + idNum + ", enterPD: " + enterPrizeD + ", email: " + email );
        string emailStr = "";
        string enterPrizeDStr = "";
        // Check if entering prize draw
        if (enterPrizeD)
        {
            enterPrizeDStr = "Yes";
            emailStr = email;
        }
        else
        {
            enterPrizeDStr = "No";
        }
        // Create Form
        WWWForm form = new WWWForm();
        form.AddField(ec_idName, idName);
        form.AddField(ec_num, idNum);
        form.AddField(ec_enterPrizeD, enterPrizeDStr);
        form.AddField(ec_email, emailStr);
        //Post form
        UnityEngine.Networking.UnityWebRequest www = UnityWebRequest.Post(googleFormURL, form);

        yield return www.SendWebRequest();
    }








}
