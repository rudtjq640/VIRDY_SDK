using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYHandBone : MonoBehaviour
    {
        public int Id;

        public VIRDYPickup.PickupHand PickupHand;

        private void Awake()
        {
            VIRDYPickup.HandBones.Add(this);
        }

        private void OnDestroy()
        {
            foreach (Transform child in this.transform)
                child.SetParent(null);

            VIRDYPickup.HandBones.Remove(this);
        }
    }
}
