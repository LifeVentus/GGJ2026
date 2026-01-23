using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PatrolEntity : BaseEntity, IController
{
    [Header("组件获取")]
    [SerializeField] private TextMeshPro entityStateText;

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();
        UpdateStateText();
    }

    private void UpdateStateText()
    {
        if(entityState == EntityStates.normal)
        {
            entityStateText.text = "(-o-)";
        }
    }
    public override void Die()
    {
        this.SendCommand(new ChangeScoreCommand(scoreValue));
        this.SendCommand(new ChangeEXPValueCommand(2));
        
        Recycle();
    }

    public override void DetectCheck()
    {
    }
}