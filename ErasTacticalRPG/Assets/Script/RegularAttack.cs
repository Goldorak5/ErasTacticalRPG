using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class RegularAttack: MonoBehaviour
{
    
    public BaseCharacter characterScript;
    public BaseCharacter targetedEnemy;
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
    private List<TMP_Text> listOfClicker;
    private bool listInitialize = false;
    private int indexText = 0;
    private bool stopCountDown = false;
    private Weapons weaponScript;
    private GameObject weaponInstance = null;
    public GameObject weaponPrefab;
    
    private Coroutine coroutine;
    private int boxValue;
    private bool isRandomWaitRunning = true;


    void Awake()
    {

        
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
                if(characterScript != null && !characterScript.isHuman)
                {
                    elapsedTime += Time.deltaTime;
                }
        }
    }

    private void ChooseClickerList()
    {
        
        if (weaponScript.ClickerNovice >= 1)
        {
            endCount = 4;
            clickerSelected = clickerNoviceSelected; 
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerNovice--;
            numClicker--;
        }
        else if (weaponScript.ClickerIntermidiaire >= 1)
        {
            endCount = 6;
            clickerSelected = clickerIntermediaireSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerIntermidiaire--;
            numClicker--;
        }
        else if (weaponScript.ClickerExpert >= 1)
        {
            endCount = 10;
            clickerSelected = clickerExpertSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerExpert--;
            numClicker--;
        }
        else if (weaponScript.ClickerMaitre >= 1)
        {
            endCount = 12;
            clickerSelected = clickerMaitreSelected;
            coroutine = CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerMaitre--;
            numClicker--;
        }
    }

    public void StartClickersBoxe(BaseCharacter target)
    {
        
        targetedEnemy = target;
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
        if(weaponInstance == null)
        {
            weaponInstance = Instantiate(weaponPrefab);
            weaponScript = weaponInstance.GetComponent<Weapons>();
        }

        switch (weaponScript.numBoxClicker)
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
        numClicker = weaponScript.numBoxClicker;
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
     
        listOfClicker = null;
        indexText = 0;
        stopCountDown = false;
        totalDamage = 0;
         listInitialize = false;
        Destroy(weaponInstance);
        weaponInstance = null;
        if (characterScript.isHuman) 
        {
            characterScript.AsAttack = true;
        }
        coroutine = null;
    }

    private IEnumerator WaitToStart()
    {
        yield return null;
        if (numClicker > 0)
        {
            //transform content of text box into integer for addition
            if (listOfClicker != null)
            {
                int.TryParse(listOfClicker[indexText].text, out boxValue);
            }
            
//             if (!characterScript.isHuman)
//             {
//                 yield return new WaitForSeconds(1);
//             }
            totalDamage += boxValue;
            indexText++;
            stopCountDown = false;
            StartClickersBoxe(targetedEnemy);
        }
        else
        {
            if (coroutine != null)
            {
                CountCoroutineManager.Instance.StopCoroutine(coroutine);
            }
            if (listOfClicker != null)
            {
                int.TryParse(listOfClicker[indexText].text, out boxValue);
            }
            totalDamage += boxValue;
//             if (!characterScript.isHuman)
//             {
//               /*  yield return new WaitForSeconds(1);*/
//                 Enemies enemie = characterScript as Enemies;
//                 enemie.hasAttacked = true;
//             }
            Debug.Log(totalDamage);
            if(targetedEnemy != null)
            {
                targetedEnemy.handleDamage(totalDamage);
            }
            if (!characterScript.isHuman)
            {
                Enemies enemie = characterScript as Enemies;
                enemie.hasAttacked = true;
                Debug.Log(enemie.hasAttacked);
            }
            yield return new WaitForSeconds(2);
            ReinitializeValue(); 

        }
    }
    private IEnumerator RandomWaitToStopBox()
    {
        isRandomWaitRunning = false;
        float randomSecondes = Random.Range(0.2f,5f);
        yield return new WaitForSeconds(randomSecondes);
        stopCountDown = true;
        if (coroutine != null)
        {
        CountCoroutineManager.Instance.StopCoroutine(coroutine);
        }
        //         yield return null;
        isRandomWaitRunning = true;
        StartCoroutine(WaitToStart());
        
    }

    void Update()
    {
        
        if (characterScript != null && characterScript.IsMyTurn && characterScript.isHuman)
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
            Debug.Log("Enemy Attack!!");
            StartCoroutine(RandomWaitToStopBox());
            }


        }
    }
}
