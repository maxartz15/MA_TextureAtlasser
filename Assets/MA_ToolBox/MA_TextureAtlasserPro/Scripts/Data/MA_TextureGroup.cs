#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MA_TextureAtlasserPro
{
	[System.Serializable]
	public class MA_TextureGroup 
	{
		public string name;
		public Texture texture = null;
	}

	[System.Serializable]
	public class MA_TextureGroupRegistration
	{
		public string name;
	}
}
#endif