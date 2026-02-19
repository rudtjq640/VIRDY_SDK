using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    public class VIRDYPickupId : MonoBehaviour
    {
        public static List<VIRDYHandBoneId> HandBones = new List<VIRDYHandBoneId>();

        private void Awake()
        {
            HandBones = new List<VIRDYHandBoneId>();
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

        private int _actorId { get; set; }

        private bool _isHeld { get; set; }

        private VIRDYHandBoneId _currentHandBone;

        public void Update()
        {
            if (_currentHandBone != null)
            {
                if (!_isHeld)
                {
                    this.transform.SetParent(_currentHandBone.transform);
                    _isHeld = true;
                    this.transform.SetLocalPositionAndRotation(_localPosition, _localRotation);
                }
            }
            else
            {
                if (_isHeld)
                {
                    this.transform.SetParent(null);
                    _isHeld = false;
                    this.transform.SetPositionAndRotation(_resetPosition, _resetRotation);
                }
            }
        }

        public void Pickup(int actorId)
        {
            _actorId = actorId;
            _currentHandBone = HandBones.Where(bone => bone.PickupHand == _pickupHand && bone.Id == _actorId).FirstOrDefault();
            _isHeld = false;
        }

        public void Drop()
        {
            _actorId = -1;
            _currentHandBone = null;
        }

        public enum PickupHand
        {
            Left = 0,
            Right = 1
        }
    }
}
