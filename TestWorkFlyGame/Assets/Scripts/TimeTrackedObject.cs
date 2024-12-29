using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrackedObject : MonoBehaviour
{
    private struct ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private Stack<ObjectState> stateHistory = new Stack<ObjectState>(); 

    private float rewindTimer = 0f;
    private Rigidbody2D rb; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void RecordState()
    {
        ObjectState state = new ObjectState
        {
            position = transform.position,
            rotation = transform.rotation
        };

        stateHistory.Push(state);
    }

    public void Rewind(float rewindSpeed)
    {
        if (stateHistory.Count > 0)
        {
            rewindTimer += Time.deltaTime * rewindSpeed; 

            if (rewindTimer >= 1f)
            {
                rewindTimer = 0f;

                ObjectState lastState = stateHistory.Pop(); 
                transform.position = lastState.position;
                transform.rotation = lastState.rotation;

                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }
    public bool HasRewoundToStart()
    {
        return stateHistory.Count == 0;
    }
}
