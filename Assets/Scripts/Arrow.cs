﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Arrow : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private int panelIndex;
    private int pairIndex;
    private GameController.SidePosition side; /// 0 for Left Arrow, 1 for Right Arrow,
    private GameController.ShiftDirection direction; /// direction in which pair moves. multiplying with -1 means moving Left, and with 1 moving Right

    private Color visibleColor = new Color(255, 255, 255, 255);
    private Color invisibleColor = new Color(255, 255, 255, 0);

    static public float RIGHT_ARROW_INITIAL_X = 60.22f;
    /*
     * Position local space:   -175.48   60.22   117.46   174.7
     * Distance between two arrows in local space: 57.24
     */


    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        GameController.getCurrentPlayer().shiftCardsForPair(this.pairIndex, this.direction);
    }

    public void OnPointerEnter(PointerEventData eventData) /// TODO it's not triggered for a picked card that is picked after moved on the board (for cards from original position, it is triggered)
        /// TODO also, currently appending cards work only for current player, exted to other players
    {
        Debug.Log("OnPointerEnter");
        if (GameController.isCardPicked() && !GameController.getCurrentPlayer().canPairShift(this.pairIndex, this.direction))
        {
            GameController.activateStickPointer(true, this.panelIndex, this.pairIndex, (int)this.side);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        if (GameController.isCardPicked() && !GameController.getCurrentPlayer().canPairShift(this.pairIndex, this.direction))
        {
            GameController.activateStickPointer(false, this.panelIndex, this.pairIndex, (int)this.side);
        }
    }



    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        this.panelIndex = (int)(this.name[0] - '0');
        this.pairIndex = (int)(this.name[1] - '0');
        this.side = this.name[2] == 'L' ? GameController.SidePosition.Left : GameController.SidePosition.Right;
        this.direction = this.name[2] == 'L' ? GameController.ShiftDirection.Right : GameController.ShiftDirection.Left;
    }

    void Awake()
    {
        init();

        this.image = this.GetComponent<Image>();

        GameController.addArrow(this, this.panelIndex, this.pairIndex, (int)this.side);
    }



    /*--------------------------*/
    /*--------- Utils ---------*/
    /*------------------------*/
    public void setVisible(bool state) { this.image.color = state ? visibleColor : invisibleColor; }

    public int getPanelIndex() { return this.panelIndex; }

    public int getPairIndex() { return this.pairIndex; }
}
