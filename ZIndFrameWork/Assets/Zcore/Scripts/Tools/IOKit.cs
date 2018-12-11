/*
* @author : ZhaoYingchao
* @time : 2018-11-03 19:09:15
* @function: IO相关
*/


using System;
using System.IO;

public class IOKit
{
    public static bool EnsureDirectory(string path)
    {
        try
        {
            path.Replace("\\", "/");
            if (path.Contains("."))
            {
                path = path.Substring(0, path.LastIndexOf("/") + 1);
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return true;
        }
        catch (Exception e)
        {
            GLog.Error(e.ToString());
        }
        return false;
    }
}
