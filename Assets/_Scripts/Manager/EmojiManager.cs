using System.Collections;
using Enums;
using States.Emojis;
using TMPro;
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
        [SerializeField] internal TMP_Text EmoteTitle;
            
        internal Material EmojiMaterial;
        internal readonly int Sprite = Shader.PropertyToID("_Sprite");
        internal readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        internal readonly int FailedColorAmount = Shader.PropertyToID("_FailedColorAmount");
        internal readonly int SuccessColorAmount = Shader.PropertyToID("_SuccessColorAmount");
        internal float ActiveAreaLeft;
        internal float ActionAreaSize;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.rotation = new Quaternion();
            EmojiMaterial = EmojiRenderer.material;
            ActionAreaSize = GameManager.Instance.ActionArea.GetComponent<Renderer>().bounds.size.z;
            
            // starting state for the state machine
            SwitchState(PreState);
        
            EventManager.OnEmotionDetected += OnEmotionDetectedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
        }

        private void OnDisable()
        {
            EventManager.OnEmotionDetected -= OnEmotionDetectedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
        }


        private void Update()
        {
            _emojiState.Update(this);
            if (_emojiState != LeavingState)
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
        
        private void OnLevelStoppedCallback()
        {
            FadeOut();
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
            if (GameManager.Instance.Level.LevelMode == ELevelMode.Training)
            {
                StartCoroutine(MathHelper.SLerp(0, 1, 1f, EmojiRenderer.material, DissolveAmount));
                StartCoroutine(DeactivateEmoji(1));
                return;
            }
                
            _rigidbody.isKinematic = false;
            _rigidbody.velocity =
                -(GameManager.Instance.ActionArea.transform.forward * GameManager.Instance.Level.EmojiMovementSpeed) +
                GameManager.Instance.ActionArea.transform.right * Random.Range(-0.1f, 0.1f);
            StartCoroutine(MathHelper.SLerp(0, 1, 6f, EmojiRenderer.material, DissolveAmount));
            StartCoroutine(DeactivateEmoji(6));
        }

    }
}
