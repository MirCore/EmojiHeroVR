#if UNITY_EDITOR

using UnityEngine.UI;

#endif

//https://forum.unity.com/threads/swap-from-horizontal-to-vertical-layout-group.485034/#post-8225898

// This is a simple class that combines HorizontalLayoutGroup and VerticalLayoutGroup,
// so we can switch between horizontal and vertical layout easily.

// From script we can use the "isVertical" bool property to control the axis.
// From inspector this bool is controlled by a more convenient "Layout Axis" enum.

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/HV Layout Group", 153)]
	public class HVLayoutGroup : HorizontalOrVerticalLayoutGroup
	{
		protected HVLayoutGroup() { }

		[SerializeField] protected bool m_IsVertical = true;
		public bool isVertical { get { return m_IsVertical; } set { SetProperty(ref m_IsVertical, value); } }

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalcAlongAxis(0, m_IsVertical);
		}

		public override void CalculateLayoutInputVertical()
		{
			CalcAlongAxis(1, m_IsVertical);
		}

		public override void SetLayoutHorizontal() => SetChildrenAlongAxis(0, m_IsVertical);
		public override void SetLayoutVertical() => SetChildrenAlongAxis(1, m_IsVertical);
		
		public void SetLayout(bool vertical)
		{
			isVertical = vertical;
			// Trigger layout update
			CalculateLayoutInputHorizontal();
			CalculateLayoutInputVertical();
			SetLayoutHorizontal();
			SetLayoutVertical();
		}
	}
}

#if UNITY_EDITOR

namespace UnityEditor.UI
{
	[CustomEditor(typeof(HVLayoutGroup), true)]
	[CanEditMultipleObjects]
	public class HVLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
	{
		private enum LayoutAxis { Horizontal, Vertical }

		private LayoutAxis m_LayoutAxis;
		private SerializedProperty m_IsVertical;

		protected override void OnEnable()
		{
			base.OnEnable();
			m_IsVertical = serializedObject.FindProperty("m_IsVertical");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//EditorGUILayout.PropertyField(m_IsVertical, true);

			m_LayoutAxis = m_IsVertical.boolValue ? LayoutAxis.Vertical : LayoutAxis.Horizontal;
			m_LayoutAxis = (LayoutAxis)EditorGUILayout.EnumPopup("Layout Axis", m_LayoutAxis);
			m_IsVertical.boolValue = m_LayoutAxis == LayoutAxis.Vertical ? true : false;

			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();
		}
	}
}

#endif