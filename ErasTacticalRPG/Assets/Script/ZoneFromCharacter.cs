using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MouseQuadran
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,

};

public class ZoneFromCharacter : MonoBehaviour
{
    //test purpose
    public int howFar;
    public int[] rowsTestOddNumOnly;

    private MouseQuadran mouseQuadran;
    private TurnManager turnManager;
    private MapManager mapManager;
    private RangeFinder rangeFinder;
    private BaseCharacter character;
    private MouseController mouseControllerScript;

    //list of the tiles highlighted for the ability
    public List<OverlayTile> abilityZone = new List<OverlayTile>();

    //first initialize vectors
    private bool rightVectorInitialize;
    private bool leftVectorInitialize;
    private bool startingTileVectorInitialize;

    //vector on each side of character CAC
    private Vector2Int playerFrontTileGridLocationTopRight;
    private Vector2Int playerFrontTileGridLocationTopLeft;
    private Vector2Int playerFrontTileGridLocationBottomRight;
    private Vector2Int playerFrontTileGridLocationBottomLeft;

    //vectors for the Zone Tiles
    private Vector2Int rightSideVector;
    private Vector2Int leftSideVector;
    private Vector2Int startingTileVector;

    void Start()
    {
        rangeFinder = new RangeFinder();
        mapManager = MapManager.Instance;
        character = GetComponent<BaseCharacter>();
        turnManager = FindAnyObjectByType<TurnManager>();
        mouseControllerScript = FindAnyObjectByType<MouseController>();
    }

    void Update()
    {
        GetQuadran();
        if (character.characterState == CharacterState.Abilities && 
            turnManager.currentPlayerTurn == character && 
            turnManager.currentPlayerTurn.IsMyTurn)
        {
             GetZoneAbility(howFar, rowsTestOddNumOnly);
            /*SingleTargetZone(character.activeTile, 1);*/
        }
        if (character.characterState != CharacterState.Abilities &&
            abilityZone.Count > 0 && 
            turnManager.currentPlayerTurn == character && 
            turnManager.currentPlayerTurn.IsMyTurn)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.isAttackingTile = false;
                tile.HideTile();
            }
        }
    }

    private void InitializeVectorCAC()
    {
        playerFrontTileGridLocationTopRight = new Vector2Int(character.activeTile.grid2DLocation.x + 1, character.activeTile.grid2DLocation.y);
        playerFrontTileGridLocationTopLeft = new Vector2Int(character.activeTile.grid2DLocation.x, character.activeTile.grid2DLocation.y + 1);
        playerFrontTileGridLocationBottomRight = new Vector2Int(character.activeTile.grid2DLocation.x, character.activeTile.grid2DLocation.y - 1);
        playerFrontTileGridLocationBottomLeft = new Vector2Int(character.activeTile.grid2DLocation.x - 1, character.activeTile.grid2DLocation.y);
    }

    private void ReinitializingVectors()
    {
        rightSideVector = default(Vector2Int);
        leftSideVector = default(Vector2Int);
        startingTileVector = default(Vector2Int);
        rightVectorInitialize = false;
        leftVectorInitialize = false;
        startingTileVectorInitialize = false;
    }

    public List<OverlayTile> GetZoneAbility(int howFarInFront, int[] rows)
    {
        InitializeVectorCAC();
        OverlayTile playerTile = character.activeTile;
        List<Vector2Int> locationToCheck = new List<Vector2Int>();

        // Top Right Quadrant
        if (mouseQuadran == MouseQuadran.TopRight)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.isAttackingTile = false;
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                  //adjusting the starting tile by what the game designer want the ability range to start
                startingTileVector = new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront + i, playerFrontTileGridLocationTopRight.y);
                locationToCheck.Add(new Vector2Int(playerFrontTileGridLocationTopRight.x + howFarInFront + i, playerFrontTileGridLocationTopRight.y));
                startingTileVectorInitialize = true;
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(rightSideVector.x, rightSideVector.y - 1);
                        locationToCheck.Add(rightSideVector);

                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTileVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(startingTileVector.x, startingTileVector.y - 1);
                        locationToCheck.Add(rightSideVector);
                        rightVectorInitialize = true;
                    }

                    //if already a tile on the right of startingTile add another one
                    if (leftVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(leftSideVector.x, leftSideVector.y + 1);
                        locationToCheck.Add(leftSideVector);
                    }

                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if (startingTileVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(startingTileVector.x, startingTileVector.y + 1);
                        locationToCheck.Add(leftSideVector);
                        leftVectorInitialize = true;
                    }
                }
                ReinitializingVectors();

            }
            foreach (Vector2Int vec in locationToCheck)
            {
                if (mapManager.map.ContainsKey(vec) && !abilityZone.Contains(mapManager.map[vec]))
                {
                    abilityZone.Add(mapManager.map[vec]);
                }
            }

            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
                tile.isAttackingTile = true;
            }
            
        }
        // Top left Quadrant
        else if (mouseQuadran == MouseQuadran.TopLeft)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.HideTile();
                tile.isAttackingTile = false;
            }
            abilityZone = new List<OverlayTile>();
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                //                     //adjusting the starting tile by what the game designer want the ability range to start
                startingTileVector = new Vector2Int(playerFrontTileGridLocationTopLeft.x, playerFrontTileGridLocationTopLeft.y + howFarInFront + i);
                locationToCheck.Add(startingTileVector);
                startingTileVectorInitialize = true;
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(rightSideVector.x + 1, rightSideVector.y);
                        locationToCheck.Add(rightSideVector);
                    }
                    //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTileVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(startingTileVector.x + 1, startingTileVector.y);
                        locationToCheck.Add(rightSideVector);
                        rightVectorInitialize = true;
                    }
                    //if already a tile on the right of startingTile add another one
                    if (leftVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(leftSideVector.x - 1, leftSideVector.y);
                        locationToCheck.Add(leftSideVector);
                    }
                    //if there are no tile on the Left add one and put it in the variable Leftside
                    else if (startingTileVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(startingTileVector.x - 1, startingTileVector.y);
                        locationToCheck.Add(leftSideVector);
                        leftVectorInitialize = true;
                    }
                }
                ReinitializingVectors();
            }
            foreach (Vector2Int vec in locationToCheck)
            {
                if (mapManager.map.ContainsKey(vec) && !abilityZone.Contains(mapManager.map[vec]))
                {
                    abilityZone.Add(mapManager.map[vec]);
                }
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
                tile.isAttackingTile = true;
            }
        }
        // Bottom left Quadrant
        else if (mouseQuadran == MouseQuadran.BottomLeft)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.isAttackingTile = false;
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();
            //add a tile for each elements in the array in front
            for (int i = 0; i < rows.Length; i++)
            {
                //                     //adjusting the starting tile by what the game designer want the ability range to start
                startingTileVector = new Vector2Int(playerFrontTileGridLocationBottomLeft.x - howFarInFront - i, playerFrontTileGridLocationBottomLeft.y);
                locationToCheck.Add(startingTileVector);
                startingTileVectorInitialize = true;
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(rightSideVector.x, rightSideVector.y - 1);
                        locationToCheck.Add(rightSideVector);
                    }
                    //                     //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTileVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(startingTileVector.x, startingTileVector.y - 1);
                        locationToCheck.Add(rightSideVector);
                        rightVectorInitialize = true;
                    }
                    //                     //if there are no tile on the Left add one and put it in the variable Leftside
                    if (leftVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(leftSideVector.x, leftSideVector.y + 1);
                        locationToCheck.Add(leftSideVector);
                    }
                    //if there are no tile on the left ad one and put it in the variable Leftside
                    else if (startingTileVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(startingTileVector.x, startingTileVector.y + 1);
                        locationToCheck.Add(leftSideVector);
                        leftVectorInitialize = true;
                    }
                }
                ReinitializingVectors();
            }
            foreach (Vector2Int vec in locationToCheck)
            {
                if (mapManager.map.ContainsKey(vec) && !abilityZone.Contains(mapManager.map[vec]))
                {
                    abilityZone.Add(mapManager.map[vec]);
                }
            }
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
                tile.isAttackingTile = true;
            }
        }
        //Bottom Right Quadrant                                
        else if (mouseQuadran == MouseQuadran.BottomRight)
        {
            foreach (OverlayTile tile in abilityZone)
            {
                tile.isAttackingTile = false;
                tile.HideTile();
            }
            abilityZone = new List<OverlayTile>();

            for (int i = 0; i < rows.Length; i++)
            {
                //                     //adjusting the starting tile by what the game designer want the ability range to start
                startingTileVector = new Vector2Int(playerFrontTileGridLocationBottomRight.x, playerFrontTileGridLocationBottomRight.y - howFarInFront - i);
                locationToCheck.Add(startingTileVector);
                startingTileVectorInitialize = true;
                for (int j = 1; j < rows[i]; j += 2)
                {
                    //if already a tile on the right of startingTile add another one
                    if (rightVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(rightSideVector.x - 1, rightSideVector.y);
                        locationToCheck.Add(rightSideVector);
                    }
                    //                     //if there are no tile on the right ad one and put it in the variable rightside
                    else if (startingTileVectorInitialize)
                    {
                        rightSideVector = new Vector2Int(startingTileVector.x - 1, startingTileVector.y);
                        locationToCheck.Add(rightSideVector);
                        rightVectorInitialize = true;
                    }
                    //                     //if there are no tile on the Left add one and put it in the variable Leftside
                    if (leftVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(leftSideVector.x + 1, leftSideVector.y);
                        locationToCheck.Add(leftSideVector);
                    }
                    //if there are no tile on the left ad one and put it in the variable Leftside
                    else if (startingTileVectorInitialize)
                    {
                        leftSideVector = new Vector2Int(startingTileVector.x + 1, startingTileVector.y);
                        locationToCheck.Add(leftSideVector);
                        leftVectorInitialize = true;
                    }

                }
                ReinitializingVectors();
            }
            foreach (Vector2Int vec in locationToCheck)
            {
                if (mapManager.map.ContainsKey(vec) && !abilityZone.Contains(mapManager.map[vec]))
                {
                    abilityZone.Add(mapManager.map[vec]);
                }
            }

            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
                tile.isAttackingTile = true;
            }
        }

        return abilityZone;
    }

    public List<OverlayTile> SingleTargetZone(OverlayTile startingTile, int range)
    {
        foreach (OverlayTile targetTile in abilityZone)
        {
            targetTile.HideTile();
            targetTile.isAttackingTile = false;
        }

        abilityZone = new List<OverlayTile>();
        abilityZone = rangeFinder.GetTilesInRange(startingTile, range, true);
        foreach (OverlayTile tile in abilityZone)
        {
            if (tile != character.activeTile)
            {
            tile.ShowAttackTile();
            tile.isAttackingTile = true;
            }
        }

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

}
