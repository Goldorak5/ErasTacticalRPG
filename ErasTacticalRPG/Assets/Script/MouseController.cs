using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject characterPrefab;
    private PaoloCharacter character;
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;

    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    public float speed;

    private List<OverlayTile> path = new List<OverlayTile>();


    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    void LateUpdate()
    {
        var focusedTileHit = GetFocusOnTile();

        if (focusedTileHit.HasValue)
        {
            //associate the overlay tile to the focus tile under the mouse cursor
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            if (Input.GetMouseButtonDown(0))
            {
                if(character == null)
                {
                    character = Instantiate(characterPrefab).GetComponent<PaoloCharacter>();
                    PositionCharacterOnTile(overlayTile);
                }
                else
                {
                   path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                }
            }
        }

        if (path.Count > 0 && character.characterMovement > 0)
        {
            MoveAlongPath();
        }
    }

    private void GetInRangeTiles()
    { 
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.characterMovement);

        foreach(var tile in inRangeTiles)
        {
            tile.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
    var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position , step);
        character.transform.position = new Vector3 (character.transform.position.x, character.transform.position.y, zIndex);

        if(Vector2.Distance(character.transform.position , path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
            character.characterMovement--;
        }
        if (path.Count == 0)
        {
            GetInRangeTiles();
        }
    }

    public RaycastHit2D? GetFocusOnTile()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousPos.x, mousPos.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if(hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;  
        character.activeTile = tile;
        GetInRangeTiles();

    }
}
