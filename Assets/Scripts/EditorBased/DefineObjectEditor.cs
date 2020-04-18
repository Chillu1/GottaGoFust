using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(DefineObject))]
public class DefineObjectEditor : Editor
{
    private bool isTrigger = false;
    private DefineObject defineObject = null;

    private string[] functions = new[] { "Move", "Rotate", "Activate", "Deactivate" };
    private int choiceIndex = 0;

    private void OnEnable()
    {
        defineObject = target as DefineObject;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Is Trigger", GUILayout.Width(145));
        isTrigger = EditorGUILayout.Toggle(isTrigger);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        if (isTrigger)
        {
            GUILayout.BeginHorizontal();
            //GUILayout.Label("Function", GUILayout.Width(145));
            //defineObject.function = EditorGUILayout.TextField(defineObject.function);//Text
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Function", GUILayout.Width(145));
            choiceIndex = EditorGUILayout.Popup(choiceIndex, functions);
            defineObject.function = functions[choiceIndex];
            GUILayout.EndHorizontal();

            if (defineObject.function == "Move")
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Is Local", GUILayout.Width(145));
                defineObject.isLocal = EditorGUILayout.Toggle(defineObject.isLocal);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ActivateSpeed();
                GUILayout.EndHorizontal();

                if (defineObject.isLocal)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("And This One", GUILayout.Width(145));
            defineObject.test = EditorGUILayout.TextField(defineObject.test);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //GUILayout.Label("And This Can Be Slider", GUILayout.Width(145));
            //defineObject.speed = EditorGUILayout.Slider(defineObject.speed, 0f, 100f);
            GUILayout.EndHorizontal();
        }

        /*
        defineObject.isTrigger = GUILayout.Toggle(defineObject.isTrigger, "Is Trigger");

        if (defineObject.isTrigger)
        {
            defineObject.index = EditorGUILayout.IntSlider("Index field: ", defineObject.index, 1, 100);
        }*/

        base.OnInspectorGUI();
    }

    private void ActivateSpeed()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", GUILayout.Width(145));
        defineObject.speed = EditorGUILayout.Slider(defineObject.speed, -5f, 5f);

        GUILayout.EndHorizontal();
    }

    private void ActivateLength()
    {
    }
}

#endif