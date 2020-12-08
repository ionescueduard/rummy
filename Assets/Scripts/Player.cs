using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static int rowMaxNumber = 16;
    private bool[] boardSlotsStatus = new bool[rowMaxNumber * 2]; // true if busy
    private List<Card> cards = new List<Card>();
    private Card upperSpecial; //TODO
    private Card lowerSpecial; //TODO

    Card[] placedOnBoard; //TODO , take care of cards placement on board, movement around and shit


    private void Awake()
    {
        GameController.addPlayer(this);
    }


    public bool isSlotEmpty(int index) { return !boardSlotsStatus[index]; }

    public void freeSlot(int index) { boardSlotsStatus[index] = false; }

    public void takeSlot(int index) { boardSlotsStatus[index] = true; }

    public void initializeBoard(List<Card> cards, bool first)
    {
        int i = 0, j = 0;

        for (; i < 14 ;)
        {
            boardSlotsStatus[j] = true;
            boardSlotsStatus[rowMaxNumber + j] = true;
            addCardOnBoardAt(0, j,   cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
        }
        if (first)
        {
            addCardOnBoardAt(0, j, cards[i]);
        }  
    }

    public void activateBoard(bool activate)
    {
        foreach (Card card in cards)
        {
            card.gameObject.SetActive(activate);
        }
    }

    private void addCardOnBoardAt(int y, int x, Card card)
    {
        cards.Add(card);
        //Debug.Log(String.Format("Card: {0} - x:{1} y:{2} z:{3}", card.gameObject.name, xPositions[index], yLevelPosition[level], 0));
        card.transform.position = new Vector3(GameController.xBoardPositions[x], GameController.yBoardPositions[y], 0); 
    }


}
