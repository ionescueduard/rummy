using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject panelToPlace;
    private int index; /// player's index, the index is always from 0 to 3 in order as on board

    static private int NUMBER_OF_CARDS_PER_ROW = 16;
    static private int NUMBER_OR_ROWS = 2;
    private List<Card> cards = new List<Card>();
    private Card[][] cardsOnPositions = new Card[NUMBER_OR_ROWS][];

    private Card upperSpecial; //TODO
    private Card lowerSpecial; //TODO

    private List<List<Card>> cardsOnTable = new List<List<Card>>();
    static public int MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE = 5;
    static public int MAX_NUMBER_OF_PAIRS_VISIBLE = 5;
    private List<int> currentFirstInPair = new List<int>(); //[MAX_NUMBER_OF_PAIRS_VISIBLE];
    private int currentFirstOnTable;
    


    /*--------------------------*/
    /*-------- Settings -------*/
    /*------------------------*/
    bool dropCardsOnlyWithOnesPairFeature = false;





    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        for (int i = 0; i < NUMBER_OR_ROWS; i++)
            cardsOnPositions[i] = new Card[NUMBER_OF_CARDS_PER_ROW];

        this.index = (int)(this.name[6] - '0');
        this.panelToPlace = GameObject.Find(String.Format("OnTable{0}", this.index));
    }

    private void Awake()
    {
        init();
        GameController.addPlayer(this);
    }




    /*-------------Pairs-------------*/
    /*--------- Right Pairs --------*/
    /*-----------------------------*/
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
        while (i < NUMBER_OF_CARDS_PER_ROW)
        {
            /// check if we have at least 3 in a row
            if (i + 2 < NUMBER_OF_CARDS_PER_ROW && row[i] != null && row[i+1] != null && row[i+2] != null)
            {
                int startIndex = i;
                i += 3;
                /// append more conneceted cards
                while (i < NUMBER_OF_CARDS_PER_ROW && row[i])
                {
                    i++;
                }

                List<Card> tmp = new List<Card>();
                for (int j = startIndex; j < i; j++)
                {
                    tmp.Add(row[j]);
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
            int prevNumber;
            int color;
            int startWith;
            if (pair[0].isJoker())
            {
                if (pair[1].getNumber() == 1)
                    return false;

                prevNumber = pair[1].getNumber();
                color = pair[1].getColor();
                startWith = 2;
            }
            else
            {
                prevNumber = pair[0].getNumber();
                color = pair[0].getColor();
                startWith = 1;
            }    
            
            for (int i = startWith; i < pair.Count; i++)
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

    private List<List<Card>> getPairs()
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




    /*-------------Pairs-------------*/
    /*--------- Pairs Placement ----*/
    /*-----------------------------*/
    private void placePairOnTable(List<Card> pair, int row)
    {
        float cardXPosition = GameController.xTableFormations[this.index, 0];
        float cardYPosition = GameController.yTableFormations[0];
        for (int x = 0; x < pair.Count; x++, cardXPosition += GameController.xTableFormationsGap)
        {
            // on table scale: 0.795
            pair[x].transform.localScale = new Vector3(0.795f, 0.795f, 0);
            pair[x].transform.position = new Vector3(cardXPosition, cardYPosition - row * GameController.yTableFormationsGap, 0);
            pair[x].transform.SetParent(panelToPlace.transform);
        }

        currentFirstInPair.Add(0);

        if (row < MAX_NUMBER_OF_PAIRS_VISIBLE)
        {
            GameController.initializeHorizontalArrows(this.index, row, pair.Count);
            GameController.initializeStickPointer(this.index, row, pair.Count);
        }
    }

    private void removePairFromBoard(List<Card> pair)
    {
        foreach (Card card in pair)
        {
            cardsOnPositions[card.getOnBoardY()][card.getOnBoardX()] = null;
            /// -1 represents on Table
            card.setOnBoardPosition(-1, -1);

            /// also make unmovable on table
            card.setMovable(false);
        }
    }

    public void placePairsOnTable()
    {
        List<List<Card>> pairs = getPairs();
        foreach (List<Card> pair in pairs)
        {
            removePairFromBoard(pair);
            placePairOnTable(pair, cardsOnTable.Count);
            cardsOnTable.Add(pair);
        }
        currentFirstOnTable = 0;
        GameController.initializeVerticalArrows(this.index, cardsOnTable.Count);

    }




    /*-------------Pairs-------------*/
    /*--------- Pairs Points -------*/
    /*-----------------------------*/
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




    /*-------------Pairs-------------*/
    /*--------- Pairs Shift --------*/
    /*-----------------------------*/
    public bool canPairShift(int pairIndex, GameController.ShiftDirection direction)
    {
        int pairIndexTransformedToVisible = pairIndex - currentFirstOnTable;
        if (direction == GameController.ShiftDirection.Left)
            return currentFirstInPair[pairIndexTransformedToVisible] + cardsOnTable[pairIndexTransformedToVisible].Count > MAX_NUMBER_OF_CARDS_IN_PAIR_VISIBLE;
        return currentFirstInPair[pairIndexTransformedToVisible] < 0;
    }
    
    public bool canPairsShift(GameController.ShiftDirection direction)
    {
        /// Direction -> Left means Down, Right means Up
        if (direction == GameController.ShiftDirection.Right)
            return currentFirstOnTable + cardsOnTable.Count > MAX_NUMBER_OF_PAIRS_VISIBLE;
        return currentFirstOnTable < 0;
    }

    /*
     * returns wether pair can be shifted again.
     * information used to make the arrow visible/invisible
     */
    public bool shiftCardsForPair(int pairIndex, GameController.ShiftDirection direction)
    {
        int pairIndexTransformedToVisible = pairIndex - currentFirstOnTable;
        if (canPairShift(pairIndex, direction))
        {
            currentFirstInPair[pairIndexTransformedToVisible] += (int)direction;
            foreach (Card card in cardsOnTable[pairIndexTransformedToVisible])
                card.transform.position = new Vector3(card.transform.position.x + (int)direction * GameController.xTableFormationsGap, card.transform.position.y, 0);
        }

        return canPairShift(pairIndex, direction); 
    }

    public bool shiftPairs(GameController.ShiftDirection direction)
    {
        /// Direction -> Left means Down, Right means Up
        if (canPairsShift(direction))
        {
            currentFirstOnTable += -(int)direction;
            foreach (List<Card> pair in cardsOnTable)
                foreach (Card card in pair)
                    card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y + (int)direction * GameController.yTableFormationsGap, 0);
        }

        GameController.reinitializeHorizontalArrows(this.index, cardsOnTable, currentFirstInPair, -currentFirstOnTable);
        GameController.reinitializeStickPointers(this.index, cardsOnTable, currentFirstInPair, -currentFirstOnTable);

        return canPairsShift(direction);
    }




    /*-------------Board-------------*/
    /*--------- Board Slots --------*/
    /*-----------------------------*/
    public bool isSlotEmpty(int index) { return cardsOnPositions[index / NUMBER_OF_CARDS_PER_ROW][index % NUMBER_OF_CARDS_PER_ROW] == null; }

    public void moveCardFromSlotToSlot(Card card, int fromIndex, int toIndex)
    {
        int fromY = fromIndex / NUMBER_OF_CARDS_PER_ROW;
        int fromX = fromIndex % NUMBER_OF_CARDS_PER_ROW;
        int toY = toIndex / NUMBER_OF_CARDS_PER_ROW;
        int toX = toIndex % NUMBER_OF_CARDS_PER_ROW;
        cardsOnPositions[toY][toX] = cardsOnPositions[fromY][fromX];
        cardsOnPositions[fromY][fromX] = null;
        card.setOnBoardPosition(toY, toX);
    }




    /*-------------Board-------------*/
    /*--------- Board Edits --------*/
    /*-----------------------------*/
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
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j++, cards[i++]);
            addCardOnBoardAt(0, j, cards[i++]);
            addCardOnBoardAt(1, j, cards[i]);
        }  
    }

    public void activateBoard(bool activate)
    {
        foreach (Card card in cards)
        {
            /// if card not placed on table
            if (card.getOnBoardX() != -1)
                card.gameObject.SetActive(activate);
        }
    }

    private void addCardOnBoardAt(int y, int x, Card card)
    {
        /// Debug.Log(String.Format("Card: {0} - x:{1} y:{2} z:{3}", card.gameObject.name, xPositions[index], yLevelPosition[level], 0));
        card.setOnBoardPosition(y, x);
        cards.Add(card);
        cardsOnPositions[y][x] = card;
        card.transform.position = new Vector3(GameController.xBoardPositions[x], GameController.yBoardPositions[y], 0);
        card.setMovable(true);
    }




    /*-------------Utils-------------*/
    /*------------------------------*/
    /*-----------------------------*/
    public int getIndex()
    {
        return this.index;
    }
}
