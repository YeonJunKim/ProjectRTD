using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Dragon : Tower
{
    public ParticleSystem flamePartical;

    float waitFlameOnCreate;    // hold flame for a while when created

    public override void OnCreate()
    {
        base.OnCreate();
        flamePartical.Stop();
        flamePartical.gameObject.SetActive(false);
        waitFlameOnCreate = nextAttkTime;
    }


    protected override void Update()
    {
        base.Update();

        // hold flame for a while when created
        if (waitFlameOnCreate > Time.time)
            return;

        Enemy target = radarSystem.GetTarget();
        if (target != null)
        {
            flamePartical.gameObject.SetActive(true);
            flamePartical.Play();
        }
        else
        {
            flamePartical.Stop();
            flamePartical.gameObject.SetActive(false);
        }
    }

    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 5);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 2 / ATTK_SPEED;    // 2 because it's already too slow..
        animator.SetInteger("animation", 2);
        Invoke("SetAnimation_Idle", ATTK_SPEED / 2);
    }
}
