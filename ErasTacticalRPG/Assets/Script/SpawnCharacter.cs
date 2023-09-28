using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    public GameObject characterPrefab; // The character prefab
    public Vector3 spawnPosition; // The position where the character should be spawned

    void Start()
    {
        Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
    }
}