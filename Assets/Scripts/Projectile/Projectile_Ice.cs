using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Ice : Projectile_Base
{
    Vector3 startPos;
    Vector3 targetPos;
    float startTime;

    public GameObject particalPrefab;

    public override void FireProjectile(Transform _target, float _damage, float _range)
    {
        base.FireProjectile(_target, _damage, _range);

        startPos = transform.position;
        startPos.y += 0.5f; // move it a little bit up to see it above ground;
        targetPos = _target.position;
        targetPos.y += 0.5f; // move it a little bit up to see it above ground;
        startTime = Time.time;

        StartCoroutine(MoveTowardTheTarget());
    }

    IEnumerator MoveTowardTheTarget()
    {
        float progress;
        while (true)
        {
            progress = (Time.time - startTime) / cur_moveSpeed;
            if (progress >= 1)
            {
                break;
            }
            else
            {
                // only update the target's position when the target is alive
                // (to prevent setting the position when the target goes back to the pool) 
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    targetPos = target.position;
                    targetPos.y += 0.5f; // move it a little bit up to see it above ground;
                    LookAt_Yaxis(targetPos);
                }
                else
                {
                    target = null;
                }
                transform.position = Vector3.Lerp(startPos, targetPos, progress);
            }
            yield return null;
        }
        OnTargetHit();
    }

    void OnTargetHit()
    {
        // the target could already be dead.. check..
        if (target != null && target.gameObject.activeInHierarchy)
        {
            BaseGameEntity targetEntity = target.GetComponent<BaseGameEntity>();
            targetEntity.DecreaseHp(cur_damage);

            if (particalPrefab != null)
            {
                GameObject partical = Instantiate<GameObject>(particalPrefab);
                partical.transform.position = target.transform.position;

                SmartParticle sp = partical.GetComponent<SmartParticle>();
                if(sp != null)
                    sp.StickToTarget(target);
            }

            if (isBufferUsed)
                targetEntity.RegisterBuffer(buffer);
        }
        EntityManager.S.ReturnEntity(this);
    }
}