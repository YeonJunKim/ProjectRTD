using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Buffer
{
    public bool isAlive;
    public float lifeTime;
    public float damage, armor, attkRange, attkSpeed, moveSpeed;    // this will be multiplied

    public float cur_LifeTime;

    public void Init()
    {
        isAlive = true;
        cur_LifeTime = lifeTime;
        // this will be multiplied, so if it's not set on the inspector
        // the default value should be 1
        if (Mathf.Approximately(damage, 0))
            damage = 1;
        if (Mathf.Approximately(armor, 0))
            armor = 1;
        if (Mathf.Approximately(attkRange, 0))
            attkRange = 1;
        if (Mathf.Approximately(attkSpeed, 0))
            attkSpeed = 1;
        if (Mathf.Approximately(moveSpeed, 0))
            moveSpeed = 1;
    }

    public void Update()
    {
        if (isAlive == false)
            return;

        cur_LifeTime -= Time.deltaTime;

        if (cur_LifeTime < 0)
            isAlive = false;
    }
}
