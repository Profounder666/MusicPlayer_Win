using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Column : MonoBehaviour
{
    //���������֡��ڵ���
    public void SelfButtonClick()//1
    {
        // ���EventSystem�Ƿ���Ч 
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            // ��ȡ������İ�ť 
            GameObject button = EventSystem.current.currentSelectedGameObject;
            //��������ڵ������
            CenterPanel.groupValues.NodeName = button.name;
        }
        else
        {
            Debug.LogWarning("No selected game object or EventSystem is null.");
        }
    }

    //ȥ����Ч��Ϣ
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
