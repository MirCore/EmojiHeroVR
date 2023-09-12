using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmoteSpawner : MonoBehaviour
{
    [SerializeField] private float Interval = 2f;
    public float XWidth = 2;
    public int Lanes = 4;

    private bool _spawnActive = false;
    
    void OnEnable()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
    }

    private void OnLevelStartedCallback()
    {
        _spawnActive = true;
        //StartCoroutine(SpawnEmote());
        SpawnEmote();
    }

    /*private IEnumerator SpawnEmote()
    {
        while (_spawnActive)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
            
            int lane = Random.Range(0, Lanes);
            float xPos = (lane - (float)(Lanes - 1) / 2)*XWidth;
            emote.transform.position = transform.position + new Vector3(xPos,0,0);
            emote.SetActive(true);
                
            yield return new WaitForSeconds(Interval);
        }
    }*/

    private async void SpawnEmote()
    {
        while (_spawnActive)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
            
            int lane = Random.Range(0, Lanes);
            float xPos = (lane - (float)(Lanes - 1) / 2)*XWidth;
            emote.transform.position = transform.position + new Vector3(xPos,0,0);
            emote.SetActive(true);

            await Task.Delay((int)(GameManager.Instance.Speed * 1000));
        }
    }
}
