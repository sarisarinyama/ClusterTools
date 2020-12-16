using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class PublicData  : ScriptableObject
    {
        [SerializeField]
        public Dictionary<string, PublicDataStruct[]> publicDatas = new Dictionary<string, PublicDataStruct[]>();
    }
}