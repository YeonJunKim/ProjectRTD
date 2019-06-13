using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Laser : Projectile_Base
{
    const float lifeTime = 0.2f;
    float laserSpeed = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void FireProjectile(Transform _target, float _damage, float _range)
    {
        base.FireProjectile(_target, _damage, _range);
        StartCoroutine(MoveForward());
    }

    IEnumerator MoveForward()
    {
        float endTime = Time.time + lifeTime;

        while (true)
        {
            if (endTime < Time.time)
            {
                break;
            }
            else
            {
                transform.Translate(Vector3.forward * Time.deltaTime * (1 / cur_moveSpeed) * laserSpeed);
            }
            yield return null;
        }
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
