using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYPieceWASDMover : MonoBehaviour
    {
        [SerializeField]
        private Transform _pieceObject;
        [SerializeField]
        private float _moveStep = 10f;

        public void MoveForward()
        {
            _pieceObject.Translate(new Vector2(_moveStep, 0));
        }

        public void MoveBackward()
        {
            _pieceObject.Translate(new Vector2(-_moveStep, 0));
        }

        public void MoveUp()
        {
            _pieceObject.Translate(new Vector2(0, _moveStep));
        }

        public void MoveDown()
        {
            _pieceObject.Translate(new Vector2(0, -_moveStep));
        }
    }
}