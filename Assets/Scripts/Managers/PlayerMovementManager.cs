using Structural;
using UnityEngine;

namespace Managers {
    public class PlayerMovementManager : MonoBehaviour {
        public const float GRAVITY = 20f;

        public Rigidbody playerBody;
        public Transform cameraTransform;

        private float initCamRad;
        private float cameraRadius;
        private float horizontalCameraAngle = 1.5f * Mathf.PI;
        private float verticalCameraAngle = 0.25f * Mathf.PI;
        private const float ONE_DEGREE_IN_RAD = (Mathf.PI / 180f);
        private const float MAX_VERTICAL_ANGLE = 85 * ONE_DEGREE_IN_RAD;
        private float influence;
        private const float BASE_INFLUENCE = 2f;
        private float bounceMultiplier;
        private bool canJump = false;
        private bool isBouncing = false;
        private bool spacePressed = false;

        private float getSpeed() {
            return Time.deltaTime * Player.speed;
        }

        public void setInitialCameraRadius() {
            // This determines the distance between the camera and the player (transform.position)
            initCamRad = (cameraTransform.position - transform.position).magnitude;
            cameraRadius = initCamRad;
        }

        private void checkJump(Collision collision) {
            int nomralDirection = (int) Mathf.Round(collision.GetContact(collision.contactCount - 1).normal.y);

            // The player can only jump exclusively when their flying and touching the ceiling
            // or not flying and touching the ground
            bool isNormalAndCanJump = (int) Mathf.Abs(nomralDirection) == 1 &&
                                      (nomralDirection > 0 ^ (Player.ActiveEffects.Contains(Effects.Fly) && !Player.ActiveEffects.Contains(Effects.FlyPress)));

            if (isNormalAndCanJump) {
                influence = Mathf.Sign(nomralDirection);
                bounceMultiplier = Mathf.Abs(bounceMultiplier) * Mathf.Sign(nomralDirection);
                if (isBouncing && spacePressed) {
                    // Only allow the bounce multiplier to increase if the ball is bouncing on rubber
                    bool isRubber = collision.collider.tag == "Rubber";
                    bounceMultiplier = isRubber ? bounceMultiplier + influence * 1.1f : gameObject.transform.localScale.x * influence * BASE_INFLUENCE;
                } else if (!spacePressed) {
                    // Reset the bounce multiplier if the ball hits the ground/ceiling and space isn't being pressed
                    bounceMultiplier = gameObject.transform.localScale.x * influence * BASE_INFLUENCE;
                }
            }

            canJump = isNormalAndCanJump ? true : canJump;
        }

        private void updateCameraPosition() {
            // Make the camera focus on the player
            cameraTransform.LookAt(transform);

            // Change the horizontal and vertical camera angles according to the arrow keys pressed
            horizontalCameraAngle += Input.GetKey(KeyCode.LeftArrow) ? -ONE_DEGREE_IN_RAD : Input.GetKey(KeyCode.RightArrow) ? ONE_DEGREE_IN_RAD : 0;
            verticalCameraAngle += Input.GetKey(KeyCode.UpArrow) ? ONE_DEGREE_IN_RAD : Input.GetKey(KeyCode.DownArrow) ? -ONE_DEGREE_IN_RAD : 0;

            // Prevent camera from being able to pitch higher than 85 degrees
            verticalCameraAngle = Mathf.Abs(verticalCameraAngle) > MAX_VERTICAL_ANGLE ?
                Mathf.Sign(verticalCameraAngle) * MAX_VERTICAL_ANGLE : verticalCameraAngle;

            // Determine if there are any objects obscuring the camera's view and
            // if so, update the camera's distance from the ball accordingly
            calcCameraRadius();

            // Update the camera's position in the sphere
            cameraTransform.position = transform.position + new Vector3(
                cameraRadius * Mathf.Cos(horizontalCameraAngle) * Mathf.Cos(verticalCameraAngle),
                cameraRadius * Mathf.Sin(verticalCameraAngle),
                cameraRadius * Mathf.Sin(horizontalCameraAngle) * Mathf.Cos(verticalCameraAngle)
            );
        }

        private void calcCameraRadius() {
            Vector3 relativePos = cameraTransform.position - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, relativePos, out hit, initCamRad + 0.5f, 1, QueryTriggerInteraction.Ignore))
                cameraRadius = Mathf.Clamp(hit.distance, 0, initCamRad);
            else
                cameraRadius = initCamRad;
        }


        private void Start() {
            Player.spawnPoint.Set(
                playerBody.transform.position.x,
                playerBody.transform.position.y,
                playerBody.transform.position.z
            );

            setInitialCameraRadius();
            Physics.gravity = new Vector3(0, -GRAVITY, 0);
            bounceMultiplier = gameObject.transform.localScale.x * BASE_INFLUENCE;
        }

        private void FixedUpdate() {
            spacePressed = Input.GetKey("space");

            transform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        
            playerBody.velocity = transform.TransformDirection(new Vector3(
                Input.GetKey("a") ? -getSpeed() : Input.GetKey("d") ? getSpeed() : 0,
                spacePressed && canJump ? Time.deltaTime * Player.NormalSpeed * bounceMultiplier : playerBody.velocity.y,
                Input.GetKey("s") ? -getSpeed() : Input.GetKey("w") ? getSpeed() : 0
            ));

            updateCameraPosition();
        }

        private void OnCollisionEnter(Collision collision) { checkJump(collision); }
        private void OnCollisionStay(Collision collision) {
            // The ball is not bouncing if it is touching its ground (which can be either the ceiling or floor 
            // depending on whether the ball is flying or not, which will determine the sign of the influence variable)
            if ((int) Mathf.Round(collision.GetContact(collision.contactCount - 1).normal.y) == Mathf.Sign(influence))
                isBouncing = false;
            checkJump(collision);
        }
        private void OnCollisionExit(Collision collision) {
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