using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class TurnManager : MonoBehaviour
{
    public List<BaseCharacter> character = new List<BaseCharacter>();
    private int indexList;
    private bool gameOver = false;

    public IEnumerator PlayGame()
    {
        while (!gameOver)
        {
            //initialize the player in the game each time so if some are dead they wont be in the list
            InitializeVariables();
            for(int i = 0; i < character.Count; i++) 
            {
                indexList = i; 
                yield return StartCoroutine(PlayTurn(i));
                yield return new WaitForSeconds(1);

            }
        }
    }

    IEnumerator PlayTurn(int playerNumber)
    {
        BaseCharacter currentPlayerTurn = character[playerNumber];
        currentPlayerTurn.IsMyTurn = true;
        if (currentPlayerTurn.isHuman)
        {
            currentPlayerTurn.AsAttack = false;
            Debug.Log("Paolo Turn!");
            while (!currentPlayerTurn.endTurn)
            {
                
                //Player Turn
                yield return null;
            }
        currentPlayerTurn.endTurn = false;
        }
        else
        {  //execute AI Turn
            Enemies enemie = currentPlayerTurn as Enemies;

            if(enemie != null)
            {
                Debug.Log("Enemies Turn!");
                enemie.IsMyTurn = true;
                currentPlayerTurn.AsAttack = false;
                enemie.movementPoints = enemie.maxMovementPoints;

                enemie.MoveEnemy();
                yield return new WaitUntil(() => enemie.hasMoved);
                enemie.HasMoveFlagFalse();

                if (enemie.FindTarget())
                {
                    enemie.AttackEnemy();
                    yield return new WaitUntil(() => enemie.hasAttacked);
                    enemie.HasAttackFlagFalse();
                    
                }
            }
            currentPlayerTurn.endTurn = false;
            enemie.IsMyTurn = false;
        }

        yield return null;
    }

    private void InitializeVariables()
    {
        if (MouseController.characterState != CharacterState.Dead)
        {
       //find all object that can play
       BaseCharacter[] listCharacterInScene = GameObject.FindObjectsOfType<BaseCharacter>();

            //add them to the list of character
            foreach (BaseCharacter elem in listCharacterInScene)
        {
            if (elem != null && !character.Contains(elem))
            {
                character.Add(elem);
            }
        }

        //put the list in order the biggest Speed first
        character.Sort((a,b) => b.turnSpeed.CompareTo(a.turnSpeed));
        }
 
    }
    public void CharacterEndTurn()
    {
        if (character != null)
        {
            character[indexList].endTurn = true;
            character[indexList].movementPoints = character[indexList].maxMovementPoints;
        }
    }
}
