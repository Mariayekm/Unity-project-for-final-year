using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow_circ1 : MonoBehaviour
{
    public float moveSpeed = 1.5f;

    private Vector3 mousePos;
    private Rigidbody2D rb;
    private Vector2 pos = new Vector2(0f, 0f);
    private bool follow;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        follow = false;
    }

    public void FollowOn() { follow = true; }
    public void FollowOff() { follow = false; }



    // Update is called once per frame
    private void Update()
    {
        if (follow)
        {
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            pos = Vector2.Lerp(transform.position, mousePos, moveSpeed);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(pos);
    }



}
