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
    // Model获取
    public PlayerDataModel playerDateModel;
    
    // 获取View组件
    [Header("等级设置")]
    public List<LevelEXPPair> levelEXPPairs = new List<LevelEXPPair>();

    [Header("UI设置")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image hpSlider;
    [SerializeField] private Image expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI expText;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
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
        
        PlayerDataModelInit();

        UpdateView();
    }
    void UpdateView()
    {
        scoreText.text = "Score: " + playerDateModel.Score.ToString();

        hpSlider.fillAmount = (float)playerDateModel.CurrentHp / playerDateModel.MaxHp;
        hpText.text = playerDateModel.CurrentHp.ToString();

        // expSlider.fillAmount = playerDateModel.MaxEXP;
        expSlider.fillAmount = (float)playerDateModel.CurrentEXPValue / playerDateModel.MaxEXP;
        expText.text = playerDateModel.CurrentEXPValue.ToString() + "/" + playerDateModel.MaxEXP.ToString();

        levelText.text = "Lv." + playerDateModel.CurrentLevel.ToString();
    }
    private void OnDestroy()
    {
        playerDateModel = null;
    }

    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }

    public void PlayerDataModelInit()
    {
        playerDateModel.Score = 0;

        playerDateModel.MaxHp = 6;
        playerDateModel.CurrentHp = playerDateModel.MaxHp;

        playerDateModel.CurrentLevel = 1;
        playerDateModel.CurrentEXPValue = 0;

        playerDateModel.MaxEXP = levelEXPPairs[playerDateModel.CurrentLevel - 1].EXPValue;
        playerDateModel.MaxLevel = 3;

        UpdateView();
    }
}