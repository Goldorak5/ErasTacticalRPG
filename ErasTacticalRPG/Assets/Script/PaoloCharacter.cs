using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// public enum CharacterState
// {       Ideling,
//         Attacking,
//         Clickers,
//         Abilities,
//         Moving
// }

public class PaoloCharacter : BaseCharacter
{
   
    private void Start()
    {
        movementPoints = 4;
        dexterity = 1;
        activeTile.isBlocked = true;
    }



}
