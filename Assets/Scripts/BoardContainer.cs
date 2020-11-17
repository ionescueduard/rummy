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
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);
        currentCollider = this.gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            //if (Input.touchCount != 0) { Debug.Log(String.Format("touching : {0}", Input.touchCount)); }
            touch = Input.GetTouch(0);
            
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Ended)
            {
                //Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                Debug.Log(String.Format("touching : {0}", touchPosition));
                Debug.Log(String.Format("touching : {0}", currentCollider));
                Debug.Log(String.Format("touching : {0}", currentCollider.bounds));
                if (currentCollider.bounds.Contains(new Vector3(touchPosition.x, touchPosition.y, -1)))
                {
                    Debug.Log(String.Format("Droped here :{0}.", this.gameObject.name));
                }
            }
        }

        
    }
}
