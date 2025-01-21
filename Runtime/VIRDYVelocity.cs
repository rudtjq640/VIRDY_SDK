using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkRigidbody))]
    [RequireComponent(typeof(Rigidbody))]
    public class VIRDYVelocity : VIRDYBehaviour
    {
        public Vector3 Force;

        public Vector3 Torque;

        public bool IsRandom = false;

        private Rigidbody _rigidbody;

        protected override void OnInitialize()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        public void RPC_SetVelocity()
        {
            _rigidbody.velocity = Force;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        public void RPC_AddTorque()
        {
            if (IsRandom)
            {
                Vector3 randomTorque = new Vector3(
                    Random.Range(-Torque.x, Torque.x),
                    Random.Range(-Torque.y, Torque.y),
                    Random.Range(-Torque.z, Torque.z)
                );
                _rigidbody.AddTorque(randomTorque);
            }
            else
            {
                _rigidbody.AddTorque(Torque);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        public void RPC_ClearVelocity()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
