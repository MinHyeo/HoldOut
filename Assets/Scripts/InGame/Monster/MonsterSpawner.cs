using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private MatchState _matchState;
    [SerializeField] private NetworkObject _monsterPrefab;

    [Header("위치")]
    [SerializeField] private float _spawnRadius = 12f;   // 기준 플레이어로부터 거리 (화면 바로 밖)
    [SerializeField] private float _safeRadius = 10f;    // 모든 플레이어와 최소 이 거리 이상 떨어져야 함
    [SerializeField] private int _maxPositionAttempts = 10;

    [Header("양")]
    [SerializeField] private float _spawnInterval = 1.5f;       // 시작 스폰 간격
    [SerializeField] private float _minSpawnInterval = 0.5f;    // 가장 빠를 때
    [SerializeField] private float _intervalDecreasePerMinute = 0.15f;
    [SerializeField] private int _countPerSpawn = 2;            // 시작 시 한 번에
    [SerializeField] private int _countIncreasePerMinute = 1;
    [SerializeField] private int _maxAlive = 150;               // 동시 최대 수
    [SerializeField] private float[] _playerCountMultiplier = { 1f, 1f, 1.4f, 1.8f };  // 인덱스 = 인원 - 1

    private readonly List<Monster> _alive = new();
    private readonly List<Transform> _players = new();
    private bool _running;
    private float _elapsed;
    private float _timer;

    private void OnEnable()
    {
        _matchState.Phase.OnValueChanged += HandlePhaseChanged;
    }

    private void OnDisable()
    {
        _matchState.Phase.OnValueChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GamePhase previous, GamePhase current)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (current == GamePhase.InGame)
            StartSpawning();
        else if (previous == GamePhase.InGame)
            StopSpawning();
    }

    private void StartSpawning()
    {
        DespawnAll();
        _elapsed = 0f;
        _timer = 1f;
        _running = true;
    }

    private void StopSpawning()
    {
        _running = false;
        DespawnAll();
    }

    private void Update()
    {
        if (_running == false || GameManager.Instance.MatchState.IsRoundPaused.Value)
            return;

        _elapsed += Time.deltaTime;
        _timer -= Time.deltaTime;
        if (_timer > 0f)
            return;

        float minutes = _elapsed / 60f;
        _timer = Mathf.Max(_minSpawnInterval, _spawnInterval - _intervalDecreasePerMinute * minutes);

        SpawnWave(minutes);
    }

    private void SpawnWave(float minutes)
    {
        CollectAlivePlayers();
        if (_players.Count == 0)
            return;

        float multiplier = _playerCountMultiplier[Mathf.Clamp(_players.Count - 1, 0, _playerCountMultiplier.Length - 1)];
        int count = Mathf.RoundToInt((_countPerSpawn + _countIncreasePerMinute * (int)minutes) * multiplier);

        for (int i = 0; i < count; i++)
        {
            if (_alive.Count >= _maxAlive)
                return;   // 상한 도달: 이번엔 그만

            if (TryGetSpawnPosition(out Vector2 position))
                SpawnMonster(position);
        }
    }

    private void SpawnMonster(Vector2 position)
    {
        NetworkObject instance = Instantiate(_monsterPrefab, position, Quaternion.identity);
        instance.Spawn(destroyWithScene: true);   // 이 순간 모든 클라이언트에 생성됨

        Monster monster = instance.GetComponent<Monster>();
        monster.OnDespawned += HandleMonsterDespawned;
        _alive.Add(monster);
    }

    private void HandleMonsterDespawned(Monster monster)
    {
        monster.OnDespawned -= HandleMonsterDespawned;
        _alive.Remove(monster);
    }

    private void DespawnAll()
    {
        // 디스폰하면 HandleMonsterDespawned가 리스트에서 지우므로 복사본으로 돈다
        foreach (Monster monster in _alive.ToArray())
        {
            if (monster != null && monster.IsSpawned)
                monster.NetworkObject.Despawn(destroy: true);
        }
        _alive.Clear();
    }

    private void CollectAlivePlayers()
    {
        _players.Clear();
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null)
                continue;
            if (player.TryGetComponent(out CharacterHealth health) && health.IsDowned)
                continue;   // 쓰러진 플레이어 주변엔 스폰하지 않음

            _players.Add(player.transform);
        }
    }

    private bool TryGetSpawnPosition(out Vector2 position)
    {
        float safeSqr = _safeRadius * _safeRadius;

        for (int attempt = 0; attempt < _maxPositionAttempts; attempt++)
        {
            Transform anchor = _players[Random.Range(0, _players.Count)];
            position = (Vector2)anchor.position + Random.insideUnitCircle.normalized * _spawnRadius;

            bool tooClose = false;
            foreach (Transform player in _players)
            {
                if (((Vector2)player.position - position).sqrMagnitude < safeSqr)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return true;
        }

        position = default;
        return false;   // 자리를 못 찾으면 이번 한 마리는 건너뜀
    }
}
