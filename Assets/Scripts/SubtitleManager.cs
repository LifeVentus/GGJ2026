using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubtitleData
{
    public List<string> commonText = new List<string>();
    public List<string> weirdText = new List<string>();
    public List<string> abstractText = new List<string>();
    public List<SubtitleDictionary> subtitlePairs = new List<SubtitleDictionary>();
}
[System.Serializable]
public class SubtitleDictionary
{
    public string key;
    public string value;
}

public class SubtitleManager : MonoBehaviour
{
    private static SubtitleManager instance;
    public static SubtitleManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Dictionary<EntityType, List<string>> randomTextDic = new Dictionary<EntityType, List<string>>();
    public List<string> commonText = new List<string>();
    public List<string> weirdText = new List<string>();
    public List<string> abstractText = new List<string>();
    public Dictionary<string, string> subtitleText = new Dictionary<string, string>();

    public TextAsset subtitleJson;
    public SubtitleData subtitleData;


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
    }

    void Start()
    {
        LoadFromJson();
        randomTextDic[EntityType.small] = subtitleData.commonText;
        randomTextDic[EntityType.medium] = subtitleData.weirdText;
        randomTextDic[EntityType.big] = subtitleData.abstractText;
        foreach(SubtitleDictionary item in subtitleData.subtitlePairs)
        {
            subtitleText[item.key] = item.value;
        }
    }

    private void LoadFromJson()
    {
        if(subtitleJson != null)
        {
            subtitleData = JsonUtility.FromJson<SubtitleData>(subtitleJson.text);

        }
        else
        {
            subtitleData = new SubtitleData();
        }
    }
}
