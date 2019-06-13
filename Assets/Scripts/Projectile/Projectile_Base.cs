using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile_Base : BaseGameEntity
{
    public ProjectileType type;

    public bool isBufferUsed;
    public Buffer buffer;

    protected Transform target;

    protected List<BaseGameEntity> collidedList;            // entity's that was hit at least once
    protected List<BaseGameEntity> curCollidingList;     // entity's that are currently colliding


    protected override void Awake()
    {
        base.Awake();
        collidedList = new List<BaseGameEntity>();
        curCollidingList = new List<BaseGameEntity>();
    }

    // parameter 'range' is needed because of the Flame tower
    public virtual void FireProjectile(Transform _target, float _damage, float _range)
    {
        target = _target;
        cur_damage = _damage;
        collidedList.Clear();
        curCollidingList.Clear();
        buffer.Init();
    }

    public virtual void OnTriggerEnter_FromCollider(BaseGameEntity target)
    {
        if (collidedList.Contains(target) == false)
            collidedList.Add(target);

        if (curCollidingList.Contains(target) == false)
            curCollidingList.Add(target);
    }

    public void OnTriggerExit_FromCollider(BaseGameEntity target)
    {
        curCollidingList.Remove(target);
    }
}
