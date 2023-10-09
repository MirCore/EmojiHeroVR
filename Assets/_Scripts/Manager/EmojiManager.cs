using System.Collections;
using Enums;
using States.Emojis;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class EmojiManager : MonoBehaviour
    {
        private EmojiState _emojiState;
        internal readonly EmojiPreState PreState = new ();
        internal readonly EmojiIntraState IntraState = new ();
        internal readonly EmojiFulfilledState FulfilledState = new ();
        internal readonly EmojiFailedState FailedState = new ();
        internal readonly EmojiLeavingState LeavingState = new();
    
        [SerializeField] internal EEmote Emote;
        [SerializeField] private Renderer EmojiRenderer;
        [SerializeField] internal Animator EmojiAnimator;

        internal Material EmojiMaterial;
        internal readonly int Sprite = Shader.PropertyToID("_Sprite");
        internal readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        internal readonly int FailedColorAmount = Shader.PropertyToID("_FailedColorAmount");
        internal readonly int SuccessColorAmount = Shader.PropertyToID("_SuccessColorAmount");
        internal float ActiveAreaLeft;
        internal float ActionAreaSize;

        private void OnEnable()
        {
            EmojiMaterial = EmojiRenderer.material;
            ActionAreaSize = GameManager.Instance.ActionArea.GetComponent<Renderer>().bounds.size.z;
            
            // starting state for the state machine
            SwitchState(PreState);
        
            EventManager.OnEmotionDetected += OnEmotionDetectedCallback;
        }

        private void OnDisable()
        {
            EventManager.OnEmotionDetected -= OnEmotionDetectedCallback;
        }

        private void Update()
        {
            _emojiState.Update(this);
            transform.position -= GameManager.Instance.ActionArea.transform.forward * (GameManager.Instance.Level.EmojiMovementSpeed * Time.deltaTime);
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

        private IEnumerator DeactivateEmoji(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            
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

        public void FadeOut()
        {
            StartCoroutine(MathHelper.SLerp(0, 1, 1, EmojiRenderer.material, DissolveAmount));
            StartCoroutine(DeactivateEmoji(1));
        }

    }
}
