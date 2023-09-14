using Enums;
using States.Emojis;
using UnityEngine;

namespace Manager
{
    public class EmojiManager : MonoBehaviour
    {
        private EmojiState _emojiState;
        internal readonly EmojiPreState PreState = new ();
        internal readonly EmojiIntraState IntraState = new ();
        internal readonly EmojiFulfilledState FulfilledState = new ();
        internal readonly EmojiFailedState FailedState = new ();
    
        [SerializeField] internal EEmote Emote;
        [SerializeField] internal Renderer EmojiRenderer;
        [SerializeField] internal Animator EmojiAnimator;

        internal readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

        private void OnEnable()
        {
            // starting state for the state machine
            _emojiState = PreState;
            _emojiState.EnterState(this);
        
            EventManager.OnEmotionDetected += OnEmotionDetectedCallback;
        }

        private void OnDisable()
        {
            EventManager.OnEmotionDetected -= OnEmotionDetectedCallback;
        }

        private void Update()
        {
            transform.position -= new Vector3(0,0,GameManager.Instance.Level.EmojiMovementSpeed * Time.deltaTime);

            if (transform.position.z < GameManager.Instance.EmojiEndPosition.position.z)
            {
                DeactivateEmoji();
            }
        }

        internal void SwitchState(EmojiState state)
        {
            _emojiState = state;
            _emojiState.EnterState(this);
        }

        private void OnEmotionDetectedCallback(EEmote emote)
        {
            _emojiState.OnEmotionDetectedCallback(this, emote);
        }

        private void DeactivateEmoji()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            _emojiState.OnTriggerEnter(this);
        }
    
        private void OnTriggerExit(Collider other)
        {
            _emojiState.OnTriggerExit(this);
        }
    }
}
