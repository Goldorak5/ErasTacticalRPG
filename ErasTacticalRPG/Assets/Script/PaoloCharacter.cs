using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaoloCharacter : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int characterMovement;
    public int dexterity;
    public OverlayTile activeTile;
    public bool canMove = false;


 /*   public UiAnimation uI;*/


    private void Start()
    {

        characterMovement = 4;
        dexterity = 1;
    }

}
