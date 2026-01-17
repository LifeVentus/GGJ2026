using System;
using QFramework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerDataController : MonoBehaviour, IController
{
    // 获取View组件
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI expText;

    // Model获取
    [SerializeField] private PlayerDataModel playerDateModel;

    // 定义事件


    void Start()
    {
        playerDateModel = this.GetModel<PlayerDataModel>();

        // 交互逻辑订阅
        this.RegisterEvent<PlayerDataChangeEvent>(e =>
        {
            UpdateView();

        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        UpdateView();

    }
    void UpdateView()
    {
        scoreText.text = playerDateModel.Score.ToString();
        Debug.Log("Score:" + playerDateModel.Score);

        hpSlider.value = playerDateModel.Hp;
        hpText.text = playerDateModel.Hp.ToString();
        Debug.Log("hpValue:" + playerDateModel.Hp);

        expSlider.maxValue = playerDateModel.maxEXP;
        expSlider.value = playerDateModel.EXPValue;
        expText.text = playerDateModel.EXPValue.ToString();
        Debug.Log("maxExp:" + playerDateModel.maxEXP);
        Debug.Log("EXPValue:" + playerDateModel.EXPValue);

        levelText.text = playerDateModel.Level.ToString();
    }

    private void Oestroy()
    {
        playerDateModel = null;
    }

    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }
}