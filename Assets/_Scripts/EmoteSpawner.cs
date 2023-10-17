using System;
using System.Collections;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmoteSpawner : MonoBehaviour
{
    [SerializeField] private float XWidth = 0.5f;
    [SerializeField] private int Lanes = 4;

    private bool _spawnActive = false;

    private ScriptableLevel _level;

    private Vector3 _spawnDistance;
    private float _lanes;
    private Vector3 _actionAreaSpawnLocation;

    private void OnEnable()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
        EventManager.OnLevelStopped += OnLevelStoppedCallback;
        EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
        EventManager.OnLevelStopped -= OnLevelStoppedCallback;
        EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
    }

    private void Start()
    {
        _spawnDistance = GameManager.Instance.EmojiSpawnPosition.position;
        _lanes = (float)(Lanes - 1) / 2;
        _actionAreaSpawnLocation = GameManager.Instance.ActionArea.transform.position + GameManager.Instance.ActionArea.transform.up * 0.15f;
    }

    private void OnLevelStartedCallback()
    {
        _level = GameManager.Instance.Level;
        _spawnActive = true;
        GameManager.Instance.SetSpawnedEmotesCount(0);
        
        if (_level.LevelStruct.LevelMode == ELevelMode.Training)
            StartCoroutine(SpawnEmoteInActionArea(0));
        else
            StartCoroutine(SpawnEmoteCoroutine());
    }

    private void OnLevelStoppedCallback() => StopSpawning();
    
    private void OnDestroy() => StopSpawning();

    private void OnEmoteFulfilledCallback(EEmote emote, float score)
    {
        if (_level.LevelStruct.LevelMode == ELevelMode.Training)
            StartCoroutine(SpawnEmoteInActionArea(1));
    }
    
    private IEnumerator SpawnEmoteCoroutine()
    {
        while (_spawnActive)
        {
            int lane = Random.Range(0, Lanes);
            Vector3 position = _spawnDistance + new Vector3((lane - _lanes) * XWidth, 0, 0);
            
            ActivatePooledEmote(position);
            CheckLevelEndConditions();

            yield return new WaitForSeconds(_level.LevelStruct.SpawnInterval);
        }
    }

    private IEnumerator SpawnEmoteInActionArea(float waitBeforeSpawn)
    {
        yield return new WaitForSeconds(waitBeforeSpawn);
        
        ActivatePooledEmote(_actionAreaSpawnLocation);
        CheckLevelEndConditions();
    }
    
    private static void ActivatePooledEmote(Vector3 position)
    {
        GameObject emote = ObjectPool.Instance.GetPooledObject();
        emote.transform.position = position;
        emote.SetActive(true);
        GameManager.Instance.IncreaseSpawnedEmotesCount();
    }

    private void CheckLevelEndConditions()
    {
        if (GameManager.Instance.CheckLevelEndConditions(GameManager.Instance.SpawnedEmotesCount))
            StopSpawning();
    }

    private void StopSpawning() => _spawnActive = false;

}
