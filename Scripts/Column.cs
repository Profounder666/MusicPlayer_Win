using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Column : MonoBehaviour
{
    //歌名、歌手、节点名
    public void SelfButtonClick()//1
    {
        // 检查EventSystem是否有效 
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            // 获取被点击的按钮 
            GameObject button = EventSystem.current.currentSelectedGameObject;
            //传递这个节点的名称
            CenterPanel.groupValues.NodeName = button.name;
        }
        else
        {
            Debug.LogWarning("No selected game object or EventSystem is null.");
        }
    }

    //去除无效信息
    private string RemoveUnrelated(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            return string.Empty;
        }

        int index = nodeName.IndexOf('-');
        if (index >= 0)
        {
            return nodeName.Substring(index + 1);
        }
        else
        {
            return nodeName;
        }

    }

    
}
//if (string.IsNullOrEmpty(nodeName))
//{
//    return string.Empty;
//}

//int lastIndex = nodeName.LastIndexOf('-');
//if (lastIndex >= 0 && lastIndex < nodeName.Length - 1)
//{
//    return nodeName.Substring(0, lastIndex);
//}
//else
//{
//    return nodeName;
//}
