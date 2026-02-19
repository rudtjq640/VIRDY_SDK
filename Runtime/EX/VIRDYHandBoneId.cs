using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYHandBoneId : MonoBehaviour
    {
        public int Id;

        public VIRDYPickupId.PickupHand PickupHand;

        private void Awake()
        {
            VIRDYPickupId.HandBones.Add(this);
        }

        private void OnDestroy()
        {
            foreach (Transform child in this.transform)
                child.SetParent(null);

            VIRDYPickupId.HandBones.Remove(this);
        }
    }
}
