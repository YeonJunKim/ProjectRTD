using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSystem : MonoBehaviour
{
    [SerializeField]
    float radarRange;

    SphereCollider radarCollider;
    List<Enemy> enemyList;
    Enemy target;

    private void Awake()
    {
        radarCollider = GetComponent<SphereCollider>();
        enemyList = new List<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // only the parent is tagged (because I'm lazy)
        if(other.transform.parent.tag == "Enemy")
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (!enemyList.Contains(enemy))
                enemyList.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.parent.tag == "Enemy")
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            RemoveEnemyFromRadarList(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRadarRange(float range)
    {
        radarRange = range;
        radarCollider.radius = range;
    }

    Enemy GetClosestTarget()
    {
        float closestDistanceSoFar = float.MaxValue;
        Enemy closestEnemy = null;
        foreach(var enemy in enemyList)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if(dist < closestDistanceSoFar)
            {
                closestDistanceSoFar = dist;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    void RemoveEnemyFromRadarList(Enemy enemy)
    {
        if (enemyList.Contains(enemy))
            enemyList.Remove(enemy);

        if (target == enemy)
            target = null;
    }


    public void OnEnemyDeath(Enemy enemy)
    {
        RemoveEnemyFromRadarList(enemy);
    }

    public Enemy GetTarget()
    {
        if (enemyList.Count == 0)
            return null;

        target = GetClosestTarget();

        return target;
    }
}
