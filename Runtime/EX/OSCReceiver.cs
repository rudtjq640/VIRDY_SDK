using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;
using UnityEngine.Playables;
using VIRDY.SDK;
using Fusion;
public class OSCReceiver : MonoBehaviour
{
    [SerializeField]
    private uOscServer _server;
    [SerializeField]
    private VIRDYGlobalFunction _globalFunction;

    private void OnEnable()
    {
        _server.onDataReceived.AddListener(OnDataReceived);
    }

    private void OnDisable()
    {
        _server.onDataReceived.RemoveListener(OnDataReceived);
    }

    private void OnDataReceived(Message message)
    {
        if (message.values.Length == 0) _globalFunction.RPC_ExecuteFunction(message.address);
        else if (message.values.Length == 1)
        {
            if (message.values[0] is int vi) _globalFunction.RPC_ExecuteFunction(message.address, vi);
            else if (message.values[0] is string vs) _globalFunction.RPC_ExecuteFunction(message.address, vs);
        }
        //_globalFunction.RPC_ExecuteFunction(message.address);
    }
}