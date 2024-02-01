using System.Collections;
using Manager;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform MainUIPosition;
    [SerializeField] private Transform ArcadePosition;

    [SerializeField] private float MoveDuration = 2;

    private void OnEnable()
    {
        EventManager.OnGameStarted += GameStartedCallback;
        EventManager.OnGameStopped += GameStoppedCallback;
        EventManager.OnLevelFinished += LevelFinishedCallback;

        MoveToScreen();
    }

    private void OnDestroy()
    {
        EventManager.OnGameStarted -= GameStartedCallback;
        EventManager.OnGameStopped -= GameStoppedCallback;
        EventManager.OnLevelFinished -= LevelFinishedCallback;
    }

    private void GameStartedCallback() => MoveToMachine();
    private void GameStoppedCallback() => MoveToScreen();
    private void LevelFinishedCallback() => MoveToScreen();

    private void MoveToMachine()
    {
        StartCoroutine(MoveToPosition(transform.position, ArcadePosition.position, MoveDuration));
        StartCoroutine(TurnToPosition(transform.forward, ArcadePosition.forward, MoveDuration));
    }


    private void MoveToScreen()
    {
        StartCoroutine(MoveToPosition(transform.position, MainUIPosition.position, MoveDuration));
        StartCoroutine(TurnToPosition(transform.forward, MainUIPosition.forward, MoveDuration));
    }

    private IEnumerator TurnToPosition(Vector3 from, Vector3 to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            Vector3 result = Vector3.Lerp(from, to, t / duration);
            t += Time.unscaledDeltaTime;
            transform.forward = result;

            yield return new WaitForEndOfFrame();
        }

        transform.forward = to;
    }

    private IEnumerator MoveToPosition(Vector3 from, Vector3 to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            Vector3 result = Vector3.Lerp(from, to, t / duration);
            t += Time.unscaledDeltaTime;
            transform.position = result;

            yield return new WaitForEndOfFrame();
        }

        transform.position = to;
    }
}
