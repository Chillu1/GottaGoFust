﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreTags
{
    public abstract class TagPattern
    {
        public abstract GameObject GameObject();
        public abstract GameObject[] GameObjects();
        public abstract int Count();
        public abstract TagPattern And();
        public abstract TagPattern Or();
        public abstract TagPattern Exclude();
        public abstract TagPattern And(TagPattern pattern);
        public abstract TagPattern Or(TagPattern pattern);
        public abstract TagPattern Exclude(TagPattern pattern);
        public abstract TagPattern Combine(TagPattern pattern);
        public abstract TagPattern All();
        public abstract TagPattern With(string tag);
        public abstract TagPattern Both(params string[] tags);
        public abstract TagPattern Either(params string[] tags);

        public static implicit operator TagPattern(string pattern)
        {
            return TagHelper.StringToPattern(pattern);
        }

        public static TagPattern operator &(TagPattern a, TagPattern b)
        {
            return a.And(b);
        }

        public static TagPattern operator |(TagPattern a, TagPattern b)
        {
            return a.Or(b);
        }

        public static TagPattern operator -(TagPattern a, TagPattern b)
        {
            return a.Exclude(b);
        }

        public static TagPattern operator -(TagPattern a)
        {
            return TagSystem.pattern.All().Exclude(a);
        }

        public static TagPattern operator &(TagName a, TagPattern b)
        {
            return (TagPattern)a & b;
        }

        public static TagPattern operator |(TagName a, TagPattern b)
        {
            return (TagPattern)a | b;
        }

        public static TagPattern operator -(TagName a, TagPattern b)
        {
            return (TagPattern)a - b;
        }
    }

    public class TagTableData
    {
        public Color color = Color.white;
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();
    }

    public static class TagSystem
    {
        public static TagPattern pattern { get { return new TagPatternImpl(); } }

        private static TagManager s_TagManager;
        private static Dictionary<string, TagTableData> s_TagTable = new Dictionary<string, TagTableData>();
        private static GameObject[] s_SearchFrom = null;

        public static void Reset()
        {
            s_TagTable.Clear();
        }

        public static void BeforeSerialize(ref TagData[] m_tags, Scene scene)
        {
            m_tags = s_TagTable.Select((kv) => new TagData() { name = kv.Key, color = kv.Value.color, gameObjects = kv.Value.gameObjects.Where(go => go != null && go.scene == scene).ToArray() }).ToArray();
        }

        public static void LoadDataToTable(TagData[] m_tags)
        {
            foreach (var data in m_tags)
            {
                var list = data.gameObjects;
                if (s_TagTable.ContainsKey(data.name))
                    list = s_TagTable[data.name].gameObjects.Union(list).ToArray();
                else
                    s_TagTable[data.name] = new TagTableData();
                s_TagTable[data.name].color = data.color;
                s_TagTable[data.name].gameObjects = new HashSet<GameObject>(list);
            }
        }

        public static void CheckTagManager(Scene scene)
        {
            if (s_TagManager != null) return;
            if (!scene.isLoaded) return;
            var manager = Object.FindObjectsOfType<TagManager>().Where(tm => tm.gameObject.scene == scene);
            s_TagManager = manager.FirstOrDefault();
            if (s_TagManager == null)
            {
                var go = new GameObject("Tag Manager");
                SceneManager.MoveGameObjectToScene(go, scene);
                go.AddComponent<TagManager>();
            }
        }

        public static void CheckGameObjectTag(GameObject go)
        {
            var tags = go.GetComponent<Tags>();
            if (tags == null)
                go.AddComponent<Tags>();
        }

        public static void RemoveUnusedTag()
        {
            RemoveNullGameObject();
            var remove = s_TagTable.Where(kv => !s_TagTable[kv.Key].gameObjects.Any()).Select(kv => kv.Key).ToArray();
            foreach (var key in remove)
                s_TagTable.Remove(key);
        }

        public static void RemoveNullGameObject()
        {
            foreach (var data in s_TagTable)
                data.Value.gameObjects.RemoveWhere(go => go == null);
        }

        public static void SearchFrom(IEnumerable<GameObject> list = null)
        {
            s_SearchFrom = list == null ? null : list.ToArray();
        }

        public static void AddTag(params string[] tags)
        {
            foreach (var tag in tags)
                if (!s_TagTable.ContainsKey(tag))
                    s_TagTable.Add(tag, new TagTableData());
        }

        public static void RemoveTag(params string[] tags)
        {
            foreach (var tag in tags)
                if (s_TagTable.ContainsKey(tag))
                    s_TagTable.Remove(tag);
        }

        public static void RenameTag(string old, string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!s_TagTable.ContainsKey(old)) return;
            if (s_TagTable.ContainsKey(tag)) return;
            s_TagTable[tag] = s_TagTable[old];
            s_TagTable.Remove(old);
        }

        public static void SetTagColor(string tag, Color col)
        {
            s_TagTable[tag].color = col;
        }

        public static Color GetTagColor(string tag)
        {
            return s_TagTable[tag].color;
        }

        public static void AddGameObjectTag(GameObject go, params string[] tags)
        {
            CheckTagManager(go.scene);
            AddTag(tags);
            foreach (var tag in tags)
                s_TagTable[tag].gameObjects.Add(go);
            CheckGameObjectTag(go);
        }

        public static void RemoveGameObjectTag(GameObject go, params string[] tags)
        {
            CheckTagManager(go.scene);
            foreach (var tag in tags)
                if (s_TagTable.ContainsKey(tag))
                    s_TagTable[tag].gameObjects.Remove(go);
            CheckGameObjectTag(go);
        }

        public static string[] GetGameObjectTags(GameObject go)
        {
            return s_TagTable.Where(kv => kv.Value.gameObjects.Contains(go)).Select(kv => kv.Key).ToArray();
        }

        public static string[] GetAllTags()
        {
            return s_TagTable.Keys.ToArray();
        }

        public static GameObject GetGameObject(string tag)
        {
            return pattern.With(tag).GameObject();
        }

        public static GameObject[] GetGameObjects(string tag)
        {
            return pattern.With(tag).GameObjects();
        }

        public static IEnumerable<string> GameObjectTags(GameObject go)
        {
            return s_TagTable.Where(kv => kv.Value.gameObjects.Contains(go)).Select(kv => kv.Key);
        }

        public static IEnumerable<string> AllTags()
        {
            return s_TagTable.Keys.AsEnumerable();
        }

        #region TagPattern
        private class TagPatternImpl : TagPattern
        {
            private enum Mode { And, Or, Exclude }

            private Mode m_Mode = Mode.And;
            private HashSet<GameObject> m_List = null;

            private HashSet<GameObject> AllGameObject()
            {
                var e = new HashSet<GameObject>(Object.FindObjectsOfType<GameObject>().Where(go => go.scene.isLoaded));
                return s_SearchFrom == null ? e : e.And(s_SearchFrom);
            }

            private HashSet<GameObject> Empty()
            {
                return new HashSet<GameObject>();
            }

            public override GameObject GameObject()
            {
                return GameObjects().FirstOrDefault();
            }

            public override GameObject[] GameObjects()
            {
                var list = m_List == null ? Empty() : m_List;
                return list.ToArray();
            }

            public override int Count()
            {
                return m_List.Count;
            }

            public override TagPattern And()
            {
                m_Mode = Mode.And;
                return this;
            }

            public override TagPattern Or()
            {
                m_Mode = Mode.Or;
                return this;
            }

            public override TagPattern Exclude()
            {
                m_Mode = Mode.Exclude;
                return this;
            }

            public override TagPattern And(TagPattern pattern)
            {
                return Combine(pattern, Mode.And);
            }

            public override TagPattern Or(TagPattern pattern)
            {
                return Combine(pattern, Mode.Or);
            }

            public override TagPattern Exclude(TagPattern pattern)
            {
                return Combine(pattern, Mode.Exclude);
            }

            public override TagPattern Combine(TagPattern pattern)
            {
                return Combine(pattern, m_Mode);
            }

            private TagPattern Combine(TagPattern pattern, Mode mode)
            {
                var pat = pattern as TagPatternImpl;
                return Combine(pat.m_List, mode);
            }

            private TagPattern Combine(HashSet<GameObject> list)
            {
                return Combine(list, m_Mode);
            }

            private TagPattern Combine(HashSet<GameObject> list, Mode mode)
            {
                list = list == null ? Empty() : list;
                switch (mode)
                {
                    case Mode.And:
                        m_List = m_List == null ? list : m_List.And(list);
                        break;
                    case Mode.Or:
                        m_List = m_List == null ? list : m_List.Or(list);
                        break;
                    case Mode.Exclude:
                        m_List = m_List == null ? AllGameObject().Exclude(list) : m_List.Exclude(list);
                        break;
                }
                return this;
            }

            public override TagPattern All()
            {
                m_List = AllGameObject();
                return this;
            }

            public override TagPattern With(string tag)
            {
                return Combine(WithInternal(tag));
            }

            private HashSet<GameObject> WithInternal(string tag)
            {
                var e = string.IsNullOrEmpty(tag) || !s_TagTable.ContainsKey(tag) ? Empty() : s_TagTable[tag].gameObjects;
                return s_SearchFrom == null ? e : e.And(s_SearchFrom);
            }

            public override TagPattern Both(params string[] tags)
            {
                return Combine(BothInternal(tags));
            }

            private HashSet<GameObject> BothInternal(IEnumerable<string> tags)
            {
                if (!tags.Any()) return Empty();
                foreach (var tag in tags)
                    if (!s_TagTable.ContainsKey(tag))
                        return Empty();
                var e = WithInternal(tags.First());
                foreach (var tag in tags.Skip(1))
                    e = e.And(s_TagTable[tag].gameObjects, s_SearchFrom);
                return e;
            }

            public override TagPattern Either(params string[] tags)
            {
                return Combine(EitherInternal(tags));
            }

            private HashSet<GameObject> EitherInternal(IEnumerable<string> tags)
            {
                var list = new List<string>();
                foreach (var tag in tags)
                    if (s_TagTable.ContainsKey(tag))
                        list.Add(tag);
                if (list.Count == 0) return Empty();
                var e = WithInternal(list.First());
                foreach (var tag in list.Skip(1))
                    e = e.Or(s_TagTable[tag].gameObjects, s_SearchFrom);
                return e;
            }
        }
        #endregion

        #region HashSet Operation
        private static HashSet<T> And<T>(this HashSet<T> a, HashSet<T> b)
        {
            var less = a.Count < b.Count ? a : b;
            var more = a.Count >= b.Count ? a : b;
            var result = new HashSet<T>();
            foreach (var item in less)
                if (more.Contains(item))
                    result.Add(item);
            return result;
        }

        private static HashSet<T> Or<T>(this HashSet<T> a, HashSet<T> b)
        {
            var less = a.Count < b.Count ? a : b;
            var more = a.Count >= b.Count ? a : b;
            var result = new HashSet<T>(more);
            foreach (var item in less)
                result.Add(item);
            return result;
        }

        private static HashSet<T> Exclude<T>(this HashSet<T> a, HashSet<T> b)
        {
            var result = new HashSet<T>(a);
            foreach (var item in b)
                result.Remove(item);
            return result;
        }

        private static HashSet<T> And<T>(this HashSet<T> a, IEnumerable<T> b)
        {
            var result = new HashSet<T>();
            foreach (var item in b)
                if (a.Contains(item))
                    result.Add(item);
            return result;
        }

        private static HashSet<T> And<T>(this HashSet<T> a, HashSet<T> b, IEnumerable<T> c)
        {
            if (c == null) return a.And(b);
            var result = new HashSet<T>();
            foreach (var item in c)
                if (a.Contains(item) && b.Contains(item))
                    result.Add(item);
            return result;
        }

        private static HashSet<T> Or<T>(this HashSet<T> a, HashSet<T> b, IEnumerable<T> c)
        {
            if (c == null) return a.Or(b);
            var result = new HashSet<T>(a);
            foreach (var item in c)
                if (b.Contains(item))
                    result.Add(item);
            return result;
        }
        #endregion
    }
}