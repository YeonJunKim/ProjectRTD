using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BaseGameEntity
{
    public EnemyType enemyType;

    Animator animator;

    NavMeshAgent navMeshAgent;
    Vector3 destination;
    const float angularSpeed = 500;    // make it fast enough to turn quickly
    const float acceleration = 30;     // make it fast enough to turn quickly

    Healthbar healthbar;
    

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        healthbar = GetComponentInChildren<Healthbar>();
    }

    protected override void Update()
    {
        base.Update();

        float sqrDist = Vector3.SqrMagnitude(transform.position - destination);
        if (sqrDist < 1)
        {
            GameManager.S.DecreaseLife();
            OnDeath();
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();
        healthbar.UpdateHealthbar(HP, cur_hp);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        EntityManager.S.OnEnemyDeath(this);
        Destroy(navMeshAgent);
    }

    // when this entity is attacked
    public override void DecreaseHp(float amount)
    {
        float dmg = Mathf.Clamp(amount - cur_armor, 0, float.MaxValue);
        cur_hp -= dmg;

        if (cur_hp <= 0)
        {
            healthbar.UpdateHealthbar(HP, 0);
            GameManager.S.IncreaseMoneyByStageLevel();
            OnDeath();
        }
        else
        {
            healthbar.UpdateHealthbar(HP, cur_hp);
        }
    }

    public void SetDestination(Vector3 pos)
    {
        PutPerfectlyOnNavMesh();
        destination = pos;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.SetDestination(destination);
        StartWalkingAnimation();
    }

    public void PutPerfectlyOnNavMesh()
    {
        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, 1))
            gameObject.transform.position = closestHit.position;
        else
            Debug.LogError("Could not find position on NavMesh!");

        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = MOVE_SPEED;
        navMeshAgent.angularSpeed = angularSpeed;    // make it fast enough to turn quickly
        navMeshAgent.acceleration = acceleration;    // make it fast enough to turn quickly
    }

    public override void RegisterBuffer(Buffer buffer)
    {
        base.RegisterBuffer(buffer);

        navMeshAgent.speed = cur_moveSpeed;
    }

    protected override void DeregisterBuffer(Buffer buffer)
    {
        base.DeregisterBuffer(buffer);

        navMeshAgent.speed = cur_moveSpeed;
    }


    void StartWalkingAnimation()
    {
        switch (enemyType)
        {
            case EnemyType.Wolf_Black:
                animator.SetBool("Run Forward", true);
                break;
            case EnemyType.GiantBee_Blue:
                animator.SetBool("Fly Forward", true);
                break;
            case EnemyType.KingCobra_Black:
                animator.SetBool("Slither Forward", true);
                break;
            case EnemyType.Golem_Blue:
                animator.SetBool("Walk Forward", true);
                break;
        }
    }
}
