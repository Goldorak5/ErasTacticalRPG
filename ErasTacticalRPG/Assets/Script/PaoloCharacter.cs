using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CharacterState
{       Ideling,
        Attacking,
        Clickers,
        Abilities,
        Moving
}

public class PaoloCharacter : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int characterMovement;
    public int dexterity;
    public OverlayTile activeTile;
    public bool canMove = false;
    public List<TMP_Text> tMP_Texts1Clicker;
    public List<TMP_Text> tMP_Texts2Clicker;
    public List<TMP_Text> tMP_Texts3Clicker;
    public List<TMP_Text> tMP_Texts4Clicker;
    

//     /*   public UiAnimation uI;*/
//     [Header("1 clickers")]
//     
// 
//     [Header("2 clickers")]
//     public TMP_Text number2ClickText1;
//     public TMP_Text number2ClickText2;
// 
//     [Header("3 clickers")]
//     public TMP_Text number3clickText1;
//     public TMP_Text number3clickText2;
//     public TMP_Text number3clickText3;
// 
//     [Header("4 clickers")]
//     public TMP_Text number4clickText1;
//     public TMP_Text number4clickText2;
//     public TMP_Text number4clickText3;
//     public TMP_Text number4clickText4;
    
    private void Start()
    {
        characterMovement = 4;
        dexterity = 1;
    }

}
