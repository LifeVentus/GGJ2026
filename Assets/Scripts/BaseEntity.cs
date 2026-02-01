using System.Collections.Generic;
using QFramework;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;

public enum EntityType
{
    small,
    medium,
    big,
    teach
}
public enum EntityStates
{
    normal,
    running
}
public abstract class BaseEntity : MonoBehaviour, IController
{
    [Header("实体属性")]
    public EntityType entityType; // 实体类型
    [SerializeField] protected int scoreValue; // 实体分数值
    [SerializeField] protected Color normalColor; // 实体颜色
    [SerializeField] protected Color specialColor;
    [SerializeField] protected float minSize;
    [SerializeField] protected float maxSize;
    public EntityStates entityState;
    [HideInInspector] public GameObject prefab;

    [Header("随机移动设置")]
    [SerializeField] protected float moveSpeed; // 移动速度
    [SerializeField] protected float moveCoolTime; // 随机移动目标设置冷却时间
    [SerializeField] protected float randomMoveRadius; // 随机移动范围
    [SerializeField] protected float distroyDistance; // 实体销毁距离
    protected Vector2 moveTargetPosition; // 移动目标点
    protected float currentSpeed;
    protected float timeCounter; // 计时器

    [Header("玩家检测")]
    [SerializeField] protected float detectRadius; // 检测范围
    [SerializeField] protected bool isPlayerdetected; // 是否检测到玩家
    [SerializeField] protected float chaseSpeed;
    protected float distanceWithPlayer; // 距离玩家距离
    protected PlayerController player; // 玩家实例
    protected PlayerDataModel playerDataModel; // 玩家数据模型

    [Header("组件获取")]
    public SpriteRenderer spriteRenderer; // 获取sprite
    public List<Sprite> spriteList = new List<Sprite>();

    protected virtual void Start()
    {
        Init();
        player.OnLevelUpEvent += ChangeEntityColor;
        player.OnVictory += Freeze;
    }
    protected virtual void Update()
    {
        RandomMove();
        distanceWithPlayer = Vector2.Distance(player.transform.position, transform.position);
        DestroyOutOfDistance();
    }

    public void Freeze()
    {
        currentSpeed = 0;
    }
    public virtual void ChangeEntityColor()
    {
        if(spriteRenderer.color == specialColor)
        {
            switch (entityType)
            {
                case EntityType.small:
                    spriteRenderer.color = normalColor;
                break;
                case EntityType.medium:
                    if(playerDataModel.CurrentLevel >= 2)
                    {
                        spriteRenderer.color = normalColor;
                    }
                break;
                case EntityType.big:
                    if(playerDataModel.CurrentLevel >= 3)
                    {
                        spriteRenderer.color = normalColor;
                    }
                break;
                case EntityType.teach:
                    spriteRenderer.color = normalColor;
                break;
                default:
                break;
            }
        }
    }
    public virtual void Init()
    {
        player = PlayerController.Instance;
        playerDataModel = this.GetModel<PlayerDataModel>();

        float scaleRatio = Random.Range(minSize, maxSize) * 0.6f;
        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);

        int randomIndex = Random.Range(0, spriteList.Count - 1);
        spriteRenderer.sprite = spriteList[randomIndex];

        spriteRenderer.color = specialColor;
        ChangeEntityColor();
        
        SetNewRandomMoveTarget();
        currentSpeed = moveSpeed;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public virtual void Recycle()
    {
        // 重置状态
        entityState = EntityStates.normal;
        currentSpeed = moveSpeed;
        isPlayerdetected = false;
        timeCounter = moveCoolTime;
    
        // 回收至对象池
        EntityManager.Instance.Return(this);
    }
    /// <summary>
    /// Entity随机移动逻辑
    /// </summary>
    protected virtual void RandomMove()
    {
        if(entityState != EntityStates.normal)
        {
            return;
        }
        timeCounter -= Time.deltaTime;
        if(timeCounter <= 0)
        {
            timeCounter = moveCoolTime;
            SetNewRandomMoveTarget();
        }
        transform.position = Vector2.MoveTowards(transform.position, moveTargetPosition, currentSpeed * Time.deltaTime);
    }
    protected virtual void SetNewRandomMoveTarget()
    {
        float randomX = Random.Range(-randomMoveRadius, randomMoveRadius);
        float randomY = Random.Range(-randomMoveRadius, randomMoveRadius);
        moveTargetPosition = (Vector2)transform.position + new Vector2(randomX, randomY);
    }
    /// <summary>
    /// 超出一定距离后回收实体
    /// </summary>
    protected virtual void DestroyOutOfDistance()
    {    
        if(distanceWithPlayer >= distroyDistance)
        {
            Recycle();
        }
    }
    /// <summary>
    /// 玩家检测函数
    /// </summary>
    public abstract void DetectCheck();
    /// <summary>
    /// 实体死亡函数
    /// </summary>
    public abstract void Die();

    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }
} 