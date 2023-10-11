using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public GameObject characterPrefab;
    private PaoloCharacter character;
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private OverlayTile overlayTile;
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    public float speed;
    private new SpriteRenderer renderer;
    private List<OverlayTile> path = new List<OverlayTile>();
    public Canvas canvas;
    bool overSpawningZone = false;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }


    private bool FocusIsOnTile(ref RaycastHit2D? focusedTileHit)
    {
        return focusedTileHit.Value.collider.gameObject.layer == LayerMask.NameToLayer("Tile");
    }

    void LateUpdate()
    {
        FindSpawningZone();
        var focusedTileHit = GetFocusOnTile();

        if (focusedTileHit.HasValue)
        {
            if(FocusIsOnTile(ref focusedTileHit))
            {
                //associate the overlay tile to the focus tile under the mouse cursor
                overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                if (overlayTile != null)
                {
                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
                }
            }
            if(character != null)
            {
                character.GetComponent<SpriteRenderer>().color = Color.white;
            }
            if(renderer != null)
            {
                renderer.color = Color.white;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (character == null && overSpawningZone)
                {
                  character = Instantiate(characterPrefab).GetComponent<PaoloCharacter>();
                    
                  PositionCharacterOnTile(overlayTile);
                    GameObject spawningZone = GameObject.Find("SpawningZone");
                    spawningZone.SetActive(false); 
                }
                else if(character != null)
                {
                    canvas.gameObject.SetActive(false);
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
            GetInRangeTiles();
        }
        if (path.Count == 0)
        {
            GetInRangeTiles();
            canvas.gameObject.SetActive(true);
        }
    }

    public void FindSpawningZone()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousPos.x, mousPos.y - 0.2f);
        RaycastHit2D[] spawningPoints = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        foreach(RaycastHit2D hit  in spawningPoints)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Spawning"))
            {
                overSpawningZone = true;
                break;
            }
            else
            {
                overSpawningZone = false;
            }
        }
    }
    
    public RaycastHit2D? GetFocusOnTile()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousPos.x, mousPos.y - 0.2f);
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int numsHits = Physics2D.RaycastNonAlloc(mousePos2d,Vector2.zero,hits);

        RaycastHit2D hit;

        for (int i = 0; i < numsHits; i++)
        {
            hit = hits[i];

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                //if it hits the character
                character.GetComponent<SpriteRenderer>().color = Color.red;

                //if clicked on the character
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Paolo!");
                    canvas.gameObject.SetActive(true);
                }
                return null;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NME"))
            {
                renderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                   renderer.color = Color.red;
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("Enemy!");
                        canvas.gameObject.SetActive(false);
                    }
                }
                return null;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                //if it hits the tile
                return hit;
            }
            
        }
        //if don't touch anything return nothing
        return null;
    }

    public void OnMoveButtonClick()
    {
        if (character != null)
        {
            GetInRangeTiles();
        }
    }
    public void ResetMovementPoint()
    {
        character.characterMovement = 4;
        canvas.gameObject.SetActive(false);
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;  
        character.activeTile = tile;
        //GetInRangeTiles();
 
    }
}
