using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Enemies : BaseCharacter
{
    private PathFinder pathFinder;
    private List<BaseCharacter> playerTargets;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private OverlayTile targetTile = null;
    private OverlayTile playerTile;
    private bool isReady = false;
    private List<OverlayTile> cacPlayerTile;
    private RegularAttack regularAttackScript;
    private int numTilesToSearch;
    public bool hasMoved { get; private set; }

    void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        StartCoroutine(InitializePosition());
        regularAttackScript = GetComponent<RegularAttack>();
        HideHealthArmor();
        tMP_TextsHealthBox = tMP_TextsHealthBox.GetComponent<TMP_Text>();
        tMP_TextsArmorBox.text = maxArmor.ToString();
        tMP_TextsHealthBox.text = maxHealth.ToString();
    }
    private void LateUpdate()
    {
        if (isReady && (targetTile == activeTile || movementPoints == 0))
        {
            hasMoved = true;
        }
        if (path.Count > 0 && movementPoints > 0)
        {
            //moving Enemy
            MoveAlongPath();
        }
        if (endTurn)
        {
            movementPoints = maxMovementPoints;
        }
    }

    private IEnumerator InitializePosition()
    {
        yield return new WaitForSeconds(0.5f);
        // Create a LayerMask that includes only tile
        int layerMask = 1 << LayerMask.NameToLayer("Tile");
        Vector2 direction = Vector2.down;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 1f, layerMask);

        //start with ridiculous big distance
        float minDistance = Mathf.Infinity;
        RaycastHit2D closestHit = new RaycastHit2D();

        //do a loop with all the hit and store the closest one in a variable
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
               if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestHit = hit;
                }
            }
        }       
        if(activeTile == null)
                {
                    activeTile = closestHit.collider.gameObject.GetComponent<OverlayTile>();
                    activeTile.isBlocked = true;

                }
        PositionCharacterOnTile(activeTile);
        isReady = true;
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.001f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        activeTile = tile;
    }

    public void MoveEnemy()
    {
        GetTargetTiles();
        numTilesToSearch = MapManager.Instance.map.Count;

        //search trough all the tiles with the movement points
        inRangeTiles = rangeFinder.GetTilesInRange(activeTile, numTilesToSearch, false);

        path = pathFinder.FindPath(activeTile, targetTile, inRangeTiles);

        //reduce the path to the number of movement points
        if(path.Count > movementPoints && path != null)
        {
            while(path.Count > movementPoints)
            {
                path.RemoveAt(path.Count - 1);
            }
        }
    }
    private OverlayTile GetTargetTiles()
    {
        FindClosestTarget();
        playerTile = playerTargets[0].activeTile;

        //find the tiles around the player
        inRangeTiles = rangeFinder.GetTilesInRange(playerTile, 1,true);

        //initialize the four tile around the player
        cacPlayerTile = inRangeTiles;



        //make a list of tiles around the target and chose the closest one
        List<OverlayTile> listOfClosestTiles = new List<OverlayTile>();
        listOfClosestTiles = MapManager.Instance.GetNeighbourTiles(playerTile, inRangeTiles, true)
                                .OrderBy(x => Vector2.Distance(activeTile.grid2DLocation, new Vector2(x.grid2DLocation.x, x.grid2DLocation.y))).ToList();

        targetTile = listOfClosestTiles.First();

        if (targetTile.isBlocked)
        {
            if (targetTile == activeTile)
            {
                return targetTile;
            }
            //as long as the tile that the NME want to move is block he remove one tile of the list 
            while (listOfClosestTiles.Count > 0 && targetTile.isBlocked)
            {
                listOfClosestTiles.RemoveAt(0);
                if (listOfClosestTiles.Count == 0)
                {
                    Debug.Log("Cant go near character");
                    return targetTile = null;

                }
                targetTile = listOfClosestTiles.First();
            }
        }
        return targetTile;
    }
    private void MoveAlongPath()
    {
        activeTile.isBlocked = false;
        var step = characterMovementSpeed * Time.deltaTime;
        var zIndex = path[0].transform.position.z;
        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);

            path.RemoveAt(0);
            movementPoints--;
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, movementPoints, false);
        }
        if (path.Count == 0)
        {
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, movementPoints, false);

        }
        activeTile.isBlocked = true;
    }

    //find all player Character put it in a list and order the list with the closest one 
    public bool FindClosestTarget()
    {
        BaseCharacter[] characterArray = FindObjectsOfType<BaseCharacter>();
        playerTargets = new List<BaseCharacter>(characterArray);

        //if the BaseCharacter is Enemy delete from the list
        for (int i = playerTargets.Count() - 1; i >= 0 ; i--)
        {
            BaseCharacter target = playerTargets[0];
            if (!playerTargets[i].isHuman)
            {
                playerTargets.RemoveAt(i);
            }
        }

        playerTargets = playerTargets.OrderBy(x => Vector2.Distance(activeTile.grid2DLocation, new Vector2(x.activeTile.grid2DLocation.x, x.activeTile.grid2DLocation.y))).ToList();
            for (int i = playerTargets.Count - 1; playerTargets.Count > 1; i--)
        {
            playerTargets.Remove(playerTargets[i]);
        }

        //if enemy is on the CAC of the player to attack
        if (cacPlayerTile != null)
        {
             foreach (var tile in cacPlayerTile)
            {
                if(activeTile == tile)
                {
                return true;
                }
            }
        }
        return false;
    }

    public void EnemyAttacking()
    {
        if(playerTargets != null)
        {
            regularAttackScript.StartClickersBoxe(playerTargets);
        }
    }

    public void HasAttackFlagFalse()
    {
        hasAttack = false;
    }

    public void HasMoveFlagFalse()
    {
        hasMoved = false;
    }
}
