using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private Button menuBtn;

    void Awake()
    {
        menuBtn.onClick.AddListener(ReturnToMenu);
    }
    public void Show()
    {
        gameObject.SetActive(true);

        PlayerController.Instance.enabled = false;
    }
    public void Hide()
    {
        PlayerController.Instance.enabled = true;
        gameObject.SetActive(false);
    }
    public void ReturnToMenu()
    {
        Hide();
        SceneManager.LoadScene("MainMenu");
    }
}
