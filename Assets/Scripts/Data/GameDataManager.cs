using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameDataManager : SingletonBase<GameDataManager>
{
    private Dictionary<string, object> _dataList = new Dictionary<string, object>();

    private void Start()
    {
        LoadData<JobData>(LabelType.Job);
    }

    public async void LoadData<T>(LabelType label) where T : GameDataBase
    {
        string key = label.ToString();
        var datas = await ResourceManager.Instance.LoadAllAsync<T>(label);
        _dataList.Add(key, new Dictionary<string, T>());

        Dictionary<string, T> dataList = new Dictionary<string, T>();
        foreach(var data in datas)
        {
            dataList.Add(data.Id, data);
        }

        _dataList[key] = dataList;
    }

    public T GetData<T>(LabelType label, string id) where T : GameDataBase
    {
        string key = label.ToString();
        if(_dataList.TryGetValue(key, out var datas) == false)
        {
            //LoadData<T>(label);
            datas = _dataList[key];
        }

        var dataList = datas as Dictionary<string, T>;
        if (dataList == null)
            return null;

        var data = dataList[id];
        return data;
    }
}
