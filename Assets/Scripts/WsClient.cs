using UnityEngine;
using WebSocketSharp;
using System.Collections.Concurrent;
using System;

public class WsClient : MonoBehaviour
{
    WebSocket ws;
    int x = 0;
    public GameObject wand;

    // A thread-safe Queue (first in first out)
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>(); 

    private void Start()
    {
        wand = GameObject.FindGameObjectWithTag("Wand");
        ws = new WebSocket("ws://localhost:8000");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from server"+", Data : "+e.Data);
            int.TryParse(e.Data, out x);
            // Dispatch into the Unity main thread's next Update routine
            _actions.Enqueue(() => wand.GetComponent<Spell>().Shoot(x));
        };
    }
private void Update()
    {
        if(ws == null)
        {
            return; 
        }
        if(Input.GetButtonDown("Fire1"))
        {
            if(!ws.IsAlive){
                ws.Connect();                
            }
            ws.Send("Hello");
        }  
        
        // Work the dispatched actions on the Unity main thread
        while(_actions.Count > 0)
        {
            if(_actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}