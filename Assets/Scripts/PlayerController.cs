using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using QFramework;


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

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerDataModel playerDateModel;

    [Header("移动平滑")]
    [SerializeField] private float smoothTime = 0.1f;  // 平滑时间
    private Vector2 movementInput;  // 存储输入
    private Vector2 smoothedMovement;  // 平滑移动
    private Vector2 smoothVelocity;  // 平滑速度
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        playerDateModel = this.GetModel<PlayerDataModel>();
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = GetMovementInput();
    }
    void FixedUpdate()
    {
        MoveCharacter();
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

        Time.timeScale = 0;
    }
    /// <summary>
    /// 范围检测
    /// </summary>
    public int DetectCheck(LayerMask targetLayer, float detectRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetLayer);
        return colliders.Length;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.DrawWireSphere(transform.position, 30f);
    }
    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }
}
