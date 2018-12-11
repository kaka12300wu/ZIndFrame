/*
* @author : ZhaoYingchao
* @time : 2018-11-03 19:43:43
* @function: 跨平台相关参数
*/

using UnityEngine;

public class PlatfomKit
{
    #region COMMON_ARGS
    
    #endregion

#if UNITY_ANDROID
    #region UNITY_ANDROID

    #endregion
#elif UNITY_IOS
    #region UNITY_IOS

    #endregion
#else
    #region UNITY_WIN

    #endregion
#endif

}
