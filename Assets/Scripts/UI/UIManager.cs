using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private IngameUI ingameUI;
    [SerializeField] private PauseUI pauseUI;
    [SerializeField] private DieUI dieUI;
    [SerializeField] private SubtitleUI subtitleUI;
    [SerializeField] private bool resumeFlag;

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
        ingameUI.Show();
        resumeFlag = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleByEsc();
        }
    }

    public void ShowRandomSubtitle(EntityType entityType)
    {
        subtitleUI.TypeRandomSubtitle(entityType);
    }

    public void ShowSubtitle(string text)
    {
        subtitleUI.TypeSubtitle(text);
    }
    public void ShowDieUI()
    {
        SoundManager.Instance.StopBGM();
        ingameUI.Hide();
        dieUI.Show();
    }

    public void ShowPauseUI()
    {
        SoundManager.Instance.PauseBGM();
        ingameUI.Hide();
        pauseUI.Show();
    }

    public void HidePauseUI()
    {
        SoundManager.Instance.ResumeBGM();
        ingameUI.Show();
        pauseUI.Hide();
    }

    public void ToggleByClick()
    {
        if (resumeFlag)
        {
            ShowPauseUI();
            resumeFlag = false;
        }
        else
        {
            HidePauseUI();
            resumeFlag = true;
        }
    }
    public void ToggleByEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (resumeFlag)
            {
                ShowPauseUI();
                resumeFlag = false;
            }
            else
            {
                HidePauseUI();
                resumeFlag = true;
            }
        }
    }
}
