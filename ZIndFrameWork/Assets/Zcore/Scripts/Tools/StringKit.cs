/*
* @author : ZhaoYingchao
* @time : 2018-11-03 19:10:19
* @function: 字符串扩展相关
*/


using System;
using System.Text;
using System.Collections.Generic;

public class StringKit
{
    
}

public static class StringExtentions
{
    public static string PathFormat(this string _str)
    {
        return _str.Replace("\\", "/");
    }

    public static string CombinePath(this string pathPre, string pathNext)
    {
        pathPre = pathPre.PathFormat();
        pathNext = pathNext.PathFormat();
        if (string.IsNullOrEmpty(pathPre))
            return pathNext;
        if (string.IsNullOrEmpty(pathNext))
            return pathPre;
        if (!pathPre.EndsWith("/") && !pathNext.StartsWith("/"))
            return string.Format("{0}/{1}", pathPre, pathNext);
        else if (pathPre.EndsWith("/") && pathNext.StartsWith("/"))
        {
            return pathPre + pathNext.Substring(1);
        }
        else
        {
            return pathPre + pathNext;
        }
    }

    public static string Concat(this string[] paramStrs, string _str)
    {
        if (paramStrs.Length == 1)
            return paramStrs[0];

        StringBuilder sbuilder = new StringBuilder();
        for (int i = 0, max = paramStrs.Length; i < max; ++i)
        {
            sbuilder.Append(paramStrs[i]);
            if (i < max - 1)
            {
                sbuilder.Append(_str);
            }
        }
        return sbuilder.ToString();
    }

    public static string Concat(string _str,params string[] paramStrs)
    {
        return paramStrs.Concat(_str);
    }

    public static string Concat(this List<string> listStr, string str)
    {
        return listStr.ToArray().Concat(str);
    }
}
