using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreUpdate : MonoBehaviour
{

    public AudioSource collectSound;


    private void OnTriggerEnter(Collider other)
    {
        collectSound.Play();
        ScoringSystem.theScore += 1;
        //Destroy(gameObject);


        if (ScoringSystem.theScore >= 20)
        {
            SceneManager.LoadScene(2);
        }
    }
}
