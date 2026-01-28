using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public SpriteRenderer sr;
    private Sprite originalSprite;
    [SerializeField] private float invincibleFlashInterval = 0.1f;
    [SerializeField] private float upgradeFlashInterval = 0.1f;
    public Color originalColor;
    public Color invincibleColor;
    private Coroutine flashCoroutine;
    private Coroutine upgradeFlashCoroutine;
    private PlayerController player;

    // 计时器
    public float upgradeCoolTime = 2f;
    private float invincibleCoolTime;

    void Start()
    {
        originalColor = sr.color;
        originalSprite = sr.sprite;
        player = PlayerController.Instance;
        player.OnLevelUpEvent += UpgradeFlash;
        invincibleCoolTime = player.invincibleCoolTime;
    }

    /// <summary>
    /// 受伤闪烁
    /// </summary>
    public void InvincibleFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine(invincibleFlashInterval, invincibleCoolTime));
    }

    /// <summary>
    /// 升级特效
    /// </summary>
    public void UpgradeFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine(upgradeFlashInterval, upgradeCoolTime));
    }

    /// <summary>
    /// 闪烁协程
    /// </summary>
    /// <param name="flashInterval"></param>
    /// <returns></returns>
    IEnumerator FlashCoroutine(float flashInterval, float flashTime)
    {
        float timer = 0;
        while(timer < flashTime)
        {
            sr.color = invincibleColor;
            yield return new WaitForSeconds(flashInterval);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        sr.color = originalColor;
    }
}
