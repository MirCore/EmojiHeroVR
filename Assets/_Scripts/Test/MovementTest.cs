using UnityEngine;

namespace Test
{
    public class MovementTest : MonoBehaviour
    {
        public Vector3 Stop = new Vector3();
        public float Speed;
        public float XOffset = 20;
        public float YOffset = 0;

        private void Update()
        {
            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Stop, step);

            if (Vector3.Distance(transform.position, Stop) < 0.001f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
