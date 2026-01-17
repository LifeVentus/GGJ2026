using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private GameObject entityPrefab;
    [SerializeField] private float generateRadiusMax = 30f;
    [SerializeField] private float generateRadiusMin = 10f;
    [SerializeField] private int entityAmount = 10;
    [SerializeField] private float coolTime = 1f;
    private float coolTimeCounter;
    private PlayerController player;
    void Start()
    {
        player = PlayerController.Instance;
        coolTimeCounter = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        CheckEntity();
    }
    private void CheckEntity()
    {
        int entityCount = player.DetectCheck(entityPrefab.layer, generateRadiusMax);
        if(entityCount < entityAmount && coolTimeCounter <= 0)
        {
            GenerateEntity();
            coolTimeCounter = coolTime;
        }
        coolTimeCounter -= Time.deltaTime;

    }
    private void GenerateEntity()
    {
        float randomX = Random.Range(generateRadiusMin, generateRadiusMax);
        float randomY = Random.Range(generateRadiusMin, generateRadiusMax);

        Vector2 generatePosition = (Vector2)player.transform.position + new Vector2(randomX, randomY);

        Instantiate(entityPrefab, generatePosition, Quaternion.identity);
    }
}
