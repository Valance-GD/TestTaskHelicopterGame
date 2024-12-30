using System;
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
    [SerializeField] private RectTransform movablePanel;
    [SerializeField] private float sensitivity = 1.0f;

    private Vector3[] trajectoryPoints = new Vector3[50];
    private float dropWay = 2;
    private bool isDropped = false;
    private bool isActive = false;
    private Animator giftAnimator;
    private AudioSource sourse;

    private void Start()
    {
        WinGameTrigger.OnGameEnd += ClearAction;
        sourse = GetComponent<AudioSource>();
        giftAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isActive)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
#endif

#if UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
#endif
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                movablePanel, Input.mousePosition, null, out localPoint);

            if (movablePanel.rect.Contains(localPoint))
            {
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                float normalizedDelta = (mouseDelta.x / movablePanel.rect.width) * sensitivity; 
                timelineSlider.value = Mathf.Clamp01(timelineSlider.value + normalizedDelta);
                ShowHelicopterGhost();
                ShowTrajectory();
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                movablePanel, touch.position, null, out localPoint);

            if (movablePanel.rect.Contains(localPoint)) 
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.deltaPosition;
                    float normalizedDelta = (delta.x / movablePanel.rect.width) * sensitivity; 
                    timelineSlider.value = Mathf.Clamp01(timelineSlider.value + normalizedDelta);
                    ShowHelicopterGhost();
                    ShowTrajectory();
                }
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive)
        {
            ActivateHandler();
        }
        else
        {
            DeactivateHandler();
        }
    }

    private void ActivateHandler()
    {
        isActive = true;
        giftAnimator.SetBool("Drag", true);
        sourse.Play();
        ShowHelicopterGhost();
        ShowTrajectory();
    }

    public void DeactivateHandler()
    {
        isActive = false;
        giftAnimator.SetBool("Drag", false);
        giftGhost.GetComponent<LineRenderer>().enabled = false;
        helicopterGhost.gameObject.SetActive(false);
        sourse.Stop();
    }



    private void ShowTrajectory()
    {
        LineRenderer trajectoryLine = giftGhost.GetComponent<LineRenderer>();
        trajectoryLine.positionCount = trajectoryPoints.Length;

        Vector3 startPosition = giftGhost.position;
        if (timelineSlider.value > 0.5) dropWay = -2;
        else dropWay = 2;
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

    private void ClearAction()
    {
        TriggerGiftDrop = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Time") && !isDropped)
        {
            TriggerGiftDrop?.Invoke();
            isDropped = true;
            DeactivateHandler();
        }
    }
}
