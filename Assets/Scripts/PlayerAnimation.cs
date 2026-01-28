using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float invincibleFlashInterval = 0.1f;
    [SerializeField] private float upgradeFlashInterval = 1.0f;

    private Color originalColor;
    private Coroutine flashCoroutine;
    private Coroutine upgradeFlashCoroutine;
    private PlayerController player;
    void Start()
    {
        originalColor = sr.color;
        player = PlayerController.Instance;
        player.OnLevelUpEvent += UpgradeInvincibleFlash;
    }

    public void InvincibleFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine(invincibleFlashInterval));
    }

    public void UpgradeInvincibleFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine(upgradeFlashInterval));
    }

    IEnumerator FlashCoroutine(float flashInterval)
    {
        float timer = 0;
        while(timer < player.invincibleCoolTime)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        sr.color = originalColor;
    }
}
