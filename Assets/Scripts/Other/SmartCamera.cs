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

            if(enemyType == EnemyType.Boss_1 || enemyType == EnemyType.Boss_2 || enemyType == EnemyType.Boss_3)
            {
                transform.Translate(_target.forward * 3f);
                transform.Translate(_target.up * 2f);
                lookAtPos.y += 1f;
            }
            else if (enemyType == EnemyType.Golem_1 || enemyType == EnemyType.Golem_2 || enemyType == EnemyType.Golem_3)
            {
                transform.Translate(_target.forward * 2f);
                transform.Translate(_target.up * 1.5f);
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
            TowerType towerType = _target.GetComponent<Tower>().towerType;
            if (towerType == TowerType.Chick_1 || towerType == TowerType.Chick_2 || towerType == TowerType.Chick_3 
                || towerType == TowerType.Penguin_1 || towerType == TowerType.Penguin_2 || towerType == TowerType.Penguin_3
                || towerType == TowerType.LittleBoar_1 || towerType == TowerType.Momo_1 || towerType == TowerType.Momo_2
                || towerType == TowerType.Mushroom_1 || towerType == TowerType.Mushroom_2 || towerType == TowerType.Dragon_1)
            {
                transform.Translate(_target.forward * 1.5f);
                transform.Translate(_target.up * 1f);
            }
            else
            {
                transform.Translate(_target.forward * 2f);
                transform.Translate(_target.up * 1.5f);
                lookAtPos.y += 0.5f;
            }
        }
        transform.LookAt(lookAtPos);
        deltaPos = transform.position - _target.position;
    }
}
