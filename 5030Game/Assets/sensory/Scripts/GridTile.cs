using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public int index = 0;
    int x = 0;
    int y = 0;
    public Vector2 posi;
    public GameObject grid_background;

    private Action<int, int> clickFunc = null;

    public void Init(int i, int j, int index, Sprite sprite, Action<int, int> clickFunc)
    {
        grid_background = GameObject.Find("Grid Background");
        this.GetComponent<SpriteRenderer>().sprite = sprite;
        this.GetComponent<SpriteRenderer>().color = new Color(0.215f, 0.23f, 0.24f, 1);
        this.transform.parent = grid_background.transform;
        //this.transform.SetParent(test_canvas.transform, false);
        UpdatePos(i, j);
        //this.gameObject.transform.localScale = new Vector2(0.24f, 0.24f);
        this.clickFunc = clickFunc;
        posi.x = i;
        posi.y = j;
    }

    public void UpdatePos(int i, int j)
    {
        x =  i;
        y = j;
        //this.gameObject.transform.localScale = new Vector2(10, 10);
        this.gameObject.transform.localPosition = new Vector2((0.25f*i)-0.35f, (0.25f*j)-0.35f);
    }

    public bool IsBlue()
    {
        return this.GetComponent<SpriteRenderer>().color == new Color(0.008f, 0.53f, 0.82f, 1);
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && clickFunc != null)
            clickFunc(x, y);

    }

    public void setClickToNull()
    {
        clickFunc = null;
    }
}
