using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class PublicData : ScriptableObject
    {
        [SerializeField] [CanBeNull]
        public Dictionary<string, PublicDataStruct[]> publicDatas = new Dictionary<string, PublicDataStruct[]>();
    }
}