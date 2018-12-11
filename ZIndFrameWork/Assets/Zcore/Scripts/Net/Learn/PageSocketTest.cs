using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ClientType
{
    Null,
    TcpS,
    TcpC,
    UdpBroadcastC,
    UdpBroadcastS,
    UdpGroupC,
    UdpGroupS,
    UniCastC,
    UniCastS,
}

public class PageSocketTest : MonoBehaviour
{
    public static PageSocketTest instance;

    public Text localIP;
    public ToggleGroup toggleGroup;
    public Toggle broadCastToggle;
    public Toggle groupCastToggle;
    public Toggle uniCastToggle;
    public Toggle tcpConnectToggle;
    public InputField inputIP;
    public InputField inputPort;
    public Button btnStart;
    public Button btnConnnect;
    public Button btnStop;
    public InputField inputSend;
    public Button btnSend;
    public Scrollbar verticalBar;
    public Text msgContent;
    [NonSerialized]
    public ClientType cType;

    private List<string> cachedMsg;

    private void Awake()
    {
        instance = this;
        cachedMsg = new List<string>();
    }

    // Use this for initialization
    void Start()
    {
        msgContent.text = "";
        localIP.text = string.Format("本机IP:{0}", NetUtils.GetSelfIP4Address());
        btnStop.gameObject.SetActive( false );
        AddMsg("start up....");
        GEvent.Add( eEvent.AddMsg, OnAddMsgEvent );
    }

    private void OnDestroy()
    {
        GEvent.RemoveEvent(eEvent.AddMsg,OnAddMsgEvent);
    }

    private void Update()
    {
        if(cachedMsg.Count > 0)
        {
            cachedMsg.ForEach( ( msg ) => AddMsg( msg ) );
            cachedMsg.Clear();
        }
    }

    private void OnAddMsgEvent(params object[] _args)
    {
        string msg = (string)_args[0];
        cachedMsg.Add( msg );
    }
       
    public void AddMsg(string msg)
    {
        msgContent.text += msg + "\n";
        msgContent.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, msgContent.preferredHeight + 20 );
        verticalBar.value = 0;
    }

    void SetControlEnable(bool canControl)
    {
        broadCastToggle.interactable = canControl;
        groupCastToggle.interactable = canControl;
        tcpConnectToggle.interactable = canControl;
        inputIP.interactable = canControl;
        inputPort.interactable = canControl;
    }

    public void OnConnect()
    {
        TcpC.instance.ConnectTo( inputIP.text, int.Parse( inputPort.text ), (ip)=> 
        {
            GEvent.OnEvent( eEvent.AddMsg, string.Format("Connect to {0} success!",ip) );
        } );
        btnConnnect.gameObject.SetActive( false );
        btnStop.gameObject.SetActive( true );
        btnStart.gameObject.SetActive( false );
        SetControlEnable( false );
        cType = ClientType.TcpC;
    }

    public void OnStart()
    {
        btnStart.gameObject.SetActive( false );
        btnConnnect.gameObject.SetActive( false );
        btnStop.gameObject.SetActive( true );
        SetControlEnable( false );
        cType = ClientType.TcpS;
        TcpS.instance.StartListen(NetUtils.GetSelfIP4Address(), 9091);
    }

    public void OnStop()
    {
        btnStart.gameObject.SetActive( true );
        btnConnnect.gameObject.SetActive( true );
        btnStop.gameObject.SetActive( false );
        if (cType == ClientType.TcpC)
        {
            TcpC.instance.StopConnect();
        }
        else if(cType == ClientType.TcpS)
        {
            TcpS.instance.Stop();
        }
        SetControlEnable( true );
        cType = ClientType.Null;
    }

    public void OnSend()
    {
        if (cType == ClientType.TcpC)
        {
            TcpC.instance.Send(inputSend.text);
        }
        else if (cType == ClientType.TcpS)
        {
            TcpS.instance.SendTo( inputSend.text );
        }
    }
}
