using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    static public int xBoardSlots = 16;
    static public int[] yBoardPositions = { -54, -85 };
    static public int[] xBoardPositions = { -166, -149, -132, -115, -98, -81, -64, -47, -30, -13, 4, 21, 38, 55, 72, 89 };
    
    static public float[] yTableFormations = { 70f, 55.2f, 40.4f, 25.6f, 10.8f };
    static public float[,] xTableFormations = { { -137f,  -126.4f, -115.8f, -105.2f, -94.6f },
                                                { -58.5f,  -47.9f,  -37.3f,  -26.7f, -16.1f },
                                                {  20.0f,   30.6f,   41.2f,   51.8f,  62.4f },
                                                {  98.5f,  109.1f,  119.7f,  130.3f, 140.9f } };
    static public float xTableFormationsGap = 10.6f;


    static private Card[] cardsToIdentifyByIndex = new Card[106];
    static private Card[] cardsToDrawFrom = new Card[106];
    static private int cardsCurrentIndex = 0;
    static private bool cardPicked;

    static private int containersNumber = 32;
    static private Container[] containers = new Container[containersNumber];

    static private List<Player> players = new List<Player>();
    static private int currentPlayer;
    static private bool playerSwitched = false;

    static private Arrow[,,] arrows = new Arrow[4, 5, 2];
    static private StickPointer[,,] stickPointers = new StickPointer[4, 5, 2];

    static private Random rng = new Random();


    public enum ShiftDirection : int
    {
        Left = -1,
        Right = 1
    }

    public enum SidePosition : int
    {
        Left = 0,
        Right = 1
    }


    /*--------------------------*/
    /*--------- Frame ---------*/
    /*------------------------*/
    private void Start()
    {
        sortPlayers();

        //ShuffleCards();

        int numToAdd;
        List<Card> init = new List<Card>();
        for (int i = 0; i < players.Count; i++)
        {
            bool first = i == 0;
            numToAdd = first ? 15 : 14;
            init.Clear();
            for (int j = 0; j < numToAdd; j++)
            {
                init.Add(cardsToDrawFrom[cardsCurrentIndex++]);
            }
            players[i].initializeBoard(init, first);
        }

        //Initialize atuu


        /// make currentPlayer last player so it would go to first and start the game
        currentPlayer = players.Count - 1; 
        playerSwitched = true;
    }

    private void Update()
    {
        if (playerSwitched)
        {
            players[currentPlayer].activateBoard(false);
            nextPlayerIndex();
            players[currentPlayer].activateBoard(true);


            playerSwitched = false;
        }
    }



    /*--------------------------*/
    /*--------- Logic ----------*/
    /*------------------------*/
    static private void ShuffleCards()
    {
        int n = 106;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card tmp = cardsToDrawFrom[k];
            cardsToDrawFrom[k] = cardsToDrawFrom[n];
            cardsToDrawFrom[n] = tmp;
        }
    }



    /*--------------------------*/
    /*--------- Card ----------*/
    /*------------------------*/

    static private Card[] getCardsPack()
    {
        return cardsToDrawFrom; //decide which card pack
    }

    static public void addCard(int index, Card card)
    {
        cardsToIdentifyByIndex[index] = card;
        cardsToDrawFrom[index] = card;
        card.gameObject.SetActive(false);
    }

    static public Card getCard(int index) { return cardsToIdentifyByIndex[index]; }

    static public void pickCard() { cardPicked = true; }

    static public void dropCard() { cardPicked = false; }

    static public bool isCardPicked() { return cardPicked; }



    /*--------------------------*/
    /*------- Container -------*/
    /*------------------------*/
    static public void addContainer(int index, Container container)
    {
        containers[index] = container;
        //container.gameObject.SetActive(false);
    }

    static public Container[] getContainers() { return containers; }

    static public int getContainersNumber() { return containersNumber; }



    /*--------------------------*/
    /*-------- Player ---------*/
    /*------------------------*/
    static public void addPlayer(Player player)
    {
        players.Add(player);
    }

    static public Player getCurrentPlayer() { return players[currentPlayer]; }

    static private void nextPlayerIndex() { currentPlayer = (currentPlayer + 1) % players.Count; }

    static private void sortPlayers()
    {
        players.Sort((x, y) => x.getIndex().CompareTo(y.getIndex()));
    }



    /*--------------------------*/
    /*--------- Arrow ---------*/
    /*------------------------*/
    static public void addArrow(Arrow arrow, int player, int row, int side)
    {
        arrows[player, row, side] = arrow;
        arrow.gameObject.SetActive(false);
    }

    static public void activateArrows(int player, int row)
    {
        arrows[player, row, (int)SidePosition.Left].gameObject.SetActive(true);
        arrows[player, row, (int)SidePosition.Right].gameObject.SetActive(true);
    }



    /*--------------------------*/
    /*----- StickPointer ------*/
    /*------------------------*/
    static public void addStickPointer(StickPointer stickPointer, int player, int row, int side)
    {
        stickPointers[player, row, side] = stickPointer;
        stickPointer.gameObject.SetActive(false);
    }

    static public void activateStickPointer(bool value, int player, int row, int side)
    {
        stickPointers[player, row, side].gameObject.SetActive(value);
    }




    /*--------------------------*/
    /*-------- Buttons --------*/
    /*------------------------*/
    public void nextPlayer() { playerSwitched = true; }

    public void placeCards()
    {
        // 94 width for every card
        //  7 for borders, 154.3 height
        players[currentPlayer].placePairsOnTable();
    }




}