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

public class MouseController : MonoBehaviour
{
    //private
    public BaseCharacter character;
    public PaoloCharacter paoloCharacter;
    public KuroCharacter kuroCharacter;
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private OverlayTile overlayTile;
    private List<OverlayTile> path = new List<OverlayTile>();
    private RegularAttack regularAttack;
    private BaseCharacter targetedEnemy;
    private List<BaseCharacter> playerList = new List<BaseCharacter>();

    //public
    /*public static CharacterState characterState;*/
    public List<GameObject> characterPrefabList;
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

    private void SetCharacterSpriteWhite(BaseCharacter character)
    {
        if (character != null)
        {
            character.GetComponent<SpriteRenderer>().color = Color.white;
            if (!character.isReceivingDamage)
            {
              character.HideHealthArmor();
            }
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
            if (FocusIsOnTile(ref focusedTileHit))
            {
                //associate the overlay tile to the focus tile under the mouse cursor
                overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                if (overlayTile != null)
                {
                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
                }
            }

            //put the color of the player back to normal after cursor is not on it
            SetCharacterSpriteWhite(character);
            SetCharacterSpriteWhite(targetedEnemy);

            //When mouse is clicked on a tile 
            if (Input.GetMouseButtonDown(0))
            {
                if(characterPrefabList!= null && characterPrefabList.Count > 0)
                {
                         character = turnManager.playerCharacterList[0];
//                     if (turnManager.playerCharacterList[0] == paoloCharacter && canSpawn)
//                     {
//                         paoloCharacter = Instantiate(characterPrefabList[0]).GetComponent<PaoloCharacter>();
//                         character = paoloCharacter;
//                         PositionCharacterOnTile(overlayTile);
//                         character.characterState = CharacterState.Ideling;
//                         character.GetComponent<SpriteRenderer>().sortingOrder = 5;
//                         characterPrefabList.RemoveAt(0);
//                         turnManager.playerCharacterList.RemoveAt(0);
//                     }
                   if (turnManager.playerCharacterList[0] == kuroCharacter && canSpawn)
                    {
                        kuroCharacter = Instantiate(characterPrefabList[0]).GetComponent<KuroCharacter>();
                        character = kuroCharacter;
                        PositionCharacterOnTile(overlayTile);
                        character.characterState = CharacterState.Ideling;
                        character.GetComponent<SpriteRenderer>().sortingOrder = 5;
                        characterPrefabList.RemoveAt(0);
                        turnManager.playerCharacterList.RemoveAt(0);
                    }

                }

                else if (character != null && turnManager.playerCharacterList.Count == 0)
                {
                    if (character.characterState == CharacterState.Moving)
                    {
                        //hiding canvas
                        canvas.gameObject.SetActive(false);

                        //initialize the path 
                        path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                    }
                }
                if(characterPrefabList != null && characterPrefabList.Count == 0  )
                {
                    GameObject spawningZone = GameObject.Find("SpawningZone");
                    Destroy(spawningZone.gameObject);
                    StartCoroutine(turnManager.PlayGame());
                    characterPrefabList = null;
                }
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
                                        //hover Characters
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                                 //if it hits the character
                character.GetComponent<SpriteRenderer>().color = Color.red;
                character.ShowHealthArmor();

                                //if clicked on the character
                if (Input.GetMouseButtonDown(0))
                {
                    canvas.gameObject.SetActive(true);
                    character = hit.collider.gameObject.GetComponent<BaseCharacter>();
                    Debug.Log("character Selected: " + character);
                }
                return null;
            }                           //hover an enemy
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NME"))
            {
                targetedEnemy = hit.collider.gameObject.GetComponent<BaseCharacter>();
                targetedEnemy.GetComponent<SpriteRenderer>().color = Color.red;
                targetedEnemy.ShowHealthArmor();
                                      //if clicked on an enemy
                 if (character != null && Input.GetMouseButtonDown(0) && character.characterState == CharacterState.Attacking)
                  {
                    if (!character.HasAttack)
                    {
                     canvas.gameObject.SetActive(false);
                            if(character != null)
                            {
                                character.characterState = CharacterState.Clickers;
                                regularAttack = character.GetComponent<RegularAttack>();
                                regularAttack.StartClickersBoxe(targetedEnemy);
                            }
                    }
                    else Debug.Log("Already Attack");
                    }
                return null;
            }                    //if it hits the tile
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
