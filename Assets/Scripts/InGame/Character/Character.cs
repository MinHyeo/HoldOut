using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> JobId;// = new NetworkVariable<FixedString64Bytes>("Job_Random");
    public CharacterStats Stats { get; private set; } = new();

    private JobData _jobData;

    // Component
    private Animator _anim;
    private CharacterCombat _combat;
    private CharacterMovement _movement;
    private CharacterHealth _health;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _combat = GetComponentInChildren<CharacterCombat>();
        _movement = GetComponentInChildren<CharacterMovement>();
        _health = GetComponentInChildren<CharacterHealth>();
    }

    public override void OnNetworkSpawn()
    {
        ApplyJob(JobId.Value.ToString());
        JobId.OnValueChanged += HandleJobChanged;
        Stats.OnValueChanged += ApplyStats;
    }

    public override void OnNetworkDespawn()
    {
        JobId.OnValueChanged -= HandleJobChanged;
        Stats.OnValueChanged -= ApplyStats;
    }

    private void HandleJobChanged(FixedString64Bytes previous, FixedString64Bytes current)
    {
        ApplyJob(current.ToString());
    }

    private void ApplyJob(string jobId)
    {
        _jobData = GameDataManager.Instance.GetData<JobData>(jobId);
        if (_jobData == null)
            return;

        Stats.InitStat(_jobData);
        _anim.runtimeAnimatorController = _jobData.AnimatorController;
        _combat.InitAbilities(_jobData, this);
        _health.SetMaxHp(Stats.MaxHp);
        _movement.SetMoveSpeed(Stats.MoveSpeed);
    }

    private void ApplyStats()
    {
        _health.SetMaxHp(Stats.MaxHp);
        _movement.SetMoveSpeed(Stats.MoveSpeed);
    }
}
