using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using QFramework;
using UnityEngine.SceneManagement;

public class DieUI : MonoBehaviour
{
    [SerializeField] private GameObject backGround;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    private CanvasGroup cg;
    public void Show()
    {
        gameObject.SetActive(true);

        PlayerController.Instance.enabled = false;

        CanvasGroup cg = backGround.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        cg.DOFade(1, 0.3f).OnComplete(() =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        });
    }

    public void Hide()
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;

        cg.DOFade(0, 0.3f);
        gameObject.SetActive(false);
    }
    void Awake()
    {
        restartBtn.onClick.AddListener(Restart);
        menuBtn.onClick.AddListener(ReturnToMenu);
    }

    public void Restart()
    {
        PlayerController.Instance.enabled = true;
        PlayerDataController.Instance.PlayerDataModelInit();
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
