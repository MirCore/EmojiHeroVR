using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Manager;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

/// <summary>
/// This class is responsible for spawning emotes in the game. It allows for emotes to be spawned at specific locations based on the game's level and mode.
/// </summary>
public class EmoteSpawner : MonoBehaviour
{
    private readonly List<SpawnPoint> _spawnPoints = new(); // The GameObject indicating the spawn position
    [SerializeField] private Transform SpawnPosition; // The GameObject indicating the spawn position
    [SerializeField] private Transform TrainingSpawnPosition; // The GameObject indicating the spawn position
    [SerializeField] private Material LaneMaterial;

    private bool _spawnActive; // Flag to control whether emotes should be spawned.

    private static ObjectPool _objectPool;

    private void OnEnable()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
        EventManager.OnLevelFinished += OnLevelFinishedCallback;
        EventManager.OnGameStarted += OnGameStartedCallback;
        EventManager.OnGameStopped += OnGameStoppedCallback;
        EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
        EventManager.OnEmoteFailed += OnEmoteFailedCallback;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
        EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        EventManager.OnGameStarted -= OnGameStartedCallback;
        EventManager.OnGameStopped -= OnGameStoppedCallback;
        EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
        EventManager.OnEmoteFailed -= OnEmoteFailedCallback;
    }

    private void Start()
    {
        _objectPool = GetComponent<ObjectPool>();
    }

    /// <summary>
    /// Set the Lanes when the game starts
    /// </summary>
    private void OnGameStartedCallback()
    {
        _spawnPoints.Clear();
        int lanes = GameManager.Instance.PlayerCount == 1 ? 4 : GameManager.Instance.PlayerCount;
        
        Vector3 position = SpawnPosition.position;
        for (int i = 0; i < lanes; i ++)
        {
            float totalWidth = position.x * 2;
            float laneWidth = totalWidth / lanes;
            float positionX = laneWidth / 2 + laneWidth * i - totalWidth / 2;
            _spawnPoints.Add(new SpawnPoint(new Vector3(positionX, position.y, position.z), SpawnPosition.forward));
        }
        
        LaneMaterial.mainTextureScale = new Vector2(lanes, 1);
    }

    /// <summary>
    /// Start spawning emotes when the level starts.
    /// </summary>
    private void OnLevelStartedCallback()
    {
        _spawnActive = true;
        
        // Determine the spawning behavior based on the level mode.
        StartCoroutine(GameManager.Instance.Level.LevelMode == ELevelMode.Training
            ? SpawnEmoteInActionArea(waitBeforeSpawn: 0)
            : SpawnEmoteAtSpawnLocation());
    }

    /// <summary>
    /// Spawn a new emote in Training mode when the previous one is failed.
    /// </summary>
    private void OnEmoteFailedCallback(Emoji emoji) => SpawnTrainingEmote();

    /// <summary>
    /// Spawn a new emote in Training mode when the previous one is fulfilled.
    /// </summary>
    private void OnEmoteFulfilledCallback(Emoji emoji, TimeSpan time) => SpawnTrainingEmote();

    private void SpawnTrainingEmote()
    {
        // In Training mode, spawn a new emote when the previous one is fulfilled.
        if (GameManager.Instance.Level.LevelMode == ELevelMode.Training)
            StartCoroutine(SpawnEmoteInActionArea(waitBeforeSpawn: 1.1f));
    }

    /// <summary>
    /// Coroutine to handle spawning of emotes at the start of the lane.
    /// </summary>
    private IEnumerator SpawnEmoteAtSpawnLocation()
    {
        while (_spawnActive)
        {
            switch (GameManager.Instance.PlayerCount)
            {
                case 1:
                    SpawnPoint position = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                    ActivatePooledEmote(position);
                    break;
                case 2:
                    ActivatePooledEmote(_spawnPoints[0], 0);
                    ActivatePooledEmote(_spawnPoints[^1], 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CheckLevelEndConditions();

            yield return new WaitForSeconds(GameManager.Instance.Level.SpawnInterval);
        }
    }

    /// <summary>
    /// Coroutine to handle spawning of emotes in the action area during Training mode.
    /// </summary>
    /// <param name="waitBeforeSpawn">Time to wait before spawning the emote.</param>
    private IEnumerator SpawnEmoteInActionArea(float waitBeforeSpawn)
    {
        yield return new WaitForSeconds(waitBeforeSpawn);
        if (!_spawnActive) 
            yield break;
        ActivatePooledEmote(new SpawnPoint(TrainingSpawnPosition.position, TrainingSpawnPosition.forward));
        CheckLevelEndConditions();
    }
    
    /// <summary>
    /// Activate an emote from the object pool and set its position.
    /// </summary>
    /// <param name="position">The position to spawn the emote at.</param>
    private static void ActivatePooledEmote(SpawnPoint position, int player = -1)
    {
        // Retrieve an emote object from the pool, set its position, and activate it.
        EmojiManager emojiManager = _objectPool.GetPooledObject();
        emojiManager.SetPosition(position);
        emojiManager.SetPlayer(player);
        emojiManager.gameObject.SetActive(true);
    }

    /// <summary>
    /// Check if the end conditions for the level are met, and stop spawning if they are.
    /// </summary>
    private void CheckLevelEndConditions()
    {
        if (LevelManager.CheckLevelEndConditions(GameManager.LevelProgress.SpawnedEmotesCount))
            StopSpawning();
    }
    
    /// <summary>
    /// Stop spawning emotes when the level stops.
    /// </summary>
    private void OnLevelFinishedCallback() => StopSpawning();
    private void OnGameStoppedCallback() => StopSpawning();
    
    /// <summary>
    /// Ensure that spawning is stopped when this object is destroyed.
    /// </summary>
    private void OnDestroy() => StopSpawning();

    /// <summary>
    /// Stop the spawning of emotes.
    /// </summary>
    private void StopSpawning() => _spawnActive = false;

}

[System.Serializable] // Make it visible in the inspector and serializable.
public struct SpawnPoint
{
    public Vector3 Position;
    public Vector3 Forward;

    public SpawnPoint(Vector3 position, Vector3 forward)
    {
        Position = position;
        Forward = forward;
    }
}