using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Flame : Projectile_Base
{
    public GameObject flameObject;

    public float scaleWidth;
    Vector3 originalScale;

    Vector3 startPos;
    Vector3 targetPos;
    float startTime;

    protected override void Awake()
    {
        base.Awake();
        originalScale = flameObject.transform.localScale;
    }


    public override void FireProjectile(Transform _target, float _damage, float _range)
    {
        base.FireProjectile(_target, _damage, _range);

        startPos = transform.position;
        targetPos = ((_target.position - startPos).normalized * _range) + transform.position;  // the position where flame will end
        startTime = Time.time;

        flameObject.transform.localScale = originalScale;   // make it as original size before fire

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
                transform.position = Vector3.Lerp(startPos, targetPos, progress);
                Vector3 scale = Vector3.Lerp(originalScale, originalScale * scaleWidth, progress);
                scale.y = originalScale.y;  // don't change Y scale because of the collider bound
                flameObject.transform.localScale = scale;
            }
            yield return null;
        }
        OnTargetPosHit();
    }

    void OnTargetPosHit()
    {
        EntityManager.S.ReturnEntity(this);
    }

    public override void OnTriggerEnter_FromCollider(BaseGameEntity target)
    {
        // the target could already be dead.. check..
        if (target.gameObject.activeInHierarchy)
        {
            if (collidedList.Contains(target) == false)
            {
                collidedList.Add(target);
                target.DecreaseHp(cur_damage);

                if (isBufferUsed)
                    target.RegisterBuffer(buffer);
            }

            if (curCollidingList.Contains(target) == false)
                curCollidingList.Add(target);
        }
    }
}
