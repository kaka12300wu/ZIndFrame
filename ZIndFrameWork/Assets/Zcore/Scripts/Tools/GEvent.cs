/*******************************************************************************
 * 全局事件管理机制
 * 创建时间：2016年3月1日00:17:54
 * 作者：Zhao
 * 
 * *********
 * 时间：2016年3月1日00:18:32
 * 事件：创建
 * 描述：利用C#内置委托机制，实现全局事件的注册，移除，抛出响应等基本功能，后续功能待完善
 * *********
 * 
 * ********************************************************************************/
using System;
using System.Collections.Generic;

public class GEvent
{
    public delegate void Callback(params object[] args);
    private class ZEventData
    {
        public Callback m_callback;
        public bool m_bOnce = false;
    }
    private static Dictionary<Enum, List<ZEventData>> dicGlobalEvent = new Dictionary<Enum, List<ZEventData>>();

    private static void AddEvent(Enum _em, ZEventData _zed)
    {
        if (dicGlobalEvent.ContainsKey(_em))
            dicGlobalEvent[_em].Add(_zed);
        else
        {
            List<ZEventData> list = new List<ZEventData>();
            list.Add(_zed);
            dicGlobalEvent.Add(_em, list);
        }
    }

    public static void Add(Enum _em, Callback _cb)
    {
        ZEventData zed = new ZEventData();
        zed.m_callback += _cb;
        zed.m_bOnce = false;
        AddEvent(_em, zed);
    }

    public static void AddOneShot(Enum _em, Callback _cb)
    {
        ZEventData zed = new ZEventData();
        zed.m_callback += _cb;
        zed.m_bOnce = true;
        AddEvent(_em, zed);
    }

    public static void ClearEvent(Enum _em)
    {
        if (dicGlobalEvent.ContainsKey(_em))
        {
            dicGlobalEvent[_em].Clear();
            dicGlobalEvent.Remove(_em);
        }
    }

    public static void RemoveEvent(Enum _em, Callback _cb)
    {
        if (dicGlobalEvent.ContainsKey(_em))
        {
            foreach (ZEventData zed in dicGlobalEvent[_em])
            {
                if (zed.m_callback.Equals(_cb))
                {
                    dicGlobalEvent[_em].Remove(zed);
                    break;
                }
            }
            if (dicGlobalEvent[_em].Count <= 0)
                dicGlobalEvent.Remove(_em);
        }
    }

    public static void OnEvent(Enum _em, params object[] args)
    {
        if (dicGlobalEvent.ContainsKey(_em))
        {
            List<ZEventData> list = dicGlobalEvent[_em];
            List<ZEventData> removeList = new List<ZEventData>();
            for (int i = 0, max = list.Count; i < max; ++i)
            {
                if (list[i].m_bOnce)
                    removeList.Add(list[i]);
                list[i].m_callback(args);
            }
            for (int i = 0, max = removeList.Count; i < max; ++i)
            {
                list.Remove(removeList[i]);
            }
            if (list.Count <= 0)
                dicGlobalEvent.Remove(_em);
        }
    }
}
