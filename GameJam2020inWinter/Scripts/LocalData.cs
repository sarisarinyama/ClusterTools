using System;
using UnityEngine;

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    [Serializable]
    public class LocalData : ScriptableObject
    {
        [SerializeField] private int _progress = 0;
        [SerializeField] private string _teamName = "";
        [SerializeField] private string _comment = "";
        [SerializeField] private string _url = "";
        [SerializeField] public string _password = "";
        [SerializeField] private string _title = "";

        public int Progress
        {
            get => _progress;
            set => _progress = value;
        }

        public string URL
        {
            get => _url;
            set => _url = value;
        }

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string TeamName
        {
            get => _teamName;
            set => _teamName = value;
        }

        public string Comment
        {
            get => _comment;
            set => _comment = value;
        }
    }
}