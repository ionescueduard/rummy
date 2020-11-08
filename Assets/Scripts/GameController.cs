using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static private Card[] cards = new Card[106];
    static private int cardsCurrentIndex = 0;
    static private List<Player> players = new List<Player>();
    static private int currentPlayer = -1;
    static private bool playerSwitched = false;


    private void Start()
    {
        int numToAdd;
        List<Card> init = new List<Card>();
        for (int i = 0; i < players.Count; i++)
        {
            bool first = i == 0;
            numToAdd = first ? 15 : 14;
            for (int j = 0; j < numToAdd; j++)
            {
                init.Add(cards[cardsCurrentIndex++]);
            }
            players[i].initializeBoard(init, first);
        }

        //Initialize atuu

        playerSwitched = true;
        nextPlayerIndex();
    }

    private void Update()
    {
        if (playerSwitched)
        {
            players[currentPlayer].activateBoard(true);



            // prepare for next player
            nextPlayerIndex();
            playerSwitched = false;
        }
    }


    static private Card[] getCardsPack()
    {
        return cards;
    }

    static public void addCard(int index, Card card)
    {
        cards[index] = card;
        card.gameObject.SetActive(false);
    }

    static public void addPlayer(Player player)
    {
        players.Add(player);
    }


    static private void nextPlayerIndex()
    {
        currentPlayer = (currentPlayer + 1) % players.Count; 
    }
    

    public void nextPlayer()
    {
        Debug.Log(string.Format("nextPlayere called, currentPlayer = {0}, and players count = {1}", currentPlayer, players.Count));
        playerSwitched = true;
        players[currentPlayer].activateBoard(false);
    }

    public void buttonClicked()
    {
        Debug.Log("got here");
        cards[0].gameObject.transform.position = new Vector2(cards[0].gameObject.transform.position.x + 5, cards[0].gameObject.transform.position.y);
    }

}