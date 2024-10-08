using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollViewPullToRefresh : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect uiScrlRect;
    public Image indicatorImg;

    public Action OnPullStart;
    public Action OnPullCancel;
    public Action OnPullOverTh;
    public Action OnPullToRefresh;

    private float pullToRefreshTh = 1.2f;
    private float indicatorRotateSpeed = -6.0f;

    private bool isPullOver = false;


    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        // インジケーター表示更新
        if(indicatorImg != null)
        {
            float margin = 0.1f;
            float alp = (Mathf.Max(1.0f, uiScrlRect.verticalNormalizedPosition - margin) - 1.0f) / (pullToRefreshTh - 1.0f - margin);
            indicatorImg.color = new Color(indicatorImg.color.r, indicatorImg.color.g, indicatorImg.color.b, alp);

            if(isPullOver)
                indicatorImg.transform.Rotate(new Vector3(0.0f, 0.0f, indicatorRotateSpeed));
        }
    }

#region interface
    public void OnBeginDrag(PointerEventData e)
    {
        OnPullStart?.Invoke();
    }

    public void OnDrag(PointerEventData e)
    {
        // リストを引っ張って更新
        if(!isPullOver)
        {
            if(uiScrlRect.verticalNormalizedPosition >= pullToRefreshTh)
            {
                isPullOver = true;
                OnPullOverTh?.Invoke();
            }
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if(isPullOver)
        {
            isPullOver = false;
            OnPullToRefresh?.Invoke();
        }
        else
            OnPullCancel?.Invoke();
    }
#endregion

    public float GetPullValue()
    {
        return uiScrlRect.verticalNormalizedPosition - 1.0f;
    }
}
