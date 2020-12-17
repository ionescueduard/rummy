using System;
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
    private RectTransform rectTransform;
    private Collider2D currentCollider;

    private int number;
    private char color;
    private int onBoardX;
    private int onBoardY;


    private Vector3 initialPositionWhenMoved;

    static private Dictionary<char, short> colorIndex = new Dictionary<char, short>() { {'R', 0}, { 'O', 2 }, { 'B', 4 }, { 'D', 6 }, { 'J', 8} };


    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        if (this.name[0] != 'J')
        {
            number = this.name[3] == '0' ? this.name[4] - '0' : 10 + (int)(this.name[4] - '0');
            color = this.name[0];
        }
        else
        {
            number = -1;
            color = ' ';
        }

        /// -2 represents nowere
        /// -1 represents on Table
        /// >= 0 represents positions on Board
        onBoardX = -2;
        onBoardY = -2;
    }

    private void Awake()
    {
        init();

        this.rectTransform = GetComponent<RectTransform>();
        this.currentCollider = GetComponent<Collider2D>();
        // for old sprite renderer
        // scale 2.4, width 7, height 9.8

        // for new image
        // on board scale: 1.272
        // on table scale: 0.795
        this.rectTransform.localScale = new Vector3(1.272f, 1.272f, 0);
        this.rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        GameController.addCard(this.getCardIndex(), this);
    }



 

    public int getNumber() { return this.number; }

    public char getColor() { return this.color; }

    public bool isJoker() { return this.number == -1; }

    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log(String.Format("OnBeginDrag : {0}", currentCollider.bounds.ToString()));
        this.initialPositionWhenMoved = rectTransform.position;
        this.rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log(String.Format("Droped {0} with bounds {1}", this.name, currentCollider.bounds));
        this.rectTransform.SetAsFirstSibling();

        Player player = GameController.getCurrentPlayer();

        int n = GameController.getContainersNumber();
        Container[] containers = GameController.getContainers();
        Container closestContainer = null;

        Bounds match = currentCollider.bounds;
        for (int i = 0; i < n; i++)
        {
            //can't find why i can't change containers's position z value from -1, so i match it before doing intersect
            match.center = new Vector3(currentCollider.bounds.center.x, currentCollider.bounds.center.y, containers[i].GetComponent<Collider2D>().bounds.center.z);
            if (match.Intersects(containers[i].GetComponent<Collider2D>().bounds) && player.isSlotEmpty(containers[i].getContainerIndex()))
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
            //Debug.Log("Collided with NONE");
            this.rectTransform.position = initialPositionWhenMoved;
        }
        else
        {
            player.moveCardFromSlotToSlot(this, Container.getContainerIndexByPosition(initialPositionWhenMoved), closestContainer.getContainerIndex());
            this.rectTransform.position = closestContainer.GetComponent<RectTransform>().position;
        }
    }



    /*--------------------------*/
    /*--------- Utils ---------*/
    /*------------------------*/
    private int getCardIndex()
    {
        int cardPackNumber = colorIndex[this.name[0]] * 13 + (int)(this.name[1] - '0') * 13;
        int cardNumber = (this.name[3] == '0' ? this.name[4] - '0' : 10 + (int)(this.name[4] - '0')) - 1;

        return cardPackNumber + cardNumber;
    }

    public void setOnBoardPosition(int y, int x)
    {
        onBoardX = x;
        onBoardY = y;
    }

    public int getOnBoardX() { return onBoardX; }

    public int getOnBoardY() { return onBoardY; }

    static public int getCardIndexByName(string name)
    {
        int cardPackNumber = colorIndex[name[0]] * 13 + (int)(name[1] - '0') * 13;
        int cardNumber = (name[3] == '0' ? name[4] - '0' : 10 + (int)(name[4] - '0')) - 1;

        return cardPackNumber + cardNumber;
    }

    static public int getCardIndexOnBoardByPosition(Vector3 point)
    {
        int x = (int)Mathf.Round(point.x);
        int y = (int)Mathf.Round(point.y);
        for (int i = 0; i < GameController.xBoardSlots; i++)
        {
            if (x == GameController.xBoardPositions[i])
            {
                return ((y == -54 ? 0 : 1) * GameController.xBoardSlots) + i;
            }
        }
        throw new Exception("Can't find any Container by its position.");
    }
}
