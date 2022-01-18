using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Following script based on tutorial video: https://www.youtube.com/watch?v=Ehk9fKBwS3Y
public class MouseFollow_circ : MonoBehaviour
{
    [SerializeField] [Range(0,1)]private float lerpPercent = 0.5f;
    [SerializeField] LayerMask layermask_player;
    [SerializeField] GameObject timerGObj;
    [SerializeField] GameObject gameManagerGObj;
    [SerializeField] LayerMask layermaskTriggers;
    private Timer timer;
    private GameMotorHandler gameManager;
    private PlayerPosLogger posLogger;
    private MotorActionMap controls;
    private Camera mainCam;
    private Vector3 mousePos;
    private Vector2 pos = new Vector2(0f, 0f);
    private GameObject playerObj;
    private Rigidbody2D rb;
    private bool follow;
    private bool canFollow;
    private float DESYNC_DIST;
    private bool freeze;
    private float currentGateX;
#region init
    private void Awake()
    {
        controls = new MotorActionMap();
        mainCam = UnityEngine.Camera.main;
        follow = false;
        canFollow = false;
        rb = this.GetComponent<Rigidbody2D>();
        pos = new Vector2(0f, 0f);
        timer = timerGObj.GetComponent<Timer>();
        gameManager = gameManagerGObj.GetComponent<GameMotorHandler>();
        posLogger = gameObject.GetComponent<PlayerPosLogger>();
        DESYNC_DIST = transform.localScale.x * 3;
        currentGateX = gameManager.GetNextGateId();
    }
    //getting errors that controls does not reference an object when player is not following mouse for ~5sec, but system still works fine.
    //added null check to remove errors
    private void OnEnable() {
        if (controls != null)
        {
            controls.Enable();
        }
        else { Debug.LogError("MotorActionMap - controls is null"); }
            
                
    }

    private void OnDisable() { controls.Disable(); }

#endregion

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        follow = false;
        controls.Mouse.ClickLeft.started += _ => ClickStarted();
        controls.Mouse.ClickLeft.performed += _ => ClickEnded();
    }

    //Run on left mouse down
    //does noting at present
    private void ClickStarted()
    {
        //Debug.Log("ClickStarted");
    }

    private void ClickEnded()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Ray ray = mainCam.ScreenPointToRay(controls.Mouse.MousePos.ReadValue<Vector2>());
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layermask_player);

        if (hit2D.collider != null) {
            FollowOn();
        }
    }


    public void FollowOn() { 
        if (canFollow){
            follow = true; 
            timer.TimerBegin(); 
            gameManager.SetFollowing(true);
            posLogger.isLogging = true;
        } 
    }      //Starts following if allowed
    public void FollowOff() { follow = false; }                     //Turns off following
    public float GetMoveSpeed() { return lerpPercent; }               //Returns the tracking speed of the player
    public void SetMoveSpeed(float speed) { lerpPercent = speed; }    //Sets the move tracking speed player
    public bool GetCanFollow() { return canFollow; }                //Returns if the player can follow the mouse
    public void SetCanFollow(bool set) { canFollow = set; }         //Sets if the playe can follow
    public void StopFollow() { canFollow = false; follow = false; posLogger.isLogging = false; } //Stops the player following     


    // Update is called once per frame
    private void Update()
    {
        UpdatMovepoint();
        Debug.DrawLine(transform.position, pos, Color.magenta);
    }

    private void FixedUpdate()
    {
        
        if (follow && canFollow)
        {

            rb.MovePosition(pos);
            if (freeze)
            {
                Debug.Break();
            }
        }
    }
    private void UpdatMovepoint()
    {
        //using new input system
        if (follow)
        {
            mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pos = Vector2.Lerp(transform.position, mousePos, lerpPercent);
            //CheckTriggerSkip();
            CheckTriggerSkip2();
        }
        #region old input system
        //using old input system
        /*        if (follow)
                {
                    mousePos = Input.mousePosition;
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    pos = Vector2.Lerp(transform.position, mousePos, moveSpeed);
                }*/
        #endregion
    }

    //If player gets stuck on wall jump to cursor can cause trigger not to happen
    //this checks if the player has desynced and then uses a line cast to check if the skip would intersect the trigger
    //it then places the new position to be on the trigger if they do
    private void CheckTriggerSkip()
    {
        float dist = Vector2.Distance(mousePos, transform.position);
        if (dist > DESYNC_DIST || pos.magnitude > DESYNC_DIST)
        {

            RaycastHit2D[] linecastHits = Physics2D.LinecastAll(transform.position, pos, layermaskTriggers);
            if (linecastHits.Length > 0)
            {
                foreach (RaycastHit2D hit in linecastHits)
                {
                    GateClass tempGC = hit.collider.GetComponentInParent<GateClass>();
                    if (tempGC.Id == gameManager.GetNextGateId())//check correct gate
                    {
                        pos = GetNewPosOnTriggerFromHit(hit, tempGC);
                    }
                }
                /*
                Debug.Log("MF/CTS - Drawing Lines and Breaking");
                Debug.DrawLine(transform.position, mousePos, Color.magenta, 5f); //Draw Line from player to mouse
                Debug.DrawLine(transform.position, pos, Color.cyan, 5f);    //Drawline from player to new2 desination
                Debug.Break();
                */
            }
                 
        }
    }
    //Checks if the player want to move to the other side of the gate, and if they are not going to pass through the trigger to remove the x movement
    //only works on levels going left to right
    private void CheckTriggerSkip2()
    {
        if (pos.x > gameManager.GetNextGatePos().x)//check new pos want to move to other side of gate
        {
            RaycastHit2D[] linecastHits = Physics2D.LinecastAll(transform.position, pos, layermaskTriggers);
            if (linecastHits.Length > 0)
            {
                foreach (RaycastHit2D hit in linecastHits)
                {
                    GateClass tempGC = hit.collider.GetComponentInParent<GateClass>();
                    if (tempGC.Id == gameManager.GetNextGateId())//check correct gate
                    {
                        pos = GetNewPosOnTriggerFromHit(hit, tempGC);
                    }
                }
            }
            else { pos.x = gameManager.GetNextGatePos().x; }
        }
    }

    //Caclulates where to move the player to if path to destination passes through a trigger
    private Vector2 GetNewPosOnTriggerFromHit(RaycastHit2D hit, GateClass gc)
    {
        Vector2 midEndPos;
        Vector3 midScale = gc.GetMidScale();
        Vector2 newPlayerPos = new Vector2(0, 0);
        if (transform.position.y > gc.GetMidTopBotPos(true).y || transform.position.y < gc.GetMidTopBotPos(false).y)//check if player is above/below gate top/bottom
        {
            if (transform.position.y > gc.GetMidPos().y)//Check if approaching from top or bottom of gate and move player to closest point where they would collide with wall
            {
                midEndPos = gc.GetMidTopBotPos(true);
                newPlayerPos.x = midEndPos.x;
                newPlayerPos.y = midEndPos.y - transform.localScale.y + 0.1f;
            }
            else
            {
                midEndPos = gc.GetMidTopBotPos(false);
                newPlayerPos.x = midEndPos.x;
                newPlayerPos.y = midEndPos.y + transform.localScale.y + 0.1f;
            }
        }
        else
        {
            newPlayerPos = hit.point;
        }
        return newPlayerPos;
    }


    private void CheckDesyncStopFollow()
    {
        float dist = Vector2.Distance(mousePos, transform.position);
        if (dist > DESYNC_DIST)
        {
            FollowOff();
        }
    }
}
