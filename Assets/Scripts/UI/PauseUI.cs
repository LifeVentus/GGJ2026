using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using QFramework;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject backGround;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    private CanvasGroup cg;
    public void Show()
    {
        gameObject.SetActive(true);

        PlayerController.Instance.enabled = false;

        Time.timeScale = 0;
    }

    public void Hide()
    {
        Time.timeScale = 1;
        PlayerController.Instance.enabled = true;
        gameObject.SetActive(false);
    }
    void Awake()
    {
        continueBtn.onClick.AddListener(ReturnToGame);
        restartBtn.onClick.AddListener(Restart);
        menuBtn.onClick.AddListener(ReturnToMenu);
    }

    public void Restart()
    {
        PlayerDataController.Instance.PlayerDataModelInit();
        Hide();
        
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        Hide();
        SceneManager.LoadScene("MainMenu");
    }
    public void ReturnToGame()
    {
        //Hide();
        UIManager.Instance.ToggleByClick();
    }

}
