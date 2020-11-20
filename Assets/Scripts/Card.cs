using System;
using System.Collections;
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
 *      1 char - color          - R/O/B/D 
 *      1 char - set number     - 0/1
 *      2 char - number         - 01/02/03/.../12/13
 *
 */


public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool moveAllowed;
    private Collider2D currentCollider;
    private Touch touch;
    private Vector2 touchPosition;

    private Dictionary<char, short> colorIndex = new Dictionary<char, short>() { {'R', 0}, { 'O', 2 }, { 'B', 4 }, { 'D', 6 }, { 'J', 8} };


    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * canvas.transform.localScale; ;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
    }


    /*--------------------------*/
    /*----- Frames Related ----*/
    /*------------------------*/
    private void Awake()
    {
        GameController.addCard(this.getCardIndex(), this);
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        currentCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
       

            if (touch.phase == TouchPhase.Began)
            {
                Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                if (currentCollider == touchedCollider)
                {
                    Debug.Log(String.Format("{0}'s id is: {1}.", this.name, this.getCardIndex()));
                    moveAllowed = true;
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (moveAllowed)
                {
                    transform.position = new Vector2(touchPosition.x, touchPosition.y);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                moveAllowed = false;
            }
        }
    }


    /*--------------------------*/
    /*--------- Logic ---------*/
    /*------------------------*/
    private int getCardIndex()
    {
        int cardPackNumber = colorIndex[this.name[0]] * 13 + (int)(this.name[1] - '0') * 13;
        int cardNumber = (this.name[3] == '0' ? this.name[4] - '0' : Int32.Parse(String.Format("{0}{1}", this.name[3], this.name[4]))) - 1;

        return cardPackNumber + cardNumber;
    }
}
