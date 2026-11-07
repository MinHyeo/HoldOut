using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum LabelType
{
    Job,
    Data,
}

public class ResourceManager : SingletonBase<ResourceManager>
{
    private readonly Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

    public async Task<T> LoadAsync<T>(string address) where T : Object
    {
        if(_handles.TryGetValue(address, out var cached))
        {
            return cached.Result as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        _handles[address] = handle;
        await handle.Task;
        return handle.Result;
    }

    public bool TryGet<T>(string address, out T asset) where T : Object
    {
        asset = _handles.TryGetValue(address, out var handle) && handle.IsDone ? handle.Result as T : null;
        return asset != null;
    }

    public T LoadSync<T>(string address) where T : Object
    {
        if (TryGet<T>(address, out T cached))
            return cached;

        if(_handles.TryGetValue(address, out AsyncOperationHandle handle) == false)
        {
            handle = Addressables.LoadAssetAsync<T>(address);
            _handles[address] = handle;
        }

        handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[ResourceManager] 동기 로드 실패: {address}");
            _handles.Remove(address);
            Addressables.Release(handle);
            return null;
        }

        return handle.Result as T;
    }

    public async Task<IList<T>> LoadAllDataAsync<T>()
    {
        string address = "Data";
        string key = typeof(T).Name;
        if(_handles.TryGetValue(key, out var cached))
        {
            return cached.Result as IList<T>;
        }

        var handle = Addressables.LoadAssetsAsync<T>(address);
        _handles[key] = handle;
        await handle.Task;
        return handle.Result;
    }
}
