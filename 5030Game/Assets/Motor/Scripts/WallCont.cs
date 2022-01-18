using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCont : MonoBehaviour
{
    [Header("Wall Objects")]
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bot;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject left;
    [Space]
    [SerializeField] private float defualtThickness;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1 , 1, 1);
        SetWalls();
    }

    public void SetWalls()
    {
        SetTop();
        SetBot();
        SetRight();
        SetLeft();

    }

    private void SetTop()
    {
        float camScaleHeight = 2f * Camera.main.orthographicSize;
        float camScaleWidth = camScaleHeight * Camera.main.aspect;

        Vector3 scale = new Vector3(camScaleWidth + defualtThickness, defualtThickness, 0);
        top.transform.localScale = scale;
        Vector3 pos = new Vector3(0, camScaleHeight / 2 + defualtThickness/2, 0);
        top.transform.position = pos;
    }
    private void SetBot()
    {
        float camScaleHeight = 2f * Camera.main.orthographicSize;
        float camScaleWidth = camScaleHeight * Camera.main.aspect;
        //Vector3 scale = new Vector3(camScaleWidth + defualtThickness, defualtThickness, 0);
        //Vector3 pos = new Vector3(0, -(camScaleHeight / 2 + defualtThickness), 0);
        bot.transform.localScale = top.transform.localScale;
        bot.transform.position = new Vector3(top.transform.position.x, -top.transform.position.y, top.transform.position.z);
    }
    private void SetRight()
    {
        float camScaleHeight = 2f * Camera.main.orthographicSize;
        float camScaleWidth = camScaleHeight * Camera.main.aspect;

        Vector3 scale = new Vector3(defualtThickness, camScaleHeight + defualtThickness, 0);
        right.transform.localScale = scale;
        Vector3 pos = new Vector3(camScaleWidth/2 + defualtThickness/2, 0, 0);
        right.transform.position = pos;
    }
    private void SetLeft()
    {
        left.transform.localScale = right.transform.localScale;
        left.transform.position = new Vector3(-right.transform.position.x, right.transform.position.y, right.transform.position.z);
    }

}
