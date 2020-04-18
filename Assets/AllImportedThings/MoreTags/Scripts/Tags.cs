using UnityEngine;

namespace MoreTags
{
    [AddComponentMenu("MoreTags/Tags", 0)]
    public class Tags : MonoBehaviour
    {
#if UNITY_EDITOR
        void CheckSingle()
        {
            foreach (var comp in GetComponents<Tags>())
                if (comp != this) DestroyImmediate(this);
        }

        void Reset()
        {
            Invoke("CheckSingle", 0);
        }
#endif
    }
}