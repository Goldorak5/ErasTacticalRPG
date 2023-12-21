using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemies : BaseCharacter
{
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private OverlayTile targetTile;
    private int numTilesToSearch;

  
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        StartCoroutine(InitializePosition());

    }

    private IEnumerator InitializePosition()
    {
        yield return new WaitForSeconds(1);
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
    }

    public void MoveEnemy()
    {
        GetTargetTiles();
        numTilesToSearch = MapManager.Instance.map.Count;

        //search trough all the tiles with the movement points
        inRangeTiles = rangeFinder.GetTilesInRange(activeTile, numTilesToSearch);

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
        OverlayTile playerTile = FindObjectOfType<PaoloCharacter>().activeTile;
        //find the tiles around the player
        inRangeTiles = rangeFinder.GetTilesInRange(playerTile, 1);

        //make a list of tiles around the player
        List<OverlayTile> listOfClosestTiles = new List<OverlayTile>();
        listOfClosestTiles = MapManager.Instance.GetNeighbourTiles(playerTile, inRangeTiles)
                                .OrderBy(x => Vector2.Distance(activeTile.grid2DLocation, new Vector2(x.grid2DLocation.x, x.grid2DLocation.y))).ToList();

        targetTile = listOfClosestTiles.First();

        if (targetTile.isBlocked)
        {
                 if (targetTile == activeTile) 
                    {
                    return targetTile;
                    }
            while(listOfClosestTiles.Count > 0 && targetTile.isBlocked)
            {
            listOfClosestTiles.RemoveAt(0);
            if (listOfClosestTiles.Count == 0)
                {
                    return targetTile = null;
                }
            targetTile = listOfClosestTiles.First();
            }
        }
        
        return targetTile;
    }

    private void LateUpdate()
    {
        if (path.Count > 0 && movementPoints > 0)
        {
            //moving character
            MoveAlongPath();
        }
        if (endTurn)
        {
            movementPoints = maxMovementPoints;
        }
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
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, movementPoints);
        }
        if (path.Count == 0)
        {
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, movementPoints);

        }
        activeTile.isBlocked = true;
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.001f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        activeTile = tile;
    }
}
