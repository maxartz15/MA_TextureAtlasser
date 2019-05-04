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
		[Header("Selection")]
		public bool autoFocus = true;

		[Header("Duplication:")]
		public bool copySelectedQuadData = false;
		public string duplicatedQuadNamePrefix = "new ";

		[Header("Hotkeys:")]
		public bool useHotkeys = false;
		public KeyCode addQuadHotKey = KeyCode.Q;
		public KeyCode removeQuadHotKey = KeyCode.R;
		public KeyCode duplicateHotKey = KeyCode.D;
	}
}
#endif