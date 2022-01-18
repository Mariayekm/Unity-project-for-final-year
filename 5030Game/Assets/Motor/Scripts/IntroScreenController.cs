using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreenController : MonoBehaviour
{
    
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadMotorGame()
    {
        SceneManager.LoadScene("GameMotorScene");
    }

}
