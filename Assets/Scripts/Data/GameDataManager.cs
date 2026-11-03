using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameDataManager : SingletonBase<GameDataManager>
{
    private Dictionary<string, object> _dataList = new Dictionary<string, object>();

    private void Start()
    {
        LoadData<JobData>(LabelType.Data);
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
        Debug.Log($"{key} 데이터를 불러왔습니다");
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

        T data = null; 
        if (dataList.TryGetValue(id, out data) == false)
        {
            Debug.LogWarning("데이터가 없습니다");
        }
        return data;
    }

    public List<T> GetAllDataByLabel<T>(LabelType label) where T : GameDataBase
    {
        string key = label.ToString();
        var datas = _dataList[key];
        var dataList = datas as Dictionary<string, T>;
        if (dataList == null)
            return null;

        return dataList.Values.ToList<T>();
    }
}
