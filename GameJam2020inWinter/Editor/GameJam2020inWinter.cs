using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Syrus.Plugins.ChartEditor;
using UnityEngine;
using sarisarinyama.common;
#if UNITY_EDITOR
using UnityEditor;
using MySql.Data.MySqlClient;

#endif

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class GameJam2020inWinter : EditorWindow
    {
        private static string database = "Cluster";
        private static string table = "GameJam2020inWinter";
        private static string userTable = "GameJam2020inWinterUser";

        private static MySqlCommand _cmd = null;
        private static MySqlDataReader _rdr = null;

        private string _statusText = "ステータス:";
        string timePrefix = "終了まで:";

        static readonly float updatetime = 1.0f;
        static double _nextUpdate = 0;

        private LocalData _localData = null;
        private PublicData _publicData = null;
        private RankingData _rankingData = null;
        bool _isDisable = false;
        private Vector2[] userPoint = new Vector2[] {new Vector2(0f, 0f)};

//        private DateTime beginDt =new DateTime(2020,12,18,20,00,00,00);
//        private static DateTime _endDt =new DateTime(2020,12,20,20,00,00,00);
        private static DateTime _endDt = new DateTime(2020, 12, 16, 20, 00, 00, 00);
        private static DateTime _nowDt;
        private static string _endDtText = "";
        private static string _nowDtText = "";
        private static float _nowLine = 0f;
        private TimeSpan duration;
        private string durationText;
        private string durationTextDays = "";
        private string durationtextHours = "";
        private string durationTextMinutes = "";

        /// <summary>
        /// アセットパス
        /// </summary>
        public const string LOCALASSET_PATH = "Assets/ClusterTools/GameJam2020inWinter/Resources/LocalData.asset";

        public const string RANKINGASSET_PATH = "Assets/ClusterTools/GameJam2020inWinter/Resources/RankingData.asset";


        private void OnEnable()
        {
            _isDisable = false;
        }


        [MenuItem("Cluster/Game Jam 2020 in Winter")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<GameJam2020inWinter>("Game Jam 2020 in Winter Progress");

            window.minSize = new Vector2(480, 400);
            window.maxSize = new Vector2(480, 400);
            window.autoRepaintOnSceneChange = true;

            DontDestroyOnLoad(window);
            _endDtText = _endDt.ToString("終了:M月d日HH時");
        }

        private void Awake()
        {
            
            if (_localData == null)
            {
                _localData = ScriptableObject.CreateInstance<LocalData>();
            }
            LocalDataImport();
            
            if (_rankingData == null)
            {
                _rankingData = ScriptableObject.CreateInstance<RankingData>();
            }
            RankingDataImport();
            MakePublicData();
        }

        void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(_isDisable);
            ShowTime();

            WriteGraph();

            DataEdit();
            EditorGUI.EndDisabledGroup();
        }

        private void WriteGraph()
        {
            GUIChartEditor.BeginChart(480, 480, 260, 260, Color.black,
                GUIChartEditorOptions.ChartBounds(-3, 50, -10, 106),
                GUIChartEditorOptions.SetOrigin(ChartOrigins.BottomLeft),
                GUIChartEditorOptions.ShowAxes(Color.white),
                GUIChartEditorOptions.ShowGrid(4, 20, new Color(0.2f, 0.2f, 0.2f, 1f), false)
                , GUIChartEditorOptions.ShowLabels("#",
                    20, -1, 22,
                    40, -1, 42,
                    60, -1, 62,
                    80, -1, 82,
                    100, -1, 102,
                    18, 1, -3,
                    19, 5, -3,
                    20, 29, -3,
                    20, 1.5f, -7,
                    24, 5.2f, -7,
                    4, 9.2f, -7,
                    8, 13.2f, -7,
                    12, 17.2f, -7,
                    16, 21.2f, -7,
                    20, 25.2f, -7,
                    24, 29.2f, -7,
                    4, 33.2f, -7,
                    8, 37.2f, -7,
                    12, 41.2f, -7,
                    16, 45.2f, -7,
                    20, 49.2f, -7
                )
            );

            Vector2[] frame1 = new Vector2[]
            {
                new Vector2(0, 100f), new Vector2(48, 100), new Vector2(48, 0)
            };
            GUIChartEditor.PushLineChart(frame1, Color.white);

            Vector2[] frame2 = new Vector2[]
            {
                new Vector2(4f, -10f), new Vector2(4f, 100)
            };
            GUIChartEditor.PushLineChart(frame2, Color.white);
            Vector2[] frame4 = new Vector2[]
            {
                new Vector2(28f, -10f), new Vector2(28f, 100)
            };
            GUIChartEditor.PushLineChart(frame4, Color.white);
            Vector2[] frame5 = new Vector2[]
            {
                new Vector2(48f, -10f), new Vector2(48f, 100)
            };
            GUIChartEditor.PushLineChart(frame5, Color.white);

            Vector2[] frame3 = new Vector2[]
            {
                new Vector2(0f, 50), new Vector2(48, 50)
            };
            GUIChartEditor.PushLineChart(frame3, Color.white);

            Vector2[] nowDtLine = new Vector2[]
            {
                new Vector2(_nowLine, -10f), new Vector2(_nowLine, 0f)
            };
            GUIChartEditor.PushLineChart(nowDtLine, Color.red);


            DrawPublicPoint();

            GUIChartEditor.PushLineChart(userPoint, Color.red);


            GUIChartEditor.EndChart();
        }

        private void MakePublicData()
        {
            _publicData = new PublicData();

            MySQLUtil.DBConnection();
            string _selectPublicData = $"SELECT Name,UpdateTime,Progress " +
                                       $"FROM {database}.{table} " +
                                       $"ORDER BY Name ASC,UpdateTime ASC,Progress ASC;";
            string[] _parameters = { };
            DataTable tbl = MySQLUtil.DBSelect(_selectPublicData, _parameters);


            PublicDataStruct[] publicDataStructs = new PublicDataStruct[] { };
            foreach (DataRow row in tbl.Rows)
            {
                PublicDataStruct publicDataStruct = new PublicDataStruct();
                publicDataStruct._teamName = row["Name"].ToString();
                publicDataStruct._progress = (int) row["Progress"];
                publicDataStruct._dateTime = (DateTime) row["UpdateTime"];

                if (_publicData.publicDatas.ContainsKey(row["Name"].ToString()))
                {
                    Array.Resize(ref publicDataStructs, publicDataStructs.Length + 1);
                    publicDataStructs[publicDataStructs.Length - 1] = publicDataStruct;
                }
                else
                {
                    //length1
                    publicDataStructs = new PublicDataStruct[] {publicDataStruct};
                }


                if (_publicData.publicDatas.ContainsKey(row["Name"].ToString()))
                {
                    _publicData.publicDatas.Remove(row["Name"].ToString());
                }

                _publicData.publicDatas.Add(row["Name"].ToString(), publicDataStructs);
            }

            if (_localData != null)
            {
                MakeUserPoint();
            }

            MySQLUtil.DBClose();
        }


        private void ShowTime()
        {
            using (var horizontalArea = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(_nowDtText);
                GUILayout.Label(_endDtText);
                GUILayout.Label(durationText);
            }
        }

        private void ChangeTime()
        {
            _nowDt = System.DateTime.Now;
            _nowDtText = _nowDt.ToString("現在:M月d日HH時mm分");

            duration = _endDt - _nowDt;
            if ((int) duration.Days > 1)
            {
                durationTextDays = (int) duration.Days + "日と";
            }
            else
            {
                durationTextDays = "";
            }

            if ((int) duration.Hours > 1)
            {
                durationtextHours = (int) duration.Hours + "時間";
            }
            else
            {
                durationtextHours = "";
            }

            if ((int) duration.Minutes > 1)
            {
                durationTextMinutes = duration.Minutes.ToString("D2") + "分";
            }
            else
            {
                durationTextMinutes = "";
            }

            durationText = timePrefix + durationTextDays
                                      + durationtextHours
                                      + durationTextMinutes
                                      + duration.Seconds.ToString("D2") + "秒"
                ;

            _nowLine = 48f - ((float) duration.TotalMinutes / 60);
        }

        private void DataEdit()
        {
            if (_localData == null)
            {
                _localData = ScriptableObject.CreateInstance<LocalData>();
            }

//            LocalDataImport();
            using (var horizontalArea = new EditorGUILayout.HorizontalScope())
            {
                using (var verticalArea = new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label("ユーザ名:");

                    using (var horizontalArea5 = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label("進捗率:", GUILayout.Height(22));
                        GUILayout.Box(_localData.Progress.ToString().PadLeft(3, ' ') + "%");
                    }

                    GUILayout.Label("コメント:");
                    GUILayout.Label("ワールド名:");
                    GUILayout.Label("ワールドURL:");
                    GUILayout.Label("データ登録:");
                }

                using (var verticalArea = new EditorGUILayout.VerticalScope())
                {
                    using (var horizontalArea4 = new EditorGUILayout.HorizontalScope())
                    {
                        _localData.TeamName = EditorGUILayout.TextField("", _localData.TeamName);
                        GUILayout.Label("　　　　　　　");
                    }

                    using (var horizontalArea2 = new EditorGUILayout.HorizontalScope())
                    {
                        using (var horizontalArea3 = new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("　 + 5% 　"))
                            {
                                ChangeProgress(5);
                            }

                            if (GUILayout.Button("　+ 10%   "))
                            {
                                ChangeProgress(10);
                            }

                            GUILayout.Space(3.0f);
                            if (GUILayout.Button(" 　- 5%  　"))
                            {
                                ChangeProgress(-5);
                            }
                        }

                        GUILayout.Label("");
                        GUILayout.Label("");
                    }

                    _localData.Comment = EditorGUILayout.TextField("", _localData.Comment);
                    _localData.Title = EditorGUILayout.TextField("", _localData.Title);
                    _localData.URL = EditorGUILayout.TextField("", _localData.URL);
                    using (var horizontalArea2 = new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("登録"))
                        {
                            UserDataUpdate();
                        }

                        GUILayout.Label("");
                        GUILayout.Label("");
                    }
                }

                using (var verticalArea = new EditorGUILayout.VerticalScope())
                {
                    if (GUILayout.Button("ランキング"))
                    {
                        OpenListWindow();
//                        RankingDataImport();
                    }

                    GUILayout.Label("");

                    GUILayout.Label("任意入力");
                    GUILayout.Label("任意入力");
                    GUILayout.Label("任意入力");
                }
            }
        }

        private bool UserDataUpdate()
        {
            LocalDataExport();
            if (_localData.TeamName == "")
            {
                EditorUtility.DisplayDialog("データ登録エラー", "ユーザ名を入力してください", "OK");
                return false;
            }

            if (_localData.Comment.Length > 24)
            {
                EditorUtility.DisplayDialog("データ登録エラー", "ごめんなさい\r\nコメントは24文字までしか登録できません", "OK");
                return false;
            }

            // 1. exist local pasward?
            if (_localData.Password == "")
            {
                _isDisable = true;
                OpenPasswordWindow(x =>
                {
                    if (string.IsNullOrEmpty(x) == false)
                    {
                        _localData.Password = x;
                    }

                    _isDisable = false;
                });
            }

            if (_localData.Password != "")
            {
                if (CheckPassword())
                {
                    string _insertSql = $"INSERT INTO {table} (Name, Comment,URL, UpdateTime, Title,Progress) " +
                                        "VALUE( @Name " +
                                        ", @Comment " +
                                        ", @URL " +
                                        ", now() " +
                                        ", @Title " +
                                        ", @Progress );";

                    string[] _parameters = new string[]
                    {
                        "Name", _localData.TeamName,
                        "Comment", _localData.Comment,
                        "Progress", _localData.Progress.ToString(),
                        "Title", _localData.Title,
                        "URL", _localData.URL
                    };
                    MySQLUtil.DBInsert(_insertSql, _parameters);

                    MakeUserPoint();
                    MySQLUtil.DBClose();
                }
            }


            MySQLUtil.DBClose();
            return true;
        }

        private void MakeUserPoint()
        {
            string _selectUserProgress = $"SELECT UpdateTime,Progress " +
                                         $"FROM {database}.{table} " +
                                         $"WHERE Name=@Name " +
                                         $"ORDER BY UpdateTime ASC;";

            string[] _parameters = new string[] {"Name", _localData.TeamName};

            DataTable tbl = MySQLUtil.DBSelect(_selectUserProgress, _parameters);
            userPoint = new Vector2[] {new Vector2(0f, 0f)};
            foreach (DataRow row in tbl.Rows)
            {
                Array.Resize(ref userPoint, userPoint.Length + 1);
                float x = 48f - ((float) (_endDt - ((DateTime) row[0])).TotalMinutes / 60f);
                float y = float.Parse(row[1].ToString());
                userPoint[userPoint.Length - 1] = new Vector2(x, y);
//                        Debug.Log(((_endDt-((DateTime)row[0])).TotalMinutes)/60);
            }
        }


        private void DrawPublicPoint()
        {
            if (_rankingData != null)
            {
                Vector2[] publicPoint;
                foreach (KeyValuePair<string, PublicDataStruct[]> dic in _publicData.publicDatas)
                {
                    if (_rankingData.rankingData.ContainsKey(dic.Key)&& _rankingData.rankingData[dic.Key]._show)
                    {
                        publicPoint = new Vector2[] {new Vector2(0f, 0f)};
                        foreach (PublicDataStruct data in dic.Value)
                        {
                            Array.Resize(ref publicPoint, publicPoint.Length + 1);
                            float x = 48f - ((float) (_endDt - ((DateTime) data._dateTime)).TotalMinutes / 60f);
                            float y = (float) data._progress;
                            publicPoint[publicPoint.Length - 1] = new Vector2(x, y);
//                        Debug.Log(((_endDt-((DateTime)row[0])).TotalMinutes)/60);
                        }

                            GUIChartEditor.PushLineChart(publicPoint, _rankingData.rankingData[dic.Key]._color);
                    }
                }
            }
            else
            {
                if (_rankingData == null)
                {
                    _rankingData = ScriptableObject.CreateInstance<RankingData>();
                }
                RankingDataImport();
            }
        }


        // 3+4+5+6+7 DBに接続してパスワードチェック
        private bool CheckPassword()
        {
            MySQLUtil.DBConnection();
            string _selectSql = $"SELECT Password FROM  {database}.{userTable} WHERE Name=@Name ;";
            string[] _parameters = new string[] {"Name", _localData.TeamName};

            DataTable tbl = MySQLUtil.DBSelect(_selectSql, _parameters);
            // 3.exist data?
            if (tbl.Rows.Count == 1)
            {
                // yes
                foreach (DataRow row in tbl.Rows)
                {
                    if (_localData.Password == row[0].ToString())
                    {
                        // 7.check ok
                        return true;
                    }
                    else
                    {
                        // 5. local data delete
                        MySQLUtil.DBClose();
                        _localData.Password = "";
                        LocalDataExport();
                        Debug.Log("password error");
                        return false;
                    }
                }
            }
            else
            {
                // no 6. db insert
                string _insertSql = $"insert into {database}.{userTable} (Name ,Password) " +
                                    " value ( @Name , @Password );";
                _parameters = new string[]
                {
                    "Name", _localData.TeamName,
                    "Password", _localData.Password
                };
                MySQLUtil.DBInsert(_insertSql, _parameters);
                return true;
            }

            return false;
        }

        private void OpenPasswordWindow(System.Action<string> callback)
        {
            var passwordWindow = EditorWindow.GetWindow<PasswordWindow>("UserPassword", true);
            passwordWindow.Setup(callback);
            passwordWindow.minSize = new Vector2(480, 400);
            passwordWindow.maxSize = new Vector2(480, 400);
        }

        private void OpenListWindow()
        {
            var rankingWindow = EditorWindow.GetWindow<RankingWindow>("ProgressList", true);
//            rankingWindow.Setup(callback);

            rankingWindow.minSize = new Vector2(480, 480);
        }


        private void ChangeProgress(int value)
        {
            _localData.Progress += value;
            if (_localData.Progress > 100)
            {
                _localData.Progress = 100;
            }

            if (_localData.Progress < 0)
            {
                _localData.Progress = 0;
            }
        }


        void OnDestroy()
        {
            LocalDataExport();
            MySQLUtil.DBClose();
        }

        void OnInspectorUpdate()
        {
            if (_nextUpdate + updatetime < EditorApplication.timeSinceStartup)
            {
                Repaint();
                ChangeTime();
                _nextUpdate = EditorApplication.timeSinceStartup;
            }
        }

        private void LocalDataExport()
        {
            // 新規の場合は作成
            if (!AssetDatabase.Contains(_localData as UnityEngine.Object))
            {
                string directory = Path.GetDirectoryName(LOCALASSET_PATH);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // アセット作成
                AssetDatabase.CreateAsset(_localData, LOCALASSET_PATH);
            }

            // インスペクターから設定できないようにする
            _localData.hideFlags = HideFlags.NotEditable;
            // 更新通知
            EditorUtility.SetDirty(_localData);
            // 保存
            AssetDatabase.SaveAssets();
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        }

        private void LocalDataImport()
        {
            LocalData localData = AssetDatabase.LoadAssetAtPath<LocalData>(LOCALASSET_PATH);
            if (localData == null)
                return;
            _localData = localData;
        }

        private void RankingDataExport()
        {
            // 新規の場合は作成
            if (!AssetDatabase.Contains(_rankingData as UnityEngine.Object))
            {
                string directory = Path.GetDirectoryName(GameJam2020inWinter.RANKINGASSET_PATH);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // アセット作成
                AssetDatabase.CreateAsset(_rankingData, GameJam2020inWinter.RANKINGASSET_PATH);
            }

            // インスペクターから設定できないようにする
            _rankingData.hideFlags = HideFlags.NotEditable;
            // 更新通知
            EditorUtility.SetDirty(_rankingData);
            // 保存
            AssetDatabase.SaveAssets();
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        }

        private void RankingDataImport()
        {
            RankingData rankingData = AssetDatabase.LoadAssetAtPath<RankingData>(GameJam2020inWinter.RANKINGASSET_PATH);
            if (rankingData == null)
                return;
            _rankingData = rankingData;
        }

    }
}