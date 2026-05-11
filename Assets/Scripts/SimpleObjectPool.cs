using UnityEngine;
using System.Collections.Generic;

public class SimpleObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 5;
    }

    [SerializeField] private PoolConfig[] prewarm = new PoolConfig[0];
    [SerializeField] private bool dontDestroyOnLoad = true;

    private static SimpleObjectPool instance;
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    public static SimpleObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SimpleObjectPool>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ObjectPool");
                    instance = go.AddComponent<SimpleObjectPool>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        Prewarm();
    }

    private void Prewarm()
    {
        if (prewarm == null) return;

        for (int i = 0; i < prewarm.Length; i++)
        {
            var config = prewarm[i];
            if (config == null || config.prefab == null || config.initialSize <= 0) continue;

            Queue<GameObject> pool = GetPool(config.prefab);
            for (int j = 0; j < config.initialSize; j++)
            {
                GameObject instanceObj = CreateInstance(config.prefab);
                AddToPool(config.prefab, instanceObj, pool);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        Queue<GameObject> pool = GetPool(prefab);
        GameObject instanceObj = null;

        while (pool.Count > 0 && instanceObj == null)
        {
            instanceObj = pool.Dequeue();
        }

        if (instanceObj == null)
        {
            instanceObj = CreateInstance(prefab);
        }

        var marker = instanceObj.GetComponent<PooledInstance>();
        if (marker != null)
        {
            marker.Init(prefab, this);
            marker.MarkInPool(false);
        }

        instanceObj.transform.SetParent(null);
        instanceObj.transform.SetPositionAndRotation(position, rotation);
        instanceObj.SetActive(true);
        return instanceObj;
    }

    public void Despawn(GameObject instanceObj)
    {
        if (instanceObj == null) return;

        var marker = instanceObj.GetComponent<PooledInstance>();
        if (marker == null || marker.Prefab == null || marker.Pool != this)
        {
            Destroy(instanceObj);
            return;
        }

        if (marker.InPool) return;

        marker.MarkInPool(true);
        instanceObj.SetActive(false);
        instanceObj.transform.SetParent(transform);
        Queue<GameObject> pool = GetPool(marker.Prefab);
        pool.Enqueue(instanceObj);
    }

    private Queue<GameObject> GetPool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new Queue<GameObject>();
            pools[prefab] = pool;
        }

        return pool;
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject instanceObj = Instantiate(prefab);
        instanceObj.SetActive(false);
        instanceObj.transform.SetParent(transform);

        var marker = instanceObj.GetComponent<PooledInstance>();
        if (marker == null)
        {
            marker = instanceObj.AddComponent<PooledInstance>();
        }
        marker.Init(prefab, this);
        marker.MarkInPool(true);

        return instanceObj;
    }

    private void AddToPool(GameObject prefab, GameObject instanceObj, Queue<GameObject> pool)
    {
        if (instanceObj == null) return;

        var marker = instanceObj.GetComponent<PooledInstance>();
        if (marker != null)
        {
            marker.Init(prefab, this);
            marker.MarkInPool(true);
        }

        instanceObj.SetActive(false);
        instanceObj.transform.SetParent(transform);
        pool.Enqueue(instanceObj);
    }
}
