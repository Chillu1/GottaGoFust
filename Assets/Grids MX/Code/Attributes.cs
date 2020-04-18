using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class NonSyncedAttribute : System.Attribute { }

		public class LabelAttribute : PropertyAttribute
		{
			public string label { get; private set; }

			public LabelAttribute(string label)
			{
				this.label = label;
			}
		}
	}
}