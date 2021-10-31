#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MA_TextureAtlasserPro
{
    [System.Serializable]
    public class MA_ModelGroup
    {
        public string name = "Model"; // public variable
        public List<Mesh> meshes = new List<Mesh>();
    }
}
#endif
