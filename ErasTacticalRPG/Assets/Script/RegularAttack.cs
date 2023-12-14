using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RegularAttack: MonoBehaviour
{
    public PaoloCharacter characterScript;
    private float delay = 0.2f;
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
    public List<TMP_Text> listOfClicker;
    public bool listInitialize = false;
    private int indexText = 0;
    private bool stopCountDown = false;
    private Weapons weaponScript;
    private GameObject weaponInstance;
    public GameObject weaponPrefab;


    void Awake()
    {
        characterScript = FindObjectOfType<PaoloCharacter>();
        stopCountDown = false;
        weaponInstance = Instantiate(weaponPrefab);
        weaponScript = weaponInstance.GetComponent<Weapons>();
        
    }

    private IEnumerator CountCoroutine(TMP_Text countDownText)
    {
        
        while (!stopCountDown)
        {
            Debug.Log(stopCountDown.ToString());

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
                if(stopCountDown)
                {
                    break;
                }
                countDownText.text = i.ToString("0");
                yield return new WaitForSeconds(delay);
            }
        }
    }

    private void ChooseClickerList()
    {
        
        if (weaponScript.ClickerNovice >= 1)
        {
            endCount = 4;
            clickerSelected = clickerNoviceSelected; 
            CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerNovice--;
            numClicker--;
        }
        else if (weaponScript.ClickerIntermidiaire >= 1)
        {
            endCount = 6;
            clickerSelected = clickerIntermediaireSelected;
            CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerIntermidiaire--;
            numClicker--;
        }
        else if (weaponScript.ClickerExpert >= 1)
        {
            endCount = 10;
            clickerSelected = clickerExpertSelected;
            CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerExpert--;
            numClicker--;
        }
        else if (weaponScript.ClickerMaitre >= 1)
        {
            endCount = 12;
            clickerSelected = clickerMaitreSelected;
            CountCoroutineManager.Instance.StartCoroutine(CountCoroutine(listOfClicker[indexText]));
            weaponScript.ClickerMaitre--;
            numClicker--;
        }
    }

    public void StartClickersBoxe()
    {
        InitializeCharacterAndList();

        switch (weaponScript.numBoxClicker)
        {
            case 1:
                ChooseClickerList();
                break;
            case 2:
                ChooseClickerList();
                break;
            case 3:
                ChooseClickerList();
                break;
            case 4:
                ChooseClickerList();
                break;
        }
    }

    private void InitializeCharacterAndList()
    {
           
            //initialize right list of boxes
//             if (weaponScript == null)
//         {
//             weaponScript = FindAnyObjectByType<Weapons>();
//         }

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

    private IEnumerator WaitToStart()
    {
        yield return null;
        if (numClicker > 0)
        {
            indexText++;
            stopCountDown = false;
            StartClickersBoxe();
        }
        else
        {
            CountCoroutineManager.Instance.StopAllCoroutines();
            stopCountDown = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
//             //stop count
            stopCountDown = true;
            CountCoroutineManager.Instance.StopAllCoroutines();
            StartCoroutine(WaitToStart());
        }
    }
}
