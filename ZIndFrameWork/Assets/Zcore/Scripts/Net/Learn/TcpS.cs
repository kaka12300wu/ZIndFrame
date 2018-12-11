using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

public class TcpS : MonoBehaviour
{
    private static TcpS _inst;
    public static TcpS instance
    {
        get
        {
            if (null == _inst)
            {
                _inst = new GameObject( "TcpS" ).AddComponent<TcpS>();
            }
            return _inst;
        }
    }
    private TcpListener listener;
    private bool working = false;
    private List<TcpClient> clientList;
    private NetworkStream stream;
    private Thread receiveThread;
    private Thread acceptThread;


    public void StartListen( string ip, int port )
    {
        try
        {
            working = true;
            clientList = new List<TcpClient>();
            listener = new TcpListener( IPAddress.Parse( ip ), port );
            listener.Start();

            ThreadStart tAcceptStart = new ThreadStart(AcceptClient);
            acceptThread = new Thread(tAcceptStart);
            acceptThread.Start();

            ThreadStart tStart = new ThreadStart( StartReceive );
            receiveThread = new Thread( tStart );
            receiveThread.Start();

        }
        catch
        {
            throw new System.Exception( "failed to start TcpS" );
        }
    }

    public void Stop()
    {
        if (null != listener)
        {
            listener.Stop();
            listener = null;
            clientList.ForEach( ( elem ) => elem.Close() );
            clientList.Clear();
            clientList = null;
            working = false;
        }
        if (null != receiveThread)
        {
            receiveThread.Abort();
            receiveThread = null;
        }
        if(null != acceptThread)
        {
            acceptThread.Abort();
            acceptThread = null;
        }
    }

    public void SendTo(string msg,string ip = "")
    {


    }

    void SendTo(byte[] buffer,string ip)
    {

    }

    void StartReceive()
    {
        while (true)
        {
            if (null == clientList)
                break;
            foreach (var c in clientList)
            {
                stream = c.GetStream();
                byte[] buffer = new byte[stream.Length];
                stream.Read( buffer, 0, buffer.Length );
                Debug.Log( Encoding.UTF8.GetString( buffer ) );
            }
            Thread.Sleep( 1000 );
        }
    }

    //TODO 去重
    void AddNewClient( TcpClient client )
    {
        if (null == client)
            return;
        Debug.Log( "New client connected:" + client.Client.RemoteEndPoint.ToString() );
        clientList.Add( client );
    }

    void AcceptClient()
    {
        while (true)
        {
            if (!working)
            {
                break;
            }
            AddNewClient(listener.AcceptTcpClient());
        }
    }

    private void OnApplicationQuit()
    {
        Stop();
    }
}
