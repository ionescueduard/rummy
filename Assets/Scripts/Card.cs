using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class Card : MonoBehaviour
{
    bool moveAllowed;
    Collider2D col;
    Touch touch;
    Vector2 touchPosition;

    private Dictionary<char, short> colorIndex = new Dictionary<char, short>() { {'R', 0}, { 'O', 1 }, { 'B', 2 }, { 'D', 3 } }; 

    private void Awake()
    {
        GameController.addCard(this.getCardIndex(), this);
    }

    private int getCardIndex()
    {
        return colorIndex[this.name[0]] * 13 + (int)(this.name[1] - '0') * 13 + Int32.Parse(String.Format("{0}{1}", this.name[3], this.name[4])) - 1;
    }


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        }

        if (touch.phase == TouchPhase.Began)
        {
            Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
            if (col == touchedCollider)
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
