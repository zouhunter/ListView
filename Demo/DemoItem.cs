using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ListView;
public class DemoItem : MonoBehaviour ,IListItem{
    [SerializeField]
    private Image m_pic;
    [SerializeField]
    private Text m_text;
    [SerializeField]
    private Button m_btn;
    private int _id;
    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
    public UnityAction<DemoItem> onClicked;
    private void Awake()
    {
        m_btn.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        if (onClicked != null) onClicked.Invoke(this);
    }
    public void InitItem()
    {
        m_text.text = "[ID:]" + _id.ToString();
    }
}
