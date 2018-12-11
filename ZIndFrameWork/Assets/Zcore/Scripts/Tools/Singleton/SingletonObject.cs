using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonObject : MonoBehaviour
{
    private static GameObject m_Container = null;
    private static string m_Name = "Singleton";
    private static Dictionary<string, object> m_SingletonMap = new Dictionary<string, object> ();
    private static bool m_IsDestroying = false;

    public static bool IsDestroying
    {
        get { return m_IsDestroying; }
    }

    public static bool IsCreatedInstance<T> () where T : class, new ()
    {
        if (m_Container == null)
        {
            return false;
        }
        if (m_SingletonMap != null && m_SingletonMap.ContainsKey (typeof (T).ToString ()))
        {
            return true;
        }
        return false;
    }

    public static T getInstance<T> (T obj = default (T)) where T : class, new ()
    {
        if (Application.isPlaying && m_IsDestroying)
        {
            GLog.Warn("SingletonObject is mark as Destroy! Can not get instance any more!");
            return null;
        }
        if (m_Container == null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                while (true)
                {
                    m_Container = GameObject.Find(m_Name);
                    if (null != m_Container)
                        GameObject.DestroyImmediate(m_Container);
                    else
                        break;
                }
            }

#endif
            m_Container = new GameObject();
            m_Container.name = m_Name;
            m_Container.AddComponent(typeof(SingletonObject));
            GLog.Log("Create Singleton from:" + typeof(T).ToString());
        }
        string name = typeof (T).ToString ();
        if (!m_SingletonMap.ContainsKey (name))
        {
            if (System.Type.GetType (name) != null)
            {
                if (null != obj)
                {
                    m_SingletonMap.Add (name, obj);
                    if (typeof(T).IsSubclassOf(typeof(Component)))
                    {
                        Component c = obj as Component;
                        DontDestroyOnLoad(c.transform.root.gameObject);
                    }
                }
                else
                {
                    if (typeof (T).IsSubclassOf (typeof (Component)))
                    {
                        // Awake -> Init -> Start -> Update ····
                        m_SingletonMap.Add (name, m_Container.AddComponent (typeof (T)));
                    }
                    else
                    {
                        m_SingletonMap.Add (name, new T ());
                    }
                    ISingletonInit sInit = m_SingletonMap[name] as ISingletonInit;
                    if (null != sInit)
                    {
                        sInit.Init ();
                    }
                }
            }
            else
            {
                GLog.Warn("Singleton Type ERROR! (" + name + ")");
            }
        }
        return m_SingletonMap[name] as T;
    }

    public static void RemoveInstance<T> () where T : class, new ()
    {
        if (m_Container != null && m_SingletonMap.ContainsKey (typeof (T).ToString ()))
        {
            string name = typeof (T).ToString ();
            IDisposable dispose = m_SingletonMap[name] as IDisposable;
            if (null != dispose)
                dispose.Dispose ();
            if (typeof (T).IsSubclassOf (typeof (Component)))
            {
                UnityEngine.Object.Destroy (m_SingletonMap[name] as UnityEngine.Component);
            }
            m_SingletonMap.Remove (name);
            GLog.Warn("Singleton REMOVE! (" + name + ")");
        }
    }

    void Awake ()
    {
        GLog.Log ("Awake Singleton.");
        DontDestroyOnLoad (gameObject);
        m_IsDestroying = false;
    }

    void OnApplicationQuit ()
    {
        GLog.Log("Destroy Singleton");
        if (m_Container != null)
        {
            GameObject.Destroy (m_Container);
            m_Container = null;
            m_IsDestroying = true;
        }
    }

}