using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int characterMovement;
    public int dexterity;
    public bool isHuman = true;
    public OverlayTile activeTile;
    public bool canMove = false;

    public List<TMP_Text> tMP_Texts1Clicker;
    public List<TMP_Text> tMP_Texts2Clicker;
    public List<TMP_Text> tMP_Texts3Clicker;
    public List<TMP_Text> tMP_Texts4Clicker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
