using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeScreenController : MonoBehaviour
{
    public void LoadConsent() { SceneManager.LoadScene("ConsentForm");  }

    public void Quit() { Application.Quit(); }

}
