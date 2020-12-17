using UnityEngine;
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
            GUILayout.Label("チームのパスワードを入力してください");
            GUILayout.Label("注意：入力したパスワードは作者個人のDBに登録されるので\r\n他のサービスで使うパスワードを入力しないでください！！");
            GUI.SetNextControlName("ForcusField");
            _input = GUILayout.TextField(_input);

            // 何かしら入力しないとOKボタンを押せないようにするDisableGroup
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_input));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(30f)))
            {
                _callback(_input);
                Close();
            }

            EditorGUI.EndDisabledGroup();

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