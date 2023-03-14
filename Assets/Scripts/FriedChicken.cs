using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriedChicken : MonoBehaviour
{
    public GameObject[] sprites;

    void Start()
    {
        sprites[Random.Range(0, sprites.Length)].SetActive(true);

    }

}
