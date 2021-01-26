using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPointer : MonoBehaviour
{
    private int panelIndex;
    private int pairIndex;
    private GameController.SidePosition side; /// 0 for Left StickPointer, 1 for Right StickPointer,

    static public float RIGHT_STICKPOINTER_INITIAL_X = -11.48f;

    /*
     * Position local space:   103  -11.48  45.76  102.2182
     * Distance between two arrows in local space: 57.24
     */

    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        this.panelIndex = (int)(this.name[0] - '0');
        this.pairIndex = (int)(this.name[1] - '0');
        this.side = this.name[2] == 'L' ? GameController.SidePosition.Left : GameController.SidePosition.Right;

    }

    void Awake()
    {
        init();

        GameController.addStickPointer(this, this.panelIndex, this.pairIndex, (int)this.side);
    }
}
