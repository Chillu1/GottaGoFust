using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mx
{
	namespace Grids
	{
		[System.Serializable]
		public class Keybinding
		{
			private const string OPEN_PAREN = "(";
			private const string CLOSE_PAREN = ")";
			private const string SHIFT_LABEL = "SHIFT+";
			private const string ALT_LABEL = "ALT+";
			private const string CTRL_LABEL = "CTRL+";

			public string label = string.Empty;
			public bool enabled = false;
			public bool control = false;
			public bool alt = false;
			public bool shift = false;
			public KeyCode key = KeyCode.A;

			[HideInInspector]
			public string id = string.Empty;

			public Callback callback = null;
			public delegate void Callback();

			public Keybinding(string label, bool enabled, bool control, bool alt, bool shift, KeyCode key, string id)
			{
				this.label = label;
				this.enabled = enabled;
				this.control = control;
				this.alt = alt;
				this.shift = shift;
				this.key = key;
				this.id = id;
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				if (enabled)
				{
					sb.Append(OPEN_PAREN);
					if (control)
					{
						sb.Append(CTRL_LABEL);
                    }
					if (alt)
					{
						sb.Append(ALT_LABEL);
					}
					if (shift)
					{
						sb.Append(SHIFT_LABEL);
					}
					sb.Append(key.ToString());
					sb.Append(CLOSE_PAREN);
				}

				return sb.ToString();
            }
		}
	}
}