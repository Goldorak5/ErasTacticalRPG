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

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public Dictionary<Vector2Int, OverlayTile> map;

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
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
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
                    var tileLocation = new Vector3Int(x, y, z);
                    //tile key for the map
                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x,cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }

        //spawning enemies

    }


    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> limiteTiles)
    {
          
        List<OverlayTile> neighbour = new List<OverlayTile>();
         

        //top neighbour
        Vector2Int locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1);

        AddTileToNeighbourList(currentOverlayTile, map, neighbour, locationCheck, limiteTiles);

        //down neighbour
        locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1);

        AddTileToNeighbourList(currentOverlayTile, map, neighbour, locationCheck, limiteTiles);

        //right neighbour
        locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y);

        AddTileToNeighbourList(currentOverlayTile, map, neighbour, locationCheck, limiteTiles);

        //left neighbour
        locationCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y);

        AddTileToNeighbourList(currentOverlayTile, map, neighbour, locationCheck, limiteTiles);

        return neighbour;
    }

    private void AddTileToNeighbourList(OverlayTile currentOverlayTile, Dictionary<Vector2Int, OverlayTile> map, List<OverlayTile> neighbour, Vector2Int locationCheck, List<OverlayTile> limiteTiles)
    {
        PaoloCharacter character = GameObject.Find("Paolo(Clone)").GetComponent<PaoloCharacter>();
        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (limiteTiles.Count > 0)
        {
        foreach (var tile in limiteTiles)
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
            //Dexterity = the max z that the character could climb
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationCheck].gridLocation.z) <= character.dexterity)
             
            neighbour.Add(tilesToSearch[locationCheck]);
        }
    }
}
