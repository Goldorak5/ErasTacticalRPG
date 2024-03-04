using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZoneFromMouse : MonoBehaviour
{
    private OverlayTile startingTile;

    public int testMaxRange;
    public int testZoneRange;

    private List<OverlayTile> abilityZone = new List<OverlayTile>();
    private MapManager mapManager;
    private RangeFinder rangeFinder;
    private MouseController mouseController;

    void Start()
    {
        mapManager = MapManager.Instance;
        rangeFinder = new RangeFinder();
        mouseController = FindObjectOfType<MouseController>();
    }

    public List<OverlayTile> SquareZone(int maxRange, int radius)
    {
        List<OverlayTile> maxRangeStartingTile = new List<OverlayTile>();
        List<Vector2Int> locationToCheck = new List<Vector2Int>();
        if (mouseController.character != null)
        {
            maxRangeStartingTile = rangeFinder.GetTilesInRange(mouseController.character.activeTile, maxRange, true);
        }
        foreach (OverlayTile tile in abilityZone)
        {
            tile.HideTile();
        }
        //show the max range of the zone
        foreach (OverlayTile tile in maxRangeStartingTile)
        {
            tile.ShowRangeTile(); 
        }
        
        if (maxRangeStartingTile.Contains(startingTile))
        {
            abilityZone = new List<OverlayTile> { startingTile };
            for(int i = 1; i <= radius; i++)
            {
                Vector2Int locationFront;
                Vector2Int locationBack;
                Vector2Int locationRight;
                Vector2Int locationLeft;
                //Draw the cross first bomberMan style

                //tile in front
                locationFront = new Vector2Int(startingTile.grid2DLocation.x + i, startingTile.grid2DLocation.y);
                locationToCheck.Add(locationFront);
                
                //tile in the back
                locationBack = new Vector2Int(startingTile.grid2DLocation.x - i, startingTile.grid2DLocation.y);
                locationToCheck.Add(locationBack);

                //tile Right
                locationRight =  new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + i);
                locationToCheck.Add(locationRight);

                //tile Left
                locationLeft = new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - i);
                locationToCheck.Add(locationLeft);

                //then draw all the tiles next to the cross
                for (int j = 1; j <= radius; j++)
                {
                    Vector2Int currentLocation;
                    //tile in front
                    currentLocation = new Vector2Int(locationFront.x, locationFront.y + j);
                    locationToCheck.Add(currentLocation);
                    
                    currentLocation = new Vector2Int(locationFront.x, locationFront.y - j);
                    locationToCheck.Add(currentLocation);

                    //tile in the back
                    currentLocation = new Vector2Int(locationBack.x, locationBack.y + j);
                    locationToCheck.Add(currentLocation);
                    
                    currentLocation = new Vector2Int(locationBack.x, locationBack.y - j);
                    locationToCheck.Add(currentLocation);

                    //tile in middle
                    currentLocation = new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + j);
                    locationToCheck.Add(currentLocation);
                    
                    currentLocation = new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - j);
                    locationToCheck.Add(currentLocation);
                }
            }
            //check if location is in the map and add the tile to ability zone
            foreach (Vector2Int currentElement in locationToCheck)
            {
                if (mapManager.map.ContainsKey(currentElement))
                {
                    abilityZone.Add(mapManager.map[currentElement]);
                }
            }

            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }

            return abilityZone;
        }
        return new List<OverlayTile>();
    }

    public List<OverlayTile> XZone(int maxRange, int radius)
    {
        List<OverlayTile> maxRangeStartingTile = new List<OverlayTile>();
        List<Vector2Int> locationToCheck = new List<Vector2Int>();
        if (mouseController.character != null)
        {
            maxRangeStartingTile = rangeFinder.GetTilesInRange(mouseController.character.activeTile, maxRange, true);
        }
        foreach (OverlayTile tile in abilityZone)
        {
            tile.HideTile();
        }
        //show the max range of the zone
        foreach (OverlayTile tile in maxRangeStartingTile)
        {
            tile.ShowRangeTile();
        }

        if (maxRangeStartingTile.Contains(startingTile))
        {
            abilityZone = new List<OverlayTile> { startingTile };
            for (int i = 1; i <= radius; i++)
            {
                Vector2Int locationFront;
                Vector2Int locationBack;

                //tile in front do not add
                locationFront = new Vector2Int(startingTile.grid2DLocation.x + i, startingTile.grid2DLocation.y);

                //tile in the back do not add
                locationBack = new Vector2Int(startingTile.grid2DLocation.x - i, startingTile.grid2DLocation.y);

                //then draw the tile in diagonal
                Vector2Int currentLocation;
                //tile in front
                currentLocation = new Vector2Int(locationFront.x, locationFront.y + i);
                locationToCheck.Add(currentLocation);

                currentLocation = new Vector2Int(locationFront.x, locationFront.y - i);
                locationToCheck.Add(currentLocation);

                //tile in the back
                currentLocation = new Vector2Int(locationBack.x, locationBack.y + i);
                locationToCheck.Add(currentLocation);

                currentLocation = new Vector2Int(locationBack.x, locationBack.y - i);
                locationToCheck.Add(currentLocation);
            }
            //check if location is in the map and add the tile to ability zone
            foreach (Vector2Int currentElement in locationToCheck)
            {
                if (mapManager.map.ContainsKey(currentElement))
                {
                    abilityZone.Add(mapManager.map[currentElement]);
                }
            }

            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }

            return abilityZone;
        }
        return new List<OverlayTile>();
    }

    public List<OverlayTile> TZone(int maxRange, int radius)
    {
        List<OverlayTile> maxRangeStartingTile = new List<OverlayTile>();
        List<Vector2Int> locationToCheck = new List<Vector2Int>();
        if (mouseController.character != null)
        {
            maxRangeStartingTile = rangeFinder.GetTilesInRange(mouseController.character.activeTile, maxRange, true);
        }
        foreach (OverlayTile tile in abilityZone)
        {
            tile.HideTile();
        }
        //show the max range of the zone
        foreach (OverlayTile tile in maxRangeStartingTile)
        {
            tile.ShowRangeTile();
        }

        if (maxRangeStartingTile.Contains(startingTile))
        {
            abilityZone = new List<OverlayTile> { startingTile };
            for (int i = 1; i <= radius; i++)
            {
                Vector2Int locationFront;
                Vector2Int locationBack;
                Vector2Int locationRight;
                Vector2Int locationLeft;
                //Draw the cross first bomberMan style

                //tile in front
                locationFront = new Vector2Int(startingTile.grid2DLocation.x + i, startingTile.grid2DLocation.y);
                locationToCheck.Add(locationFront);

                //tile in the back
                locationBack = new Vector2Int(startingTile.grid2DLocation.x - i, startingTile.grid2DLocation.y);
                locationToCheck.Add(locationBack);

                //tile Right
                locationRight = new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y + i);
                locationToCheck.Add(locationRight);

                //tile Left
                locationLeft = new Vector2Int(startingTile.grid2DLocation.x, startingTile.grid2DLocation.y - i);
                locationToCheck.Add(locationLeft);
            }
        //check if location is in the map and add the tile to ability zone
        foreach (Vector2Int currentElement in locationToCheck)
        {
            if (mapManager.map.ContainsKey(currentElement))
            {
                abilityZone.Add(mapManager.map[currentElement]);
            }
        }

        foreach (OverlayTile tile in abilityZone)
        {
            tile.ShowAttackTile();
        }

        return abilityZone;
        }
        return new List<OverlayTile>();
    }

    public List<OverlayTile> RangeZone(int maxRange, int abilityRange )
    {
        OverlayTile playerTile;
        playerTile = mouseController.character.activeTile;
        foreach (OverlayTile tile in abilityZone)
        {
            tile.HideTile();
        }
        List<OverlayTile> maxRangeStartingTile = rangeFinder.GetTilesInRange(playerTile, maxRange, true);
        abilityZone = new List<OverlayTile>();
        foreach (OverlayTile tile in maxRangeStartingTile)
        {
            tile.ShowRangeTile();
        }
        if (maxRangeStartingTile.Contains(startingTile))
        {
            abilityZone = rangeFinder.GetTilesInRange(startingTile, abilityRange, true);
            foreach (OverlayTile tile in abilityZone)
            {
                tile.ShowAttackTile();
            }
        }

        return abilityZone;
    }

    void Update()
    {
        if (mouseController.overlayTile != null)
        {
        startingTile = mouseController.overlayTile;
        }
        //SquareZone(testMaxRange,testZoneRange);
       // RangeZone(testMaxRange, testZoneRange);
       //TZone(testMaxRange, testZoneRange);
       //XZone(testMaxRange, testZoneRange);
    }
}
