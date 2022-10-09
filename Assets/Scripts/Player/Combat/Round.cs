using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{
    public float damage;
    private bool timerStart = false;
    private float timer = 10;

    void OnCollisionEnter(Collision other)
    {
        /*
        //if other collider has health script deal damage then destroy bullet, else destroy bullet
        if (other.gameObject.GetComponent<PlayerHealth>() != null)
        {
            float targetHealth = other.gameObject.GetComponent<PlayerHealth>().health;

            targetHealth -= damage;

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
        float targetHealth = other.gameObject.GetComponent<PlayerHealth>().health;
        /*
        if (target != null)
        {

            target.Hit(damage);
            Destroy(gameObject); // Deletes the round if hits
        }
        else
        {
            timerStart = true;
        }
        */
        if (targetHealth != null)
        {
            targetHealth -= damage; //.Hit(damage);
            Destroy(gameObject);// Deletes the round if hits
        }
        else
        {
            timerStart = true;
        }
        
    }

    private void Start()
    {
        timerStart = true;
    }
    private void FixedUpdate()
    {
        // Despawns bullets after a set time to reduce lag
        if (timerStart == true)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
