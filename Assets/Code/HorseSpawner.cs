using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseSpawner : MonoBehaviour
{
    public float timeBetweenHorses = 1f;

    float timeSinceHorse = 0f;

    void Update()
    {
        timeSinceHorse += Time.deltaTime;
        
        if(timeSinceHorse > timeBetweenHorses)
        {
            timeSinceHorse -= timeBetweenHorses;

            Horse horse = Instantiate(PrefabLibrary.instance.Horse);

            horse.Neigh();

            horse.transform.position = Random.insideUnitSphere * 5f;
            horse.transform.rotation = Random.rotation;
        }
    }
}
