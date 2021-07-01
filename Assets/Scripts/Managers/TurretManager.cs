using System;
using System.Collections;
using UnityEngine;

namespace Managers {
    public class TurretManager : MonoBehaviour {
        public Transform ballTransform;
        public GameObject laser;
        public Renderer diamondRendererOne;
        public Renderer diamondRendererTwo;
        public Material diamondUncharged;
        public Material diamondCharging;
        public Material diamondFiring;
        
        
        public float lag = 0.2f;
        public float chargeTime = 1.25f;
        public float firingThickness = 10f;
        public float aimingThickness = 1f;

        private float startTime;
        private float thickness;

        private void Start() {
            thickness = aimingThickness;
        }

        private void FixedUpdate() {
            StartCoroutine(nameof(UpdatePosition));
        }


        private IEnumerator UpdatePosition() {
            Vector3 position = transform.position;
            Vector3 target = ballTransform.position;
            Vector3 offset = target - position;

            yield return new WaitForSeconds(lag);
            
            if (target.y >= transform.position.y) {
                
                if (Physics.Raycast(position, offset, out RaycastHit hit, Mathf.Infinity)) {
                    if (!laser.activeSelf) {
                        laser.SetActive(true);
                        startTime = Time.time;
                        diamondRendererOne.material = diamondCharging;
                        diamondRendererTwo.material = diamondCharging;
                    }

                    if (Time.time - startTime >= chargeTime) {
                        thickness = firingThickness;
                        laser.tag = "Pop";
                        diamondRendererOne.material = diamondFiring;
                        diamondRendererTwo.material = diamondFiring;
                    }

                    laser.transform.localScale = new Vector3(thickness, hit.distance * 10, thickness);
                } 
            } else {
                laser.SetActive(false);
                laser.tag = "Untagged";
                diamondRendererOne.material = diamondUncharged;
                diamondRendererTwo.material = diamondUncharged;

                target.y = transform.position.y;
                thickness = aimingThickness;
                startTime = Mathf.Infinity;
            }

            transform.LookAt(target);
        }
    }
}