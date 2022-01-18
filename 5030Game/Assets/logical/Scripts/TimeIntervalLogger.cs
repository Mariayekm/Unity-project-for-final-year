using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIntervalLogger : MonoBehaviour
{
    //public static TimeIntervalLogger instance;
    private CSVHandler csvHandler;

    [SerializeField] private GameObject CSVHandlerGObj;

    private void Awake()
    {
        CSVHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
    }
        // Start is called before the first frame update
        void Start()
    {

    }

    public void AddTimeInterval(float t)
    {// Update is called once per frame
        int s = LevelController.instance.GetShuffleIndex();

        DataSet_Logic dataSet = new DataSet_Logic();
        dataSet.Reset();
        dataSet.split_time = t;

        csvHandler.AddToLogicBuffer(dataSet, s);
    }
}
