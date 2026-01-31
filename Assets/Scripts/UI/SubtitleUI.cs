using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    // public TextMeshProUGUI headerText;

    public AudioClip typeSound;
    public AudioSource audioSource;
    public float typeSpeed = 0.2f;
    public float durationTime = 2f;
    private string fullText;
    private int currentIndex = 0;
    void Start()
    {
        subtitleText.text = "";
        // headerText.text = "";
        currentIndex = 0;
    }

    public void StartGameSubtile()
    {
        StartCoroutine(StartGameCoroutine());
    }
    public IEnumerator StartGameCoroutine()
    {
        string text1 = SubtitleManager.Instance.subtitleText["startgame"];
        string text2 = SubtitleManager.Instance.subtitleText["task"];
        TypeSubtitle(text1);
        yield return new WaitForSeconds(3f);
        TypeSubtitle(text2);
    }
    public void TypeStartSubtitle()
    {
        StartCoroutine(StartSubtitleCoroutine());
    }

    public IEnumerator StartSubtitleCoroutine()
    {
        string text1 = SubtitleManager.Instance.subtitleText["start1"];
        string text2 = SubtitleManager.Instance.subtitleText["start2"];

        TypeSubtitle(text1);
        yield return new WaitForSeconds(2.5f);
        TypeSubtitle(text2);
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
