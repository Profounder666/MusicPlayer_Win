using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CustomSlider : Slider, IBeginDragHandler, IEndDragHandler
{
    public Action onBeginDragAction;
    public Action onEndDragAction;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �����϶���ʼ�Ļص�
        onBeginDragAction?.Invoke();
        //Debug.Log("�϶���ʼ");
        DownPanel.isDraggingProgress = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �����϶������Ļص�
        onEndDragAction?.Invoke();
        //Debug.Log("�϶�����");
        if (transform.name == "ProgressBar")
        {
            DownPanel.isDraggingProgress = false;
            DownPanel.audioSource.time = transform.GetComponent<CustomSlider>().value;
            //DownPanel.audioSource.time = DownPanel.slider.value;
        }
        //else if (transform.name == "VolumeSlider")
        //{
        //    DownPanel.isDraggingProgress = false;
        //    DownPanel.audioSource.volume = transform.GetComponent<CustomSlider>().value;
        //}
           
    }
}

