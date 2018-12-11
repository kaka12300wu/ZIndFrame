/*
* @author : ZhaoYingchao
* @time : 2018-11-03 19:11:23
* @function: 时间处理相关
*/


using System;

public class Timekit
{
    public static long GetCurrentTimeStamp()
    {
        return DateTime.Now.Ticks;
    }
}
