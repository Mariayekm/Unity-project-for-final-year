using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Congratulations : MonoBehaviour
{
    public static Congratulations instance;
    public Text myText;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        myText.enabled = false;
    }

    public void EnableText()
    {
        myText.enabled = true;
    }

}
