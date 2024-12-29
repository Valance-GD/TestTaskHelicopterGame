using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GiftEventTrigger : MonoBehaviour, IPointerDownHandler
{
    public static Action TriggerGiftDrop;

    [SerializeField] private Slider timelineSlider;
     

    [SerializeField] private Transform giftGhost;
    [SerializeField] private Transform helicopterGhost;
    [SerializeField] private Transform helicopter;

    private Vector3[] trajectoryPoints = new Vector3[50];
    private float dropWay = 2;
    private bool isDropped = false;
    private Coroutine currentCor;
    private Animator giftAnimator;
    private AudioSource sourse;

    private void Start()
    {
        WinGameTrigger.OnGameEnd += ClearAction;
        sourse = GetComponent<AudioSource>();
        giftAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Time") && !isDropped)
        {
            TriggerGiftDrop?.Invoke();
            isDropped = true;
            DeactivateEvent();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentCor == null)
        {
            giftAnimator.SetBool("Drag", true);
            currentCor = StartCoroutine(CheckIsSliderMove());
            sourse.Play();
        }
        if (isDropped) return;

        ShowHelicopterGhost();
        if (timelineSlider.value >= 0.5)
        {
            dropWay = -2;
        }
        else
        {
            dropWay = 2;
        }
        ShowTrajectory();
    }

    public void OnSliderValueChanged()
    {
        if (currentCor == null)
        {
            giftAnimator.SetBool("Drag", true);
            currentCor = StartCoroutine(CheckIsSliderMove());
            sourse.Play();
        }

        if (isDropped) return;

        ShowHelicopterGhost();
        if (timelineSlider.value >= 0.5)
        {
            dropWay = -2;
        }
        else
        {
            dropWay = 2;
        }
        ShowTrajectory();   
    }
    private IEnumerator CheckIsSliderMove()
    {
        float lastSliderValue = timelineSlider.value;

        float elapsedTime = 0f;

        while (timelineSlider.value != lastSliderValue)
        {
            lastSliderValue = timelineSlider.value;
            elapsedTime = 0f; 
            yield return null; 
        }

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            if (timelineSlider.value != lastSliderValue)
            {
                elapsedTime = 0f;
                lastSliderValue = timelineSlider.value;
            }

            yield return null;  
        }

        OnSliderMovementFinished();
    }
    private void OnSliderMovementFinished()
    {
        giftAnimator.SetBool("Drag", false);
        DeactivateEvent();
        sourse.Stop();
        StopAllCoroutines();
        currentCor = null;
    }
    private void ShowTrajectory()
    {
        LineRenderer trajectoryLine = giftGhost.GetComponent<LineRenderer>();
        trajectoryLine.positionCount = trajectoryPoints.Length;

        Vector3 startPosition = giftGhost.position;
        Vector2 initialVelocity = new Vector2(dropWay, -2); 

        for (int i = 0; i < trajectoryPoints.Length; i++)
        {
            float time = i * 0.1f; 
            trajectoryPoints[i] = startPosition + new Vector3(
                initialVelocity.x * time,
                initialVelocity.y * time + 0.5f * Physics2D.gravity.y * time * time, 
                0);
        }

        trajectoryLine.SetPositions(trajectoryPoints);
        trajectoryLine.enabled = true;
    }

    private void ShowHelicopterGhost()
    {
        float giftDropTime = timelineSlider.value;
        Vector3 helicopterPosition = Vector3.Lerp(
            helicopter.GetComponent<HelicopterMove>().StartPoint,
            helicopter.GetComponent<HelicopterMove>().FinishPoint,
            giftDropTime
        );

        helicopterGhost.position = helicopterPosition;
        helicopterGhost.gameObject.SetActive(true);
    }
    public void SetIsDropped(bool isReturn)
    {
        isDropped = isReturn;
    }
    private void DeactivateEvent()
    {
        giftGhost.GetComponent<LineRenderer>().enabled = false;
        helicopterGhost.gameObject.SetActive(false);
    }
    private void ClearAction()
    {
        TriggerGiftDrop = null;
    }
}
