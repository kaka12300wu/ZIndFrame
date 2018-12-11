/*
* @author : ZhaoYingchao
* @time : 2018-11-11
* @function: 全局输出使用这个输出，可以方便屏蔽输出
*/

#define LOG_ON
using UnityEngine;

public class GLog
{
    public static void Log(string _msg,params object[] _args)
    {
#if LOG_ON
        Debug.Log(string.Format(_msg,_args));
#endif
    }

    public static void Warn(string _msg, params object[] _args)
    {
#if LOG_ON
        Debug.LogWarning(string.Format(_msg, _args));
#endif
    }

    public static void Error(string _msg, params object[] _args)
    {
#if LOG_ON
        Debug.LogError(string.Format(_msg, _args));
#endif
    }
}
