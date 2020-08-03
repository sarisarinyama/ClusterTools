using System.Collections.Generic;
using ClusterVR.CreatorKit.Item.Implements;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace sarisarinyama.cluster
{
    public class NestItemSeacher : MonoBehaviour
    {
        [SerializeField] private bool outputItemID=true;
        [SerializeField] private bool outputItemName=true;
        [SerializeField] private bool searchPrefab=true;

        private List<string> parentAndChild = new List<string>();

        void Start()
        {
            foreach (var o in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Item)))
            {
                var item = (Item) o;
                // アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得.
                string path = AssetDatabase.GetAssetOrScenePath(item.gameObject);
                bool isScene = true;
                // シーン上に存在するオブジェクトかどうか文字列で判定.
                if(!searchPrefab)isScene= path.Contains(".unity");
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
            if (parentAndChild.IsNullOrEmpty())
            {
                parentAndChild.Add(debugText);
                Debug.Log(debugText);
            }
            else
            {
                if (null==parentAndChild.Find(n => n.Equals(debugText)))
                {
                    parentAndChild.Add(debugText);
                    Debug.Log(debugText);   
                }
            }
        }
    }
}