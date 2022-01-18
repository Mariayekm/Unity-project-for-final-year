using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] GameObject spawnerObj;
    public DataSet_Motor datasetM;
    private Spawner spawner;
    [SerializeField] GameObject gateGObj;
    private GateClass gateC;
    private List<GameObject> contactTimerGObjs;
    // Start is called before the first frame update
    private void Start()
    {
        datasetM = new DataSet_Motor();
        spawner = spawnerObj.GetComponent<Spawner>();
        contactTimerGObjs = new List<GameObject>();
        gateC = gateGObj.GetComponent<GateClass>();
    }

    // Update is called once per frame
    private void Update()
    {

    }
    //used for debugging keyboard problems
    public void DetectPressedKeyOrButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
    }

    public void PrintTest()
    {
        Debug.Log("==================   Testing   ======================");
    }
}
