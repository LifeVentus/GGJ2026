using System.Collections.Generic;
using System.Diagnostics;
using QFramework;
using TMPro;
using UnityEngine;

public class EntityController : MonoBehaviour, IController
{
    [Header("基础属性设定")]
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private int hp = 100;
    [SerializeField] private int expValue = 10;
    [SerializeField] private int level = 1;
    [SerializeField] private int damage = -10;
    private int deltaLevel;  // 等级差

    [Header("索敌范围检测")]
    [SerializeField] private float detectRadius = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float disappearDistance = 50f;
    private Vector2 targetPosition;  
    private float timeCounter = 3f; // 计时器

    [Header("获取组件")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro levelText;
    private PlayerDataModel playerDataModel;
    private PlayerController player;


    // Start is called before the first frame update
    void Start()
    {
        playerDataModel = this.GetModel<PlayerDataModel>();
        player = PlayerController.Instance;
        InitEntity();
    }

    // Update is called once per frame
    void Update()
    {
        SetColor();
        // 距离太远销毁
        float distanceWithPlayer = Vector2.Distance(player.transform.position, transform.position);
        if(distanceWithPlayer > disappearDistance)
        {
            Destroy(gameObject);
        }
        if(hp <= 0)
        {
            Die();
        }
        Move();
        hpText.text = hp.ToString();
        levelText.text = "Lv." + level.ToString();
    }
    public void InitEntity()
    {
        int initLevel = Random.Range(playerDataModel.Level - 2, playerDataModel.Level + 5);
        level = Mathf.Clamp(initLevel, 1, 10);

        deltaLevel = level - playerDataModel.Level;
        SetColor();
        SetNewTarget();
        scoreValue = level * 50;
        expValue += level * 10;
        damage -= level;
    }
    private void DetectCheck()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, detectRadius);

    }
    private void SetColor()
    {
        deltaLevel = level - playerDataModel.Level;
        if(deltaLevel > 2)
        {
            spriteRenderer.color = Color.red;
        }
        else if(deltaLevel < -2)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.yellow;
        }
    }
    /// <summary>
    /// entity移动逻辑
    /// </summary>
    private void Move()
    {
        // 测试用随机移动
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        timeCounter -= Time.deltaTime;
        if(timeCounter <= 0)
        {
            timeCounter = 3f;
            SetNewTarget();
        }
    }
    private void SetNewTarget()
    {
        float randomX = Random.Range(-detectRadius, detectRadius);
        float randomY = Random.Range(-detectRadius, detectRadius);
        targetPosition = (Vector2)transform.position + new Vector2(randomX, randomY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(deltaLevel > 2)
            {
                // 角色死亡
            }
            else if(deltaLevel < -2)
            {
                Die();
            }
            else
            {
                this.SendCommand(new ChangeHpCommand(damage));
                hp -= 50;
            }
        }
    }
    private void Die()
    {
        this.SendCommand(new ChangeScoreCommand(scoreValue));
        this.SendCommand(new ChangeEXPValueCommand(expValue));

        Destroy(gameObject);
    }
    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
