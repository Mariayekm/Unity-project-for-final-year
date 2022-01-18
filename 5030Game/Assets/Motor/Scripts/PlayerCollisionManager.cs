using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Logs when player enteres gate
//Begins timer when player is in contact with walls or 

public class PlayerCollisionManager : MonoBehaviour
{
    public static PlayerCollisionManager instance;

    private bool playerWallContact = false;                     //is player incontact with wall
    private bool playerWallContactMessageOn = true;             //boardcast player riding wall
    [SerializeField] private float maxContactTime;              //maximum wall ride time
    private float contactTime, contactTimeStart, contactTimeEnd;//current time of contact
    private int numContactsCurr;                                //current number of walls in contact
    private CircleCollider2D playerCollider;
    DataSet_Motor data;
    [SerializeField] private GameObject gameManagerGObj;
    [SerializeField] private  GameObject CSVHandlerGObj;
    [SerializeField] private GameObject timerGObj;
    private GameMotorHandler gameManager;
    private CSVHandler csvHandler;
    private Timer timer;
    private SpriteRenderer spriteRenderer;
    private Vector2 offscreenCoord;

    private List<Collider2D> colliderList;
    [SerializeField] private LayerMask exitCheckMask;

    [SerializeField] private GameObject spawnerGObj;
    private Spawner spawner;


    public int totalCollisions, totalCollisionsLv;
    //public List<GameObject> crossGObjList;

    [SerializeField] GameObject collisionCountTextGObj;
    Text collText;
    public bool countCollisions;



    //creates a referance to this instance of itsself
    private void Awake()
    {
        instance = this;
        gameManager = gameManagerGObj.GetComponent<GameMotorHandler>();
        playerCollider = gameObject.GetComponentInParent<CircleCollider2D>();
        data = new DataSet_Motor();
        data.Reset();
        timer = timerGObj.GetComponent<Timer>();
        colliderList = new List<Collider2D>();
        float ph = GameObject.Find("Player").transform.localScale.y;
        csvHandler = CSVHandlerGObj.GetComponent<CSVHandler>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        totalCollisions = 0;
        totalCollisionsLv = 0;
        spawner = spawnerGObj.GetComponent<Spawner>();
        countCollisions = true;
        collText = collisionCountTextGObj.GetComponent<Text>();
    }

    private void Start()
    {
        //Camera.main.orthographicSize * 2 = camera height, Camera.main.orthographicSize * 2 * Camera.main.aspect = camera width
        offscreenCoord.x = Camera.main.orthographicSize * 4;
        offscreenCoord.y = Camera.main.orthographicSize * 4 * Camera.main.aspect;
        totalCollisionsLv = -1;
        UpdateCollisionCount();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("GateMid")) { LogTriggerEnter(collider); }
    }

    private void GateEnter(Collider2D gate)
    {
        //Log gate entry

    }
    private void LogTriggerEnter(Collider2D collider)
    {
        GateClass gateC = collider.GetComponentInParent<GateClass>();
        DataSet_Motor.CollisionDataType colType = GetCollisionDataType(collider.tag, true);
        int Gnum = GetGnum(collider, colType);
        ColliderDistance2D dist = playerCollider.Distance(collider);
        SaveData(new Vector2(0,0), colType, 0, dist.pointA, Gnum);
        gateC.SetGateOnOff(false);
        gameManager.UpdateGatePassed(gateC.Id);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "GateWall" || collision.collider.tag == "Wall")
        {
            LogCollisionEnter(collision);
            //UpdatePlayerColor(2);
        }
    }


    private void LogCollisionEnter(Collision2D collision)
    {
        /*
            1. Get Collision point of latest collision, latest collision will be the only point that has a collider
            2. Set Collision timer variables
            3. Add Collider to list of current objects being collided with 
            4. Get Gate Number
            5. Save Data
        */

        DataSet_Motor.CollisionDataType cdt = GetCollisionDataType(collision.collider.tag, true);
        ContactPoint2D[] contactPoints = new ContactPoint2D[5];
        int numContactPoints = collision.GetContacts(contactPoints);
        int Gnum;
        Vector2 contactPoint = offscreenCoord;
        for (int i = 0; i < numContactPoints; i++)
        {
            if (contactPoints[i].collider != null)
            {
                contactPoint = contactPoints[i].point;
            }
        }
        if (contactPoint == offscreenCoord) { Debug.LogError("Contact point not found"); Debug.DebugBreak(); }
        else {
            contactTimeEnd = timer.GetCurrentTime_float();
            if (numContactsCurr <= 0) {
                contactTimeStart = timer.GetCurrentTime_float(); 
            }
            contactTime = contactTimeEnd - contactTimeStart;
            colliderList.Add(collision.collider);
            numContactsCurr++;
            playerWallContact = true;
        }
        Gnum = GetGnum(collision.collider, cdt);
        SaveData(contactPoint, cdt, contactTime, new Vector2(0,0), Gnum);
        gameManager.crossGObjList.Add(spawner.SpawnCross(contactPoint));
        UpdateCollisionCount();
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        //checks that the player has left contact will all gates and walls before resetting timer
        if (collision.collider.CompareTag("GateWall") || collision.collider.CompareTag("Wall")){
            //UpdatePlayerColor(1);
            LogCollisionExit(collision);
        }
    }

    private void LogCollisionExit(Collision2D collision)
    {
        //Debug.Log("Exiting: " + collision.collider.tag + " - " + collision.collider.gameObject.name);

        ColliderDistance2D colDistTemp = playerCollider.Distance(collision.collider);
        //Debug.DrawLine(colDistTemp.pointA, colDistTemp.pointB, Color.magenta, 5f);
        DataSet_Motor.CollisionDataType colType = GetCollisionDataType(collision.collider.tag, false);
        int Gnum = GetGnum(collision.collider, colType);
        contactTimeEnd = timer.GetCurrentTime_float();
        contactTime = contactTimeEnd - contactTimeStart;
        SaveData(colDistTemp.pointB, colType, contactTime,new Vector2(0,0), Gnum);

        colliderList.Remove(collision.collider);
        numContactsCurr--;
        if (numContactsCurr <= 0)
        {
            playerWallContact = false;
            playerWallContactMessageOn = true;
            contactTime = 0f;
            contactTimeStart = 0f;
            contactTimeEnd = 0f;
        }
        
    }




    //Checks if the player is still in contact set incontact flag to be true
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("GateWall") || collision.collider.CompareTag("Wall")) 
        { 
            playerWallContact = true;
            //UpdatePlayerColor(2);
        } 
    }

    //If player stays in contact with wall for too long
    private void WallRide()
    {
        //if (playerWallContactMessageOn) {Debug.Log("player spent too long on wall");}
        playerWallContactMessageOn = false;
    }

    //Player enters gate



    //retuns the gate Id number from a collider
    // Gnum == -2, Error retriving Gate Id
    // Gnum == -1, None/wall
    // Gnum >= 0, Gate ID
    private int GetGnum(Collider2D collider, DataSet_Motor.CollisionDataType Cdt)
    {
        int Gnum = -1;
        DataSet_Motor.CollisionDataType[] gateCdt = new DataSet_Motor.CollisionDataType[]{ DataSet_Motor.CollisionDataType.gateEnter, DataSet_Motor.CollisionDataType.gateExit, DataSet_Motor.CollisionDataType.triggerEnter, DataSet_Motor.CollisionDataType.triggerExit };
        if (Cdt == gateCdt[0] || Cdt == gateCdt[1] || Cdt == gateCdt[2] || Cdt == gateCdt[3]) {
            try { Gnum = collider.GetComponentInParent<GateClass>().Id; }
            catch(System.Exception ex) {
                Gnum = -2;
                Debug.LogError("Could not get GateId number: " + ex);
            }
        }
        return Gnum;
    }


    private DataSet_Motor.CollisionDataType GetCollisionDataType(string tag, bool isEnter) 
    {
        DataSet_Motor.CollisionDataType output;
        string wallTag = "Wall";
        string gateTag = "GateWall";
        string trigTag = "GateMid";
        if (tag == wallTag && isEnter) { output = DataSet_Motor.CollisionDataType.wallEnter;
        } else if(tag == wallTag && !isEnter) { output = DataSet_Motor.CollisionDataType.wallExit;
        } else if(tag == gateTag && isEnter) { output = DataSet_Motor.CollisionDataType.gateEnter ;
        } else if(tag == gateTag && !isEnter) { output = DataSet_Motor.CollisionDataType.gateExit;
        } else if(tag == trigTag && isEnter) { output = DataSet_Motor.CollisionDataType.triggerEnter;
        } else if (tag == trigTag && !isEnter){ output = DataSet_Motor.CollisionDataType.triggerExit;
        } else { Debug.LogError("Unknown Collsion type, annot onert to DataSet_Motor.CollisionDataType"); output = DataSet_Motor.CollisionDataType.None; }

            return output;
    }

    private void SaveData(Vector2 Cpos, DataSet_Motor.CollisionDataType cdt, float CtimeS, Vector2 Gpos, int Gnum) 
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        data.Reset();
        data.timeS = timer.GetCurrentTime_float();
        data.Mx = mousePos.x;
        data.My = mousePos.y;
        data.Px = transform.position.x;
        data.Py = transform.position.y;
        data.Cx = Cpos.x;
        data.Cy = Cpos.y;
        data.Cdt = cdt;
        data.CtimeS = CtimeS;
        data.Gx = Gpos.x;
        data.Gy = Gpos.y;
        data.Gnum = Gnum;
        data.UpdateTs();
        data.UpdatePMDist();
        //data.DebugPrint();
        csvHandler.AddToMotorBuffer(data);
    }
    private void Update()
    {
        if (playerWallContact)
        {
            contactTime += Time.deltaTime;
            if (contactTime >= maxContactTime)
            {
                WallRide();
            }
        }
        
    }

    private void UpdateCollisionCount()
    {
        if (countCollisions)
        {
            totalCollisionsLv++;
            collText.text = "Hits: " + totalCollisionsLv;
        }
    
    }

    public void ResetCollisionCount()
    {
        totalCollisionsLv = 0;
        collText.text = "Hits: " + totalCollisionsLv;
    }

    private void UpdatePlayerColor(int state)
    {
        Color pcIdle = Color.grey;
        Color pcFollow = Color.white;
        Color pcColl = Color.red;
        Color pcError = Color.magenta;

        switch (state)
        {
            case 0:
                spriteRenderer.color = pcIdle;
                break;
            case 1:
                spriteRenderer.color = pcFollow;
                break;
            case 2:
                spriteRenderer.color = pcColl;
                break;

            default:
                spriteRenderer.color = pcError;
                break;
        }
    }

    #region Legacy code dump

    //======= Code for creating new timers for each individual collsision =======
    /*    [SerializeField] private GameObject spawnerGObj;
    private Spawner spawner;
    private List<ContactTimer> contactTimerList;*/

    /*    private void CreateContactTimer(Collider2D coll)
        {
            GameObject tempGObj = spawner.SpawnContactTimer();
            ContactTimer tempCT = tempGObj.GetComponent<ContactTimer>();

            if (coll.tag == "Wall")
            {
                tempCT.Id = coll.tag;
            }else if (coll.tag == "GateWall")
            {
                tempCT.Id = "Gate-" + coll.GetComponentInParent<GateClass>().Id.ToString();
            }

        }*/
    /*
    private Collider2D DetectColliderLeaving()
    {
        float radius = playerCollider.radius * RAYEXITDISTCHECK;
        RaycastHit2D[] rayCircHits = Physics2D.CircleCastAll(transform.position, radius, transform.forward, radius, exitCheckMask);
        CircleCollider2D playerColl = this.GetComponentInParent<CircleCollider2D>();



        Debug.Log("Num Hits: " + rayCircHits.Length);
        foreach (RaycastHit2D hit in rayCircHits)
        {
            drawGizmos = true;
            //Debug.Log("Drawing, (" + transform.position.x.ToString("n3") + "," + transform.position.y.ToString("n3") + ")->(" + hit.point.x.ToString("n3") + "," + hit.point.y.ToString("n3") + ")");
            Debug.Log("Drawing");
            Debug.DrawLine(hit.centroid, hit.point, Color.magenta, 5f);
            if (colliderList.Contains(hit.collider))
            {

                colliderList.Remove(hit.collider);
            }

        }
        return new Collider2D();
    }
    */
    /*
         private CollExitPackage[] GetLastContactPoint()
    {
        //colliderDistace2D pointA on collidercalled from, point B on argument collider
        List<CollExitPackage> cpList = new List<CollExitPackage>();
        CollExitPackage cpTemp;
        CollExitPackage[] cpAr = null;

        if (colliderList.Count == 0) { Debug.LogError("colliderList is empty"); }
        else
        {
            foreach (Collider2D coll in colliderList)
            {
                if (!coll.IsTouching(playerCollider))
                {
                    cpTemp.dist = playerCollider.Distance(coll);
                    cpTemp.colliderExiting = coll;
                    cpList.Add(cpTemp);
                    colliderList.Remove(coll);
                }
            }
            cpAr = new CollExitPackage[cpList.Count];
            for (int i = 0; i < cpList.Count; i++)
            {
                cpAr[i] = cpList[i];
            }
        }
        return cpAr;
    }
    */
    /*
         //occurs before Awake() so must get direct ref rather than one got at start of game
    private void DrawExitCircRaycast()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.GetComponentInParent<CircleCollider2D>().radius * RAYEXITDISTCHECK);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawExitCircRaycast();
        }
    }

    private struct CollExitPackage
    {
        public Collider2D colliderExiting;
        public ColliderDistance2D dist;
    }
     */

    #endregion

}
