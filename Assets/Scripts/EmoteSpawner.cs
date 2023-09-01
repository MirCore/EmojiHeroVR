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
        System.Random rnd = new();
        while (true)
        {
            GameObject emote = ObjectPool.Instance.GetPooledObject();
            if (emote != null)
            {
                emote.transform.position = transform.position + new Vector3((rnd.Next(Lanes) - (float)(Lanes-1)/2)*XWidth,0,0);
                emote.SetActive(true);
            }
            yield return new WaitForSeconds(Interval);
        }
    }
}
