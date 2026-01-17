using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;

public class FoodController : MonoBehaviour, IController
{
    [Header("食物价值")]
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private int expValue = 20;
    [SerializeField] private int hp = 10;

    public IArchitecture GetArchitecture()
    {
        return PlayerData.Interface;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.SendCommand(new ChangeScoreCommand(scoreValue));
            this.SendCommand(new ChangeEXPValueCommand(expValue));
            this.SendCommand(new ChangeHpCommand(hp));

            Destroy(gameObject);
        }
    }
}
