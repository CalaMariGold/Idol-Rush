using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator am;
    PlayerController pm;
    SpriteRenderer sr;

    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerController>();
        sr = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if(pm.movement.x != 0 || pm.movement.y != 0)
        {
            am.SetBool("Move", true);
            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if(pm.movement.x > 0)
        {
            sr.flipX = true;
        }
        else if(pm.movement.x < 0)
        {
            sr.flipX = false;
        }
    }
}
