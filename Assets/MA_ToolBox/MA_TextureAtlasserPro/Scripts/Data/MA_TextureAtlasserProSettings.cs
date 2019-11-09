#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;

namespace MA_TextureAtlasserPro
{
	[System.Serializable]
	public class MA_TextureAtlasserProSettings : ScriptableObject
	{
		[Header("Hotkeys:")]
		public bool useHotkeys = false;
		public EventModifiers modifierKey = EventModifiers.Alt;
		public KeyCode addQuadHotKey = KeyCode.Q;
		public KeyCode removeQuadHotKey = KeyCode.R;
		public KeyCode duplicateHotKey = KeyCode.D;
		public KeyCode zoomInHotKey = KeyCode.Equals;
		public KeyCode zoomOutHotKey = KeyCode.Minus;

		[Header("Duplication:")]
		public bool copySelectedQuadData = true;
		public string duplicatedQuadNamePrefix = "new ";

		[Header("Selection")]
		public bool autoFocus = true;

		public bool GetHotKey(Event e, KeyCode shortKey)
		{
			if (e.type == EventType.KeyDown && e.modifiers == modifierKey && e.keyCode == shortKey)
			{
				return true;
			}

			return false;
		}
	}
}
#endif