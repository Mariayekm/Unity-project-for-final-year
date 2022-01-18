using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberBox : MonoBehaviour
{
    public int index = 0;
    int x = 0;
    int y = 0;
    public bool colour = false;

    private Action<int, int> swapFunc = null;

    public void Init(int i, int j, int index, Sprite sprite, Action<int, int> swapFunc)
    {
        float r = 0.4f;
        float b = 0.4f;
        this.index = index;
        float g = 0.4f + index * 0.06f;

        if (index == 5) 
        {
            r = 0;
            b = 1;
            g = 0.5f;
        } else
        { 
            r = 0.4f;
            b = 0.4f;
        }
        
        this.GetComponent<SpriteRenderer>().sprite = sprite;
        this.GetComponent<SpriteRenderer>().color = new Color(r, g, b, 1);
        this.GetComponent<SpriteRenderer>().sortingOrder = 5;
        UpdatePos(i, j);
        this.swapFunc = swapFunc;
    }

    public void UpdatePos(int i, int j)
    {
        x = i;
        y = j;
        this.gameObject.transform.localPosition = new Vector2((2*i)-2, (2*j)-2);
    }

    public bool IsFive()
    {
        return index == 5;
    }

    public int GetIndex()
    {
        return index;
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && swapFunc != null)
            swapFunc(x, y);
        
    }

}
