using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooling : MonoBehaviour
{
    public static ObstaclePooling Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int poolSize;
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Init tile in pool
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        isInitialized = true;
    }

    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count > 0)
        {
            GameObject obj = poolDictionary[tag].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Instantiate prefab if pool is null
            Pool pool = pools.Find(p => p.tag == tag);

            if (pool != null)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                return obj;
            }
            else
            {
                Debug.LogWarning($"No prefab found for tag {tag}.");
                return null;
            }
        }
    }

    public void ReturnObject(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
