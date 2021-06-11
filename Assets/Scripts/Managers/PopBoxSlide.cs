using UnityEngine;

namespace Managers {
    public class PopBoxSlide : MonoBehaviour {
        private Vector3 endOfSlideSpace;
        private Vector3 slideSpace;
        private Vector3 slideSpaceUnitVector;
        private Vector3 spawnPoint;
        private int distance;
        private int maxDistance;
        private float actualSpeed;
    
        public GameObject popBox;
        public float speed = 10;


        private void Start() {
            spawnPoint = popBox.transform.position;

            slideSpace = transform.lossyScale - new Vector3(2, 2, 2);
            slideSpaceUnitVector = slideSpace / slideSpace.magnitude;

            endOfSlideSpace = spawnPoint + slideSpace;
            maxDistance = Mathf.CeilToInt((spawnPoint - endOfSlideSpace).magnitude);

            actualSpeed = speed / 100;

            Debug.Log($"{slideSpace} {endOfSlideSpace}");
        }

        private void FixedUpdate() {
            // Change the direction only if the pop box has reached a boundary
            var position = popBox.transform.position;
            distance = Mathf.CeilToInt((position - endOfSlideSpace).magnitude + (position - spawnPoint).magnitude);
            if (distance > maxDistance) actualSpeed *= -1;
            popBox.transform.position += actualSpeed * slideSpaceUnitVector;
        }
    }
}