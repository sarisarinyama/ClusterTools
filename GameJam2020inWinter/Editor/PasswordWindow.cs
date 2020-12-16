﻿using UnityEngine;
using UnityEditor;


namespace sarisarinyama.cluster.GameJam2020inWinter
{
    public class PasswordWindow : EditorWindow
    {
        private System.Action<string> _callback;

        public void Setup(System.Action<string> callback)
        {
            _callback = callback;
        }

        string _input = "";
        bool _isInit = false;


        void OnGUI()
        {
            GUILayout.Label("パスワードを入力してください");
            GUILayout.Space(10f);
            GUI.SetNextControlName("ForcusField");
            _input = GUILayout.TextField(_input);
            GUILayout.Space(10f);

            // 何かしら入力しないとOKボタンを押せないようにするDisableGroup
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_input));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(30f)))
            {
                _callback(_input);
                Close();
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("CANCEL", GUILayout.Height(30f)))
            {
                _callback(null);
                Close();
            }

            GUILayout.EndHorizontal();

            if (_isInit == false)
            {
                // テキストフィールドにフォーカスをあてる
                EditorGUI.FocusTextInControl("ForcusField");
            }

            _isInit = true;
        }

        private void OnDestroy()
        {
            _callback(_input);
        }
    }
}