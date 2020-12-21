using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Arrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    static private int[] xMiddles = {-115, -37, 41, 119};
    
    private int panelIndex;
    private int pairIndex;
    private int direction;


    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameController.getCurrentPlayer().shiftCardsForPair(this.pairIndex, this.direction);
    }



    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        this.panelIndex = (int)(this.name[0] - '0');
        this.pairIndex = (int)(this.name[1] - '0');
        this.direction = this.name[2] == 'L' ? 1 : -1;
    }

    void Awake()
    {
        init();

        GameController.addArrow(this, this.panelIndex, this.pairIndex, this.direction == 1 ? 0 : 1);
    }



    /*--------------------------*/
    /*--------- Utils ---------*/
    /*------------------------*/
    public int getPanelIndex()
    {
        return this.panelIndex;
    }

    public int getPairIndex()
    {
        return this.pairIndex;
    }
}
