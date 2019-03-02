/*
* @author : ZhaoYingchao
* @time : 2018-11-25
* @function: 网络通用工具类
*/


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public class NetUtils 
{
    public static string GetSelfIP4Address()
    {
        string retAddress = string.Empty;
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            return Network.player.ipAddress;
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        AndroidJavaClass ajClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
        AndroidJavaObject ajObj = ajClass.GetStatic<AndroidJavaObject>( "currentActivity" );
        if (ajObj.Call<bool>( "IsWifiApOpen" ))
            retAddress = "192.168.43.1";
#elif UNITY_IOS

#endif
#endif
        }
        return retAddress;
    }

    public static string GetBroadcastIPAddress()
    {
        string localIP = GetSelfIP4Address();
        string retAddress = localIP.Substring(0, localIP.LastIndexOf('.') + 1) + "255";
//#if !UNITY_EDITOR
//#if UNITY_ANDROID
//        AndroidJavaClass ajClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
//        AndroidJavaObject ajObj = ajClass.GetStatic<AndroidJavaObject>( "currentActivity" );
//        if (ajObj.Call<bool>( "IsWifiApOpen" ))
//            retAddress = "192.168.43.255";
//#elif UNITY_IOS

//#endif
//#endif
        return retAddress;
    }

    public static int broadcastPort
    {
        get
        {
            return 49092;
        }
    }
        
}
