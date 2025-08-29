using UnityEngine;

public class PillarsMover : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // units/sec to the left

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
