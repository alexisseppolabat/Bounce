using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Managers {
    public class TurretManager : MonoBehaviour {
        public Transform ballTransform;
        
        private void FixedUpdate() {
            Vector3 target = ballTransform.position;
            target.y = Mathf.Max(target.y, transform.position.y);
            transform.LookAt(target);
        }
    }
}