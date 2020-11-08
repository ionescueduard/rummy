using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static private Card[] cards = new Card[106];

    static private Card[] getCardsPack()
    {
        return cards;
    }

    static public void addCard(int index, Card card)
    {
        cards[index] = card;
    }


    public void buttonClicked()
    {
        Debug.Log("got here");
        cards[0].gameObject.transform.position = new Vector2(cards[0].gameObject.transform.position.x + 5, cards[0].gameObject.transform.position.y);
    }

}