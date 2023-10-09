using System;
using System.Collections;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmoteSpawner : MonoBehaviour
{
    public float XWidth = 2;
    public int Lanes = 4;

    private bool _spawnActive = false;
    private int _count;
    private float _startTime;

    private ScriptableLevel _level;
    
    void OnEnable()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
        EventManager.OnLevelStopped += OnLevelStoppedCallback;
        EventManager.OnEmojiFulfilled += OnEmojiFulfilledCallback;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
        EventManager.OnLevelStopped -= OnLevelStoppedCallback;
        EventManager.OnEmojiFulfilled -= OnEmojiFulfilledCallback;
    }

    private void OnLevelStartedCallback()
    {
        _level = GameManager.Instance.Level;
        _spawnActive = true;
        _count = 0;
        _startTime = Time.time;
        if (_level.LevelMode == ELevelMode.Training)
            StartCoroutine(SpawnEmote(0));
        else
            StartCoroutine(SpawnEmoteCoroutine());
    }

    private void OnLevelStoppedCallback()
    {
        _spawnActive = false;
    }
    
    private void OnEmojiFulfilledCallback(EEmote emote, float score)
    {
        if (_level.LevelMode == ELevelMode.Training)
            StartCoroutine(SpawnEmote(1));
    }
    
    private IEnumerator SpawnEmoteCoroutine()
    {
        Vector3 spawnDistance = transform.position;
        float lanes = (float)(Lanes - 1) / 2;
        while (_spawnActive)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
        
            int lane = Random.Range(0, Lanes);
            float xPos = (lane - lanes) * XWidth;
            emote.transform.position = spawnDistance + new Vector3(xPos, 0, 0);
            emote.SetActive(true);
            _count++;

            if (CheckLevelEnded())
                StopSpawning();

            yield return new WaitForSeconds(_level.EmojiSpawnInterval);
        }
    }
    
    private IEnumerator SpawnEmote(float waitBeforeSpawn)
    {
        yield return new WaitForSeconds(waitBeforeSpawn);
        
        GameObject emote = ObjectPool.Instance.GetPooledObject();

        emote.transform.position = GameManager.Instance.ActionArea.transform.position;
        emote.SetActive(true);
        _count++;
    }

    private bool CheckLevelEnded()
    {
        switch (_level.LevelMode)
        {
            case ELevelMode.Count:
                if (_count >= _level.Count)
                    return true;
                break;
            case ELevelMode.Time:
                if (Time.time - _startTime >= _level.Time)
                    return true;
                break;
            case ELevelMode.Training:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    private void StopSpawning()
    {
        _spawnActive = false;
    }

    private void OnDestroy()
    {
        StopSpawning();
    }
}
