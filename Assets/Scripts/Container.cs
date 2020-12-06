using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    static int[] xPositions = { -166, -149, -132, -115, -98, -81, -64, -47, -30, -13, 4, 21, 38, 55, 72, 89 };
    static int[] yLevelPosition = { -54, -85 };

    static int[] xyWholeBoard = { -161, -54};

    private bool busy = false; // TODO make a pack of containers for every player. make busy when moving over, and not busy when moving from


    private int getContainerIndex(bool onlyContainerNumber = false)
    {
        int row = (int)(this.name[1] - '0') * 16;
        int containerNumber = (this.name[2] == '0' ? (int)(this.name[3] - '0') : 10 + (int)(this.name[3] - '0')) - 1;

        if (onlyContainerNumber)
            return containerNumber;

        return row + containerNumber;
    }

    void Start()
    {
        if (this.name == "WholeBoard")
        {
            this.gameObject.transform.position = new Vector3(xyWholeBoard[0], xyWholeBoard[1], -1);
            return;
        }

        GameController.addContainer(getContainerIndex(), this);
        this.gameObject.transform.position = new Vector3(xPositions[getContainerIndex(true)], yLevelPosition[(getContainerIndex() < 16 ? 0 : 1)], 0);
    }

    
}
