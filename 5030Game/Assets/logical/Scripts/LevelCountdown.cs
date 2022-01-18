using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCountdown : MonoBehaviour
{
    public Text Countdown;
    int _count = 4;
    // Start is called before the first frame update
    void Start()
    {
        Countdown.text = "4";
        StartCoroutine(Count());
    }

    // Update is called once per frame
    private IEnumerator Count()
    {
        while (_count > 0)
        {
            yield return new WaitForSeconds(1);
            _count--;
            if (_count > 0)
            {
                string str = _count.ToString();
                Countdown.text = str;
            }
        }
        //LevelController.instance.LoadNextLevel();
    }
}
