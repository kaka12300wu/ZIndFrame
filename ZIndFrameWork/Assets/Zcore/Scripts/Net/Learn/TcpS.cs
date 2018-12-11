using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;

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
    private Dictionary<string, TcpClient> clientDic;
    private NetworkStream stream;
    private Thread serverThread;
    private BinaryReader reader;
    private BinaryWriter writer;

    public void StartListen( string ip, int port )
    {
        try
        {
            working = true;
            clientDic = new Dictionary<string, TcpClient>();
            listener = new TcpListener( IPAddress.Parse( ip ), port );
            listener.Start();

            ThreadStart tStart = new ThreadStart( ServerProc );
            serverThread = new Thread( tStart );
            serverThread.Start();

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
            
            foreach(var pair in clientDic)
            {
                pair.Value.Close();
            }
            clientDic.Clear();
            clientDic = null;
            working = false;
        }
        if (null != serverThread)
        {
            serverThread.Abort();
            serverThread = null;
        }
    }

    public void SendTo(string msg,string ip = "")
    {
        byte[] buffer = Encoding.UTF8.GetBytes( msg );
        if (!string.IsNullOrEmpty(ip))
        {
            SendTo( buffer, ip );
        }
        else
        {
            foreach(var pair in clientDic)
            {
                SendTo( buffer, pair.Key );
            }
        }

    }

    void SendTo(byte[] buffer,string ip)
    {
        TcpClient client = null;
        clientDic.TryGetValue(ip,out client);
        if(null == client || !client.Connected)
        {
            GLog.Warn("Failed to send data to "+ip);
            return;
        }
        NetworkStream cStream = client.GetStream();
        writer = new BinaryWriter( cStream );
        writer.Write( (short)buffer.Length );
        writer.Write(buffer,0,buffer.Length);
        cStream.Flush();
    }

    void ServerProc()
    {
        while (true)
        {
            if (!working)
            {
                break;
            }
            if (listener.Pending())
                AddNewClient( listener.AcceptTcpClient() );

            foreach (var pair in clientDic)
            {
                if (!pair.Value.Connected)
                    continue;
                try
                {
                    stream = pair.Value.GetStream();
                    reader = new BinaryReader( stream );
                    short length = reader.ReadInt16();
                    byte[] buffer = reader.ReadBytes( length );
                    string msg = Encoding.UTF8.GetString( buffer );
                    GEvent.OnEvent( eEvent.AddMsg, string.Format( "{0}:{1}", pair.Key, msg ) );
                }
                catch
                {

                }
            }
        }
    }

    //TODO 去重
    void AddNewClient( TcpClient client )
    {
        if (null == client)
            return;
        IPEndPoint ipe = client.Client.RemoteEndPoint as IPEndPoint;
        if(clientDic.ContainsKey(ipe.Address.ToString()))
        {
            clientDic[ipe.Address.ToString()].Close();
            clientDic.Remove( ipe.Address.ToString() );
        }
        clientDic.Add( ipe.Address.ToString(), client );
        GEvent.OnEvent( eEvent.AddMsg, string.Format( "{0}: client connected!", ipe.Address.ToString() ) );
    }

    private void OnApplicationQuit()
    {
        Stop();
    }
}
