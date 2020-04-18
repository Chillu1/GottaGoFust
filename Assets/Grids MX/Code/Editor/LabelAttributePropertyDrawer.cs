using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		[CustomPropertyDrawer(typeof(LabelAttribute))]
		public class LabelAttributePropertyDrawer : PropertyDrawer
		{
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				LabelAttribute labelAttrib = this.attribute as LabelAttribute;
				EditorGUI.PropertyField(position, property, new GUIContent(labelAttrib.label));
			}
		}
	}
}