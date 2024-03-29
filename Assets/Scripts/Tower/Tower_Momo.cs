﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Momo : Tower
{
    public ParticleSystem charge1;
    public ParticleSystem charge2;
    public ParticleSystem fire;

    float chargeTime;
    const float laserDuration = 0.3f;       // laserDuration * laserFrequency * cur_damage = damage
    const float laserFrequency = 0.02f;     // laserDuration * laserFrequency * cur_damage = damage
    float laserBulletsPerAttack;
    bool lookAtTarget;    // this is used in laser tower, when charging laser

    AudioSource audioSource;

    public override void OnCreate()
    {
        base.OnCreate();
        charge1.Stop();
        charge2.Stop();
        chargeTime = ATTK_SPEED / 3f;
        lookAtTarget = true;
        audioSource = GetComponent<AudioSource>();
        laserBulletsPerAttack = laserDuration / laserFrequency;
    }

    // Update is called once per frame
    protected override void Update()
    {
        BufferManagement();

        if(lookAtTarget)
            LookAtTarget();

        AttackTarget();
    }

    public override void Attack(BaseGameEntity target)
    {
        StartCoroutine("StartCharge");
    }

    IEnumerator StartCharge()
    {
        lookAtTarget = false;

        audioSource.Play();
        charge1.Play();
        charge2.Play();

        float endTime = Time.time + chargeTime;
        while (true)
        {
            if (endTime < Time.time)
                break;

            yield return null;
        }

        charge1.Stop();
        charge2.Stop();

        Enemy target = radarSystem.GetTarget();
        if (target != null)
        {
            fire.Play();
            SetAnimation_Attack();

            endTime = Time.time + laserDuration;
            while (true)
            {
                if (endTime < Time.time)
                    break;

                Projectile_Base projectile = EntityManager.S.GetEntity(projectileType) as Projectile_Base;
                Vector3 pos = transform.position;
                pos.y += 0.5f;  // increase Y pos, so the laser is above the ground
                projectile.transform.position = pos;
                projectile.transform.rotation = transform.rotation; // becuase the tower is already looking at the enemy
                projectile.FireProjectile(target.transform, cur_damage / laserBulletsPerAttack, cur_attkRange); // Laser projectile doesn't use target, so we don't need to check null
                yield return new WaitForSeconds(laserFrequency);
            }
        }
        else
        {
            audioSource.Stop();
        }
        yield return new WaitForSeconds(chargeTime);
        audioSource.Stop();
        lookAtTarget = true;
    }


    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 5);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 2;
        animator.SetInteger("animation", 2);
        Invoke("SetAnimation_Idle", 0.3f);
    }
}
