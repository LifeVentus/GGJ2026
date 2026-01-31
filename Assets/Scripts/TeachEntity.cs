using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachEntity : BaseEntity
{

    public override void Init()
    {
        base.Init();
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void DetectCheck()
    {
    }

    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
