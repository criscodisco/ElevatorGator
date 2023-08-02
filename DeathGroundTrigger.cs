using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGroundTrigger : MonoBehaviour
{
    public bool isPlayerDead = false;
    public bool isGameOver = false;
    public Vector3 currentPlayerPosition;

    public Canvas deathCanvas;
    public GatorController player;

    void Start()
    {
        deathCanvas.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {    
            isPlayerDead = true;
            currentPlayerPosition = coll.transform.position;
            deathCanvas.gameObject.SetActive(true);
            GatorController.alligatorPointTotal = GatorController.alligatorPointTotal - 1;
            player.alligatorLifeCounterText.text = GatorController.alligatorPointTotal.ToString();

            if (GatorController.alligatorPointTotal <= 0)
            {
                isGameOver = true;
            }
        }
    }
}
