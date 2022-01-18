using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SRulesText : MonoBehaviour
{
    public Image myImage;
    public static SRulesText instance;
    bool _rules = false;

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
        _rules = true;
    }

    public void DisableRules()
    {
        myImage.enabled = false;
        GetComponentInChildren<Text>().enabled = false;
        _rules = false;
    }

    public bool RulesEnabled()
    {
        return _rules;
    }
}