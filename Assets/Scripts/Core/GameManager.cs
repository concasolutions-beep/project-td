using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [Header("Economy")]
    [SerializeField] private int startingGold = 100;
    [SerializeField] private int startingLives = 10;

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public int Gold { get; private set; }
    public int Lives { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnLivesChanged;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        Gold = startingGold;
        Lives = startingLives;
    }

    void OnEnable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnEnemyKilled += HandleEnemyKilled;
        waveManager.OnEnemyReachedBase += HandleEnemyReachedBase;
        waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
    }

    void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnEnemyKilled -= HandleEnemyKilled;
        waveManager.OnEnemyReachedBase -= HandleEnemyReachedBase;
        waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
    }

    public bool TrySpendGold(int amount)
    {
        if (CurrentState != GameState.Playing || amount > Gold)
        {
            return false;
        }

        Gold -= amount;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    private void HandleEnemyKilled(EnemyData data)
    {
        if (data == null)
        {
            return;
        }

        AddGold(data.gold);
    }

    private void HandleEnemyReachedBase(EnemyData data)
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        Lives = Mathf.Max(0, Lives - data.damage);
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            Time.timeScale = 0f;
            SetState(GameState.GameOver);
        }
    }

    private void HandleAllWavesCompleted()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        Time.timeScale = 0f;
        SetState(GameState.Victory);
    }

    private void SetState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
