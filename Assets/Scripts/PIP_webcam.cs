using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net.WebSockets;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;

public class PIP_webcam : MonoBehaviour
{
    public RawImage image = null; 
    public byte[] imageBytes = null;
    //[SerializeField] string IPAddressString = "192.168.0.1"; //probably not needed
    [SerializeField] int Port = 9999;
    WebSocketServer webSocketServer;
    bool lockObj = true; 
    Texture2D myTexture;
    // Use this for initialization
    void Start ()
    {
        InitAndStartServer();
        //Get Raw Image Reference
        image = gameObject.GetComponent<RawImage>();
        myTexture = new Texture2D(426,240);
    }

    void InitAndStartServer()
    {
        webSocketServer = new WebSocketServer(Port);
        //this constructor doesn't take an ip address
        webSocketServer.AddWebSocketService<MyWebSocketBehavior>("/",() => new MyWebSocketBehavior(this));
        Debug.Log("Starting websocket server at " + webSocketServer.Address.ToString() + " : " + webSocketServer.Port );
        webSocketServer.Start();
    }
 
 
    private void OnDestroy()
    {
        webSocketServer.Stop();
    }

    void Update()
    {
        //Change The Image
        if(imageBytes!=null&&!lockObj){
            myTexture.LoadImage(imageBytes);
            myTexture.Apply();  
            image.enabled = true;      
            image.texture = myTexture;
            lockObj = true;
        }
    }
    public void func(byte[] fromServer){
        lockObj = true;
        imageBytes = fromServer;
        fromServer = null;
        lockObj = false;
    }
    
}
public class MyWebSocketBehavior : WebSocketBehavior
{
    PIP_webcam pip;
    byte[] bytes = null;
    public MyWebSocketBehavior(PIP_webcam _pip){
        pip = _pip;
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        string receivedString = Encoding.UTF8.GetString(e.RawData);
        bytes = Convert.FromBase64String(receivedString);
        pip.func(bytes);
        bytes = null;
    }
    
    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log("MyWebSocketBehavior OnClose - " + e.Reason);
    }
 
    protected override void OnError(ErrorEventArgs e)
    {
        Debug.LogError("MyWebSocketBehavior OnError - " + e.Message);
    }
 
    protected override void OnOpen()
    {      
        Debug.Log("MyWebSocketBehavior OnOpen");
    }
}


