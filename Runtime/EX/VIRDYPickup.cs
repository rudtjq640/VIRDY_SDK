using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    public class VIRDYPickup : VIRDYNetworkBehaviour
    {
        public static List<VIRDYHandBone> HandBones = new List<VIRDYHandBone>();

        public static int ActorId = -1;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            HandBones = new List<VIRDYHandBone>();
        }

        [SerializeField]
        private Vector3 _localPosition;
        [SerializeField]
        private Quaternion _localRotation;

        [SerializeField]
        private PickupHand _pickupHand;
        
        [Space]

        [SerializeField]
        private Vector3 _resetPosition;
        [SerializeField]
        private Quaternion _resetRotation;

        [Networked]
        private int _actorId { get; set; }

        [Networked]
        private bool _isHeld { get; set; }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

#if VIRDY_CORE
            if (_isHeld)
            {
                var bone = HandBones.Where(bone => bone.PickupHand == _pickupHand && bone.Id == _actorId).FirstOrDefault();
                if (bone != null)
                {
                    this.transform.SetParent(bone.transform);
                    this.transform.SetLocalPositionAndRotation(_localPosition, _localRotation);
                }
            }
            else
            {
                this.transform.SetParent(null);
                this.transform.SetPositionAndRotation(_resetPosition, _resetRotation);
            }
#endif
        }

        public void Pickup()
        {
#if VIRDY_CORE
            Object.RequestStateAuthority();
#endif
            _actorId = ActorId;
            _isHeld = true;
        }

        public void Drop()
        {
#if VIRDY_CORE
            Object.RequestStateAuthority();
#endif
            _actorId = -1;
            _isHeld = false;
        }

        public enum PickupHand
        {
            Left = 0,
            Right = 1
        }

#if UNITY_EDITOR
        [ContextMenu("Set Local Transform")]
        public void SetLocalTransform()
        {
            _localPosition = this.transform.localPosition;
            _localRotation = this.transform.localRotation;
        }

        [ContextMenu("Set Reset Transform")]
        public void SetResetTransform()
        {
            _resetPosition = this.transform.position;
            _resetRotation = this.transform.rotation;
        }
#endif
    }
}
