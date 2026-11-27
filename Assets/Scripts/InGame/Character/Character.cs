using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> JobId;
    public CharacterStats Stats { get; private set; } = new();

    private JobData _jobData;
    private bool _wasDowned = false;
    private int _downTriggerHash = Animator.StringToHash("Down");
    private int _isDownedHash = Animator.StringToHash("IsDowned");

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
        _health.OnHpChanged += HandleHpChanged;
    }

    public override void OnNetworkDespawn()
    {
        JobId.OnValueChanged -= HandleJobChanged;
        Stats.OnValueChanged -= ApplyStats;
        _health.OnHpChanged -= HandleHpChanged;
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

    private void HandleHpChanged(float hp, float maxHp)
    {
        bool isDowned = hp <= 0f;
        if (isDowned == _wasDowned)
            return;

        _wasDowned = isDowned;
        if (isDowned)
        {
            Die();
        }
        else
        {
            Revive();
        }
    }

    private void Die()
    {
        _movement.enabled = false;
        _combat.enabled = false;
        _anim.SetTrigger(_downTriggerHash);
        _anim.SetBool(_isDownedHash, true);

        if (IsServer)
        {
            GameManager.Instance.MatchState.CheckAllDowned();
        }
    }

    private void Revive()
    {
        _movement.enabled = true;
        _combat.enabled = true;
        _anim.SetBool(_isDownedHash, false);
    }

    // Rpc
    [Rpc(SendTo.Server)]
    public void RequestUpgradeRpc(string path)
    {
        var data = GameDataManager.Instance.GetData<UpgradeData>(path);
        data.Apply(this);

        GameManager.Instance.PartyProgress.NotifyUpgradeChosen(OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    public void RequestReviveRpc(NetworkObjectReference targetRef)
    {
        if (!targetRef.TryGet(out NetworkObject targetObject))
            return;

        if (!targetObject.TryGetComponent(out CharacterHealth targetHealth))
            return;

        targetHealth.TickRevive(Time.deltaTime);
    }
}