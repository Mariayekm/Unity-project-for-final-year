using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimumDistances : MonoBehaviour
{
    //public static MinimumDistances instance;
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

    public void AddMininumDistances(float m, Vector2 d)
    {// Update is called once per frame
        int lvl= LevelController.instance.GetGridIndex();

        DataSet_Sensory dataSet = new DataSet_Sensory();
        dataSet.Reset();
        dataSet.min_dist = d;
        dataSet.min_mag = m;

        csvHandler.AddToSensoryBuffer(dataSet, lvl);
    }
}
