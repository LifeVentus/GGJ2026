using QFramework;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;

public enum EntityType
{
    small,
    medium,
    big
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
    [SerializeField] protected Color color; // 实体颜色
    [SerializeField] protected float minSize;
    [SerializeField] protected float maxSize;
    public EntityStates entityState;

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
    [SerializeField] protected SpriteRenderer spriteRenderer; // 获取sprite

    protected virtual void Start()
    {
        player = PlayerController.Instance;
        playerDataModel = this.GetModel<PlayerDataModel>();
        transform.localScale *= Random.Range(minSize, maxSize);
        spriteRenderer.color = color;
        SetNewRandomMoveTarget();
        currentSpeed = moveSpeed;
    }
    protected virtual void Update()
    {
        RandomMove();
        distanceWithPlayer = Vector2.Distance(player.transform.position, transform.position);
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
    /// 超出一定距离后删除实体
    /// </summary>
    protected virtual void DestroyOutOfDistance()
    {    
        if(distanceWithPlayer >= distroyDistance)
        {
            Destroy(gameObject);
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