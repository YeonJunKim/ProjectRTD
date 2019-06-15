﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Penguin : Tower
{
    protected override void SetAnimation_Idle()
    {
        animator.speed = 1;
        animator.SetInteger("animation", 3);
    }

    protected override void SetAnimation_Attack()
    {
        CancelInvoke();
        animator.speed = 3;    // 2 because it's already too slow..
        animator.SetInteger("animation", 2);
        Invoke("SetAnimation_Idle", 0.3f);
    }
}