using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteSpawner : MonoBehaviour
{
    [SerializeField] private float Interval = 2f;
    public float XWidth = 2;
    public int Lanes = 4;
    
    void Start()
    {
        StartCoroutine(SpawnEmote());
    }

    private IEnumerator SpawnEmote()
    {
        while (true)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
            if (emote != null)
            {
                int lane = Random.Range(0, Lanes);
                float xPos = (lane - (float)(Lanes - 1) / 2)*XWidth;
                emote.transform.position = transform.position + new Vector3(xPos,0,0);
                emote.SetActive(true);
            }
            yield return new WaitForSeconds(Interval);
        }
    }
}
