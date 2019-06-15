using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Enemy,
    Tower,
    Projectile,
}

public enum EnemyType
{
    Wolf_1,
    KingCobra_1,
    GiantBee_1,
    Magma_1,
    Golem_1,
};

public enum TowerType
{
    // level 1
    Chick_1,
    LittleBoar_1,
    Dragon_1,
    Penguin_1,
    Mushroom_1,
    Momo_1,

    // level 2
    Chick_2,
    LittleBoar_2,
    Dragon_2,
    Penguin_2,
    Mushroom_2,
    Momo_2,

    // level 3
    Chick_3,
    LittleBoar_3,
    Dragon_3,
    Penguin_3,
    Mushroom_3,
    Momo_3,
};

public enum ProjectileType
{
    Bullet,
    Flame,
    Bomb,
    Ice,
    Laser_1,
    Laser_2,
    Laser_3,
};


public class PoolManager : MonoBehaviour
{
    public Transform enemyPoolTrans;
    public Transform towerPoolTrans;
    public Transform projectilePoolTrans;

    public GameObject[] enemyPrefabs;
    public GameObject[] towerPrefabs;
    public GameObject[] projectilePrefabs;

    List<List<BaseGameEntity>> enemyPool;
    List<List<BaseGameEntity>> towerPool;
    List<List<BaseGameEntity>> projectilePool;

    // the initial amount of objects that is intantiated in each pool
    const int ENEMY_POOL_AMOUNT = 30;
    const int TOWER_POOL_AMOUNT = 20;
    const int PROJECTILE_POOL_AMOUNT = 100;
    Vector3 INIT_POSITION = new Vector3(-100, -100, -100);

    // a unique id to assign each entity
    int countEntityID = 0;


    void Awake()
    {
        CreateEnemyPool();
        CreateTowerPool();
        CreateProjectilePool();
    }

    void CreateEnemyPool()
    {
        enemyPool = new List<List<BaseGameEntity>>();

        for (int i = 0; i < System.Enum.GetValues(typeof(EnemyType)).Length; i++)
        {
            enemyPool.Add(new List<BaseGameEntity>());

            for (int j = 0; j < ENEMY_POOL_AMOUNT; j++)
            {
                InstantiateEnemy((EnemyType)i);
            }
        }
    }

    void CreateTowerPool()
    {
        towerPool = new List<List<BaseGameEntity>>();

        for (int i = 0; i < System.Enum.GetValues(typeof(TowerType)).Length; i++)
        {
            towerPool.Add(new List<BaseGameEntity>());

            for (int j = 0; j < TOWER_POOL_AMOUNT; j++)
            {
                InstantiateTower((TowerType)i);
            }
        }
    }

    void CreateProjectilePool()
    {
        projectilePool = new List<List<BaseGameEntity>>();

        for (int i = 0; i < System.Enum.GetValues(typeof(ProjectileType)).Length; i++)
        {
            projectilePool.Add(new List<BaseGameEntity>());

            for (int j = 0; j < PROJECTILE_POOL_AMOUNT; j++)
            {
                InstantiateProjectile((ProjectileType)i);
            }
        }
    }
    


    // gets a resting Entity in the Pool
    // if no resting Entity is found, it instantiates one and add it to the pool 
    public BaseGameEntity GetEntity(EnemyType enemyType)
    {
        foreach (var entity in enemyPool[(int)enemyType])
        {
            if(entity.gameObject.activeInHierarchy == false)
            {
                entity.gameObject.SetActive(true);
                return entity;
            }
        }

        GameObject temp = InstantiateEnemy(enemyType);
        temp.SetActive(true);
        return temp.GetComponent<BaseGameEntity>();
    }

    public BaseGameEntity GetEntity(TowerType towerType)
    {
        foreach (var entity in towerPool[(int)towerType])
        {
            if (entity.gameObject.activeInHierarchy == false)
            {
                entity.gameObject.SetActive(true);
                return entity;
            }
        }

        GameObject temp = InstantiateTower(towerType);
        temp.SetActive(true);
        return temp.GetComponent<BaseGameEntity>();
    }

    public BaseGameEntity GetEntity(ProjectileType projectileType)
    {
        foreach (var entity in projectilePool[(int)projectileType])
        {
            if (entity.gameObject.activeInHierarchy == false)
            {
                entity.gameObject.SetActive(true);
                return entity;
            }
        }

        GameObject temp = InstantiateProjectile(projectileType);
        temp.SetActive(true);
        return temp.GetComponent<BaseGameEntity>();
    }

    public void ReturnEntity(BaseGameEntity entity)
    {
        entity.gameObject.SetActive(false);
        entity.transform.position = INIT_POSITION;
    }

    public void ReturnEntity(int entityID)
    {
        BaseGameEntity entity = FindEntityWithID(entityID);
        entity.gameObject.SetActive(false);
        entity.transform.position = INIT_POSITION;
    }


    GameObject InstantiateEnemy(EnemyType enemyType)
    {
        GameObject tempGO = Instantiate<GameObject>(enemyPrefabs[(int)enemyType]);
        tempGO.transform.parent = enemyPoolTrans;
        tempGO.transform.position = INIT_POSITION;
        tempGO.SetActive(false);

        BaseGameEntity tempEntity = tempGO.GetComponent<BaseGameEntity>();
        tempEntity.ID = countEntityID++;

        enemyPool[(int)enemyType].Add(tempEntity);

        return tempGO;
    }

    GameObject InstantiateTower(TowerType towerType)
    {
        GameObject tempGO = Instantiate<GameObject>(towerPrefabs[(int)towerType]);
        tempGO.transform.parent = towerPoolTrans;
        tempGO.transform.position = INIT_POSITION;
        tempGO.SetActive(false);

        BaseGameEntity tempEntity = tempGO.GetComponent<BaseGameEntity>();
        tempEntity.ID = countEntityID++;

        towerPool[(int)towerType].Add(tempEntity);

        return tempGO;
    }

    GameObject InstantiateProjectile(ProjectileType projectileType)
    {
        GameObject tempGO = Instantiate<GameObject>(projectilePrefabs[(int)projectileType]);
        tempGO.transform.parent = projectilePoolTrans;
        tempGO.transform.position = INIT_POSITION;
        tempGO.SetActive(false);

        BaseGameEntity tempEntity = tempGO.GetComponent<BaseGameEntity>();
        tempEntity.ID = countEntityID++;

        projectilePool[(int)projectileType].Add(tempEntity);

        return tempGO;
    }

    public BaseGameEntity FindEntityWithID(int entityID)
    {
        foreach(var entityList in enemyPool)
        {
            foreach(var entity in entityList)
            {
                if (entity.ID == entityID)
                    return entity;
            }
        }
        foreach (var entityList in towerPool)
        {
            foreach (var entity in entityList)
            {
                if (entity.ID == entityID)
                    return entity;
            }
        }
        foreach (var entityList in projectilePool)
        {
            foreach (var entity in entityList)
            {
                if (entity.ID == entityID)
                    return entity;
            }
        }
        Debug.LogError("No such Entity found by ID:" + entityID);
        return null;
    }
}
