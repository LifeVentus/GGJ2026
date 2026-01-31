using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using QFramework;
using UnityEngine.Timeline;
using System;

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
    [SerializeField] private float dashSpeed = 8.0f;
    private float currentSpeed;
    // [SerializeField] private float volume = 10f;

    [Header("移动平滑")]
    [SerializeField] private float smoothTime = 0.1f;  // 平滑时间
    private Vector2 movementInput;  // 存储输入
    private Vector2 smoothedMovement;  // 平滑移动
    private Vector2 smoothVelocity;  // 平滑速度

    [Header("战斗设置")]
    [SerializeField] private bool swallow;
    [SerializeField] private float swallowRadius;
    [SerializeField] private float swallowAngle;
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private float swallowCoolTime;
    public float invincibleCoolTime;
    public float dashCoolTime;

    private bool isInvincible;
    private bool isDash;
    private float invincibleTimeCounter;
    private float swallowTimeCounter;
    private float dashTimeCounter;

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerDataModel playerDateModel;
    [SerializeField] private SwallowSector swallowSector;
    private PlayerAnimation playerAnimation;

    // 玩家升级事件
    private int lastLevel;
    public event Action OnLevelUpEvent;

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
        lastLevel = 1;
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = GetMovementInput();
        CheckState();
        PlayerUpgradeDetect();
        if (swallow)
        {
            TriggerSwallowSkill();
            if(playerDateModel.CurrentLevel >= 3)
            {
                if(!swallowSector.isVisible) 
                    swallowSector.InitSector(swallowRadius, swallowAngle, swallowCoolTime);
            }
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
        if (isDash)
        {
            dashTimeCounter -= Time.deltaTime;
            if(dashTimeCounter < 0)
            {
                isDash = false;
                currentSpeed = moveSpeed;
            }

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
                    SoundManager.Instance.PlaySwallowAudio();
                    entity.Die();
                    playerAnimation.sr.sprite = collision.gameObject.GetComponent<BaseEntity>().spriteRenderer.sprite;
                    UIManager.Instance.ShowRandomSubtitle(EntityType.small);
                    break;
                case EntityType.medium:
                    if(playerDateModel.CurrentLevel < 2)
                    {
                        GetHurt();
                    }
                    else
                    {
                        SoundManager.Instance.PlaySwallowAudio();
                        entity.Die();
                        playerAnimation.sr.sprite = collision.gameObject.GetComponent<BaseEntity>().spriteRenderer.sprite;
                        UIManager.Instance.ShowRandomSubtitle(EntityType.medium);
                    }
                    break;
                case EntityType.big:
                    if(playerDateModel.CurrentLevel < 3)
                    {
                        GetHurt();
                    }
                    else
                    {
                        SoundManager.Instance.PlaySwallowAudio();
                        entity.Die();
                        playerAnimation.sr.sprite = collision.gameObject.GetComponent<BaseEntity>().spriteRenderer.sprite;
                        UIManager.Instance.ShowRandomSubtitle(EntityType.big);
                    }
                    break;
                default:
                    break;
            }
            if(playerDateModel.CurrentLevel >= 3)
            {
                Dash();
            }
        }
    }

    public void Dash()
    {
        isDash = true;
        currentSpeed = dashSpeed;
        dashTimeCounter = dashCoolTime;
    }
    public void GetHurt()
    {
        if (isInvincible)
        {
            return;
        }
        SoundManager.Instance.PlayGetHurtAudio();
        this.SendCommand(new ChangeHpCommand(-1));
        isInvincible = true;
        invincibleTimeCounter = invincibleCoolTime;
        playerAnimation.InvincibleFlash();
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
            Vector2 targetVelocity = movementInput * currentSpeed;
            
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
        SoundManager.Instance.PlayFailAudio();
        UIManager.Instance.ShowDieUI();
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

            if (Vector2.Angle((Vector2)mouseToPlayer, toEnemy) <= (sectorAngleOffset / 2f))
            {
                hit.GetComponent<BaseEntity>().Die();
                playerAnimation.sr.sprite = hit.gameObject.GetComponent<BaseEntity>().spriteRenderer.sprite;
                UIManager.Instance.ShowRandomSubtitle(hit.GetComponent<BaseEntity>().entityType);
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

    /// <summary>
    /// 玩家升级检测
    /// </summary>
    public void PlayerUpgradeDetect()
    {
        
        if (lastLevel < playerDateModel.CurrentLevel)
        {
            //Debug.Log("lastLevel" + lastLevel);
            //Debug.Log("CurrentLevel" + playerDateModel.CurrentLevel);
            isInvincible = true;
            invincibleTimeCounter = invincibleCoolTime;
            SoundManager.Instance.PlayLevelUpAudio();
            OnLevelUpEvent?.Invoke();
            lastLevel = playerDateModel.CurrentLevel;
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
