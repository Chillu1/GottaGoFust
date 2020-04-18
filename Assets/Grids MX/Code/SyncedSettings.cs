using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class SyncedSettings : ScriptableObject
		{
			[HideInInspector][SerializeField][NonSynced]
			private int m_userVersion = 0;

			public int userVersion { get { return m_userVersion; } }

			public string GetUserVersionPropertyName()
			{
				return "m_userVersion";
			}
        }
	}
}