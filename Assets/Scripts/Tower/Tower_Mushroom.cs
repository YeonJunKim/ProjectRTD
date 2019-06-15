 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Mushroom : Tower
{
    
    public ParticleSystem poisonPartical;
    public int mushRoomDotdamage;
    public int mushRoomLifetime;

    bool setBuff = true;
    protected override void Update()
    {
        List<Enemy> enemies = radarSystem.GetEnemyList();

        if (setBuff)
        {
            setBuff = false;
            if (enemies.Count != 0)
            {
                poisonPartical.Play();
                Buffer Dotbuf = new Buffer();
                Dotbuf.Init();
                Dotbuf.lifeTime = mushRoomLifetime;
                Dotbuf.dotDamge = mushRoomDotdamage;

                foreach (Enemy enemy in enemies)
                {
                    enemy.RegisterBuffer(Dotbuf);
                }
            }
            else
            {
                poisonPartical.Stop();
            }
            StartCoroutine("SetBuffwaiting");
        }
    }
    IEnumerator SetBuffwaiting()
    {
        yield return new WaitForSeconds(1f);
        setBuff = true;
    }
    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 1);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 1 / ATTK_SPEED;    // 2 because it's already too slow..
        animator.SetInteger("animation", 3);
        Invoke("SetAnimation_Idle", ATTK_SPEED / 2);
    }
    //추가해야할게 독구름
}
