/*
* @author : ZhaoYingchao
* @time : 2019年2月17日
* @function: 用于测试指定功能的函数
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GUIHandlerTest : MonoBehaviour 
{
    GUIStyle style;
    private void OnGUI()
    {
        
        if (GUI.Button(new Rect(30,30,320,80),"<size=40>HandleTest</size>"))
        {
            Handler();
        }
    }

    [Serializable]
    public class TestObjA
    {
        public string name;
        public int level;
        public float pow;
        public short stage;
        public bool isArcher;
    }

    public class TestObjB
    {
        public string mark;
        public List<TestObjA> objList;
    }

    void Handler()
    {
        TestObjB b = new TestObjB();
        b.mark = "hello world";
        b.objList = new List<TestObjA>();
        for(int i = 0;i<5;++i)
        {
            TestObjA a = new TestObjA();
            a.name = "objName_" + (i + 1);
            a.level = Random.Range(1,50);
            a.pow = Random.Range(100.0f,300.0f);
            a.stage = (short)Random.Range(1,255);
            a.isArcher = Random.Range(0, 1.0f) > 0.5f ? true : false;
            b.objList.Add(a);
        }
        string jsonStr = JsonUtility.ToJson(b);
        Debug.Log(jsonStr);
    }
}
