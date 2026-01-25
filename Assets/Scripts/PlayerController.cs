using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using QFramework;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour, IController
{
    static private PlayerController instance;
    static public PlayerController Instance
    {
        get
        {
            if(instance == null) return null;
            return instance;
        }
    }


    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5.0f;
    // [SerializeField] private float volume = 10f;

    [Header("移动平滑")]
    [SerializeField] private float smoothTime = 0.1f;  // 平滑时间
    private Vector2 movementInput;  // 存储输入
    private Vector2 smoothedMovement;  // 平滑移动
    private Vector2 smoothVelocity;  // 平滑速度

    [Header("战斗设置")]
    [SerializeField] private float swallowRadius;
    [SerializeField] private float swallowAngle;
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private float swallowCoolTime;
    public float invincibleCoolTime;
    private bool isInvincible;
    private float invincibleTimeCounter;
    private float swallowTimeCounter;

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerDataModel playerDateModel;
    [SerializeField] private SwallowSector swallowSector;
    private PlayerAnimation playerAnimation;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }
    void Start()
    {
        playerDateModel = this.GetModel<PlayerDataModel>();
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = GetMovementInput();
        CheckState();
        TriggerSwallowSkill();
        if(playerDateModel.CurrentLevel >= 3)
        {
            if(!swallowSector.isVisible) 
                swallowSector.InitSector(swallowRadius, swallowAngle, swallowCoolTime);
        }
    }
    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void CheckState()
    {
        if (isInvincible)
        { 
            invincibleTimeCounter -= Time.deltaTime;
            if(invincibleTimeCounter < 0) isInvincible = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Entity"))
        {
            BaseEntity entity = collision.gameObject.GetComponent<BaseEntity>();
            switch (entity.entityType)
            {
                case EntityType.small:
                    entity.Die();
                    break;
                case EntityType.medium:
                    if(playerDateModel.CurrentLevel < 2)
                    {
                        GetHurt();
                    }
                    else
                    {
                        entity.Die();
                    }
                    break;
                case EntityType.big:
                    if(playerDateModel.CurrentLevel < 3)
                    {
                        GetHurt();
                    }
                    else
                    {
                        entity.Die();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void GetHurt()
    {
        if (isInvincible)
        {
            return;
        }
        this.SendCommand(new ChangeHpCommand(-1));
        isInvincible = true;
        invincibleTimeCounter = invincibleCoolTime;
        playerAnimation.Flash();
        if(playerDateModel.CurrentHp == 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 获取移动输入
    /// </summary>
    private Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        return new Vector2(horizontal, vertical).normalized;  // 标准化防止斜向移动更快
    }

    /// <summary>
    /// 移动角色
    /// </summary>
    private void MoveCharacter()
    {
        if (movementInput.magnitude > 0.1f)
        {
            // 计算目标速度
            Vector2 targetVelocity = movementInput * moveSpeed;
            
            // 应用平滑移动
            smoothedMovement = Vector2.SmoothDamp(
                smoothedMovement, 
                targetVelocity, 
                ref smoothVelocity, 
                smoothTime
            );
            
            // 应用速度
            rb.velocity = smoothedMovement;

        }
        else
        {
            // 无输入时平滑停止
            smoothedMovement = Vector2.SmoothDamp(
                smoothedMovement, 
                Vector2.zero, 
                ref smoothVelocity, 
                smoothTime
            );
            
            rb.velocity = smoothedMovement;
        }
    }
    /// <summary>
    /// 死亡逻辑
    /// </summary>
    public void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        UIManager.Instance.ShowDieUI();
    }
    /// <summary>
    /// 范围检测
    /// </summary>
    public int DetectCheck(LayerMask targetLayer, float detectRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetLayer);
        return colliders.Length;
    }

    /// <summary>
    /// 判断敌人是否在扇形区域内
    /// </summary>
    public void IsInSwallowRange(float sectorAngleOffset, float sectorRadius, LayerMask enemyLayerMask)
    {
        //Get the mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 mouseToPlayer = mousePos - transform.position;

        //Debug.Log("enemyLayer为"+LayerMask.LayerToName(enemyLayerMask));

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sectorRadius, enemyLayerMask);

        foreach (var hit in hits)
        {
            Vector2 toEnemy =
                ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;

            if (Vector2.Angle((Vector2)mouseToPlayer, toEnemy) <= sectorAngleOffset)
            {
                hit.GetComponent<BaseEntity>().Die();
                //Debug.Log("扇形吞噬已触发，敌人已被吞噬");
            }
        }
    }

    /// <summary>
    /// 启用范围吞噬功能
    /// </summary>
    public void TriggerSwallowSkill()
    {
        swallowTimeCounter -= Time.deltaTime;
        if(swallowTimeCounter >= 0)
            return;
        
        if (Input.GetMouseButtonDown(0) && playerDateModel.CurrentLevel >=3)
        {
            IsInSwallowRange(swallowAngle, swallowRadius, entityLayer);
            swallowSector.SwitchSectorColor();
            swallowTimeCounter = swallowCoolTime;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.DrawWireSphere(transform.position, 30f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, swallowRadius);
    }
    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }
}
