using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KuroCharacter : BaseCharacter
{
    private void Start()
    {
        movementPoints = 4;
        dexterity = 1;
        activeTile.isBlocked = true;
        healthArmorCanvasTransform = transform.Find("HealthArmorCanvas");
        healthImage = healthArmorCanvasTransform.Find("HealthImage")?.GetComponent<Image>();
        armorImage = healthArmorCanvasTransform.Find("ArmorImage")?.GetComponent<Image>();
        HideHealthArmor();
        tMP_TextsArmorBox.text = maxArmor.ToString();
        tMP_TextsHealthBox.text = maxHealth.ToString();
    }
}
