using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health, maxhealth = 100;




    private void Start()
    {
        health = maxhealth;



    }
    void Update()
    {
        // updates the canvas

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// 'Hits' the target for a certain amount of damage
    public void Hit(float damage)
    {

        health -= damage;
        Debug.Log("hit");

    }


}

