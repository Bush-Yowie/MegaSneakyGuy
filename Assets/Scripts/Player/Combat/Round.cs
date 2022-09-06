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

        Target target = other.gameObject.GetComponent<Target>();
        PlayerHealth Health = other.gameObject.GetComponent<PlayerHealth>();
        // Only attempts to inflict damage if the other game object has
        // the 'Target' component

        if (target != null)
        {

            target.Hit(damage);
            Destroy(gameObject); // Deletes the round if hits
        }
        else
        {
            timerStart = true;
        }
        if (Health != null)
        {

            Health.Hit(damage);
            Destroy(gameObject);// Deletes the round if hits
        }
        else
        {
            timerStart = true;
        }
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
            timer = 10;
            timerStart = false;
        }
    }
}
