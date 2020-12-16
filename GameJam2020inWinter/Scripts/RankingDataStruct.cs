using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class RankingDataStruct
    {
        [SerializeField] public bool _show = false;
        [SerializeField] public Color _color = Color.gray;
        [SerializeField] public int _progress = 0;
        [SerializeField] public DateTime _dateTime;
        [SerializeField] public string _url = "";
        [SerializeField] public string _title = "";
        [SerializeField] public string _comment = "";
    }
}