using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Spawner : MonoBehaviour
{
    [Header("Gate spawning")]
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallsPrefab;
    [SerializeField] private GameObject parent;
    [SerializeField] private const float MINGATEDISTERROR = 0.1f;
    private Camera mainCam;
    public List<GateClass> gateList;

    [Header("ContactTimers")]
    [SerializeField] private GameObject contactTimerPrefab;

    [Header("Collision Crosses")]
    [SerializeField] private GameObject crossPrefab;
    [SerializeField] private GameObject crossParent;

    private void Awake()
    {
        mainCam = Camera.main;
        gateList = new List<GateClass>();
    }

    public List<GateClass> GetGateList() { return gateList; }
    public void SetGateList(List<GateClass> list) { gateList = list; }

    //poss add check got spaning gates too close or ontop of eachother, but not completeley nessesary
    public GameObject SpawnGate(Vector2 pos, float gateHeight)
    {
        GameObject gateObj = GameObject.Instantiate(gatePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, parent.transform);
        GateClass gateC = gateObj.GetComponent<GateClass>();
        gateC.SetId(gateList.Count);
        gateObj.GetComponent<GateClass>().SetGate(pos, gateHeight);
        gateObj.name = "Gate-" + gateC.GetId();
        gateList.Add(gateC);

        return gateObj;
    }

    public GameObject SpawnContactTimer()
    {
        return GameObject.Instantiate(contactTimerPrefab, new Vector3(1,1,1), Quaternion.identity);
    }

    public GameObject SpawnCross(Vector2 pos)
    {
        GameObject tempGObj = GameObject.Instantiate(crossPrefab, new Vector3(pos.x, pos.y, 1), Quaternion.identity, crossParent.transform);
        tempGObj.tag = "ColCross";
        return tempGObj;
    }


    //add func to get all existing gates
    //need to add func to sort this array byt GateClass
/*    public List<GateClass> GetExistingGates()
    {
        GameObject[] gateObjAr = GameObject.FindGameObjectsWithTag("Gate");


    }*/




}
