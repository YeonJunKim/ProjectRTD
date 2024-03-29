﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmartParticle : MonoBehaviour
{
    Transform target;   // if a target is set, it will stick to it
    new ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particleSystem)
        {
            if (!particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
            if (target != null)
            {
                if (target.gameObject.activeInHierarchy)
                    transform.position = target.position;
                else
                    Destroy(gameObject);
            }
        }
    }

    public void StickToTarget(Transform _target)
    {
        target = _target;
    }
}