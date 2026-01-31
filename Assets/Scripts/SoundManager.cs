using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    [Header("背景音效")]
    [SerializeField] private AudioClip bgmAudio;

    [Header("点击音效")]
    [SerializeField] private AudioClip clickAudio;

    [Header("角色音效")]
    [SerializeField] private AudioClip levelUpAudio;
    [SerializeField] private AudioClip swallowAudio;
    [SerializeField] private AudioClip getHurtAudio;

    [Header("场景音效")]
    [SerializeField] private AudioClip failAudio;
    [SerializeField] private AudioClip victoryAudio;

    private AudioSource bgmSource, clickSource, levelUpSource, failSource, victorySource, swallowSource, getHurtSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        //初始化音源
        InitSource();

        //开始时播放背景音乐
        InitBGM();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PlayClickAudio();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayClickAudio();
        }
    }

    private void InitSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        clickSource = gameObject.AddComponent<AudioSource>();
        levelUpSource = gameObject.AddComponent<AudioSource>();
        failSource = gameObject.AddComponent<AudioSource>();
        victorySource = gameObject.AddComponent<AudioSource>();
        swallowSource = gameObject.AddComponent<AudioSource>();
        getHurtSource = gameObject.AddComponent<AudioSource>();
    }

    private void InitBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.clip = bgmAudio;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
    }

    public void PauseBGM()
    {
        bgmSource.loop = false;
        bgmSource.Stop();
    }

    public void ResumeBGM()
    {
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.loop = false;
        bgmSource.Stop();
    }

    public void PlayClickAudio()
    {
        clickSource.clip = clickAudio;
        clickSource.Stop();
        clickSource.Play();
    }

    public void PlayFailAudio()
    {
        failSource.clip = failAudio;
        failSource.Stop();
        failSource.Play();

    }

    public void PlayLevelUpAudio()
    {
        levelUpSource.clip = levelUpAudio;
        levelUpSource.Stop();
        levelUpSource.Play();
    }

    public void PlayVictoryAudio()
    {
        victorySource.clip = victoryAudio;
        victorySource.Stop();
        victorySource.Play();
    }

    public void PlaySwallowAudio()
    {
        swallowSource.clip = swallowAudio;
        swallowSource.Stop();
        swallowSource.Play();
    }

    public void PlayGetHurtAudio()
    {

        getHurtSource.clip = getHurtAudio;
        getHurtSource.Stop();
        getHurtSource.Play();
    }
}
