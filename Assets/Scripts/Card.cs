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
        this.initialPositionWhenMoved = rectTransform.position;
        //this.GetComponent<SpriteRenderer>().sortingOrder = layerWhenPicked;
        this.rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;// * canvas.transform.localScale;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log(String.Format("Droped {0} with bounds {1}", this.name, currentCollider.bounds));
        //this.GetComponent<SpriteRenderer>().sortingOrder = layerWhenOnBoard;
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
            player.freeSlot(Container.getContainerIndexByPosition(initialPositionWhenMoved));
            player.takeSlot(closestContainer.getContainerIndex());
            this.rectTransform.position = closestContainer.GetComponent<RectTransform>().position;
        }
    }


    /*--------------------------*/
    /*----- Frames Related ----*/
    /*------------------------*/
    private void Awake()
    {
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
