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
    static public float yTableFormationsGap = 14.8f;


    static private Card[] cardsToIdentifyByIndex = new Card[106];
    static private Card[] cardsToDrawFrom = new Card[106];
    static private int cardsCurrentIndex = 0;
    static private bool cardPicked;

    static private int containersNumber = 32;
    static private Container[] containers = new Container[containersNumber];

    static private List<Player> players = new List<Player>();
    static private int currentPlayer;
    static private bool playerSwitched = false;

    static private HorizontalArrow[,,] horizontalArrows = new HorizontalArrow[4, 5, 2];
    static private VerticalArrow[,] verticalArrows = new VerticalArrow[4, 2];
    static private StickPointer[,,] stickPointers = new StickPointer[4, 5, 2];
    static private StickPointer lastActiveStickPointer;

    static private Random rng = new Random();

    static public float DISTANCE_BETWEEN_TWO_OBJECTS_LOCAL_SPACE = 57.24f;

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
            numToAdd = first ? 28 : 14;
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

    static public Player getPlayer(int index)
    {
        return players[index];
    }

    static private void nextPlayerIndex() { currentPlayer = (currentPlayer + 1) % players.Count; }

    static private void sortPlayers()
    {
        players.Sort((x, y) => x.getIndex().CompareTo(y.getIndex()));
    }




    /*--------------------------*/
    /*---- VerticalArrow ----*/
    /*------------------------*/
    internal static void addVerticalArrow(VerticalArrow arrow, int player, int side)
    {
        verticalArrows[player, side] = arrow;
        arrow.gameObject.SetActive(false);
    }

    static public void initializeVerticalArrows(int player, int pairsCount)
    {
        VerticalArrow[] ar = { verticalArrows[player, (int)SidePosition.Left], verticalArrows[player, (int)SidePosition.Right] };

        bool morePairsThenMaxSpace = pairsCount > Player.MAX_NUMBER_OF_PAIRS_VISIBLE; /// make active for shifting if more pairs then max max and if not, no need for shifting,

        ar[0].gameObject.SetActive(false); /// ar[0] will be made active when shift action will be possible (meaning after first up shift)
        ar[1].gameObject.SetActive(morePairsThenMaxSpace);
    }

    internal static void setVerticalArrowVisiblity(int player, SidePosition side, bool visible)
    {
        verticalArrows[player, (int)side].gameObject.SetActive(visible);
    }




    /*--------------------------*/
    /*---- HorizontalArrow ----*/
    /*------------------------*/
    static public void addHorizontalArrow(HorizontalArrow arrow, int player, int row, int side)
    {
        horizontalArrows[player, row, side] = arrow;
        arrow.gameObject.SetActive(false);
    }

    static public void initializeHorizontalArrows(int player, int row, int pairCount)
    {
        HorizontalArrow[] ar = { horizontalArrows[player, row, (int)SidePosition.Left], horizontalArrows[player, row, (int)SidePosition.Right] };

        bool pairLongerThenMaxSpace = pairCount > Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE; /// make visible for shifting if pair longer than max and if not, no need for shifting,
                                                                                              /// active, but invisible, only to get mouseEnter/Exit for appending cards to the pairs
        ar[0].setVisible(false); /// ar[0] will be made visible when shift action will be possible (meaning after first left shift)
        ar[1].setVisible(pairLongerThenMaxSpace);

        /// ar[0] never moves
        int arPositionIndex = Math.Min(Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE, pairCount) - 3;
        float x = HorizontalArrow.RIGHT_ARROW_INITIAL_X + arPositionIndex * DISTANCE_BETWEEN_TWO_OBJECTS_LOCAL_SPACE;
        ar[1].transform.localPosition = new Vector3(x, ar[1].transform.localPosition.y, 0);

        ar[0].gameObject.SetActive(true);
        ar[1].gameObject.SetActive(true);
    }

    static public void reinitializeHorizontalArrows(int player, List<List<Card>> pairs, List<int> currentFirstInPair, int startFromPair)
    {
        int i = startFromPair;
        int n = startFromPair + Player.MAX_NUMBER_OF_PAIRS_VISIBLE;
        for (int j = 0; i < n; i++, j++)
        {
            HorizontalArrow[] ar = { horizontalArrows[player, j, (int)SidePosition.Left], horizontalArrows[player, j, (int)SidePosition.Right] };

            ar[0].setVisible(currentFirstInPair[i] < 0);
            ar[1].setVisible(currentFirstInPair[i] + pairs[i].Count > Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE);

            /// ar[0] never moves
            int arPositionIndex = Math.Min(Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE, pairs[i].Count) - 3;
            float x = HorizontalArrow.RIGHT_ARROW_INITIAL_X + arPositionIndex * DISTANCE_BETWEEN_TWO_OBJECTS_LOCAL_SPACE;
            ar[1].transform.localPosition = new Vector3(x, ar[1].transform.localPosition.y, 0);
        }
    }

    static public void setHorizontalArrowVisiblity(int player, int row, SidePosition side, bool visible)
    {
        horizontalArrows[player, row, (int)side].setVisible(visible);
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
        lastActiveStickPointer = stickPointers[player, row, side];
    }

    static public void deactivateStickPointerByObject()
    {
        if (lastActiveStickPointer != null)
        {
            lastActiveStickPointer.gameObject.SetActive(false);
        }
    }

    static public void initializeStickPointer(int player, int row, int pairCount)
    {
        /// sp[0] never moves
        int spPositionIndex = Math.Min(Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE, pairCount) - 3;
        float x = StickPointer.RIGHT_STICKPOINTER_INITIAL_X + spPositionIndex * DISTANCE_BETWEEN_TWO_OBJECTS_LOCAL_SPACE;
        stickPointers[player, row, (int)SidePosition.Right].transform.localPosition = new Vector3(x, stickPointers[player, row, (int)SidePosition.Right].transform.localPosition.y, 0);
    }

    static public void reinitializeStickPointers(int player, List<List<Card>> pairs, List<int> currentFirstInPair, int startFromPair)
    {
        int i = startFromPair;
        int n = startFromPair + Player.MAX_NUMBER_OF_PAIRS_VISIBLE;
        for (int j = 0; i < n; i++, j++)
        {
            /// sp[0] never moves
            int spPositionIndex = Math.Min(Player.MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE, pairs[i].Count) - 3;
            float x = StickPointer.RIGHT_STICKPOINTER_INITIAL_X + spPositionIndex * DISTANCE_BETWEEN_TWO_OBJECTS_LOCAL_SPACE;
            stickPointers[player, j, (int)SidePosition.Right].transform.localPosition = new Vector3(x, stickPointers[player, j, (int)SidePosition.Right].transform.localPosition.y, 0);
        }
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