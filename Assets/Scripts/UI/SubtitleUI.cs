using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SubtitleType
{
    Common,
    Weird,
    Abstract
}
public class SubtitleUI : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;

    public AudioClip typeSound;
    public AudioSource audioSource;
    public float typeSpeed = 0.1f;
    public float durationTime = 2f;
    private string fullText;
    private int currentIndex = 0;
    void Start()
    {
        subtitleText.text = "";
        currentIndex = 0;
    }

    // 测试用
    public void Subtitle()
    {
        List<string> test1List = new List<string>
        {
            "test1,abcdddd",
            "test1,jshdaih",
            "jahfdho",
            "ajdhohah"
        };
        int randomIndex = UnityEngine.Random.Range(0, 3);
        TypeSubtitle(test1List[randomIndex]);
    }

    public void TypeRandomSubtitle(EntityType entityType)
    {
        List<string> strings = SubtitleManager.Instance.randomTextDic[entityType];
        int randomIndex = UnityEngine.Random.Range(0, strings.Count - 1);
        TypeSubtitle(strings[randomIndex]);
    }
    public void TypeSubtitle(string text)
    {
        currentIndex = 0;
        subtitleText.text = "";
        fullText = text;
        StartCoroutine(Typewriter());
    }

    public IEnumerator Typewriter()
    {
        while(currentIndex < fullText.Length)
        {
            subtitleText.text = fullText.Substring(0, currentIndex + 1);
            currentIndex++;
            // 打字机音效
            if(typeSound != null) audioSource.PlayOneShot(typeSound);
            yield return new WaitForSeconds(typeSpeed);
        }
        yield return new WaitForSeconds(durationTime);
        subtitleText.text = "";
    }
}
