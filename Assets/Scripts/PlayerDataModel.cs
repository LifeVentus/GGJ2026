using QFramework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

[System.Serializable]
public class LevelEXPPair
{
    public int Level;
    public int EXPValue;
}

// Model对象定义
public class PlayerDataModel: AbstractModel
{
    public int Score; // 当前分数
    public int CurrentHp; // 当前生命值
    public int MaxHp; // 最大生命值
    public int CurrentLevel; // 当前等级
    public int MaxLevel; // 最大等级
    public int CurrentEXPValue; // 当前经验值
    public int MaxEXP; // 最大经验值
    // public List<LevelEXPPair> levelEXPPairs;  //各等级所需经验值序列
    // public float SwallowRadius; //范围吞噬半径
    // public float SwallowAngleOffset; //范围吞噬角度偏移量
    // public LayerMask EnemyLayerMask;

    protected override void OnInit()
    {
        Score = 0;

        MaxHp = 6;
        CurrentHp = MaxHp;

        CurrentLevel = 1;
        CurrentEXPValue = 0;
        MaxEXP = 10;
        MaxLevel = 3;

    }

    public void Reset()
    {
        OnInit();
    }


}

// 架构定义
public class PlayerData : Architecture<PlayerData>
{
    protected override void Init()
    {
        this.RegisterModel(new PlayerDataModel());
    }
}

// 定义数据变更事件
public class PlayerDataChangeEvent
{
}



// 分数改变命令
public class ChangeScoreCommand : AbstractCommand
{
    private int changedValue_;
    public ChangeScoreCommand(int changedValue)
    {
        changedValue_ = changedValue;
    }
    protected override void OnExecute()
    {
        this.GetModel<PlayerDataModel>().Score += changedValue_;
        this.SendEvent<PlayerDataChangeEvent>();
    }
}

// 生命值改变指令
public class ChangeHpCommand : AbstractCommand
{
    private int changedHp_;
    public ChangeHpCommand(int changedHp)
    {
        changedHp_ = changedHp;
    }
    protected override void OnExecute()
    {
        PlayerDataModel playerDataModel = this.GetModel<PlayerDataModel>();
        playerDataModel.CurrentHp = Mathf.Clamp(playerDataModel.CurrentHp + changedHp_, 0, playerDataModel.MaxHp);
        this.SendEvent<PlayerDataChangeEvent>();
    }
}

// 经验值改变命令
public class ChangeEXPValueCommand : AbstractCommand
{
    private int changedEXPValue_;
    public ChangeEXPValueCommand(int changedEXPValue)
    {
        changedEXPValue_ = changedEXPValue;
    }
    protected override void OnExecute()
    {
        PlayerDataModel playerDataModel = this.GetModel<PlayerDataModel>();
        playerDataModel.CurrentEXPValue += changedEXPValue_;

        int currentLevel = playerDataModel.CurrentLevel;
        int currentExp = playerDataModel.CurrentEXPValue;
        int maxExp = PlayerDataController.Instance.levelEXPPairs[currentLevel - 1].EXPValue;

        if(currentLevel < playerDataModel.MaxLevel)
        {
            if(currentExp >= maxExp)
            {
                // 重新设置当前经验值
                currentExp = 0;
                currentLevel++;
            }
        }
        else
        {
            currentExp = (currentExp + changedEXPValue_) > maxExp ? maxExp : currentExp + changedEXPValue_;
        }
        // 升级后重新设置
        playerDataModel.CurrentEXPValue = currentExp;
        playerDataModel.CurrentLevel = currentLevel;
        playerDataModel.MaxEXP = PlayerDataController.Instance.levelEXPPairs[currentLevel - 1].EXPValue;
        
        this.SendEvent<PlayerDataChangeEvent>();
    }
}