using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTest : MonoBehaviour
{
    private Camera mainCam;
    private System.Collections.Generic.List<Vector3> Lm;
    private System.Collections.Generic.List<Vector3> Ls;
    private int numM, numS;
    // Start is called before the first frame update

    //Notes
    // Camera.
    // Camera.main.ScreenToWorldPoint(new Vector3(screen.width, screen.height, 0);
    // Camera.ScreenToViewportPoint - Viewport space is normalized and relative to the camera. The bottom-left of the camera is (0,0); the top-right is (1,1). The z position is in world units from the camera.
    
    //Need to set z to 0 manually, and read screen width and height separately then input into functions
    
    private void Awake()
    {

        mainCam = Camera.main;
        int ws = mainCam.scaledPixelWidth;
        int hs = mainCam.scaledPixelHeight;
        float camOrthHeight = mainCam.orthographicSize;
        float camAspect = mainCam.aspect;
        //Debug.Log("Pixel W x H = " + mainCam.pixelWidth + " x " + mainCam.pixelHeight);
        //Debug.Log("Scaled pixel W x H = " + ws + " x " + hs);
        //Debug.Log("Orth Height = " + camOrthHeight + ", aspect = " + camAspect);

        numM  = 0;
        numS = 0;
        Vector3 v3_1 = Camera.main.ScreenToWorldPoint(new Vector3(ws / 2, 0, 0));
        v3_1.z = 0;
      
        Vector3 v3_2 = Camera.main.ScreenToWorldPoint(new Vector3(0, hs/2, 0));
        v3_2.z = 0;


        Vector3 v3_3 = -v3_1;
        Vector3 v3_4 = -v3_2;


        Lm = new List<Vector3>() {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, -1, 0),
        new Vector3(2, 2, 0),
/*        Camera.main.ScreenToWorldPoint (new Vector3(0, 0, 0) ),
        Camera.main.ScreenToWorldPoint (new Vector3(1, 0, 0) ),
        Camera.main.ScreenToWorldPoint (new Vector3(0, 1, 0) ),
        new Vector3(2, 2, 0),*/
/*        Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)),
        Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)),
        Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)),
        new Vector3(2, 2, 0),*/
        
        v3_1,
        v3_2,
        v3_3,
        v3_4

        };

        Ls = new List<Vector3>() {
        new Vector3(1, 1, 1),
        new Vector3(1, camOrthHeight, 1),
        new Vector3(1, 2f*camOrthHeight, 1),
        new Vector3(2f*camOrthHeight*camAspect, 1, 1),
        new Vector3(0.5f, 0.5f, 1),
        new Vector3(2, 2, 1),
        new Vector3(1, 2, 1),
        new Vector3(2, 1, 1),
        
        };

    }
    void Start()
    {
        
    }

    private void Move()
    {
        if (numM < Lm.Count)
        {
            transform.position = Lm[numM];
            Debug.Log("pos: " + Lm[numM].ToString());
            numM++;

        }
        else { numM = 0; }
    }
    private void Scale()
    {
        if (numS < Ls.Count)
        {
            transform.localScale = Ls[numS];
            Debug.Log("scale" + Ls[numS].ToString());
            numS++;
        }
        else { numS = 0; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Move();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Scale();
        }
    }
}
