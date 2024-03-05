using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ShowCursorState
{
    Hide,
    Show
}

public class MouseController : MonoBehaviour
{
    //private
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private RegularAttack regularAttack;
    private List<BaseCharacter> targetedEnemies = new List<BaseCharacter>();
    private BaseCharacter enemyHit;
    private List<GameObject> playerList = new List<GameObject>();
    private BaseCharacter hightlightCharacter;
    private ShowCursorState showCursorState = ShowCursorState.Hide;

    //public
    public GameObject spawningZone;
    public OverlayTile overlayTile;
    //cursor image
    public SpriteRenderer cursorSprite;
    public BaseCharacter character;
    public List<GameObject> characterPrefabList;
    public TurnManager turnManager;
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    public float movementSpeed;
    public Canvas debugCanvas;
    public Canvas abilityCanvas;
    [HideInInspector]public bool canSpawn = false;


    private void Awake()
    {
        playerList = characterPrefabList;
        cursorSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    private bool FocusIsOnTile(ref RaycastHit2D? focusedTileHit)
    {
        return focusedTileHit.Value.collider.gameObject.layer == LayerMask.NameToLayer("Tile");
    }

    private void SetCharacterSpriteWhite(BaseCharacter character)
    {
        if (hightlightCharacter != null)
        {
            hightlightCharacter.GetComponent<SpriteRenderer>().color = Color.white;
            if (!hightlightCharacter.isReceivingDamage)
            {
              hightlightCharacter.HideHealthArmor();
            }
        }
    }

    void LateUpdate()
    {
        if(playerList != null)
        {
            //raycast funtion to check if mouse controler is over spanwning zone;
            FindSpawningZone();
        }

        RaycastHit2D? focusedTileHit = GetFocusOnTile();

        if (focusedTileHit.HasValue)
        {
            if (FocusIsOnTile(ref focusedTileHit))
            {
                //associate the overlay tile to the focus tile under the mouse cursor
                overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                if (overlayTile != null)
                {
                
                    if (showCursorState == ShowCursorState.Show)
                    {
                        cursorSprite.enabled = true;
                    }
                    else if(showCursorState == ShowCursorState.Hide)
                    {
                        cursorSprite.enabled = false;
                    }

                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                    //if cursor is on a movable tile show cursor else hide
                    if (inRangeTiles != null && character != null && character.characterState == CharacterState.Moving)
                    {
                        if (inRangeTiles.Contains(overlayTile))
                        {
                            showCursorState = ShowCursorState.Show;
                        }
                        else
                        {
                            showCursorState = ShowCursorState.Hide;
                        }
                    }
                }
            }
                    //put the color of the player back to normal after cursor is not on it
            SetCharacterSpriteWhite(hightlightCharacter);

                         //When mouse is clicked on a tile 
            if (Input.GetMouseButtonDown(0))
            {
                if(playerList != null && playerList.Count > 0 )
                {
                         character = playerList[0].GetComponent<BaseCharacter>() ;
                    if (canSpawn && !overlayTile.isBlocked)
                    {
                        character = Instantiate(playerList[0]).GetComponent<BaseCharacter>();
                        PositionCharacterOnTile(overlayTile);
                        character.characterState = CharacterState.Ideling;
                        character.GetComponent<SpriteRenderer>().sortingOrder = 5;
                        playerList.RemoveAt(0);
                        showCursorState = ShowCursorState.Hide;
                    }
                }
                //if all players are spawn when click on tile = moving
                else if (character != null && playerList == null)
                {
                    if (character.characterState == CharacterState.Moving)
                    {
                        //hiding canvas
                        debugCanvas.gameObject.SetActive(false);

                        //initialize the path 
                        path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                    }
                }
                // destroy spawning zone after all players are placed 
                if(playerList != null && playerList.Count == 0)
                {
                    Destroy(spawningZone.gameObject);
                    StartCoroutine(turnManager.PlayGame());
                    playerList = null;

                }
            }
        }else
        {
            if (showCursorState == ShowCursorState.Hide)
            {
            cursorSprite.enabled = false;
            }
        }
        if (path.Count > 0 && character.movementPoints > 0 && character.characterState == CharacterState.Moving)
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

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.movementPoints, false);

        foreach(OverlayTile tile in inRangeTiles)
        {
            //calculer le nombre de mouvement pour chaque tuile
            pathFinder.FindPath(character.activeTile, tile, inRangeTiles);
            if(pathFinder.PathPossible)
            {
            tile.ShowTile();
            }
        }
    }

    private void MoveAlongPath()
    {
        float step = character.characterMovementSpeed * Time.deltaTime;
        character.activeTile.isBlocked = false;
        float zIndex = path[0].transform.position.z;
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
            debugCanvas.gameObject.SetActive(true);
        }
       character.activeTile.isBlocked = true;
    }

    public void FindSpawningZone()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousPos.x, mousPos.y);
        RaycastHit2D[] spawningPoints = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        foreach(RaycastHit2D hit  in spawningPoints)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Spawning"))
            {
                showCursorState = ShowCursorState.Show;
                canSpawn = true;
            }
            else
            {
                showCursorState = ShowCursorState.Hide;
                canSpawn = false;
            }
        }
    }

    public RaycastHit2D? GetFocusOnTile()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousPos.x, mousPos.y - 0.2f);
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int numsHits = Physics2D.RaycastNonAlloc(mousePos2d, Vector2.zero, hits);

        RaycastHit2D hit;

        for (int i = 0; i < numsHits; i++)
        {
            hit = hits[i];
                                //hover Characters
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            { 
                                //if it hits a character
                hightlightCharacter = hit.collider.gameObject.GetComponent<BaseCharacter>();
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                hightlightCharacter.ShowHealthArmor();
                            //if clicked on a character
                if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.GetComponent<BaseCharacter>().isHuman && character.characterState != CharacterState.Attacking)
                {
                    debugCanvas.gameObject.SetActive(true);
                    character = hit.collider.gameObject.GetComponent<BaseCharacter>();
                    /*Debug.Log("character Selected: " + character.name);*/
                    character.characterState = CharacterState.Ideling;
                }
                            //attacking enemy or healing ally
                if (Input.GetMouseButtonDown(0) && character.characterState == CharacterState.Attacking)
                {
                    debugCanvas.gameObject.SetActive(true);
                    if (!character.HasAttack)
                    {
                        debugCanvas.gameObject.SetActive(false);
                        if (character != null)
                        {
                            enemyHit = hit.collider.gameObject.GetComponent<BaseCharacter>();
                            targetedEnemies.Add(enemyHit);
                            character.characterState = CharacterState.Clickers;
                            regularAttack = character.GetComponent<RegularAttack>();
                            regularAttack.StartClickersBoxe(targetedEnemies);
                        }
                    }
                    else Debug.Log(character.name + " As Already Attack");
                }
                return null;
            }  //if it hits the tile
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                return hit;
            }
        }
        //if don't touch anything return nothing
        return null;
    }


    public void OnMoveButtonClick()
    {
        if (character != null && character.IsMyTurn)
        {
            GetInRangeTiles();
            SetCharacterMoveState();
        }
        else Debug.Log("Can't Move Now!");
    }

    public void EndTurnButton()
    {

        /*character.movementPoints = character.maxMovementPoints;*/
        debugCanvas.gameObject.SetActive(false);
        HideMovementTiles();
        SetCharacterIdleState();
    }

    public void AttackButton()
    {
        if (character.IsMyTurn)
        {
        HideMovementTiles();
        debugCanvas.gameObject.SetActive(false);
        SetCharacterAttackState();
        }else Debug.Log("Not the turn of: " +  character.name);
    }

    public void AbilitiesButton()
    {
        if (character.IsMyTurn)
        {
            HideMovementTiles();
            debugCanvas.gameObject.SetActive(true);
            abilityCanvas.gameObject.SetActive(true);

            character.characterState = CharacterState.Abilities;
        }else Debug.Log("Not the turn of: " + character.name);
    }

    private void SetCharacterAttackState()
    {
        character.characterState = CharacterState.Attacking;
    }
    private void SetCharacterIdleState()
    {
        character.characterState = CharacterState.Ideling;
    }
    private void SetCharacterMoveState()
    {
        character.characterState = CharacterState.Moving;
    }

    public void HideMovementTiles()
    {
        foreach (OverlayTile tile in inRangeTiles)
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
