using UnityEngine;
using QFramework;
using UnityEditor.SearchService;

// Model对象定义
public class PlayerDataModel: AbstractModel
{
    public int Score;
    public int Hp;
    public int maxHp;
    public int Level;
    public int EXPValue;
    public int maxEXP;
    public int maxLevel;

    protected override void OnInit()
    {
        Score = 0;
        Hp = 100;
        Level = 1;
        EXPValue = 0;
        maxHp = 100;
        maxEXP = 100;
        maxLevel = 10;
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

// Command定义
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
        Debug.Log("changedHP:" + changedHp_);
        playerDataModel.Hp = Mathf.Clamp(playerDataModel.Hp + changedHp_, 0, playerDataModel.maxHp);
        this.SendEvent<PlayerDataChangeEvent>();
    }
}

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
        playerDataModel.EXPValue += changedEXPValue_;
        if(playerDataModel.Level == playerDataModel.maxLevel)
        {
            playerDataModel.EXPValue = playerDataModel.maxEXP;
            this.SendEvent<PlayerDataChangeEvent>();
            return;
        }
        if(playerDataModel.EXPValue >= playerDataModel.maxEXP)
        {
            playerDataModel.EXPValue -= playerDataModel.maxEXP;
            playerDataModel.Level++;
            playerDataModel.maxEXP = playerDataModel.Level * 100;
        }
        this.SendEvent<PlayerDataChangeEvent>();
    }
}