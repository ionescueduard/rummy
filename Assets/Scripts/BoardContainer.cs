using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardContainer : MonoBehaviour
{
    Collider2D currentCollider;
    Touch touch;
    Vector2 touchPosition;


    // Start is called before the first frame update
    void Start()
    {
        currentCollider = this.gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Ended)
            {
                Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                if (currentCollider == touchedCollider)
                {
                    Debug.Log(String.Format("Droped here :{0}.", this.gameObject.name));
                }
            }
        }

        
    }
}
