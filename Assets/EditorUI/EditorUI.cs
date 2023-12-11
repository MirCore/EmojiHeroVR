#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace EditorUI
{
    public class EditorUI : EditorWindow
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset;
        private static VisualElement _root;


        [MenuItem("Window/EmojiHero Editor Window")]
        public static void ShowExample()
        {
            EditorUI wnd = GetWindow<EditorUI>();
            wnd.titleContent = new GUIContent("EmojiHero");
        }

        public static EditorUI Instance;

        private void OnEnable()
        {
            Instance = this;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            _root = rootVisualElement;

            // Instantiate UXML
            VisualElement visualElement = VisualTreeAsset.Instantiate();
            _root.Add(visualElement);

            if (EditorUIFerStats.Instance == null)
                return;
            
            SerializedObject ferStats = new(EditorUIFerStats.Instance);
            _root.Q<Label>("PendingRestResponses").BindProperty(ferStats.FindProperty("CurrentActiveRestPosts"));
            _root.Q<Label>("PostsFPS").BindProperty(ferStats.FindProperty("CurrentPostsFPS"));
            _root.Q<Label>("TotalRestCalls").BindProperty(ferStats.FindProperty("TotalPosts"));
            _root.Q<Label>("SnapshotFPS").BindProperty(ferStats.FindProperty("SnapshotFPS"));
        }

        public static void SetFerResponseData(Probabilities probabilities)
        {
            _root.Q<ProgressBar>("Anger").value = probabilities.anger;
            _root.Q<ProgressBar>("Disgust").value = probabilities.disgust;
            _root.Q<ProgressBar>("Fear").value = probabilities.fear;
            _root.Q<ProgressBar>("Happiness").value = probabilities.happiness;
            _root.Q<ProgressBar>("Neutral").value = probabilities.neutral;
            _root.Q<ProgressBar>("Sadness").value = probabilities.sadness;
            _root.Q<ProgressBar>("Surprise").value = probabilities.surprise;
        }
    }
}
#endif
