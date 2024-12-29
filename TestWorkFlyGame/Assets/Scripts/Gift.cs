using UnityEngine;

public class Gift : MonoBehaviour
{
    [SerializeField] private Transform helicopter;
    [SerializeField] private GiftEventTrigger giftTrigger;

    private float timeSinceDrop = 0f; 
    private float delay = 0.3f;   
    private bool isDropped = false;

    private float dropWay =2;
    private Vector3 startPosition;
    private Vector3 inHelicopterPosition;
    
    private void Start()
    {
        inHelicopterPosition = transform.localPosition;
        GiftEventTrigger.TriggerGiftDrop += Drop;
        TimeController.Instance.RemoveTrackedObject(GetComponent<TimeTrackedObject>());
    }
    private void Update()
    {
        if (!isDropped) return;


        timeSinceDrop += Time.deltaTime;

        if (timeSinceDrop >= delay && Vector3.Distance(transform.position, startPosition) <= 0.2f)
        {
            GiftReturn();
        }
    }
    private void GiftReturn()
    {
        TimeController.Instance.RemoveTrackedObject(GetComponent<TimeTrackedObject>());
        GetComponent<SpriteRenderer>().sortingOrder = -1;
        GetComponent<Rigidbody2D>().simulated = false;
        transform.SetParent(helicopter);
        transform.localPosition = inHelicopterPosition;
        giftTrigger.SetIsDropped(false);
    }
    private void Drop()
    {
        TimeController.Instance.AddTrackedObject(GetComponent<TimeTrackedObject>());
        Rigidbody2D giftRigidbody = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        giftRigidbody.simulated = true;

        float initialSpeedX = dropWay;
        float initialSpeedY = -2;

        giftRigidbody.velocity = new Vector2(initialSpeedX, initialSpeedY);

        transform.SetParent(null);
        startPosition = transform.position;
        timeSinceDrop = 0f;
        isDropped = true;
    }
    public void ChangeDropWay(float value)
    {
        dropWay = value;
    }
}
