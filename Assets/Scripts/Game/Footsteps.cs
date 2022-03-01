﻿using UnityEngine;
using System.Collections;

public class WalkScript : MonoBehaviour
{

    CharacterController cc;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (cc.isGrounded == true && cc.velocity.magnitude > 1f && GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
