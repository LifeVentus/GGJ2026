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
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseUI();
        }
    }

    public void ShowDieUI()
    {
        ingameUI.Hide();
        dieUI.Show();
    }

    public void ShowPauseUI()
    {
        ingameUI.Hide();
        pauseUI.Show();
    }

}
