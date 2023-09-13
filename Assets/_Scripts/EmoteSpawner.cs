using System;
using System.Threading.Tasks;
using Enums;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmoteSpawner : MonoBehaviour
{
    public float XWidth = 2;
    public int Lanes = 4;

    private bool _spawnActive = false;
    private int _count;
    private float _startTime;
    
    void OnEnable()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
        EventManager.OnLevelStopped += OnLevelStoppedCallback;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
        EventManager.OnLevelStopped -= OnLevelStoppedCallback;
    }

    private void OnLevelStartedCallback()
    {
        _spawnActive = true;
        _startTime = Time.time;
        SpawnEmote();
    }

    private void OnLevelStoppedCallback()
    {
        _spawnActive = false;
    }

    private async void SpawnEmote()
    {
        while (_spawnActive)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
            
            int lane = Random.Range(0, Lanes);
            float xPos = (lane - (float)(Lanes - 1) / 2)*XWidth;
            emote.transform.position = transform.position + new Vector3(xPos,0,0);
            emote.SetActive(true);
            _count++;
            
            if (CheckLevelEnded())
                StopSpawning();

            await Task.Delay((int)(GameManager.Instance.SpawnInterval * 1000));
        }
    }

    private bool CheckLevelEnded()
    {
        switch (GameManager.Instance.Level.LevelMode)
        {
            case ELevelMode.Count:
                if (_count >= GameManager.Instance.Level.Count)
                    return true;
                break;
            case ELevelMode.Time:
                if (Time.time - _startTime >= GameManager.Instance.Level.Time)
                    return true;
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
