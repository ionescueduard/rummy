using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    static public int getContainerIndexByPosition(Vector3 point)
    {
        int x = (int)Mathf.Round(point.x);
        int y = (int)Mathf.Round(point.y);
        for (int i = 0; i < GameController.xBoardSlots; i++)
        {
            if (x == GameController.xBoardPositions[i])
            {
                return ((y == -54 ? 0 : 1) * GameController.xBoardSlots) + i;
            }
        }
        throw new Exception("Can't find any Container by its position.");
    }

    public int getContainerIndex() { return getYIndex() * GameController.xBoardSlots + getXIndex(); }

    private int getXIndex() { return (this.name[2] == '0' ? (int)(this.name[3] - '0') : 10 + (int)(this.name[3] - '0')) - 1; }

    private int getYIndex() { return (int)(this.name[1] - '0'); }


    void Start()
    {
        GameController.addContainer(getContainerIndex(), this);
        GetComponent<RectTransform>().position = new Vector3(GameController.xBoardPositions[getXIndex()], GameController.yBoardPositions[getYIndex()], 0);
    }

}
