using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesText : MonoBehaviour
{
    public Image myImage;
    public static RulesText instance;
    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        myImage.enabled = false;
        GetComponentInChildren<Text>().enabled = false;
    }

    public void EnableRules()
    {
        myImage.enabled = true;
        GetComponentInChildren<Text>().enabled = true;
    }

    public void DisableRules()
    {
        myImage.enabled = false;
        GetComponentInChildren<Text>().enabled = false;
    }
}
