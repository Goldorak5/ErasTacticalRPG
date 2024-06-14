using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance 
    { 
        get { return _instance; } 
    }
    private MouseController mouseController;
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public Dictionary<Vector2Int, OverlayTile> map;
    public TurnManager turnManager;

    internal object instance;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        mouseController = GetComponent<MouseController>();
        turnManager = FindAnyObjectByType<TurnManager>();
        Tilemap tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile> ();
        BoundsInt bounds = tileMap.cellBounds;

        //looping through all tiles
        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    //the tile where we are in the loop
                    Vector3Int tileLocation = new Vector3Int(x, y, z);
                    //tile key for the map
                    Vector2Int tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        Vector3 cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x,cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }

    }

    //gives the four neighbors
    //if limitList is new list 
    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> limitList, bool forAttack)
    {
        List<OverlayTile> neighbourList = new List<OverlayTile>();
         
        //top neighbour
            Vector2Int locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1);

        AddTileToNeighbourList(currentOverlayTile, map, neighbourList, locationCheck, limitList, forAttack);

        //down neighbour
            locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1);

        AddTileToNeighbourList(currentOverlayTile, map, neighbourList, locationCheck, limitList, forAttack);

        //right neighbour
            locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y);

        AddTileToNeighbourList(currentOverlayTile, map, neighbourList, locationCheck, limitList, forAttack);

        //left neighbour
            locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y);

        AddTileToNeighbourList(currentOverlayTile, map, neighbourList, locationCheck, limitList, forAttack);

        return neighbourList;
    }

    /*if tiles have a particular spec adding it here
     * add 4 tiles around the character,
     * if an empty list is giving here it will search the entire map but still gonna give the 4 neighbors
     * The limitList is for when the character run out of movement.
     * this function also gives the probability of going higher if the character can.
     */
    private void AddTileToNeighbourList(OverlayTile currentOverlayTile, Dictionary<Vector2Int, OverlayTile> map, List<OverlayTile> neighbour, Vector2Int locationCheck, List<OverlayTile> limiteTiles, bool forAttack)
    {
        BaseCharacter playerCharacter = turnManager.currentPlayerTurn;
/*        PaoloCharacter character = GameObject.Find("Paolo(Clone)").GetComponent<PaoloCharacter>();*/

        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (limiteTiles.Count > 0)
        {
        foreach (OverlayTile tile in limiteTiles)
            {
                tilesToSearch.Add(tile.grid2DLocation, tile);
            }
        }
        else
        {
            tilesToSearch = map;
        }

        if (tilesToSearch.ContainsKey(locationCheck))
        {
            if (!tilesToSearch[locationCheck].isBlocked || forAttack == true)
            {
                //Dexterity = the max z that the character could climb
                if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationCheck].gridLocation.z) <= playerCharacter.dexterity)
                {
                    neighbour.Add(tilesToSearch[locationCheck]);
                }
            }
         }
    }

    public bool IsTileInMap(OverlayTile tile = null, Vector2Int gridLocation = default)
    {
        if (!gridLocation.Equals(default(Vector2Int)) &&  map.ContainsKey(gridLocation))
        {
            return true;
        }else if(tile != null && map.ContainsValue(tile))
        {
            return true;
        }
        return false;
    }

}
