using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;
    public static Action OnStartRewind;
    public static Action OnTimeStart;
    public static Action OnTimeStoped;

    [SerializeField] private float rewindSpeed; 
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private AudioSource startSourse, stopSourse, rewindSourse;
    [SerializeField] private Gift gift;
    [SerializeField] private GiftEventTrigger giftEvent;

    private bool isRewinding = false;
    private bool isPaused = false;

    private List<TimeTrackedObject> trackedObjects = new List<TimeTrackedObject>(); 
    private float timeSinceLastRecord = 0f;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        TimeTrackedObject[] objects = FindObjectsOfType<TimeTrackedObject>();
        foreach (var obj in objects)
        {
            AddTrackedObject(obj);
        }
        WinGameTrigger.OnGameEnd += ClearActions; 
    }

    private void Update()
    {
        if (isPaused && !isRewinding) return;

        if (isRewinding)
        {
            RewindTime();
        }
        else
        {
            RecordStates();
        }
    }

    public void StartTime()
    {
        giftEvent.DeactivateHandler();
        OnTimeStart.Invoke();
        isPaused = false;
        isRewinding = false;
        Time.timeScale = 1f; 
        startSourse.Play();
    }

    public void PauseTime()
    {
        giftEvent.DeactivateHandler();
        isPaused = true;
        isRewinding = false;
        Time.timeScale = 0f;
        stopSourse.Play();
        OnTimeStoped.Invoke();
    }

    public void RewindTime()
    {
        if (!isRewinding)
        {
            OnStartRewind.Invoke();
            rewindSourse.Play();
            isRewinding = true;
            giftEvent.DeactivateHandler();
            Time.timeScale = 1f;
        }
        foreach (var obj in trackedObjects)
        {
            obj.Rewind(rewindSpeed);
        }

        CheckIfAllObjectsRewound(); 
    }

    public void StopRewind()
    {
        isRewinding = false;
    }

    private void CheckIfAllObjectsRewound()
    {
        bool allRewound = true;

        foreach (var obj in trackedObjects)
        {
            if (!obj.HasRewoundToStart())
            {
                allRewound = false;
                break;
            }
        }

        if (allRewound)
        {
            StopRewind();
            PauseTime(); 
        }
    }
    private void RecordStates()
    {
        timeSinceLastRecord += Time.deltaTime;

        if (timeSinceLastRecord >= recordInterval)
        {
            foreach (var obj in trackedObjects)
            {
                obj.RecordState();
            }
            timeSinceLastRecord = 0f;
        }
    }

    public void AddTrackedObject(TimeTrackedObject obj)
    {
        if (!trackedObjects.Contains(obj))
        {
            trackedObjects.Add(obj);
        }
    }
    public void RemoveTrackedObject(TimeTrackedObject obj)
    {
        if (trackedObjects.Contains(obj))
        {
            trackedObjects.Remove(obj);
        }
    }
    private void ClearActions()
    {
        OnStartRewind = null;
        OnTimeStart = null;
        OnTimeStoped = null;
    }
}

