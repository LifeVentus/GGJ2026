using QFramework;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EscapeEntity : BaseEntity, IController
{
    [Header("实体属性")]
    [SerializeField] private float chaseCoolTime;
    private float chaseTimeCounter;

    [Header("组件获取")]
    [SerializeField] private TextMeshPro entityStateText;

    public override void Init()
    {
        base.Init();
    }

    protected override void Update()
    {
        base.Update();
        DetectCheck();
        Chase();
        UpdateStateText();
    }

    private void UpdateStateText()
    {
        if(entityState == EntityStates.normal)
        {
            entityStateText.text = "(+o+)";
        }
        else if(entityState == EntityStates.running)
        {
            entityStateText.text = "(*o*)";
        }
    }
    public override void Die()
    {
        this.SendCommand(new ChangeScoreCommand(scoreValue));
        this.SendCommand(new ChangeEXPValueCommand(1));
        
        Recycle();
    }

    public override void DetectCheck()
    {
        if(distanceWithPlayer <= detectRadius)
        {
            isPlayerdetected = true;
            chaseTimeCounter = chaseCoolTime;
        }
        else
        {
            isPlayerdetected = false;
        }
        chaseTimeCounter -= Time.deltaTime;

        if(isPlayerdetected || chaseTimeCounter > 0)
        {
            entityState = EntityStates.running;
        }
        else
        {
            entityState = EntityStates.normal;
        }
    }

    private void Chase()
    {
        if(entityState == EntityStates.running)
        {
            currentSpeed = chaseSpeed;
            Vector2 escapeDir = FindEscapeTargetPosition();
            transform.Translate(escapeDir * currentSpeed * Time.deltaTime, Space.World);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distroyDistance);
    }

    private Vector2 FindEscapeTargetPosition()
    {
        return (player.transform.position - transform.position).normalized;
    }
}