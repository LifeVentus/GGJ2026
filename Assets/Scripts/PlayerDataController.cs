using System;
using System.Collections.Generic;
using QFramework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerDataController : MonoBehaviour, IController
{
    static private PlayerDataController instance;
    static public PlayerDataController Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }
    
    // 获取View组件
    [Header("等级设置")]
    public List<LevelEXPPair> levelEXPPairs = new List<LevelEXPPair>();
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI expText;
    // Model获取
    public PlayerDataModel playerDateModel;

    // 定义事件

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        
    }

    void Start()
    {
        playerDateModel = this.GetModel<PlayerDataModel>();

        // 交互逻辑订阅
        this.RegisterEvent<PlayerDataChangeEvent>(e =>
        {
            UpdateView();

        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        Init();

    }
    void UpdateView()
    {
        scoreText.text = playerDateModel.Score.ToString();

        hpSlider.value = playerDateModel.CurrentHp;
        hpText.text = playerDateModel.CurrentHp.ToString();

        expSlider.maxValue = playerDateModel.MaxEXP;
        expSlider.value = playerDateModel.CurrentEXPValue;
        expText.text = playerDateModel.CurrentEXPValue.ToString();

        levelText.text = playerDateModel.CurrentLevel.ToString();

    }
    private void OnDestroy()
    {
        playerDateModel = null;
    }

    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }

    public void Init()
    {
        playerDateModel.Reset();
        UpdateView();
    }
}