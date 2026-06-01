using UnityEngine;

public class PooledInstance : MonoBehaviour
{
    public GameObject Prefab { get; private set; }
    public SimpleObjectPool Pool { get; private set; }
    public bool InPool { get; private set; }

    public void Init(GameObject prefab, SimpleObjectPool pool)
    {
        Prefab = prefab;
        Pool = pool;
    }

    public void MarkInPool(bool inPool)
    {
        InPool = inPool;
    }
}
