using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_LittleBoar : Tower
{
    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 5);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 3f;
        animator.SetInteger("animation", 2);
        Invoke("SetAnimation_Idle", 0.3f);
    }
}
