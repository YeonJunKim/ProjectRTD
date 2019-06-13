using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : BaseGameEntity
{
    public TowerType towerType;
    public ProjectileType projectileType;

    protected RadarSystem radarSystem;
    protected float nextAttkTime;

    protected float nextLookAtTime;
    protected const float LOOK_AT_DELAY = 0.2f;

    protected Animator animator;


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        radarSystem = GetComponentInChildren<RadarSystem>();
        radarSystem.SetRadarRange(ATTK_RANGE);
        nextAttkTime = 0;
        nextLookAtTime = 0;
        SetAnimation_Idle();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Enemy target = radarSystem.GetTarget();
        if (target != null && nextLookAtTime < Time.time)
        {
            LookAt_Yaxis(target.transform.position);
            nextLookAtTime = Time.time + LOOK_AT_DELAY;
        }

        if (target != null && nextAttkTime < Time.time)
        {
            Attack(target);
            nextAttkTime = Time.time + cur_attkSpeed;
        }
    }

    public override void Attack(BaseGameEntity target)
    {
        Projectile_Base projectile = EntityManager.S.GetEntity(projectileType) as Projectile_Base;
        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation; // becuase the tower is already looking at the enemy
        projectile.FireProjectile(target.transform, cur_damage, cur_attkRange);
        SetAnimation_Attack();
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        if (enemy != null)
            radarSystem.OnEnemyDeath(enemy);
        else
            Debug.Log("enemy is null!");
    }


    public override void RegisterBuffer(Buffer buffer)
    {
        base.RegisterBuffer(buffer);
        radarSystem.SetRadarRange(cur_attkRange);
    }

    protected override void DeregisterBuffer(Buffer buffer)
    {
        base.RegisterBuffer(buffer);
        radarSystem.SetRadarRange(cur_attkRange);
    }

    protected virtual void SetAnimation_Idle()
    {
        // defined by child
    }

    protected virtual void SetAnimation_Attack()
    {
        // defined by child
    }
}
