using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerticalArrow : MonoBehaviour, IPointerDownHandler
{
    private int panelIndex; // coresponds to player index
    private GameController.SidePosition side; /// 0 for Left VerticalArrow, 1 for Right VerticalArrow,
    private GameController.ShiftDirection direction; /// direction in which pair moves. multiplying with -1 means moving Left, and with 1 moving Right


    /*--------------------------*/
    /*--------- Events --------*/
    /*------------------------*/
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        bool canShiftAgain = GameController.getPlayer(this.panelIndex).shiftPairs(this.direction);
        /// make opposite arrow active, as it can be a valid action to shift pair backwards
        makeOpositeVisible();

        if (!canShiftAgain)
        {
            this.gameObject.SetActive(false);
        }
    }



    /*--------------------------*/
    /*----- Initialization ----*/
    /*------------------------*/
    private void init()
    {
        this.panelIndex = (int)(this.name[0] - '0');
        this.side = this.name[1] == 'L' ? GameController.SidePosition.Left : GameController.SidePosition.Right;
        this.direction = this.name[1] == 'L' ? GameController.ShiftDirection.Left : GameController.ShiftDirection.Right;
    }

    void Awake()
    {
        init();

        GameController.addVerticalArrow(this, this.panelIndex, (int)this.side);
    }



    /*--------------------------*/
    /*--------- Utils ---------*/
    /*------------------------*/
    private void makeOpositeVisible()
    {
        GameController.setVerticalArrowVisiblity(this.panelIndex, this.side == GameController.SidePosition.Left ? GameController.SidePosition.Right : GameController.SidePosition.Left, true);
    }


    public int getPanelIndex() { return this.panelIndex; }

}
