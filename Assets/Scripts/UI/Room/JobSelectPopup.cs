using System;
using System.Collections.Generic;
using UnityEngine;

public class JobSelectPopup : UIBase
{
    private Dictionary<string, JobSlot> _jobSlotList = new();

    [SerializeField] private GameObject _jobSlotPerfab;
    [SerializeField] private Transform _content;

    public event Action<string> OnJobSelected;

    private void Start()
    {
        UpdateJobSlots();
    }

    private void UpdateJobSlots()
    {
        var datas = GameDataManager.Instance.GetAllDataByLabel<JobData>(LabelType.Job);

        foreach(var data in datas)
        {
            var slotObject = Instantiate(_jobSlotPerfab, _content);
            var slot = slotObject.GetComponent<JobSlot>();
            slot.UpdateSlot(data);

            _jobSlotList.Add(data.Id, slot);
            slot.OnClickJobButton += SelectJob;
        }
    }

    private void SelectJob(string id)
    {
        OnJobSelected?.Invoke(id);
    }
}
