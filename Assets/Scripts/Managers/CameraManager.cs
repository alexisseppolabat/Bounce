using System.Collections.Generic;
using Structural;
using UnityEngine;

namespace Managers {
    public class CameraManager : MonoBehaviour {
        public UIManager uiManager;
        public Transform ballTransform;
        public Material ballMaterial;

        private static readonly HashSet<string> IgnoreList = new HashSet<string>();
        
        private float initCamRad;
        private const float OneDegreeInRad = Mathf.PI / 180f;
        private const float MAXVerticalAngle = 85 * OneDegreeInRad;
        private float cameraRadius;
        private float horizontalCameraAngle = 1.5f * Mathf.PI;
        private float verticalCameraAngle = 0.25f * Mathf.PI;

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
            // Calculate the position of the camera relative to the ball
            Vector3 relativePos = transform.position - ballTransform.position;

            // Determine if the view from the camera to the ball has been obstructed
            if (Physics.Raycast(ballTransform.position, relativePos, out RaycastHit hit,
                initCamRad + 0.5f, 1, QueryTriggerInteraction.Ignore)) {
                if (!IgnoreList.Contains(hit.collider.tag))
                    cameraRadius = Mathf.Clamp(hit.distance - 0.5f, 1.5f, initCamRad);
            } else
                cameraRadius = initCamRad;
            
            // If the camera is too close to the ball then make the ball transparent,
            // so that the player can see beyond the ball
            if (cameraRadius <= 2f)
                StandardShaderUtils.SetTransparency(ballMaterial, Mathf.Clamp((cameraRadius - 1.5f) / 0.5f, 0.4f, 1f));
            else
                StandardShaderUtils.ChangeRenderMode(ballMaterial, BlendMode.Opaque);
        }
        
        private void SetInitialCameraRadius() {
            // This determines the distance between the camera and the player (ballTransform.position)
            initCamRad = (transform.position - ballTransform.position).magnitude;
            cameraRadius = initCamRad;
        }

        private void Start() {
            foreach (string ignoreTag in new [] {"Deflator", "Fly Press", "Fly", "Health", "Pop", "Inflator", "Actual Ring", "Speed"})
                IgnoreList.Add(ignoreTag);

            SetInitialCameraRadius();
        }

        private void FixedUpdate() {
            UpdateCameraPosition();
        }

        private void OnTriggerEnter(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) {
                if (!Player.limitedLives)
                    uiManager.ActivateWaterEffect();
                else if (Player.lives != 0)
                    uiManager.ActivateWaterEffect();
                else uiManager.DeactivateWaterEffect();
            }
        }

        private void OnTriggerExit(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) {
                if (!Player.limitedLives)
                    uiManager.DeactivateWaterEffect();
                else if (Player.lives != 0)
                    uiManager.DeactivateWaterEffect();
                else uiManager.ActivateWaterEffect();
            }
        }

        private void OnTriggerStay(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) {
                if (!Player.limitedLives)
                    uiManager.ActivateWaterEffect();
                else if (Player.lives != 0)
                    uiManager.ActivateWaterEffect();
                else uiManager.DeactivateWaterEffect();
            }
        }
    }
}