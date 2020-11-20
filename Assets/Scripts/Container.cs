using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour, IDropHandler
{
    private Collider2D currentCollider;
    private int xContainer;
    private int yContainer;
    private Touch touch;
    private Vector2 touchPosition;

    static int[] xPositions = { -166, -149, -132, -115, -98, -81, -64, -47, -30, -13, 4, 21, 38, 55, 72, 89 };
    static int[] yLevelPosition = { -54, -85 };



    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(String.Format("OnDrop: {0}", eventData.pointerDrag.name));
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }

    /*--------------------------*/
    /*------ Events Done ------*/
    /*------------------------*/


    private int getContainerIndex(bool onlyContainerNumber = false)
    {
        int row = (int)(this.name[1] - '0') * 16;
        int containerNumber = (this.name[2] == '0' ? (int)(this.name[3] - '0') : 10 + (int)(this.name[3] - '0')) - 1;

        if (onlyContainerNumber)
            return containerNumber;

        return row + containerNumber;
    }

    void Start()
    {
        GameController.addContainer(getContainerIndex(), this);

        xContainer = xPositions[getContainerIndex(true)];
        yContainer = yLevelPosition[(getContainerIndex() < 16 ? 0 : 1)];

        this.gameObject.transform.position = new Vector3(xContainer, yContainer, 0);
        currentCollider = this.gameObject.GetComponent<Collider2D>();
    }


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
                //Debug.Log(String.Format("touching : {0}", touchPosition));
                //Debug.Log(String.Format("touching : {0}", currentCollider));
                //Debug.Log(String.Format("touching : {0}", currentCollider.bounds));
                if (currentCollider.bounds.Contains(new Vector3(touchPosition.x, touchPosition.y, -1)))
                {
                    Debug.Log(String.Format("Droped at {0}.", this.gameObject.name));
                }
            }
        }

        
    }

    
}
