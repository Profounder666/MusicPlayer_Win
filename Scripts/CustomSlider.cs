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
        // 调用拖动开始的回调
        onBeginDragAction?.Invoke();
        //Debug.Log("拖动开始");
        DownPanel.isDraggingProgress = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 调用拖动结束的回调
        onEndDragAction?.Invoke();
        //Debug.Log("拖动结束");
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

