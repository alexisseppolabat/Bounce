using Structural;
using UnityEngine;

namespace Managers {
    public class PlayerMovementManager : MonoBehaviour {
        public const float Gravity = 20f;

        public Rigidbody playerBody;
        public Transform cameraTransform;

        private Transform originalParent;
        private float influence;
        private const float BaseInfluence = 2f;
        private float bounceMultiplier;
        private bool canJump;
        private bool isBouncing;
        private bool spacePressed;

        private float getSpeed() {
            return Time.deltaTime * Player.speed;
        }

        private void CheckJump(Collision collision) {
            int normalDirection = (int) Mathf.Round(collision.GetContact(collision.contactCount - 1).normal.y);

            // The player can only jump exclusively when their flying and touching the ceiling
            // or not flying and touching the ground
            bool isNormalAndCanJump = Mathf.Abs(normalDirection) == 1 &&
                                      (normalDirection > 0 ^ (Player.ActiveEffects.Contains(Effects.Fly) && !Player.ActiveEffects.Contains(Effects.FlyPress)));

            if (isNormalAndCanJump) {
                influence = Mathf.Sign(normalDirection);
                bounceMultiplier = Mathf.Abs(bounceMultiplier) * Mathf.Sign(normalDirection);
                if (isBouncing && spacePressed) {
                    // Only allow the bounce multiplier to increase if the ball is bouncing on rubber
                    bool isRubber = collision.collider.CompareTag("Rubber");
                    bounceMultiplier = isRubber ? bounceMultiplier + influence * 1.1f : gameObject.transform.localScale.x * influence * BaseInfluence;
                } else if (!spacePressed) {
                    // Reset the bounce multiplier if the ball hits the ground/ceiling and space isn't being pressed
                    bounceMultiplier = gameObject.transform.localScale.x * influence * BaseInfluence;
                }
            }

            canJump = isNormalAndCanJump || canJump;
        }


        private void Start() {
            originalParent = transform.parent;
            
            Vector3 position = playerBody.transform.position;
            Player.spawnPoint.Set(
                position.x,
                position.y,
                position.z
            );
            
            Physics.gravity = new Vector3(0, -Gravity, 0);
            bounceMultiplier = gameObject.transform.localScale.x * BaseInfluence;
        }

        private void FixedUpdate() {
            spacePressed = Input.GetKey("space");

            Vector3 eulerAngles = cameraTransform.eulerAngles;
            transform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
        
            playerBody.velocity = transform.TransformDirection(new Vector3(
                Input.GetKey("a") ? -getSpeed() : Input.GetKey("d") ? getSpeed() : 0,
                spacePressed && canJump ? Time.deltaTime * Player.NormalSpeed * bounceMultiplier : playerBody.velocity.y,
                Input.GetKey("s") ? -getSpeed() : Input.GetKey("w") ? getSpeed() : 0
            ));
        }

        private void OnCollisionEnter(Collision collision) {
            CheckJump(collision);
        }
        private void OnCollisionStay(Collision collision) {
            // The ball is not bouncing if it is touching its ground (which can be either the ceiling or floor 
            // depending on whether the ball is flying or not, which will determine the sign of the influence variable)
            if ((int) Mathf.Round(collision.GetContact(collision.contactCount - 1).normal.y) == Mathf.Sign(influence))
                isBouncing = false;
            CheckJump(collision);
        }
        private void OnCollisionExit(Collision collision) {
            transform.SetParent(originalParent);
            
            // The ball cannot jump and is thus considered to be bouncing if it isn't touching anything or
            // has just left its ground (which, again, can be either the floor or the ceiling)
            if (
                collision.contactCount == 0 ||
                (int) Mathf.Round(collision.GetContact(collision.contactCount - 1).normal.y) == Mathf.Sign(influence)
            ) {
                canJump = false;
                isBouncing = true;
            }
        }
    }
}