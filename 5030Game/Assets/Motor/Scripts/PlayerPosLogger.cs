using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosLogger : MonoBehaviour
{

    [SerializeField] private float TimePerLog;
    private float timeCount;
    [SerializeField] private GameObject CSVHandlerGObj;
    [SerializeField] private GameObject timerGObj;
    private CSVHandler csvHandler;
    private Timer timerLv;
    private Camera mainCam;
    public bool isLogging;

    // Start is called before the first frame update
    private void Awake()
    {
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
        timerLv = timerGObj.GetComponent<Timer>();
        mainCam = Camera.main;
        isLogging = false;
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (isLogging) {
            timeCount += Time.deltaTime;
            CheckTime();
        }
    }

    private void CheckTime()
    {
        if (timeCount >= TimePerLog || TimePerLog < 0 )
        {
            DataSet_Motor dataSet = new DataSet_Motor();
            Vector2 mPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pPos = transform.position;
            dataSet.Reset();
            dataSet.timeS = timerLv.GetCurrentTime_float();
            dataSet.Mx = mPos.x;
            dataSet.My = mPos.y;
            dataSet.Px = pPos.x;
            dataSet.Py = pPos.y;
            dataSet.UpdateTs();
            dataSet.UpdatePMDist();
            //dataSet.DebugPrint();

            csvHandler.AddToMotorBuffer(dataSet);
            timeCount = 0f;
        }
    }
}
