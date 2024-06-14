using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PaoloCharacter : BaseCharacter
{
    private Weapon weapon;




    private void Start()
    {
        movementPoints = maxMovementPoints;
        dexterity = 0;
        activeTile.isBlocked = true;
        healthArmorCanvasTransform = transform.Find("HealthArmorCanvas");
        healthImage = healthArmorCanvasTransform.Find("HealthImage")?.GetComponent<Image>();
        armorImage = healthArmorCanvasTransform.Find("ArmorImage")?.GetComponent<Image>();
        HideHealthArmor();
        tMP_TextsArmorBox.text = maxArmor.ToString();
        tMP_TextsHealthBox.text = maxHealth.ToString();
        endTurn = false;

        //weapon initialize
        weapon = new Weapon();
        weapon.ClickerNovice = clickerNovice;
        weapon.ClickerIntermidiaire = clickerIntermediaire;
        weapon.ClickerExpert = clickerExpert;
        weapon.ClickerMaitre = clickerMaitre;
    }
}
