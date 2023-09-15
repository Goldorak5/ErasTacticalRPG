using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaoloCharacter : MonoBehaviour
{

    public int characterMovement;
    public int dexterity;
    public OverlayTile activeTile;

    private void Start()
    {
        characterMovement = 4;
        dexterity = 1;
    }
}
