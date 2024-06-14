using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    singleTarget,
    multiTarget,
    zone,
    multiZone
};

public class Ability : MonoBehaviour
{
    public int howManyTarget;
    public bool allTarget;

    public int clickerNovice;
    public int clickerIntermidiaire;
    public int clickerExpert;
    public int clickerMaitre;

    [HideInInspector]
    public int numBoxClicker;

    // Start is called before the first frame update
    void Awake()
    {
        numBoxClicker = clickerNovice + clickerIntermidiaire + clickerExpert + clickerMaitre;
    }

}
