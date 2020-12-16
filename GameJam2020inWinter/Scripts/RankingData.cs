using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class RankingData : ScriptableObject
    {
        [SerializeField] [CanBeNull]
        public Dictionary<string, RankingDataStruct> rankingData = new Dictionary<string, RankingDataStruct>();
    }
}