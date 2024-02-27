using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public enum TurnState 
{
PlayersTurn,
EnemiesTurn
}

public class TurnManager : MonoBehaviour
{
    private List<BaseCharacter> charactersList = new List<BaseCharacter>();
    private int indexList;
    private bool gameOver = false;
    public TurnState turnState;
    private int turnNumber = 1;

    public IEnumerator PlayGame()
    {
        while (!gameOver)
        {
            //initialize the player in the game each time so if some are dead they wont be in the list
            InitializeVariables();

            Debug.Log("Turn: " + turnNumber);
            for(int i = 0; i < charactersList.Count; i++) 
            {
                indexList = i; 
                yield return StartCoroutine(PlayTurn(i));
                yield return new WaitForSeconds(0.5f);
            }
            turnNumber++;
        }
    }

    IEnumerator PlayTurn(int playerNumber)
    {
        BaseCharacter currentPlayerTurn = charactersList[playerNumber];
        currentPlayerTurn.IsMyTurn = true;
        if (currentPlayerTurn.isHuman)
        {
            turnState = TurnState.PlayersTurn;
            currentPlayerTurn.hasAttack = false;
            Debug.Log("Player Turn: " + currentPlayerTurn.name);

            currentPlayerTurn.movementPoints = currentPlayerTurn.maxMovementPoints;
            while (!currentPlayerTurn.endTurn)
            {
                //Player Turn
                yield return null;
            }
        currentPlayerTurn.endTurn = false;
        currentPlayerTurn.IsMyTurn = false;
            
        }
        else
        {  //execute AI Turn
            Enemies enemie = currentPlayerTurn as Enemies;
            turnState = TurnState.EnemiesTurn;
            if(enemie != null)
            {
                Debug.Log("Enemies Turn: " + currentPlayerTurn.name);
                enemie.IsMyTurn = true;
                currentPlayerTurn.hasAttack = false;

                enemie.MoveEnemy();
                yield return new WaitUntil(() => enemie.hasMoved);
                enemie.movementPoints = enemie.maxMovementPoints;
                enemie.HasMoveFlagFalse();

                if (enemie.FindClosestTarget())
                {
                    /*Debug.Log("Attacking");*/
                    enemie.EnemyAttacking();
                    /*Debug.Log("Enemy Attacking");*/
                    yield return new WaitUntil(() => enemie.hasAttack);
                  /*  Debug.Log("Enemy As attack");*/
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
       //find all object that can play
       BaseCharacter[] listCharacterInScene = GameObject.FindObjectsOfType<BaseCharacter>();

            //add them to the list of character
        foreach (BaseCharacter elem in listCharacterInScene)
        {
            if (elem != null && !charactersList.Contains(elem) && IsNotDead(elem))
            {
                charactersList.Add(elem);
            }
        }
        //put the list in order the biggest Speed first
        charactersList.Sort((a,b) => b.turnSpeed.CompareTo(a.turnSpeed));
    }

    private void StillRemainsPlayer()
    {
        if(charactersList.Any(character => character.isHuman))
        {
            gameOver = false;
        }
        gameOver = true;
    }

    private static bool IsNotDead(BaseCharacter elem)
    {
        return elem.characterState != CharacterState.Dead;
    }

    public void CharacterEndTurn()
    {
        if (charactersList != null)
        {
            charactersList[indexList].endTurn = true;
            charactersList[indexList].movementPoints = charactersList[indexList].maxMovementPoints;
        }
    }
}
