using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using sarisarinyama.common;
using UnityEditor;
using UnityEngine;

namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class RankingWindow : EditorWindow
    {
        private long _height = 17;
        private int _rankingNumber = 0;

        private RankingData _rankingData = null;
        private LocalData _localData = null;

        private int _dataSize ;
        private Vector2 _scrollPos = Vector2.zero;

        void Awake()
        {
            try
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

                MySQLUtil.DBConnection();

                string _selectUserProgress =
                    "select tbl1.Name ,tbl1.UpdateTime,tbl1.Progress,tbl1.URL,tbl1.Comment,tbl1.Title " +
                    "FROM Cluster.GameJam2020inWinter as tbl1 " +
                    "INNER JOIN (	 " +
                    "   SELECT tbl2.Name,UpdateTime ,MAX(Progress) as MAXProgress " +
                    "FROM Cluster.GameJam2020inWinter " +
                    "   INNER JOIN ( " +
                    "SELECT Name, MAX(UpdateTime) as LastTime " +
                    "FROM Cluster.GameJam2020inWinter " +
                    "GROUP BY Name  " +
                    ")as tbl2 " +
                    "Where UpdateTime = tbl2.LastTime " +
                    "GROUP BY Name	 " +
                    ") as tbl3 " +
                    "where tbl1.UpdateTime = tbl3.UpdateTime and Progress =tbl3.MAXProgress AND tbl1.Name =tbl3.Name " +
                    "ORDER BY Progress DESC,tbl1.UpdateTime ASC "+
                    "; ";


                DataTable tbl = MySQLUtil.DBSelect(_selectUserProgress);

                foreach (DataRow row in tbl.Rows)
                {
                    if (_rankingData.rankingData.ContainsKey(row["Name"].ToString()))
                    {
                        _rankingData.rankingData[row["Name"].ToString()]._progress = (int) row["Progress"];
                        _rankingData.rankingData[row["Name"].ToString()]._comment = row["Comment"].ToString();
                        _rankingData.rankingData[row["Name"].ToString()]._url = row["URL"].ToString();
                        _rankingData.rankingData[row["Name"].ToString()]._title = row["Title"].ToString();
                        _rankingData.rankingData[row["Name"].ToString()]._dateTime = (DateTime) row["UPdateTime"];
                    }
                    else
                    {
                        RankingDataStruct _rankingDataStruct = new RankingDataStruct();
                        _rankingDataStruct._progress = (int) row["Progress"];
                        _rankingDataStruct._comment = row["Comment"].ToString();
                        _rankingDataStruct._url = row["URL"].ToString();
                        _rankingDataStruct._title = row["Title"].ToString();
                        _rankingDataStruct._dateTime = (DateTime) row["UPdateTime"];
                        _rankingData.rankingData.Add(row["Name"].ToString(), _rankingDataStruct);
                    }
                }

                MySQLUtil.DBClose();
            }
            catch (Exception e)
            {
                Debug.Log("error");
                Debug.Log(e);
                MySQLUtil.DBClose();
            }
        }

        void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                using (var HorizontallArea = new EditorGUILayout.HorizontalScope())
                {
                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("#");
                        _rankingNumber = 0;
                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUIStyle style = new GUIStyle();

                            if (_localData.TeamName != data.Key)
                            {
                                style.normal.textColor = data.Value._color;
                            }
                            else
                            {
                                style.normal.textColor = Color.red;
                            }

                            style.fontStyle = FontStyle.Bold;


                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            _rankingNumber++;
                            GUILayout.Label("  " + _rankingNumber.ToString(), style, GUILayout.Height(_height));
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("チーム名");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            GUILayout.Label(data.Key, GUILayout.Height(_height));
                        }
                    }


                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("進捗率");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            GUILayout.Label(data.Value._progress.ToString() + "%", GUILayout.Height(_height));
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("コメント");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            GUILayout.Label(data.Value._comment.ToString(), GUILayout.Height(_height));
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("ワールド名");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            GUILayout.Label(data.Value._title.ToString(), GUILayout.Height(_height));
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        {
                            GUILayout.Label("ワールドURL");

                            foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                            {
                                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                                if (data.Value._url.Length != 0)
                                {
                                    if (GUILayout.Button("コピー".ToString(), GUILayout.Height(_height)))
                                    {
                                        GUIUtility.systemCopyBuffer = data.Value._url;
                                    }
                                }
                                else
                                {
                                    GUILayout.Label("登録なし".ToString(), GUILayout.Height(_height));
                                }
                            }
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("グラフ表示");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                            if (_localData.TeamName != data.Key)
                            {
                                data.Value._show =
                                    GUILayout.Toggle(data.Value._show, data.Value._show.ToString(),
                                        GUILayout.Height(_height));
                            }
                            else
                            {
                                GUILayout.Toggle(true, "User",
                                    GUILayout.Height(_height));
                            }
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("R");
                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                            if (_localData.TeamName != data.Key)
                            {
                                data.Value._color.r = GUILayout.HorizontalSlider(data.Value._color.r, 0f, 1f,
                                    GUILayout.Height(_height));
                            }
                            else
                            {
                                GUILayout.Label("".ToString(), GUILayout.Height(_height));
                                data.Value._color.r = 0f;
                            }
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("G");
                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                            if (_localData.TeamName != data.Key)
                            {
                                data.Value._color.g = GUILayout.HorizontalSlider(data.Value._color.g, 0.0f, 1.0f,
                                    GUILayout.Height(_height));
                            }
                            else
                            {
                                GUILayout.Label("".ToString(), GUILayout.Height(_height));

                                data.Value._color.g = 1f;
                            }
                        }
                    }


                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("B");
                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                            if (_localData.TeamName != data.Key)
                            {
                                data.Value._color.b = GUILayout.HorizontalSlider(data.Value._color.b, 0f, 1f,
                                    GUILayout.Height(_height));
                            }
                            else
                            {
                                GUILayout.Label("".ToString(), GUILayout.Height(_height));
                                data.Value._color.b = 1f;
                            }
                        }
                    }

                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("最終更新");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
                            GUIStyle style = new GUIStyle();

                            if (_localData.TeamName != data.Key)
                            {
                                style.normal.textColor = data.Value._color;
                            }
                            else
                            {
                                style.normal.textColor = Color.red;
                            }

                            style.fontStyle = FontStyle.Bold;

                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            GUILayout.Label(data.Value._dateTime.ToString(" MM月 dd日 HH時 mm分 ss秒"), style,
                                GUILayout.Height(_height));
                        }
                    }


                    using (var verticallArea = new EditorGUILayout.VerticalScope())
                    {
                        //                            GUILayout.Label("");

                        foreach (KeyValuePair<string, RankingDataStruct> data in _rankingData.rankingData)
                        {
//                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                            //                            GUILayout.TextField(data.Value._color.ToString(),GUILayout.Height(_height));
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("OK", GUILayout.Height(30f)))
            {
//                LocalDataExport();
                Close();
            }
        }

        private void OnDestroy()
        {
//            RankingDataExport();
            RankingDataExport();

            MySQLUtil.DBClose();
        }


        private void RankingDataExport()
        {
            // 新規の場合は作成
            if (!AssetDatabase.Contains(_rankingData as UnityEngine.Object))
            {
                string directory = Path.GetDirectoryName(GameJam2020inWinterProgress.RankingAssetPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // アセット作成
                AssetDatabase.CreateAsset(_rankingData, GameJam2020inWinterProgress.RankingAssetPath);
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
            RankingData rankingData = AssetDatabase.LoadAssetAtPath<RankingData>(GameJam2020inWinterProgress.RankingAssetPath);
            if (rankingData == null)
                return;
            _rankingData = rankingData;
        }

        private void LocalDataImport()
        {
            LocalData localData = AssetDatabase.LoadAssetAtPath<LocalData>(GameJam2020inWinterProgress.LocalAssetPath);
            if (localData == null)
                return;
            _localData = localData;
        }

//        private void LocalDataExport()
//        {
//            // 新規の場合は作成
//            if (!AssetDatabase.Contains(_localData as UnityEngine.Object))
//            {
//                string directory = Path.GetDirectoryName(GameJam2020inWinter.LOCALASSET_PATH);
//                if (!Directory.Exists(directory))
//                {
//                    Directory.CreateDirectory(directory);
//                }
//                // アセット作成
//                AssetDatabase.CreateAsset(_localData, GameJam2020inWinter.LOCALASSET_PATH);
//            }
//            // インスペクターから設定できないようにする
//            _localData.hideFlags = HideFlags.NotEditable;
//            // 更新通知
//            EditorUtility.SetDirty(_localData);
//            // 保存
//            AssetDatabase.SaveAssets();
//            // エディタを最新の状態にする
//            AssetDatabase.Refresh();
//        }
    }
}