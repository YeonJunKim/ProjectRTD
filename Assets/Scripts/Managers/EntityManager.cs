using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    public static EntityManager S;

    public PoolManager PoolManager;

    // currently active objects (out from pool)
    List<BaseGameEntity> enemyList;
    List<BaseGameEntity> towerList;
    List<BaseGameEntity> projectileList;

    private void Awake()
    {
        if(S == null)
            S = this;

        enemyList = new List<BaseGameEntity>();
        towerList = new List<BaseGameEntity>();
        projectileList = new List<BaseGameEntity>();
    }

    // gets a resting Entity from the PoolManager
    public BaseGameEntity GetEntity(EnemyType enemyType)
    {
        BaseGameEntity entity = PoolManager.GetEntity(enemyType);
        entity.OnCreate();
        enemyList.Add(entity);
        return entity;
    }

    public BaseGameEntity GetEntity(TowerType towerType)
    {
        BaseGameEntity entity = PoolManager.GetEntity(towerType);
        entity.OnCreate();
        towerList.Add(entity);
        return entity;
    }

    public BaseGameEntity GetEntity(ProjectileType projectileType)
    {
        BaseGameEntity entity = PoolManager.GetEntity(projectileType);
        entity.OnCreate();
        projectileList.Add(entity);
        return entity;
    }

    public void ReturnEntity(BaseGameEntity entity)
    {
        switch (entity.ENTITY_TYPE)
        {
            case EntityType.Enemy:
                enemyList.Remove(entity);
                break;
            case EntityType.Tower:
                towerList.Remove(entity);
                break;
            case EntityType.Projectile:
                projectileList.Remove(entity);
                break;
        }
        PoolManager.ReturnEntity(entity);
    }

    public void ReturnEntity(int entityID)
    {
        FindRemoveEntityFromLists(entityID);
        PoolManager.ReturnEntity(entityID);
    }

    public BaseGameEntity FindEntityWithID(int entityID)
    {

        foreach (var tower in towerList)
        {
            if (tower.ID == entityID)
                return tower;
        }

        foreach (var enemy in enemyList)
        {
            if (enemy.ID == entityID)
                return enemy;
        }

        foreach (var projectile in projectileList)
        {
            if (projectile.ID == entityID)
                return projectile;
        }
        Debug.LogError("No such Entity found by ID:" + entityID);
        return null;
    }

    public void FindRemoveEntityFromLists(int entityID)
    {
        foreach (var tower in towerList)
        {
            if (tower.ID == entityID)
            {
                towerList.Remove(tower);
                return;
            }
        }

        foreach (var enemy in enemyList)
        {
            if (enemy.ID == entityID)
            {
                enemyList.Remove(enemy);
                return;
            }
        }
    
        foreach (var projectile in projectileList)
        {
            if (projectile.ID == entityID)
            {
                projectileList.Remove(projectile);
                return;
            }
        }
        Debug.LogError("No such Entity found by ID:" + entityID);
    }


    // notify all towers that a enemy is dead
    // to refresh their radar target list
    public void OnEnemyDeath(Enemy enemy)
    {
        foreach(var tower in towerList)
        {
            Tower t = tower as Tower;
            t.OnEnemyDeath(enemy);
        }
    }
}
