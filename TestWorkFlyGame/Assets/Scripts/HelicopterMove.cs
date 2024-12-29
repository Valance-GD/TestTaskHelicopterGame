using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HelicopterMove : MonoBehaviour
{
    [SerializeField] private Transform finishPoint;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Animator animator;
    [SerializeField] private Gift gift;
    private Vector3 startPoint;
    private bool _canMove = true;
    private bool isGameEnd = false;
    private AudioSource source;
    public Vector3 StartPoint => startPoint;
    public Vector3 FinishPoint => finishPoint.position;

    private void Start()
    {
        startPoint = transform.position;
        timeSlider.value = 0f;
        source = GetComponent<AudioSource>();
        GiftEventTrigger.TriggerGiftDrop += DropDoorAnimation;
        TimeController.OnStartRewind += StopMove;
        TimeController.OnTimeStart += StartMove;
        TimeController.OnTimeStoped += StopMusic;
        WinGameTrigger.OnGameEnd += ChangeVolumeMusic;
    }

    private void Update()
    {
        UpdateSlider();
        if (!_canMove) return;
        MoveTowardsTarget(); 
    }
    private void StartMove()
    {
        _canMove = true;
        source.pitch = 1;
        source.Play();
    }
    private void StopMove()
    {
        _canMove = false;
        source.pitch = 2;
        source.Play();
    }
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, finishPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, finishPoint.position) < 0.01f)
        {
            _canMove = false;
            if (isGameEnd) StopMusic();
        }
    }
    private void UpdateSlider()
    {
        float totalDistance = Vector3.Distance(startPoint, finishPoint.position);
        float currentDistance = Vector3.Distance(transform.position, finishPoint.position);

        timeSlider.value = 1 - (currentDistance / totalDistance);
        if (timeSlider.value >= 0.5)
        {
            gift.ChangeDropWay(-2);
        }
        else
        {
            gift.ChangeDropWay(2);
        }
    }
    private void DropDoorAnimation()
    {
        StartCoroutine(DropGiftProces());
    }
    private IEnumerator DropGiftProces()
    {
        animator.SetTrigger("Open");
        yield return new WaitForSeconds(0.7f);     
        animator.SetTrigger("Close");
    }
    private void StopMusic()
    {
        source.Stop();
    }
    private void ChangeVolumeMusic()
    {
        source.volume = 0.1f;
        isGameEnd = true;
    }
}
