using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> JobId = new NetworkVariable<FixedString64Bytes>("Job_Random");

    private JobData _jobData;

    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        ApplyJob(JobId.Value.ToString());
        JobId.OnValueChanged += HandleJobChanged;
    }

    public override void OnNetworkDespawn()
    {
        JobId.OnValueChanged -= HandleJobChanged;
    }

    private void HandleJobChanged(FixedString64Bytes previous, FixedString64Bytes current)
    {
        ApplyJob(current.ToString());
    }

    private void ApplyJob(string jobId)
    {
        _jobData = GameDataManager.Instance.GetData<JobData>(LabelType.Data, jobId);
        if (_jobData == null)
            return;

        _sprite.sprite = _jobData.JobIcon;
    }
}
