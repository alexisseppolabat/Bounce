using System;
using System.Collections.Generic;
using Structural;
using UnityEngine;

namespace Managers {
    public class CameraManager : MonoBehaviour {
        public UIManager uiManager;
        public Transform ballTransform;
        
        private float initCamRad;
        private const float OneDegreeInRad = (Mathf.PI / 180f);
        private const float MAXVerticalAngle = 85 * OneDegreeInRad;
        private float cameraRadius;
        private float horizontalCameraAngle = 1.5f * Mathf.PI;
        private float verticalCameraAngle = 0.25f * Mathf.PI;
        
        private void Check(Collision collision) {
            List<ContactPoint> contacts =  new List<ContactPoint>();
            collision.GetContacts(contacts);

            contacts.ForEach(contact => {
                Vector3 normal = transform.TransformDirection(contact.normal);
                // Ignore random empty collision normal vectors
                if (normal.magnitude != 0) {
                    Debug.Log($"{Mathf.RoundToInt(normal.x)} {Mathf.RoundToInt(normal.y)} {Mathf.RoundToInt(normal.z)}");
                }
            });
        }

        private void UpdateCameraPosition() {
            // Make the camera focus on the player
            transform.LookAt(ballTransform);

            // Change the horizontal and vertical camera angles according to the arrow keys pressed
            horizontalCameraAngle += Input.GetKey(KeyCode.LeftArrow) ? -OneDegreeInRad : Input.GetKey(KeyCode.RightArrow) ? OneDegreeInRad : 0;
            verticalCameraAngle += Input.GetKey(KeyCode.UpArrow) ? OneDegreeInRad : Input.GetKey(KeyCode.DownArrow) ? -OneDegreeInRad : 0;

            // Prevent camera from being able to pitch higher than 85 degrees
            verticalCameraAngle = Mathf.Abs(verticalCameraAngle) > MAXVerticalAngle ?
                Mathf.Sign(verticalCameraAngle) * MAXVerticalAngle : verticalCameraAngle;

            // Determine if there are any objects obscuring the camera's view and
            // if so, update the camera's distance from the ball accordingly
            CalcCameraRadius();

            // Update the camera's position in the sphere
            transform.position = ballTransform.position + new Vector3(
                cameraRadius * Mathf.Cos(horizontalCameraAngle) * Mathf.Cos(verticalCameraAngle),
                cameraRadius * Mathf.Sin(verticalCameraAngle),
                cameraRadius * Mathf.Sin(horizontalCameraAngle) * Mathf.Cos(verticalCameraAngle)
            );
        }

        private void CalcCameraRadius() {
            Vector3 relativePos = transform.position - ballTransform.position;

            RaycastHit hit;
            if (Physics.Raycast(ballTransform.position, relativePos, out hit, initCamRad + 0.5f, 1, QueryTriggerInteraction.Ignore))
                cameraRadius = Mathf.Clamp(hit.distance, 0, initCamRad);
            else
                cameraRadius = initCamRad;
        }
        
        private void SetInitialCameraRadius() {
            // This determines the distance between the camera and the player (ballTransform.position)
            initCamRad = (transform.position - ballTransform.position).magnitude;
            cameraRadius = initCamRad;
        }

        private void Start() {
            SetInitialCameraRadius();
        }

        private void FixedUpdate() {
            UpdateCameraPosition();
        }

        private void OnTriggerEnter(Collider triggerCollider) {
            if (Player.lives != 0) {
                if (triggerCollider.CompareTag("Water")) uiManager.ActivateWaterEffect();
            } else uiManager.DeactivateWaterEffect();
        }

        private void OnTriggerExit(Collider triggerCollider) {
            if (Player.lives != 0) {
                if (triggerCollider.CompareTag("Water")) uiManager.DeactivateWaterEffect();
            } else uiManager.DeactivateWaterEffect();
        }

        private void OnTriggerStay(Collider triggerCollider) {
            if (Player.lives != 0) {
                if (triggerCollider.CompareTag("Water")) uiManager.ActivateWaterEffect();
            } else uiManager.DeactivateWaterEffect();
        }

        private void OnCollisionEnter(Collision collision) {
            Check(collision);
        }

        private void OnCollisionExit(Collision collision) {
            Check(collision);
        }
    }
}