using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UdpBroadcast : MonoBehaviour
{

    private static UdpBroadcast _inst;
    public static UdpBroadcast instance
    {
        get
        {
            if (null == _inst)
            {
                _inst = new GameObject("UdpBroadcast").AddComponent<UdpBroadcast>();
            }
            return _inst;
        }
    }

    private UdpClient udpSpeaker;
    private UdpClient udpListener;
    private Thread receiveThread;
    private IPEndPoint commuIPE;
    private string selfIP4Address;

    public void StartBroadcast()
    {
        selfIP4Address = NetUtils.GetSelfIP4Address();
        udpSpeaker = new UdpClient(0);

        commuIPE = new IPEndPoint(IPAddress.Parse(NetUtils.GetBroadcastIPAddress()), NetUtils.broadcastPort);

        ThreadStart tStart = new ThreadStart(Receive);
        receiveThread = new Thread(tStart)
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    public void Send( string msg )
    {
        if (null != udpSpeaker)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            udpSpeaker.Send(buffer, buffer.Length, commuIPE);
        }
    }

    public void Stop()
    {
        if (null != udpSpeaker)
        {
            udpSpeaker.Close();
            udpSpeaker = null;
        }
        if (null != udpListener)
        {
            udpListener.Close();
            udpListener = null;
        }
        if (null != receiveThread)
        {
            receiveThread.Interrupt();
            receiveThread.Abort();
            receiveThread = null;
        }
    }

    public void Receive()
    {
        udpListener = new UdpClient(NetUtils.broadcastPort);
        IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            byte[] buffer = udpListener.Receive(ref ipe);
            if (ipe.Address.ToString() == selfIP4Address)
            {
                Debug.Log("Received self send msg:" + Encoding.UTF8.GetString(buffer));
            }
            else
            {
                GEvent.OnEvent(eEvent.AddMsg, string.Format("{0}({1}):{2}", ipe.Address, ipe.Port, Encoding.UTF8.GetString(buffer)));
            }
        }
    }

    private void OnApplicationQuit()
    {
        Stop();
    }
}
