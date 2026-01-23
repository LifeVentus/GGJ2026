using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float flashInterval = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;
    private PlayerController player;
    void Start()
    {
        originalColor = sr.color;
        player = PlayerController.Instance;
    }

    public void Flash()
    {
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
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
