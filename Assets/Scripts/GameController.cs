using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    static private Card[] cards = new Card[106];
    static private int cardsCurrentIndex = 0;
    static private List<Player> players = new List<Player>();
    static private int currentPlayer = 1;
    static private bool playerSwitched = false;
    static private Random rng = new Random();


    private void Start()
    {
        ShuffleCards();

        int numToAdd;
        List<Card> init = new List<Card>();
        for (int i = 0; i < players.Count; i++)
        {
            bool first = i == 0;
            numToAdd = first ? 15 : 14;
            init.Clear();
            for (int j = 0; j < numToAdd; j++)
            {
                init.Add(cards[cardsCurrentIndex++]);
            }
            players[i].initializeBoard(init, first);
        }

        //Initialize atuu

        playerSwitched = true;
    }

    private void Update()
    {
        if (playerSwitched)
        {
            Debug.Log(String.Format("Deactivating player {0}", currentPlayer));
            players[currentPlayer].activateBoard(false);
            nextPlayerIndex();

            Debug.Log(String.Format("Activating player {0}", currentPlayer));
            players[currentPlayer].activateBoard(true);


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

    static private void ShuffleCards()
    {
        int n = 106;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card tmp = cards[k];
            cards[k] = cards[n];
            cards[n] = tmp;
        }
    }

    public void nextPlayer()
    {
        Debug.Log(string.Format("nextPlayere called, currentPlayer = {0}, and players count = {1}", currentPlayer, players.Count));
        playerSwitched = true;
        
    }

    public void buttonClicked()
    {
        Debug.Log("got here");
    }

}