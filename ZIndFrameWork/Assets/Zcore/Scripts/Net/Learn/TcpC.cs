using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine;
using System.Threading;

public class TcpC : MonoBehaviour
{
    private TcpClient client;
    private Thread listenThread;
    private NetworkStream stream;
    private byte[] data;
    private BinaryWriter writer;
    private BinaryReader reader;


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

    public void ConnectTo(string ip,int port,Action<string> callback = null)
    {
        try
        { 
            client = new TcpClient( AddressFamily.InterNetwork );
            client.BeginConnect(ip, port, new AsyncCallback((ar)=> {
                if(null == ar.AsyncState)
                {
                    return;
                }
                if (null != callback)
                {
                    TcpClient c = ar.AsyncState as TcpClient;
                    IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                    callback( iep.Address.ToString() );
                }

                stream = client.GetStream();
                writer = new BinaryWriter( stream );
                reader = new BinaryReader( stream );

                client.EndConnect(ar);

                ThreadStart tStart = new ThreadStart( ReceiveFromServer );
                listenThread = new Thread( tStart );
                listenThread.Start();
            } ), client);            
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
            listenThread.Interrupt();
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
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            writer.Write( (short)buffer.Length );
            writer.Write( buffer, 0, buffer.Length );
        }
    }

    void ReceiveFromServer()
    {
        while(client.Connected)
        {
            try
            {
                short length = reader.ReadInt16();
                byte[] buffer = new byte[length];
                reader.Read( buffer, 0, length );
                string msg = Encoding.UTF8.GetString( buffer );
                GEvent.OnEvent( eEvent.AddMsg, "Server:" + msg );
                stream.Flush();
            }
            catch { }
        }
    }

    private void OnApplicationQuit()
    {
        StopConnect();
    }

}
