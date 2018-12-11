using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;

public class TcpC : MonoBehaviour
{
    private TcpClient client;
    private Thread listenThread;
    private NetworkStream stream;
    private byte[] data;


    private static TcpC _instance;
    public static TcpC instance
    {
        get
        {
            if(null == _instance)
            {
                _instance = new GameObject( "TcpC" ).AddComponent<TcpC>();
            }
            return _instance;
        }
    }

    public void ConnectTo(string ip,int port,Action callback = null)
    {
        try
        { 
            client = new TcpClient( AddressFamily.InterNetwork );
            client.BeginConnect(ip, port, new AsyncCallback((ar)=> {
                Debug.Log("Connect AsyncCallback");
                if(null != callback)
                    callback();
                stream = client.GetStream();
                data = new byte[1024];
                client.EndConnect(ar);
            }), client);
            ThreadStart tStart = new ThreadStart(ReceiveFromServer);
            listenThread = new Thread(tStart);
            listenThread.Start();
        }
        catch
        {
            throw new Exception( "[TcpC]:Failed to connect to " + ip );
        }
    }

    public void StopConnect()
    {
        if(null != listenThread)
        {
            listenThread.Abort();
            listenThread = null;
        }
        if(null != client)
        {
            client.Close();
            client = null;
        }
    }

    public void Send(string msg)
    {
        if (string.IsNullOrEmpty( msg ))
            return;
        if(null != client && client.Connected)
        {
            msg = msg.Trim();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data,0,data.Length);
        }
    }

    void ReceiveFromServer()
    {
        while(client.Connected)
        {
            int leng = stream.Read(data,0,(int)stream.Length);
            if(leng > 0)
            {
                string msg = Encoding.UTF8.GetString( data );
                stream.Flush();
                Debug.Log( msg );
            }
        }
    }

    private void OnApplicationQuit()
    {
        StopConnect();
    }
}
