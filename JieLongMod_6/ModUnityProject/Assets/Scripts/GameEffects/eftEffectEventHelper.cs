using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class eftEffectEventHelper : MonoBehaviour {

    public class EffectEventItem
    {
        public float eventTime = 0;

        public string eventCmd = null;

        public bool isExected = false;

        public EffectEventItem()
        {
            eventTime = 0;
            eventCmd = null;
            isExected = false;
        }
    }

    [SerializeField, Tooltip("指令格式见文档第13章")]
    public string[] curEventCmdLines;

    private List<EffectEventItem> curEventItemList = null;

    private bool isInited = false;

    private float curTimer = 0;

    private void InitCtl()
    {
        if (isInited) return;

        curEventItemList = new List<EffectEventItem>();
        if (curEventCmdLines != null)
        {
            foreach (string tagCmdLine in curEventCmdLines)
            {
                int lIndex = tagCmdLine.IndexOf(":", 0);
                if (lIndex >= 0 && tagCmdLine.Length > lIndex + 1)
                {
                    EffectEventItem newEvent = new EffectEventItem();
                    newEvent.eventTime = float.Parse(tagCmdLine.Substring(0, lIndex));
                    newEvent.eventCmd = tagCmdLine.Substring(lIndex + 1, tagCmdLine.Length - (lIndex + 1));
                    newEvent.isExected = false;
                    curEventItemList.Add(newEvent);
                }
            }
        }

        isInited = true;
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        curTimer = 0;
        InitCtl();
        if (curEventItemList != null)
        {
            foreach (EffectEventItem tagEvent in curEventItemList)
            {
                tagEvent.isExected = false;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (curEventItemList == null) return;
        float ct = Time.deltaTime;
        curTimer += ct;
        if (curEventItemList != null)
        {
            foreach (EffectEventItem tagEvent in curEventItemList)
            {
                if (!tagEvent.isExected && curTimer >= tagEvent.eventTime)
                {
                    OnExecEffectEvent(tagEvent.eventCmd);
                    tagEvent.isExected = true;
                }
            }
        }
    }

    private void OnExecEffectEvent(string eventCmd)
    {
        //GameShared.Instance.SendSceneMessage("effect_event", eventCmd);
    }
    
}
