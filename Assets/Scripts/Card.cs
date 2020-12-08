﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* Name of the cards is 
 *      Red - R
 *      Orange - O
 *      Blue - B
 *      Dark - D
 *      
 * Id/Name of the card object is:
 *      1 char  - color          - R/O/B/D 
 *      1 char  - set number     - 0/1
 *      2 chars - number         - 01/02/03/.../12/13
 *
 */


public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;


    private RectTransform rectTransform;
    private Collider2D currentCollider;

    private Vector3 initialPositionWhenMoved;

    static private Dictionary<char, short> colorIndex = new Dictionary<char, short>() { {'R', 0}, { 'O', 2 }, { 'B', 4 }, { 'D', 6 }, { 'J', 8} };
    static private int layerWhenOnBoard = 10;
    static private int layerWhenPicked = 11;

    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log(String.Format("OnBeginDrag : {0}", currentCollider.bounds.ToString()));
        initialPositionWhenMoved = rectTransform.position;
        this.GetComponent<SpriteRenderer>().sortingOrder = layerWhenPicked;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * canvas.transform.localScale;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(String.Format("Droped {0} with bounds {1}", this.name, currentCollider.bounds));
        this.GetComponent<SpriteRenderer>().sortingOrder = layerWhenOnBoard;

        Player player = GameController.getCurrentPlayer();

        int n = GameController.getContainersNumber();
        Container[] containers = GameController.getContainers();
        Container closestContainer = null;
        for (int i = 0; i < n; i++)
        {
            if (currentCollider.bounds.Intersects(containers[i].GetComponent<Collider2D>().bounds) && player.isSlotEmpty(containers[i].getContainerIndex()))
            {
                if (closestContainer == null)
                    closestContainer = containers[i];
                else
                {
                    float prev = Mathf.Abs(closestContainer.GetComponent<Collider2D>().bounds.center.x - currentCollider.bounds.center.x) +
                         Mathf.Abs(closestContainer.GetComponent<Collider2D>().bounds.center.y - currentCollider.bounds.center.y);
                    float current = Mathf.Abs(containers[i].GetComponent<Collider2D>().bounds.center.x - currentCollider.bounds.center.x) +
                         Mathf.Abs(containers[i].GetComponent<Collider2D>().bounds.center.y - currentCollider.bounds.center.y);
                    if (current < prev)
                        closestContainer = containers[i];
                }
            }
        }

        if (!closestContainer)
        {
            Debug.Log("Collided with NONE");
            rectTransform.position = initialPositionWhenMoved;
        }
        else
        {
            player.freeSlot(Container.getContainerIndexByPosition(initialPositionWhenMoved));
            player.takeSlot(closestContainer.getContainerIndex());
            rectTransform.anchoredPosition = closestContainer.GetComponent<RectTransform>().anchoredPosition;
        }
    }


    /*--------------------------*/
    /*----- Frames Related ----*/
    /*------------------------*/
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        currentCollider = GetComponent<Collider2D>();

        GameController.addCard(this.getCardIndex(), this); 
    }


    /*--------------------------*/
    /*--------- Logic ---------*/
    /*------------------------*/
    private int getCardIndex()
    {
        int cardPackNumber = colorIndex[this.name[0]] * 13 + (int)(this.name[1] - '0') * 13;
        int cardNumber = (this.name[3] == '0' ? this.name[4] - '0' : 10 + (int)(this.name[4] - '0')) - 1;

        return cardPackNumber + cardNumber;
    }

    static public int getCardIndexByName(string name)
    {
        int cardPackNumber = colorIndex[name[0]] * 13 + (int)(name[1] - '0') * 13;
        int cardNumber = (name[3] == '0' ? name[4] - '0' : 10 + (int)(name[4] - '0')) - 1;

        return cardPackNumber + cardNumber;
    }
}
