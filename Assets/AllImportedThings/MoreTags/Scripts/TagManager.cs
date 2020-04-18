using System;
using UnityEngine;

namespace MoreTags
{
    [Serializable]
    public class TagData
    {
        public string name;
        public Color color;
        public GameObject[] gameObjects;
    }

    [ExecuteInEditMode]
    [AddComponentMenu("MoreTags/TagManager", 1)]
    public class TagManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private TagData[] m_tags = new TagData[] { };

#if UNITY_EDITOR
        private void CheckSingle()
        {
            foreach (var comp in FindObjectsOfType<TagManager>())
                if (comp != this) DestroyImmediate(this);
        }

        private void Reset()
        {
            Invoke("CheckSingle", 0);
            var tm = FindObjectsOfType<TagManager>();
            if (tm.Length == 0 || (tm.Length == 1 && tm[0] == this))
                TagSystem.Reset();
        }
#endif
        private void Awake()
        {
            var tm = FindObjectsOfType<TagManager>();
            if (tm.Length == 0 || (tm.Length == 1 && tm[0] == this))
                TagSystem.Reset();
            TagSystem.LoadDataToTable(m_tags);
        }

        public void OnAfterDeserialize()
        {
            TagSystem.LoadDataToTable(m_tags);
        }

        public void OnBeforeSerialize()
        {
            if (gameObject == null) return;
            TagSystem.BeforeSerialize(ref m_tags, gameObject.scene);
        }
    }
}