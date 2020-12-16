using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject panelToPlace;

    private static int numberOfCardsPerRow = 16;
    private static int numberOfRows = 2;
    private List<Card> cards = new List<Card>();
    private Card[][] cardsOnPositions = new Card[numberOfRows][];

    private Card upperSpecial; //TODO
    private Card lowerSpecial; //TODO

    /*--------------------------*/
    /*-------- Settings -------*/
    /*------------------------*/
    bool dropCardsOnlyWithOnesPairFeature = false;




    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        for (int i = 0; i < numberOfRows; i++)
            cardsOnPositions[i] = new Card[numberOfCardsPerRow];
    }

    private void Awake()
    {
        init();
        GameController.addPlayer(this);
    }




    private bool isRunPair(List<Card> pair)
    {
        if (pair[0].isJoker())
        {
            return pair[1].getNumber() != pair[2].getNumber();
        }
        else
        {
            return pair[1].isJoker() ? (pair[0].getNumber() != pair[2].getNumber()) : (pair[0].getNumber() != pair[1].getNumber());
        }
    }

    private List<List<Card>> getConnectedPairs(Card[] row)
    {
        List<List<Card>> rez = new List<List<Card>>();

        int i = 0;
        while (i < numberOfCardsPerRow)
        {
            /// check if we have at least 3 in a row
            if (i + 2 < numberOfCardsPerRow && row[i] != null && row[i+1] != null && row[i+2] != null)
            {
                int startIndex = i;
                i += 3;
                /// append more conneceted cards
                while (i < numberOfCardsPerRow && row[i])
                {
                    i++;
                }

                List<Card> tmp = new List<Card>();
                for (int j = 0; j < i; j++)
                {
                    tmp.Add(row[j++]);
                }
                rez.Add(tmp);
            }
            else
            {
                i++;
            }
        }

        return rez;
    }

    private bool isRightPair(List<Card> pair)
    {
        bool hasJoker = false;
        foreach (Card card in pair)
        {
            if (card.isJoker())
            {
                if (hasJoker)
                    return false;
                hasJoker = true;
            }
        }

        if (isRunPair(pair))
        {
            int color = pair[0].getColor();
            int prevNumber = pair[0].getColor();
            for (int i = 1; i < pair.Count; i++)
            {
                if (pair[i].isJoker())
                {
                    prevNumber++;
                    continue;
                }

                if ((pair[i].getColor() != color) || (prevNumber != 13 && prevNumber + 1 != pair[i].getNumber()) || (prevNumber == 13 && (pair[i].getNumber() != 1 || i != pair.Count - 1)) )
                    return false;

                prevNumber = pair[i].getNumber();
            }
        }
        else
        {
            if (pair.Count > 4)
                return false;

            for (int i = 0; i < pair.Count; i++)
                for (int j = i + 1; j < pair.Count; j++)
                    if (pair[i].getColor() == pair[j].getColor()) // joker color is " ", so it would pass this test
                        return false;
        }

        return true;
    }

    public List<List<Card>> getPairs()
    {
        List<List<Card>> rez = new List<List<Card>>();
        for (int row = 0; row <= 1; row++)
        {
            List<List<Card>> pairs = getConnectedPairs(cardsOnPositions[row]);
            for (int i = 0; i < pairs.Count; i++)
            {
                if (isRightPair(pairs[i]))
                {
                    rez.Add(pairs[i]);
                }
            }
        }
        return rez;
    }

    private int getJokerReplacementNumber(List<Card> pair, int i)
    {
        if (isRunPair(pair))
        {
            if (i == 0)
                return pair[i + 1].getNumber() - 1;
            if (i > 0 && i < pair.Count - 1)
                return pair[i - 1].getNumber() + 1;
            if (i == pair.Count - 1)
                return pair[i - 1].getNumber() == 13 ? 1 : pair[i - 1].getNumber() + 1;
        }
        else
        {
            if (i == 0)
                return pair[1].getNumber();
            else
                return pair[0].getNumber();
        }

        throw new Exception("Should never get here! Has a return on every branch.");
    }

    public int getPairPoints(List<Card> pair, bool beforeEndGame)
    {
        if (dropCardsOnlyWithOnesPairFeature)
            if (pair[0].getNumber() == 1 && pair[1].getNumber() == 1)
                return 25 * pair.Count;

        int rez = 0;
        for (int i = 0; i < pair.Count; i++)
        {
            /// joker
            if (pair[i].getNumber() == -1)
            {
                if (beforeEndGame)
                    rez += getJokerReplacementNumber(pair, i);
                else
                    rez += 50;
                continue;
            }
            if (pair[i].getNumber() < 10 && pair[i].getNumber() > 1)
                rez += 5; 
            else if (pair[i].getNumber() >= 10)
                rez += 10;
            else if (pair[i].getNumber() == 1)
                if (beforeEndGame)
                    if (i == 0)   /// if is 1 2 3
                        rez += 5;
                    else          /// if is 12 13 1
                        rez += 10;
                else
                    rez += 25;
        }

        return rez;
    }


    public bool isSlotEmpty(int index) { return cardsOnPositions[index / numberOfCardsPerRow][index % numberOfCardsPerRow] == null; }

    public void moveCardFromSlotToSlot(int fromIndex, int toIndex)
    {
        int fromY = fromIndex / numberOfCardsPerRow;
        int fromX = fromIndex % numberOfCardsPerRow;
        int toY = toIndex / numberOfCardsPerRow;
        int toX = toIndex % numberOfCardsPerRow;
        cardsOnPositions[toY][toX] = cardsOnPositions[fromY][fromX];
        cardsOnPositions[fromY][fromX] = null;
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

    public void activateBoard(bool activate) { foreach (Card card in cards) { card.gameObject.SetActive(activate); } }

    private void addCardOnBoardAt(int y, int x, Card card)
    {
        /// Debug.Log(String.Format("Card: {0} - x:{1} y:{2} z:{3}", card.gameObject.name, xPositions[index], yLevelPosition[level], 0));
        cards.Add(card);
        cardsOnPositions[y][x] = card;
        card.transform.position = new Vector3(GameController.xBoardPositions[x], GameController.yBoardPositions[y], 0); 
    }


}
