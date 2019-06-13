using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseGameEntity : MonoBehaviour
{
    [SerializeField]
    EntityType entityType;

    [SerializeField]
    int id;

    [SerializeField]
    protected float HP, DAMAGE, ARMOR, ATTK_RANGE, ATTK_SPEED, MOVE_SPEED;

    [SerializeField]
    protected float cur_hp, cur_damage, cur_armor, cur_attkRange, cur_attkSpeed, cur_moveSpeed;

    List<Buffer> curBuffers;

    protected virtual void Awake()
    {
        curBuffers = new List<Buffer>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (curBuffers.Count == 0)
            return;

        foreach (var buffer in curBuffers)
        {
            buffer.Update();
            if (buffer.isAlive == false)
            {
                DeregisterBuffer(buffer);
                break;
            }
        }
    }

    public EntityType ENTITY_TYPE
    {
        get {
            return entityType;
        }
        set
        {
            entityType = value; // but not used
        }
    }

    public int ID
    {
        get {
            return id;
        }
        set {
            id = value;
        }
    }
    
    
    // when this entity is attacked
    public virtual void DecreaseHp(float amount)
    {
        float dmg = Mathf.Clamp(amount - cur_armor, 0, float.MaxValue);

        cur_hp -= dmg;

        if (cur_hp <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnCreate()
    {
        cur_hp = HP;
        cur_damage = DAMAGE;
        cur_armor = ARMOR;
        cur_attkRange = ATTK_RANGE;
        cur_attkSpeed = ATTK_SPEED;
        cur_moveSpeed = MOVE_SPEED;
    }

    public virtual void OnDeath()
    {
        EntityManager.S.ReturnEntity(this);
    }

    public virtual void Attack(BaseGameEntity target)
    {
        // defined by child
    }

    public virtual void RegisterBuffer(Buffer buffer)
    {
        curBuffers.Add(buffer);
        ApplyStrongestBuffer();
    }

    protected virtual void DeregisterBuffer(Buffer buffer)
    {
        curBuffers.Remove(buffer);
        ApplyStrongestBuffer();
    }

    void ApplyStrongestBuffer()
    {
        cur_damage = DAMAGE;
        cur_armor = ARMOR;
        cur_attkRange = ATTK_RANGE;
        cur_attkSpeed = ATTK_SPEED;
        cur_moveSpeed = MOVE_SPEED;

        float sDamage = 1;
        float sArmor = 1;
        float sAttkRange = 1;
        float sAttkSpeed = 1;
        float sMoveSpeed = 1;

        foreach (var buf in curBuffers)
        {
            sDamage = Mathf.Max(sDamage, buf.damage);           // buf should be Max value
            sArmor = Mathf.Min(sArmor, buf.armor);             // debuf should be Min Value
            sAttkRange = Mathf.Max(sAttkRange, buf.attkRange);
            sAttkSpeed = Mathf.Max(sAttkSpeed, buf.attkSpeed);
            sMoveSpeed = Mathf.Min(sMoveSpeed, buf.moveSpeed);     // min value will make it faster 
        }

        cur_damage *= sDamage;
        cur_moveSpeed *= sMoveSpeed;
        cur_armor *= sArmor;
        cur_attkRange *= sAttkRange;
        cur_attkSpeed *= sAttkSpeed;
    }


    // we only want to rotate the Y axis when looking at the target
    protected void LookAt_Yaxis(Vector3 targetPos)
    {
        Vector3 difference = targetPos - transform.position;
        float rotationY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
    }

    //public float DAMAGE
    //{
    //    get {
    //        return damage;
    //    }
    //    set {
    //        damage = value;
    //    }
    //}


    //public float HP
    //{
    //    get {
    //        return hp;
    //    }
    //    set {
    //        hp = value;
    //    }
    //}


    //public float ARMOR
    //{
    //    get {
    //        return armor;
    //    }
    //    set {
    //        armor = value;
    //    }
    //}

    //public float ATTK_RANGE
    //{
    //    get {
    //        return attkRange;
    //    }
    //    set {
    //        attkRange = value;
    //    }
    //}

    //public float ATTK_SPEED
    //{
    //    get {
    //        return attkSpeed;
    //    }
    //    set {
    //        attkSpeed = value;
    //    }
    //}

    //public float MOVE_SPEED
    //{
    //    get {
    //        return moveSpeed;
    //    }
    //    set {
    //        moveSpeed = value;
    //    }
    //}
}
