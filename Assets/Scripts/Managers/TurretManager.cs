using System.Collections;
using UnityEngine;

namespace Managers {
    public class TurretManager : MonoBehaviour {
        public Transform ballTransform;
        public Material laser;
        public float lag = 0.2f;
        public float chargeTime = 1.25f;

        private bool isAbove;

        public Transform aim;
        public Transform fire;
        
        private void FixedUpdate() {
            StartCoroutine(nameof(UpdatePosition));
        }


        private IEnumerator UpdatePosition() {
            Vector3 position = transform.position;
            Vector3 target = ballTransform.position;
            Vector3 offset = target - position;

            yield return new WaitForSeconds(lag);
            
            if (target.y >= transform.position.y) {
                transform.LookAt(target);
                
                if (Physics.Raycast(position, offset, out RaycastHit hit, Mathf.Infinity)) {
                    DrawLine(position, hit.point, Color.red);
                    aim.position = hit.point;
                    isAbove = true;
                    yield return new WaitForSeconds(chargeTime);
                    

                    // Update position again and ensure the ball hasn't dropped too far
                    target = ballTransform.position;
                    if (target.y < transform.position.y) yield break;
                    
                    yield return new WaitForSeconds(lag);
                    if (target.y >= transform.position.y && Physics.Raycast(position, offset, out hit, Mathf.Infinity)) {
                        DrawLine(position, hit.point, Color.red, 0.2f);
                        fire.position = hit.point;
                    }
                }
            } else {
                isAbove = false;
                target.y = transform.position.y;
                transform.LookAt(target);
            }
        }
        
        private void DrawLine(Vector3 start, Vector3 end, Color color, float thickness = 0.01f) {
            GameObject myLine = new GameObject();
            
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            
            LineRenderer renderer = myLine.GetComponent<LineRenderer>();
            
            renderer.material = laser;
            renderer.SetColors(color, color);
            renderer.SetWidth(thickness, thickness);
            renderer.SetPosition(0, start);
            renderer.SetPosition(1, end);
            
            Destroy(myLine, 0.01f);
        }
    }
}