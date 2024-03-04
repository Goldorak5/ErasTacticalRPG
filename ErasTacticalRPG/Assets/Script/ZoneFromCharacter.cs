using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseQuadran
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,

};

public class ZoneFromCharacter : MonoBehaviour
{

    public int howFar;
    public int[] rowsTestOddNumOnly;

    private MouseQuadran mouseQuadran;
    private MapManager mapManager;
    private RangeFinder rangeFinder;
    private BaseCharacter character;
    private List<OverlayTile> abilityZone = new List<OverlayTile>();
    private List<Vector2Int> locationList = new List<Vector2Int>();
    private Vector2Int playerFrontTileGridLocationTopRight;
    private Vector2Int playerFrontTileGridLocationTopLeft;
    private Vector2Int playerFrontTileGridLocationBottomRight;
    private Vector2Int playerFrontTileGridLocationBottomLeft;
    private OverlayTile startingTile;
    private OverlayTile leftSideTile;
    private OverlayTile rightSideTile;

    void Start()
    {
        rangeFinder = new RangeFinder();
        mapManager = MapManager.Instance;
        character = GetComponent<BaseCharacter>();
        startingTile = character.activeTile;
    }

    public List<OverlayTile> GetZoneAbility(int howFarInFront, int[] rows)
    {
        playerFrontTileGridLocationTopRight = new Vector2Int(character.activeTile.grid2DLocation.x + 1, character.activeTile.grid2DLocation.y);
        playerFrontTileGridLocationTopLeft = new Vector2Int(character.activeTile.grid2DLocation.x, character.activeTile.grid2DLocation.y + 1);
        playerFrontTileGridLocationBottomRight = new Vector2Int(character.activeTile.grid2DLocation.x, character.activeTile.grid2DLocation.y - 1);
        playerFrontTileGridLocationBottomLeft = new Vector2Int(character.activeTile.grid2DLocation.x - 1, character.activeTile.grid2DLocation.y);
        OverlayTile playerTile = character.activeTile;
                       // Top Right Quadrant
        if (mouseQuadran == MouseQuadran.TopRight)
        {

            foreach (OverlayTile tile in abilityZone)
            {
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront, playerFrontTileGridLocationTopRight.y)))
            {
                //adjusting the starting tile by what the game designer want the ability range to start
                startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront, playerFrontTileGridLocationTopRight.y)];
                abilityZone.Add(startingTile);
            }
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront + i, playerFrontTileGridLocationTopRight.y)))
                {
                    //adjusting the starting tile by what the game designer want the ability range to start
                    if (!abilityZone.Contains(mapManager.map[new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront + i, playerFrontTileGridLocationTopRight.y)]))
                    {
                        startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront + i, playerFrontTileGridLocationTopRight.y)];
                        abilityZone.Add(startingTile);
                    }
                }
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(rightSideTile.grid2DLocation.x, rightSideTile.grid2DLocation.y - 1)))
                        {
                            rightSideTile = mapManager.map[new Vector2Int(rightSideTile.grid2DLocation.x, rightSideTile.grid2DLocation.y - 1)];
                            abilityZone.Add(rightSideTile);
                        }
                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - 1)))
                    {
                            rightSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - 1)];
                            abilityZone.Add(rightSideTile);
                    }
                    //if already a tile on the right of startingTile add another one
                    if (leftSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(leftSideTile.grid2DLocation.x, leftSideTile.grid2DLocation.y + 1)))
                        {
                            leftSideTile = mapManager.map[new Vector2Int(leftSideTile.grid2DLocation.x, leftSideTile.grid2DLocation.y + 1)];
                            abilityZone.Add(leftSideTile);
                        }
                    }
                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + 1)))
                    {
                            leftSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + 1)];
                            abilityZone.Add(leftSideTile);
                    }
                }
            startingTile = null;
            rightSideTile = null;
            leftSideTile = null;
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }
        }
                      // Top left Quadrant
        else if (mouseQuadran == MouseQuadran.TopLeft)
        {

            foreach (OverlayTile tile in abilityZone)
            {
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront)))
            {
                //adjusting the starting tile by what the game designer want the ability range to start
                startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront)];
                abilityZone.Add(startingTile);
            }
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront + i)))
                {
                    //adjusting the starting tile by what the game designer want the ability range to start
                    if (!abilityZone.Contains(mapManager.map[new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront + i)]))
                    {
                        startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront + i)];
                        abilityZone.Add(startingTile);
                    }
                }
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(rightSideTile.grid2DLocation.x + 1, rightSideTile.grid2DLocation.y)))
                        {
                            rightSideTile = mapManager.map[new Vector2Int(rightSideTile.grid2DLocation.x + 1, rightSideTile.grid2DLocation.y)];
                            abilityZone.Add(rightSideTile);
                        }
                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x + 1, startingTile.grid2DLocation.y)))
                    {
                            rightSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x + 1, startingTile.grid2DLocation.y)];
                            abilityZone.Add(rightSideTile);
                    }
                    //if already a tile on the right of startingTile add another one
                    if (leftSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(leftSideTile.grid2DLocation.x - 1, leftSideTile.grid2DLocation.y)))
                        {
                            leftSideTile = mapManager.map[new Vector2Int(leftSideTile.grid2DLocation.x - 1, leftSideTile.grid2DLocation.y)];
                            abilityZone.Add(leftSideTile);
                        }
                    }
                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if(startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x - 1, startingTile.grid2DLocation.y)))
                    {
                            leftSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x - 1, startingTile.grid2DLocation.y)];
                            abilityZone.Add(leftSideTile);
                    }
                }
                startingTile = null;
                rightSideTile = null;
                leftSideTile = null;
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }
        }
                    // Bottom left Quadrant
        else if (mouseQuadran == MouseQuadran.BottomLeft)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront, playerFrontTileGridLocationBottomLeft.y)))
            {
                //adjusting the starting tile by what the game designer want the ability range to start
                startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront, playerFrontTileGridLocationBottomLeft.y)];
                abilityZone.Add(startingTile);
            }
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront - i, playerFrontTileGridLocationBottomLeft.y)))
                {
                    //adjusting the starting tile by what the game designer want the ability range to start
                    if (!abilityZone.Contains(mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront - i, playerFrontTileGridLocationBottomLeft.y)]))
                    {
                        startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront - i, playerFrontTileGridLocationBottomLeft.y)];
                        abilityZone.Add(startingTile);
                    }
                }
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(rightSideTile.grid2DLocation.x, rightSideTile.grid2DLocation.y + 1)))
                        {
                            rightSideTile = mapManager.map[new Vector2Int(rightSideTile.grid2DLocation.x, rightSideTile.grid2DLocation.y + 1)];
                            abilityZone.Add(rightSideTile);
                        }
                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + 1)))
                    {
                            rightSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + 1)];
                            abilityZone.Add(rightSideTile);
                    }
                    //if already a tile on the right of startingTile add another one
                    if (leftSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(leftSideTile.grid2DLocation.x, leftSideTile.grid2DLocation.y - 1)))
                        {
                            leftSideTile = mapManager.map[new Vector2Int(leftSideTile.grid2DLocation.x, leftSideTile.grid2DLocation.y - 1)];
                            abilityZone.Add(leftSideTile);
                        }
                    }
                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if(startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - 1)))
                    {
                            leftSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - 1 )];
                            abilityZone.Add(leftSideTile);
                    }
                }
                startingTile = null;
                rightSideTile = null;
                leftSideTile = null;
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }
        }
                    //Bottom Right Quadrant                                
        else if (mouseQuadran == MouseQuadran.BottomRight)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront)))
            {
                //adjusting the starting tile by what the game designer want the ability range to start
                startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront)];
                abilityZone.Add(startingTile);
            }
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
//                 Vector2Int locationFront;
//                 Vector2Int locationBack;
//                 Vector2Int locationRight;
//                 Vector2Int locationLeft;
// 
//                 locationFront = new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront - i);
                if (mapManager.map.ContainsKey(new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront - i)))
                {
                    //adjusting the starting tile by what the game designer want the ability range to start
                    if (!abilityZone.Contains(mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront - i)]))
                    {
                        startingTile = mapManager.map[new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront - i)];
                        abilityZone.Add(startingTile);
                    }
                }
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(rightSideTile.grid2DLocation.x - 1, rightSideTile.grid2DLocation.y)))
                        {
                            rightSideTile = mapManager.map[new Vector2Int(rightSideTile.grid2DLocation.x - 1, rightSideTile.grid2DLocation.y)];
                            abilityZone.Add(rightSideTile);
                        }
                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x - 1, startingTile.grid2DLocation.y)))
                    {
                     
                            rightSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x - 1, startingTile.grid2DLocation.y)];
                            abilityZone.Add(rightSideTile);
                    }
                    //if already a tile on the right of startingTile add another one
                    if (leftSideTile != null)
                    {
                        if (mapManager.map.ContainsKey(new Vector2Int(leftSideTile.grid2DLocation.x + 1, leftSideTile.grid2DLocation.y)))
                        {
                            leftSideTile = mapManager.map[new Vector2Int(leftSideTile.grid2DLocation.x + 1, leftSideTile.grid2DLocation.y)];
                            abilityZone.Add(leftSideTile);
                        }
                    }
                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if (startingTile != null && mapManager.map.ContainsKey(new Vector2Int(startingTile.grid2DLocation.x + 1, startingTile.grid2DLocation.y)))
                    {
                            leftSideTile = mapManager.map[new Vector2Int(startingTile.grid2DLocation.x + 1, startingTile.grid2DLocation.y)];
                            abilityZone.Add(leftSideTile);
                    }
                }
                startingTile = null;
                rightSideTile = null;
                leftSideTile = null;
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }
        }

        return abilityZone;
    }

    public List<OverlayTile> SingleTargetZone(OverlayTile startingTile, int range)
    {
        abilityZone = rangeFinder.GetTilesInRange(startingTile, range, true);

        return abilityZone;
    }

    private void GetQuadran()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPosition = character.transform.position;
        Vector3 relativePosition = mousePos - playerPosition;


        if (relativePosition.x >= 0 && relativePosition.y >= 0)
        {
        mouseQuadran = MouseQuadran.TopRight;
        }
        else if (relativePosition.x < 0 && relativePosition.y > 0)
        {
        mouseQuadran = MouseQuadran.TopLeft;
          
        }else if(relativePosition.x <= 0 && relativePosition.y <= 0)
        {
        mouseQuadran = MouseQuadran.BottomLeft;
         
        }else if(relativePosition.x > 0 && relativePosition.y < 0)
        {
        mouseQuadran = MouseQuadran.BottomRight;
        }
    }

    void Update()
    {
        GetQuadran();
        if (character.characterState == CharacterState.Abilities)
        {
             GetZoneAbility(howFar, rowsTestOddNumOnly);
        }

    }
}
