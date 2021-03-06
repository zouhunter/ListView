﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ListView;
using System;

public class DemoPanel : MonoBehaviour
{
    [SerializeField]
    private Button m_insert;
    [SerializeField]
    private ScrollRect m_scrollRect;
    [SerializeField]
    private DemoItem m_itemPfb;
    private AsyncListCreater<DemoItem> creater;
    public Direction direction;
    public int datacount;

    private void Awake()
    {
        creater = new ListView.AsyncListCreater<global::DemoItem>(m_scrollRect, m_itemPfb, direction);
        creater.onVisiable += OnCreateDemoItem;
        creater.CreateItems(datacount);
        m_insert.onClick.AddListener(InsertAnElement);
    }

    private void InsertAnElement()
    {
        creater.AddItem();
    }

    private void OnCreateDemoItem(DemoItem arg0)
    {
        arg0.onClicked = OnClickDemoItem;
        arg0.InitItem();
    }

    private void OnClickDemoItem(DemoItem arg0)
    {
        Debug.LogFormat("移除id为 ：{0}的条目", arg0.Id);
        creater.RemoveItem(arg0);
    }

    
}
