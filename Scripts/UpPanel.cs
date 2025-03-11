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
        // 订阅Submit Event事件-用于获取用户输入回车时候的内容
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }
    }
    public void OnSubmit(string value)
    {
        string userInput = value;//获取用户输入的值
        Debug.Log(userInput);
    }
    //预实现：按下回车后，返回所有含有关键字的歌名、歌手列表

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
