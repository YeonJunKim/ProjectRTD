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
    bool poisonLock;

    Stack<Node> pathToGO;

    public bool testingPathFinding;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        healthbar = GetComponentInChildren<Healthbar>();
        pathToGO = new Stack<Node>();
    }

    protected override void Update()
    {
        if(!testingPathFinding)
        {
            base.Update();

            float sqrDist = Vector3.SqrMagnitude(transform.position - destination);
            if (sqrDist < 0.5f)
            {
                GameManager.S.DecreaseLife();
                OnDeath();
            }
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();
        healthbar.UpdateHealthbar(HP, cur_hp);
        poisonLock = true;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        EntityManager.S.OnEnemyDeath(this);
        Destroy(navMeshAgent);

        if(enemyType == EnemyType.Boss_1 || enemyType == EnemyType.Boss_2 || enemyType == EnemyType.Boss_3)
            GameManager.S.IncreaseMoneyBossStageLevel();
        else
            GameManager.S.IncreaseMoneyByStageLevel();
    }

    // when this entity is attacked
    public override void DecreaseHp(float amount)
    {
        float dmg = Mathf.Clamp(amount - cur_armor, 0, float.MaxValue);
        cur_hp -= dmg;

        if (cur_hp <= 0)
        {
            healthbar.UpdateHealthbar(HP, 0);
            OnDeath();
        }
        else
        {
            healthbar.UpdateHealthbar(HP, cur_hp);
        }
    }

    public void SetDestination(Vector3 pos)
    {
        //// unity navMesh version
        //PutPerfectlyOnNavMesh();
        //destination = pos;
        //navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        //navMeshAgent.SetDestination(destination);
        //StartWalkingAnimation();

        // handmade A* algorithm version
        destination = pos;
        StartWalkingAnimation();
    }

    // handmade A* algorithm version
    public void SetPath(Stack<Node> path)
    {
        pathToGO = path;
        StartWalkingAnimation();
        StartCoroutine("StartMove");
    }

    IEnumerator StartMove()
    {
        while(pathToGO.Count != 0)
        {
            Vector3 targetPos = pathToGO.Pop().transform.position;
            targetPos.y += 1;
            while (true)
            {
                LookAt_Yaxis(targetPos);
                transform.Translate(Vector3.forward * Time.deltaTime * MOVE_SPEED * 2);

                float sqrMagnitude = (transform.position - targetPos).sqrMagnitude;
                if (sqrMagnitude < 0.5f)
                {
                    break;
                }
                yield return null;
            }
        }
        animator.SetBool("Run Forward", false);
    }


    void PutPerfectlyOnNavMesh()
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
        if(cur_dotDamge > 0.0f&&poisonLock)
        {
            float curdotDamge = cur_dotDamge;
            StartCoroutine(DotDamage(Mathf.RoundToInt(cur_dotBuffer.lifeTime), curdotDamge));
        }
    }

    protected override void DeregisterBuffer(Buffer buffer)
    {
        base.DeregisterBuffer(buffer);
        if (cur_dotDamge > 0.0f && poisonLock)
        {
            float curdotDamge = cur_dotDamge;
            StartCoroutine(DotDamage(Mathf.RoundToInt(cur_dotBuffer.lifeTime), curdotDamge));
        }
        navMeshAgent.speed = cur_moveSpeed;
    }
    
    IEnumerator DotDamage(int repeatTime,float damage)
    {
        poisonLock = false;

        DecreaseHp(damage);
        yield return new WaitForSeconds(1f);
        poisonLock = true;

        if (repeatTime > 0 && cur_hp > cur_dotDamge)
        {
            repeatTime--;
            StartCoroutine (DotDamage(repeatTime, damage));
        }

    }

    void StartWalkingAnimation()
    {
        switch (enemyType)
        {
            case EnemyType.Wolf_1:
                animator.SetBool("Run Forward", true);
                break;
            case EnemyType.KingCobra_1:
                animator.SetBool("Slither Forward", true);
                break;
            case EnemyType.GiantBee_1:
                animator.SetBool("Fly Forward", true);
                break;
            case EnemyType.Magma_1:
                animator.SetBool("Move Forward", true);
                break;
            case EnemyType.Golem_1:
                animator.SetBool("Walk Forward", true);
                break;
            case EnemyType.Wolf_2:
                animator.SetBool("Run Forward", true);
                break;
            case EnemyType.KingCobra_2:
                animator.SetBool("Slither Forward", true);
                break;
            case EnemyType.GiantBee_2:
                animator.SetBool("Fly Forward", true);
                break;
            case EnemyType.Magma_2:
                animator.SetBool("Move Forward", true);
                break;
            case EnemyType.Golem_2:
                animator.SetBool("Walk Forward", true);
                break;
            case EnemyType.Wolf_3:
                animator.SetBool("Run Forward", true);
                break;
            case EnemyType.KingCobra_3:
                animator.SetBool("Slither Forward", true);
                break;
            case EnemyType.GiantBee_3:
                animator.SetBool("Fly Forward", true);
                break;
            case EnemyType.Magma_3:
                animator.SetBool("Move Forward", true);
                break;
            case EnemyType.Golem_3:
                animator.SetBool("Walk Forward", true);
                break;
            case EnemyType.Boss_1:
                animator.SetBool("Walk Forward", true);
                break;
            case EnemyType.Boss_2:
                animator.SetBool("Move Forward Slow", true);
                break;
            case EnemyType.Boss_3:
                animator.SetBool("Walk Forward", true);
                break;
        }
    }
}
