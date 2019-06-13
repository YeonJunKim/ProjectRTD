using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bomb : Projectile_Base
{
    Vector3 startPos;
    Vector3 targetPos;
    float startTime;

    public float explodeSize;
    public GameObject explodeMesh;
    public GameObject projectileMesh;
    public GameObject particalPrefab;

    const float explodeDuration = 0.15f;

    protected override void Start()
    {
        base.Start();
        explodeMesh.SetActive(false);
    }

    public override void FireProjectile(Transform _target, float _damage, float _range)
    {
        base.FireProjectile(_target, _damage, _range);

        startPos = transform.position;
        targetPos = _target.position;
        startTime = Time.time;

        explodeMesh.transform.localScale = Vector3.zero;
        explodeMesh.SetActive(false);
        projectileMesh.SetActive(true);

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
                float yPos = Mathf.PingPong(progress, 0.5f) * 2;
                Vector3 pos = Vector3.Lerp(startPos, targetPos, progress);
                pos.y = yPos;
                transform.position = pos;
            }
            yield return null;
        }
        StartCoroutine(OnTargetHit());
    }

    IEnumerator OnTargetHit()
    {
        explodeMesh.SetActive(true);
        projectileMesh.SetActive(false);
        GameObject partical = Instantiate<GameObject>(particalPrefab);
        partical.transform.position = transform.position;

        float explodeTime = Time.time;
        while (true)
        {
            float progress;
            progress = (Time.time - explodeTime) / explodeDuration;
            if (progress >= 1)
            {
                break;
            }
            else
            {
                explodeMesh.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explodeSize, progress);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);  // looks better..
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
