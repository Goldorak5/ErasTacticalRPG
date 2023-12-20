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
    //private
    private PaoloCharacter character;
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private OverlayTile overlayTile;
    private new SpriteRenderer renderer;
    private List<OverlayTile> path = new List<OverlayTile>();
    private RegularAttack regularAttack;

    //public
    public static CharacterState characterState;
    public GameObject characterPrefab;
    public TurnManager turnManager;
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    public float movementSpeed;
    public Canvas canvas;
    [HideInInspector]public bool canSpawn = false;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        
    }

    private bool FocusIsOnTile(ref RaycastHit2D? focusedTileHit)
    {
        return focusedTileHit.Value.collider.gameObject.layer == LayerMask.NameToLayer("Tile");
    }

    private void SetCharacterSpriteWhite()
    {
        if (character != null)
        {
            character.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void LateUpdate()
    {
        
        if(character == null)
        {
            //raycast funtion to check if mouse controler is over spanwning zone;
            FindSpawningZone();
        }
        
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
            //put the colr of the player back to normal after cursor is not on it
            SetCharacterSpriteWhite();

            if (renderer != null)
            {
                renderer.color = Color.white;
            }

            //lorsque la souris est cliquer
            if (Input.GetMouseButtonDown(0))
            {
                if (character == null && canSpawn)
                {
                    character = Instantiate(characterPrefab).GetComponent<PaoloCharacter>();
                    PositionCharacterOnTile(overlayTile);
                    character.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    GameObject spawningZone = GameObject.Find("SpawningZone");
                    Destroy(spawningZone);
                    StartCoroutine(turnManager.PlayGame());
                }
                else if(character != null)
                {

                    if (characterState == CharacterState.Moving)
                    {
                        //hiding canvas
                        canvas.gameObject.SetActive(false);

                        //initialize the path 
                        path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                    }
                }
            }
        }
        if (path.Count > 0 && character.movementPoints > 0 && characterState == CharacterState.Moving)
        {
            //moving character
            MoveAlongPath();
        }
        if(character != null)
        {
            if (character.movementPoints == 0)
            {
                 HideMovementTiles();
            }
        }
    }

    private void GetInRangeTiles()
    {
        HideMovementTiles();

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.movementPoints);

        foreach(var tile in inRangeTiles)
        {
            tile.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
        var step = character.characterMovementSpeed * Time.deltaTime;
       character.activeTile.isBlocked = false;
        var zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position , step);
        character.transform.position = new Vector3 (character.transform.position.x, character.transform.position.y, zIndex);

        if(Vector2.Distance(character.transform.position , path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);

            path.RemoveAt(0);
            character.movementPoints--;
            GetInRangeTiles();
        }
        if (path.Count == 0)
        {
            GetInRangeTiles();
            //make debug menu appear
            canvas.gameObject.SetActive(true);
        }
       character.activeTile.isBlocked = true;
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
                canSpawn = true;
                break;
            }
            else
            {
                canSpawn = false;
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
                    if (Input.GetMouseButtonDown(0) && characterState == CharacterState.Attacking)
                    {
                        //if clicked on an enemy
                        Debug.Log("Enemy Selected!");
                        canvas.gameObject.SetActive(false);
                        characterState = CharacterState.Clickers;
                        regularAttack = character.GetComponent<RegularAttack>();
                        regularAttack.StartClickersBoxe();
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
            SetCharacterMoveState();
      
        }
        SetCharacterMoveState();
    }

    public void EndTurnButton()
    {
        character.movementPoints = 4;
        canvas.gameObject.SetActive(false);
        HideMovementTiles();
        SetCharacterIdleState();
    }

    public void AttackButton()
    {
        HideMovementTiles();
        canvas.gameObject.SetActive(false);
        SetCharacterAttackState();
    }

    private void SetCharacterAttackState()
    {
        characterState = CharacterState.Attacking;
    }
    private void SetCharacterIdleState()
    {
        characterState = CharacterState.Ideling;
    }
    private void SetCharacterMoveState()
    {
        characterState = CharacterState.Moving;
    }

    public void HideMovementTiles()
    {
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;  
        character.activeTile = tile;
    }
}
