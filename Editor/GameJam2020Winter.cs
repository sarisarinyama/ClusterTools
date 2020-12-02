using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;

public class GameJam2020Winter : EditorWindow
{


    private string host = "sarisarinyama.ddns.net";
    private string database = "Cluster";
    private string user = "guest";
    private string password = "guest";
    private bool pooling = true;
    private string connectionString;


    [MenuItem("Cluster/Game Jam 2020 Winter")]
     static void Init()
    {
        var window = EditorWindow.GetWindow<GameJam2020Winter>("Game Jam 2020 Winter");
//        DontDestroyOnLoad(window);
        window.Show();
    }


        private MySqlConnection con = null;
        private MySqlCommand cmd = null;
        private MySqlDataReader rdr = null;

        void OnGUI()
        {
//        string progressTitle = "";
//        string progressInfo = "";
//        float progress = 0f;
//
//        progressTitle = "データベース接続状況";
//        progressInfo = "connection open";
//        EditorUtility.DisplayProgressBar(progressTitle, progressInfo, progress);

            Debug.Log("db connecting");
            connectionString = $"Server={host};Database={database};User={user};Password={password};Pooling=";
            if (pooling)
            {
                connectionString += "True";
            }
            else
            {
                connectionString += "False";
            }

            try
            {
                con = new MySqlConnection(connectionString);
                con.Open();
                Debug.Log("connection open");

            }
            catch (Exception e)
            {
                Debug.Log("db connection error");
                con.Dispose ();
                Debug.Log(e);
                this.Close();
            }

        }

        void OnDestroy(){
		if (con != null) {
			if (con.State.ToString () != "Closed") {
				con.Close ();
				Debug.Log ("db connection closed");
			}
			con.Dispose ();
		}
	
        
    } 

}
