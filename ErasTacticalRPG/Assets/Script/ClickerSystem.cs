using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ClickerSystem: MonoBehaviour
{    
    public Weapon weaponScript;

    public BaseCharacter characterScript;
    private GameObject characterGameObject;
    public List<BaseCharacter> targetedEnemies;
    [SerializeField]private float rollingSpeed = 0.2f;
    private int totalDamage;

    private int count = 1;
    private int endCount;
    private int startCountNovice = 1;
    private int startCountIntermediaire = 2;
    private int startCountExpert = 4;
    private int startCountMaitre = 8;

    private int numClicker;
    private string clickerSelected;
    private string clickerNoviceSelected = "Novice";
    private string clickerIntermediaireSelected = "Intermediaire";
    private string clickerExpertSelected = "Expert";
    private string clickerMaitreSelected = "Maitre";
    private int numClickerNovice;
    private int numClickerIntermediaire;
    private int numClickerExpert;
    private int numClickerMaitre;


    private List<TMP_Text> listOfClicker;
    private bool listInitialize = false;
    private int indexText = 0;
    private bool stopCountDown = false;

    private Coroutine coroutine;
    private int boxValue;
    private bool isRandomWaitRunning = true;
    private bool isReinitializingVariables = false;
    public TurnManager turnManager;


    void Awake()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
    }

    private IEnumerator CountCoroutine(TMP_Text countDownText)
    {
        float elapsedTime = 0f;
        float randomDuration = Random.Range(1f, 5f);

        while (elapsedTime < randomDuration)
        {
            if (stopCountDown)
            {
               yield break;
            }
            if (clickerSelected == clickerNoviceSelected)
              {
                  count = startCountNovice;
              }
              else if (clickerSelected == clickerIntermediaireSelected)
              {
                  count = startCountIntermediaire;
              }
              else if (clickerSelected == clickerExpertSelected)
              {
                  count = startCountExpert;
              }
              else if (clickerSelected == clickerMaitreSelected)
              {
                  count = startCountMaitre;
              }
               int startCount = count;

                for (int i = startCount; i <= endCount; i++)
                {
                    if (stopCountDown)
                    {
                        yield break;
                    }
                    countDownText.text = i.ToString("0");
                    yield return new WaitForSeconds(rollingSpeed);
                        if (characterScript != null && !characterScript.isHuman)
                        {
                        elapsedTime += Time.deltaTime;
                        }
                }
        }
    }

    private void ChooseClickerList()
    {
        
        if (numClickerNovice >= 1)
        {
            endCount = 4;
            clickerSelected = clickerNoviceSelected; 
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            numClickerNovice--;
            numClicker--;
        }
        else if (numClickerIntermediaire >= 1)
        {
            endCount = 6;
            clickerSelected = clickerIntermediaireSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            numClickerIntermediaire--;
            numClicker--;
        }
        else if (numClickerExpert >= 1)
        {
            endCount = 10;
            clickerSelected = clickerExpertSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            numClickerExpert--;
            numClicker--;
        }
        else if (numClickerMaitre >= 1)
        {
            endCount = 12;
            clickerSelected = clickerMaitreSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            numClickerMaitre--;
            numClicker--;
        }
    }

    public void StartClickersBoxe(List<BaseCharacter> targets)
    {
        targetedEnemies = targets;
        if (!listInitialize)
        {
            InitializeCharacterAndList();
            listInitialize = true;
        }
        ChooseClickerList();
    }

    private void InitializeCharacterAndList()
    {
        //initialize right list of boxes
        characterScript = GetComponent<BaseCharacter>();
        stopCountDown = false;
        isRandomWaitRunning = true;

        characterGameObject = characterScript.gameObject;
        weaponScript = characterScript.GetComponentInParent<Weapon>();

        //numClicker = weaponScript.ClickerNovice + weaponScript.ClickerIntermidiaire + weaponScript.ClickerExpert + weaponScript.ClickerMaitre;
        numClickerNovice = characterScript.clickerNovice;
        numClickerIntermediaire = characterScript.clickerIntermediaire;
        numClickerExpert = characterScript.clickerExpert;
        numClickerMaitre = characterScript.clickerMaitre;
        numClicker = numClickerNovice + numClickerIntermediaire + numClickerExpert + numClickerMaitre;
        // }
        switch (numClicker)
                {
                    case 1:
                        listOfClicker = characterScript.tMP_Texts1Clicker; break;
                    case 2:
                        listOfClicker = characterScript.tMP_Texts2Clicker; break;
                    case 3:
                        listOfClicker = characterScript.tMP_Texts3Clicker; break;
                    case 4:
                        listOfClicker = characterScript.tMP_Texts4Clicker; break;
                default:
                        break;
                }
    }

    private void ReinitializeValue()
    {
        int indexToErase = 0;
        if (listOfClicker != null)
        {
            while (indexToErase < listOfClicker.Count)
            {
                listOfClicker[indexToErase].text = "";
                indexToErase++;
            }
        }
     
        indexText = 0;
        stopCountDown = false;
        totalDamage = 0;
         listInitialize = false;
        targetedEnemies.Clear();
        if (characterScript.isHuman) 
        {
            characterScript.HasAttack = true;
        }

        isReinitializingVariables = true;
    }

    private IEnumerator WaitToStart()
    {
        yield return null;
        if (numClicker > 0)
        {
            if (listOfClicker != null)
            {//transform content of text box into integer for addition
                int.TryParse(listOfClicker[indexText].text, out boxValue);
            }
            
            totalDamage += boxValue;
            indexText++;
            stopCountDown = false;
            StartClickersBoxe(targetedEnemies);
            isRandomWaitRunning = true;
        }
        else
        {
            isRandomWaitRunning = false;
            if (coroutine != null)
            {
                CountCoroutineManager.Instance.StopCoroutine(coroutine);
            }
            if(listOfClicker != null)
                {
                //transform content of text box into integer for addition else take ToString function
                int.TryParse(listOfClicker[indexText].text, out boxValue);
                }
            totalDamage += boxValue;

            if(targetedEnemies != null)
            {
                foreach(BaseCharacter target in targetedEnemies)
                {
                        //Healing
                   if(characterScript.isHuman && target.isHuman) 
                   {
                        target.HandleHealing(totalDamage);
                        Debug.Log(totalDamage + " Healing To " + target.name);
                   }
                   else
                   {
                        //Damaging
                        target.handleDamage(totalDamage, target);
                        Debug.Log(totalDamage + " Damage To " + target.name);
                   }
                }
            }
            if (!characterScript.isHuman)
            {
                Enemies enemie = characterScript as Enemies;
                enemie.hasAttack = true; 
            }
            
            ReinitializeValue();
            yield return new WaitUntil(() => isReinitializingVariables);
            isReinitializingVariables = false;
            StopAllCoroutines();
        }
    }
    private IEnumerator RandomWaitToStopBox()
    {
        isRandomWaitRunning = false;
        float randomSecondes = Random.Range(0.1f,3f);
        yield return new WaitForSeconds(randomSecondes);
        stopCountDown = true;
        if (coroutine != null)
        {
        CountCoroutineManager.Instance.StopCoroutine(coroutine);
        }
        StartCoroutine(WaitToStart());
    }

    void Update()
    { 
        if (characterScript != null && characterScript.IsMyTurn && characterScript.isHuman && characterScript.characterState == CharacterState.Clickers)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //stop count
                stopCountDown = true;
                CountCoroutineManager.Instance.StopCoroutine(coroutine);
                coroutine = null;
                StartCoroutine(WaitToStart());
            }
        }else if (characterScript != null && characterScript.IsMyTurn && !characterScript.isHuman)
        {
            if(isRandomWaitRunning)
            {
            //stop count
            StartCoroutine(RandomWaitToStopBox());
            }
        }
    }
}
