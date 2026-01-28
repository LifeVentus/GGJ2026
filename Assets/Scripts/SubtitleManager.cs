using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<string> commonText = new List<string>{"commmon1", "common2", "common3"};
    public List<string> weirdText = new List<string>{"Weird1", "Weird2", "Weird3"};
    public List<string> abstractText = new List<string>{"Abstract1", "Abstract2", "Abstract3"};
    public List<string> subtitleText = new List<string>();


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
        randomTextDic[EntityType.small] = commonText;
        randomTextDic[EntityType.medium] = weirdText;
        randomTextDic[EntityType.big] = abstractText;
    }
}
