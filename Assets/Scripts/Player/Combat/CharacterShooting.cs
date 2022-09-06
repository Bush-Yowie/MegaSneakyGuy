using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    public Gun gun;

    public int shootButton;
    public KeyCode reloadKey;

    void Update()
    {
        // shoots gun on mouse button 0
        if (Input.GetMouseButton(shootButton))
        {
            gun.Shoot();
        }

        // reloads on reload key (set to tab as default)
        if (Input.GetKeyDown(reloadKey))
        {
            gun.Reload();
        }
    }
}
