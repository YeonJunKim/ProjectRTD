using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Penguin : Tower
{
    public override void Attack(BaseGameEntity target)
    {
        if (towerType == TowerType.Penguin_1)
            base.Attack(target);
        else if (towerType == TowerType.Penguin_2)
            AttackMultipleEnemies(3);
        else if (towerType == TowerType.Penguin_3)
            AttackMultipleEnemies(5);
        else
            base.Attack(target);
    }

    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 3);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 3;    // 2 because it's already too slow..
        animator.SetInteger("animation", 2);
        Invoke("SetAnimation_Idle", 0.3f);
    }

    void AttackMultipleEnemies(int amount)
    {
        List<Enemy> enemyInRange = radarSystem.GetEnemyList();

        for (int i = 0; i < enemyInRange.Count; i++)
        {
            float dist1 = (enemyInRange[i].transform.position - transform.position).sqrMagnitude;
            for (int j = i + 1; j < enemyInRange.Count; j++)
            {
                float dist2 = (enemyInRange[j].transform.position - transform.position).sqrMagnitude;
                if (dist1 > dist2)
                {
                    Enemy temp = enemyInRange[i];
                    enemyInRange[i] = enemyInRange[j];
                    enemyInRange[j] = temp;
                }
            }
        }

        for (int i = 0; i < amount; i++)
        {
            if (i < enemyInRange.Count)
            {
                Projectile_Base projectile = EntityManager.S.GetEntity(projectileType) as Projectile_Base;
                projectile.transform.position = transform.position;
                projectile.transform.rotation = transform.rotation; // becuase the tower is already looking at the enemy
                projectile.FireProjectile(enemyInRange[i].transform, cur_damage, cur_attkRange);
            }
        }
        SetAnimation_Attack();
    }
}
