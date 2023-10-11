using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UiAnimation : MonoBehaviour
{
    public Button attackButton;
    public Button moveButton;
    public Button abilityButton;



    public void OpenAttackMenu()
    {
        attackButton.transform.LeanScale(Vector3.zero, 0.5f);
        moveButton.transform.LeanScale(Vector3.zero, 0.5f);
        abilityButton.transform.LeanScale(Vector3.zero, 0.5f);
        //transform.LeanMoveLocal(new Vector2());
    }

    public void OpenMenu()
    {
    }

    public void OpenAbilityMenu()
    {

    }

    public void ShowMovement()
    {
        
    }

}
