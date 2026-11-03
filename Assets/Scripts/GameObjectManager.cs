using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectManager : SingletonBase<GameObjectManager>
{
    private Transform _rootTransform;
    private PoolManager _poolManager;

    private void OnEnable()
    {
        _poolManager = new PoolManager();
        _rootTransform = this.transform;
    }

    public GameObject CreateObject(string path, Vector3 spawnSpot)
    {
        if(!ResourceManager.Instance.TryGet(path, out GameObject prefab))
        {
            prefab = ResourceManager.Instance.LoadSync<GameObject>(path);
        }

        return Spawn(prefab, spawnSpot);
    }

    private GameObject Spawn(GameObject prefab, Vector3 spawnSpot)
    {
        return _poolManager.Pop(prefab, spawnSpot);
    }

    public void RequestDestroyObject(GameObject gameObject)
    {
        _poolManager.Push(gameObject);
    }

    public void RemoveAllObject()
    {
        _poolManager.Clear();

        if (_rootTransform == null)
            return;

        for (int i = _rootTransform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(_rootTransform.GetChild(i).gameObject);
        }
    }
}
