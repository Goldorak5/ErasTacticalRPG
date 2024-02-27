using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterState
{
    Ideling,
    Attacking,
    Clickers,
    Abilities,
    Moving,
    Dead
}

public class BaseCharacter : MonoBehaviour
{
    public int health;
    public int armor;
    public int maxArmor;
    public int maxHealth;
    public int movementPoints;
    public int maxMovementPoints;
    public int characterMovementSpeed;
    public int turnSpeed;
    public CharacterState characterState;
    public int dexterity;
    public OverlayTile activeTile;
    public Transform healthArmorCanvasTransform;
    public Image healthImage;
    public Image armorImage;
    public List<TMP_Text> tMP_Texts1Clicker;
    public List<TMP_Text> tMP_Texts2Clicker;
    public List<TMP_Text> tMP_Texts3Clicker;
    public List<TMP_Text> tMP_Texts4Clicker;
    public TMP_Text tMP_TextsTotalDamage;
    public TMP_Text tMP_TextsHealthBox;
    public TMP_Text tMP_TextsArmorBox;

    public bool isHuman = true;
    /*[HideInInspector] */public bool endTurn = false;
    /*[HideInInspector] */public bool canMove = false;
    private bool isMyTurn = false;
    public new string name;
    public bool IsMyTurn
    {
        get { return isMyTurn; }
        set { isMyTurn = value; }
    }
    public bool hasAttack = false;
    public bool HasAttack
    {
        get { return hasAttack; }
        set { hasAttack = value; }
    }
    public Canvas healthArmorCanvas;
    public bool isReceivingDamage;


    public void HandleHealing(int totalHealing)
    {
        tMP_TextsTotalDamage.color = Color.green;
        int overHealing = 0;
        //if character have no more armor and missing health
        if (health < maxHealth)
        {
            health += totalHealing;
            if (health > maxHealth) 
            {
                overHealing = maxHealth - health;
                health = maxHealth;
            }
        }
        // if character is max health and missing armor
        else
        {
            armor += totalHealing;
            if(armor > maxArmor)
            {
                armor = maxArmor;
            }
        }
        if (overHealing > 0)
        {
            armor += overHealing;
            if (armor > maxArmor)
            {
                armor = maxArmor;
            }
        }
        StartCoroutine(ShowDamage(totalHealing));
    }

    public void handleDamage(int damage, BaseCharacter damagedCharacter)
    {
        tMP_TextsTotalDamage.color = Color.red;
        int overDamage = 0;
        if (armor > 0)
        {
            armor -= damage;
            if (armor <= 0)
            {
                overDamage = Mathf.Abs(armor);
                armor = 0;
            }
        }
        else health -= damage;
        
        if (health > 0)
            {
                if (overDamage > 0)
                {
                    health -= overDamage;
                }

            }else if (health <= 0) 
        { 
            health = 0;
            Debug.Log(damagedCharacter + "is Dead!!!");
            damagedCharacter.characterState = CharacterState.Dead;
            damagedCharacter.activeTile.isBlocked = false;
            Destroy(damagedCharacter.gameObject);
        }
        StartCoroutine(ShowDamage(damage));
    }

    private IEnumerator ShowDamage(int totalDamage)
    {
        isReceivingDamage = true;
        tMP_TextsTotalDamage.text = totalDamage.ToString();
        ShowHealthArmor();
        yield return new WaitForSeconds(.5f);
        UpdateHealthArmorText();
        UpdateFillAmountHealthArmor();
        yield return new WaitForSeconds(2);
        isReceivingDamage = false;
        HideHealthArmor();
        tMP_TextsTotalDamage.text = " ";
    }

    public void UpdateHealthArmorText()
    {
        tMP_TextsHealthBox.text = health.ToString();
        tMP_TextsArmorBox.text = armor.ToString();
    }

    public void UpdateFillAmountHealthArmor()
    {  
        float healthFillValue;
        float armorFillValue;

        if(healthImage != null && armorImage != null)
        {
            //make sure that they don't go under or over the limit floor to int cut decimal
            health = Mathf.Clamp(health, 0, Mathf.FloorToInt(maxHealth));
            armor = Mathf.Clamp(armor, 0, Mathf.FloorToInt(maxArmor));

        healthFillValue = (float)health/maxHealth;
        armorFillValue = (float)armor /maxArmor;

        healthImage.fillAmount = healthFillValue;
        armorImage.fillAmount = armorFillValue;
        }
    }

    public void ShowHealthArmor()
    {
        healthArmorCanvas.enabled = true;
        healthArmorCanvas.sortingOrder = 10;
    }
    public void HideHealthArmor()
    {
        healthArmorCanvas.enabled = false;
    }

}
