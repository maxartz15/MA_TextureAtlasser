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
		[Header("GUI (requires reload):")]
		public MA_TextureAtlasserProGuiSettings editorGuiSettings = new MA_TextureAtlasserProGuiSettings();

		[Header("HotKeys:")]
		public bool useHotkeys = false;
		public EventModifiers modifierKey = EventModifiers.Alt;
		public KeyCode addQuadHotKey = KeyCode.Q;
		public KeyCode removeQuadHotKey = KeyCode.R;
		public KeyCode duplicateHotKey = KeyCode.D;
		public KeyCode zoomInHotKey = KeyCode.Equals;
		public KeyCode zoomOutHotKey = KeyCode.Minus;

		[Header("Duplication:")]
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

	[System.Serializable]
    public class MA_TextureAtlasserProGuiSettings
    {
        public MA_EditorGuiMode editorGuiMode = MA_EditorGuiMode.IconAndText;
        public bool enableToolTips = true;
    }

    public enum MA_EditorGuiMode
    {
        IconAndText = 0,
        Icon = 1,
        Text = 2
    }
}
#endif