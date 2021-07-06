using System;
using System.Collections;
using UnityEngine;

namespace Managers {
    public class TurretManager : MonoBehaviour {
        public Transform ballTransform;
        public GameObject laser;
        public Renderer diamondRendererOne;
        public Renderer diamondRendererTwo;
        public Renderer laserRenderer;
        public Material greendot;
        public Material reddot;
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
            
            void DeactivateLaser() {
                laser.SetActive(false);
                laser.tag = "Untagged";
                laserRenderer.material = greendot;
                
                diamondRendererOne.material = diamondUncharged;
                diamondRendererTwo.material = diamondUncharged;

                thickness = aimingThickness;
                startTime = Mathf.Infinity;
            }

            if (Physics.Raycast(position, offset, out RaycastHit initialHit, Mathf.Infinity) &&
                initialHit.collider.CompareTag("Player")) {
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
                            laserRenderer.material = reddot;
                            thickness = firingThickness;
                            laser.tag = "Pop";
                            diamondRendererOne.material = diamondFiring;
                            diamondRendererTwo.material = diamondFiring;
                        }

                        laser.transform.localScale = new Vector3(thickness, hit.distance * 10, thickness);
                    }
                } else {
                    DeactivateLaser();
                    target.y = transform.position.y;
                }
            } else DeactivateLaser();

            transform.LookAt(target);
        }
    }
}