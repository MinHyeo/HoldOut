using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum LabelType
{
    Job,
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

    public async Task<IList<T>> LoadAllAsync<T>(LabelType label)
    {
        string address = label.ToString();
        if(_handles.TryGetValue(address, out var cached))
        {
            return cached.Result as IList<T>;
        }

        var handle = Addressables.LoadAssetsAsync<T>(address);
        _handles[address] = handle;
        await handle.Task;
        return handle.Result;
    }
}
