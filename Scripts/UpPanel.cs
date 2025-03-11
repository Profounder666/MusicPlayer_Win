using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpPanel : MonoBehaviour
{
    private Transform searchBox;
    private InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        searchBox = transform.Find("SearchBox");
        inputField = searchBox.GetComponent<InputField>();
        // ����Submit Event�¼�-���ڻ�ȡ�û�����س�ʱ�������
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }
    }
    public void OnSubmit(string value)
    {
        string userInput = value;//��ȡ�û������ֵ
        Debug.Log(userInput);
    }
    //Ԥʵ�֣����»س��󣬷������к��йؼ��ֵĸ����������б�

    public void OnClickTip()
    {
        Transform tipNode = transform.Find("Tip");
        Transform textNode = tipNode.Find("Text");
        if (textNode.gameObject.activeSelf == false)
        {
            textNode.gameObject.SetActive(true);
        }
        else
        {
            textNode.gameObject.SetActive(false);
        }
    }
}
