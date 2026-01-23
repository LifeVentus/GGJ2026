using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private static EntityManager instance;
    public static EntityManager Instance
    {
        get
        {
            if(instance == null) return null;
            return instance;
        }
    }
    
    private Dictionary<string, IObjectPool> pools = new Dictionary<string, IObjectPool>();

    [Header("参数设置")]
    [SerializeField] private List<BaseEntity> entityList;
    [Range(0, 100)]
    [SerializeField] private List<float> entityRatio;
    [SerializeField] private int entityMaxAmount;
    [SerializeField] private float generateMinRadius;
    [SerializeField] private float generateMaxRadius;

    private float coolTimeCounter;
    private PlayerController player;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // 初始化对象池
        for(int i = 0; i < entityList.Count; i++)
        {
            int entityCount = (int)(entityMaxAmount * (entityRatio[i] / 100));
            CreatePool(entityList[i], entityCount);
        }
    }
    void Start()
    {
        player = PlayerController.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateEntity();
    }
    private void GenerateEntity()
    {
        Vector2 playerPosition = player.transform.position;
        int sign = Random.Range(0, 2) * 2 - 1;
        float randomX = playerPosition.x + sign * Random.Range(generateMinRadius, generateMaxRadius);
        sign = Random.Range(0, 2) * 2 - 1;
        float randomY = playerPosition.y + sign * Random.Range(generateMinRadius, generateMaxRadius);

        for(int i = 0; i < entityList.Count; i++)
        {
            string key = entityList[i].name;
            int activeCount = GetPoolActiveCount(key);
            int entityCount = (int)(entityMaxAmount * (entityRatio[i] / 100));
            if(activeCount < entityCount)
            {
                BaseEntity entity = Get(entityList[i]);
                entity.prefab = entityList[i].gameObject;
                entity.transform.position = new Vector2(randomX, randomY);
                entity.Init();
                
            }
        }
    }

    public int GetPoolActiveCount(string poolKey)
    {
        if(pools.TryGetValue(poolKey, out IObjectPool pool))
        {
            return pool.ActiveCount;
        }
        return 0;
    }
    public void CreatePool<T>(T prefab, int initialSize) where T : MonoBehaviour
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            pools[key] = new ObjectPool<T>(prefab, initialSize, transform);
        }
    }

    public T Get<T>(T prefab) where T : MonoBehaviour
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            CreatePool(prefab, 1);
        }
        ObjectPool<T> pool = (ObjectPool<T>)pools[key];
        return pool.Get(); 
    }

    public void Return<T>(T obj) where T : MonoBehaviour
    {
        string key = obj.name.Replace("(Clone)", ""); // 移除克隆后缀
        if (pools.ContainsKey(key))
        {
            ObjectPool<T> pool = (ObjectPool<T>)pools[key];
            pool.Return(obj);
        }
    }
}
