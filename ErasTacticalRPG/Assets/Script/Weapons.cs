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

    public new string name;

    public int ClickerNovice;
    public int ClickerIntermidiaire;
    public int ClickerExpert;
    public int ClickerMaitre;
    public int numBoxClicker;


    void Awake()
    {
         numBoxClicker = ClickerNovice + ClickerIntermidiaire + ClickerExpert + ClickerMaitre;
    }
}
