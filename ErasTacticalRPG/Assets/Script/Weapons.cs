using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum WeaponState
{
    Equipped,
    Unequipped
};
public class Weapons: MonoBehaviour
{
    public BaseCharacter character;
    public new string name;
    public int ClickerNovice;
    public int ClickerIntermidiaire;
    public int ClickerExpert;
    public int ClickerMaitre;
    public int numBoxClicker;

    //array first number = first zone width etc...
    public int[] attackZone;
    // 0 equal CAC than numbers equal empty space between character and the zone
    public int attackRange;

    void Awake()
    {
         numBoxClicker = ClickerNovice + ClickerIntermidiaire + ClickerExpert + ClickerMaitre;
        //character = GetComponent<BaseCharacter>();
    }
}
