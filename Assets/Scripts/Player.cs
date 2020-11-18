using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Card[] upperSlots = new Card[15];
    Card[] lowerSlots = new Card[15];
    List<Card> cards = new List<Card>();
    Card upperSpecial;
    Card lowerSpecial;

    Card[] placedOnBoard; //TODO , take care of cards placement on board, movement around and shit

    //static int[] xPositions = { -897, -804, -711, -618, -525, -432, -339, -246, -153, -60, 33, 126, 219, 312, 405, 498 };
    //static int[] yLevelPosition = { -292, -460 };

    static int[] xPositions = { -166, -149, -132, -115, -98, -81, -64, -47, -30, -13, 4, 21, 38, 55, 72, 89 };
    static int[] yLevelPosition = { -54, -85 };


    private void Awake()
    {
        GameController.addPlayer(this);
    }


    public void initializeBoard(List<Card> cards, bool first)
    {
        int i = 0, j = 0;

        for (; i < 14 ;)
        {
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

    private void addCardOnBoardAt(int level, int index, Card card)
    {
        cards.Add(card);
        //Debug.Log(String.Format("Card: {0} - x:{1} y:{2} z:{3}", card.gameObject.name, xPositions[index], yLevelPosition[level], 0));
        card.transform.position = new Vector2(xPositions[index], yLevelPosition[level]); 
    }


}
