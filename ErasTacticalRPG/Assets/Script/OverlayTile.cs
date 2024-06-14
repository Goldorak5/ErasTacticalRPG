using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    //BaseCharacter
    public BaseCharacter tileCharacter;

    //cost from the origin point
    public int G;

    //total cost to the end point
    public int H;

    //total best cost to move for manathan distance for movement
    public int F { get { return G + H; } }

    int moveCost;
    public bool isBlocked;

    //initialize in pathfinder for movement purposes
    public OverlayTile previousTile;

    public bool isMovingTile = false;
    public bool isAttackingTile = false;

    //it is initialize in the map manager in the 3 nested loop
    public Vector3Int gridLocation;

    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowAttackTile()
    {
        //red Tile
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
    }
    public void ShowHealingTile()
    {
        //Green tile
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.3f);
    }
    public void ShowRangeTile()
    {
        //blue tile
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0.3f);
    }
}
