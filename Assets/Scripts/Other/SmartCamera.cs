using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    Transform target;
    bool keepFollow;

    Vector3 deltaPos;

    void Awake()
    {
        target = null;
        keepFollow = false;
    } 

    // Update is called once per frame
    void Update()
    {
        if(keepFollow && target != null)
        {
            transform.position = target.position + deltaPos;
        }
    }

    public void SetLookAtTarget(Transform _target, bool _keepFollow)
    {
        if (_target == null)
            return;

        target = _target;
        keepFollow = _keepFollow;

        transform.position = _target.position;
        transform.rotation = Quaternion.identity;

        Vector3 lookAtPos = _target.position;
        if (keepFollow)
        {
            EnemyType enemyType = _target.GetComponent<Enemy>().enemyType;

            if(enemyType == EnemyType.Golem_1)
            {
                transform.Translate(_target.forward * 3f);
                transform.Translate(_target.up * 2f);
                lookAtPos.y += 1f;
            }
            else
            {
                transform.Translate(_target.forward * 2f);
                transform.Translate(_target.up * 1.5f);
                lookAtPos.y += 0.5f;
            }
        }
        else
        {
            transform.Translate(_target.forward * 1.5f);
            transform.Translate(_target.up * 1f);
        }
        transform.LookAt(lookAtPos);
        deltaPos = transform.position - _target.position;
    }
}
