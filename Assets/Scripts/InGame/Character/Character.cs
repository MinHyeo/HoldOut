using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> JobId = new NetworkVariable<FixedString64Bytes>("Job_Random");

    private JobData _jobData;
    private Animator _anim;
    private CharacterCombat _combat;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _combat = GetComponentInChildren<CharacterCombat>();
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

        _anim.runtimeAnimatorController = _jobData.AnimatorController;
        _combat.InitAbilities(_jobData);
    }
}
