using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    public class VIRDYGlobalFunction : VIRDYNetworkBehaviour
    {
        public List<GlobalFunction> Functions = new List<GlobalFunction>();

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        public void RPC_ExecuteFunction(string key)
        {
            var function = Functions.Find(f => f.Key == key);
            if (function != null) function.Function?.Invoke();
        }
    }

    [Serializable]
    public class GlobalFunction
    {
        public string Key;

        public UnityEvent Function = new UnityEvent();
    }
}
