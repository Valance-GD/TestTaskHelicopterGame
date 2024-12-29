using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform platform;
    [SerializeField] private float speed = 2f;

    private Transform targetPoint;
    private bool _canMove = true;

    private void Start()
    {
        targetPoint = pointA;
        TimeController.OnStartRewind += CantMove;
        TimeController.OnTimeStart += CanMove;
    }

    private void Update()
    {
        if (!_canMove) return;
        MoveTowardsTarget();
    }
    private void CanMove()
    {
        _canMove = true;
    }
    public void CantMove()
    {
        _canMove = false;
    }
    private void MoveTowardsTarget()
    {
        platform.position = Vector3.MoveTowards(platform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(platform.position, targetPoint.position) < 0.01f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }
}
