using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    public class CameraManager : MonoBehaviour {
        public UIManager UIManager;

        private void OnTriggerEnter(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) UIManager.activateWaterEffect();
        }

        private void OnTriggerExit(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) UIManager.deactivateWaterEffect();
        }

        private void OnTriggerStay(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) UIManager.activateWaterEffect();
        }

        private void OnCollisionEnter(Collision collision) {
            Check(collision);
        }

        private void OnCollisionExit(Collision collision) {
            Check(collision);
        }

        private void Check(Collision collision) {
            List<ContactPoint> contacts =  new List<ContactPoint>();
            collision.GetContacts(contacts);

            contacts.ForEach(contact => {
                Vector3 normal = transform.TransformDirection(contact.normal);
                // Ignore random empty collision normal vectors
                if (normal.magnitude != 0) {
                    // Debug.Log($"{Mathf.RoundToInt(normal.x)} {Mathf.RoundToInt(normal.y)} {Mathf.RoundToInt(normal.z)}");
                }
            });
        }
    }
}