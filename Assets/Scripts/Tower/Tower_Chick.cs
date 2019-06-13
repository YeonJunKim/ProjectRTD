using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Chick : Tower
{
    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 1);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 2 / ATTK_SPEED;    // 2 because it's already too slow..
        animator.SetInteger("animation", 3);
        Invoke("SetAnimation_Idle", ATTK_SPEED / 2);
    }
}
