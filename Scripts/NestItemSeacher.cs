using ClusterVR.CreatorKit.Item.Implements;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace sarisarinyama.cluster
{
    public class NestItemSeacher : MonoBehaviour
    {
        [InfoBox("プレイモードに入ると全てのオブジェクトから親子になっているItemを検索し、\nNestItem : 親オブジェクト名 - 小オブジェクト名\nの形式でDebug.Logに出力します。")]
        public bool outputItemID;
        public bool outputItemName;

        void Start()
        {
            foreach (Item item in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Item)))
            {
                // アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得.
                string path = AssetDatabase.GetAssetOrScenePath(item.gameObject);
                // シーン上に存在するオブジェクトかどうか文字列で判定.
                bool isScene = path.Contains(".unity");
                // シーン上に存在するオブジェクトならば処理.
                if (isScene)
                {
                    // List<GameObject> itemObjects;
                    Item[] items = item.gameObject.GetComponentsInChildren<Item>(true);
                    for (int i = 0; i < items.Length; i++)
                    {
                        SearchChildItem(items[i].gameObject);
                    }
                }
            }
        }

        void SearchChildItem(GameObject currentObject)
        {
            Item[] childItems = currentObject.GetComponentsInChildren<Item>(true);
            for (int i = 0; i < childItems.Length; i++)
            {
                if (Equals(childItems[i].gameObject, currentObject))
                {
                    // do nothing 
                }
                else
                {
                    OutputLog(currentObject, childItems[i].gameObject);
                }
            }
        }

        void OutputLog(GameObject currentObject, GameObject childObject)
        {
            string debugText = "";
            debugText = "NestItem :";
            debugText += currentObject.name.ToString();
            if (outputItemID) debugText += " [" + currentObject.GetComponent<Item>().Id + "]";
            if (outputItemName) debugText += " [" + currentObject.GetComponent<Item>().ItemName + "]";
            debugText += " - " + childObject.name.ToString();
            if (outputItemID) debugText += " [" + childObject.GetComponent<Item>().Id + "]";
            if (outputItemName) debugText += " [" + childObject.GetComponent<Item>().ItemName + "]";
            Debug.Log(debugText);
        }
    }
}