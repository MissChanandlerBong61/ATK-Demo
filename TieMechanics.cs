using UnityEngine;

public class TieMechanics : MonoBehaviour
{
    [SerializeField] private DistanceJoint2D distanceJoint;

    [Header("References")]
    public Transform tieVisual; 

    [Header("Settings")]
    public float tieLaunchSpeed = 20f;
    public float tieRetractSpeed = 40f;
    public float maxTieLength = 10f;

    private Vector2 direction;
    private float currentLength = 0f;
    private Vector3 tieOffset = Vector3.zero;

    private bool isExtending = false;
    private bool isAttached = false;
    private bool isRetracting = false;
    
    private GameObject player;
    private Rigidbody2D playerRb;
    

    public System.Action OnRetracted;

    void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        Invoke(nameof(EnableCollider), 0.05f);
    }

    void EnableCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    private void Update()
    {
        if (isAttached)
        {
            if (Input.GetMouseButtonUp(1))
            {
                DetachAndRetract();
            }
            else
            {
                UpdateAttachedVisual();
            }
        }
        else if (isExtending)
        {
            Extend();
        }
        else if (isRetracting)
        {
            Retract();
        }
    }

    public void StartTie(Vector2 start, Vector2 target, Rigidbody2D playerRb, Vector3 offset)
    {               
        this.player = playerRb.gameObject;
        this.playerRb = playerRb;
        this.tieOffset = offset;
        this.direction = (target - (Vector2)(player.transform.position + tieOffset)).normalized;

        transform.position = player.transform.position + tieOffset; 
        currentLength = 0f;
        isExtending = true;
        isRetracting = false;
        isAttached = false;

        if (distanceJoint != null)
        {
            distanceJoint.enabled = false;
            distanceJoint.connectedBody = null;
        }
        else
        {
            Debug.LogWarning("Missing DistanceJoint2D on tie prefab.");
        }

        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("Missing Rigidbody2D on Tie!");
        }
    }    

    private void Extend()
    {
        currentLength += tieLaunchSpeed * Time.deltaTime;

        if (currentLength >= maxTieLength || Input.GetMouseButtonUp(1))
        {
            StartRetract();
            return;
        }

        UpdateVisual();
    }

    private void StartRetract()
    {        
        isExtending = false;
        isRetracting = true;
    }
    public void Retract()

    {
        currentLength -= tieRetractSpeed * Time.deltaTime;

        if (currentLength <= 0.1f)
        {
            OnRetracted?.Invoke();
            Destroy(gameObject);
            return;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        transform.position = player.transform.position + tieOffset;
        
        tieVisual.localScale = new Vector3(Mathf.Max(currentLength, 0.01f), tieVisual.localScale.y, 1f);
        tieVisual.localPosition = new Vector3(currentLength * 0.5f, 0f, 0f);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

    }
    private void UpdateAttachedVisual()
    {
        Debug.DrawLine(transform.position, player.transform.position, Color.green, 0.2f);
        float dist = Vector2.Distance(player.transform.position + tieOffset, transform.position);
        currentLength = dist;

        if (tieVisual != null)
        {
            tieVisual.localScale = new Vector3(dist, tieVisual.localScale.y, 1f);
            tieVisual.localPosition = new Vector3(dist * 0.5f, 0f, 0f);

            Vector2 dir = (transform.position - (player.transform.position + tieOffset)).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        }
    }

    private void AttachToPoint(Vector2 attachPoint)
    {
        isAttached = true;
        isExtending = false;
        isRetracting = false;
        
        transform.position = attachPoint;

        if (playerRb != null && distanceJoint !=null)
        {
            Debug.Log("Attaching to point: " + attachPoint);


            distanceJoint.connectedBody = playerRb;
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.autoConfigureConnectedAnchor = false;

            float currentDistance = Vector2.Distance(player.transform.position, attachPoint);
            distanceJoint.distance = currentDistance;

            distanceJoint.anchor = Vector2.zero;
            distanceJoint.connectedAnchor = Vector2.zero;

            distanceJoint.enabled = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb !=null)
            {
                rb.linearVelocity = new Vector2(0f,0f);
                rb.bodyType = RigidbodyType2D.Dynamic;
            } 

        }
        else
        {
            Debug.LogError("Attach failed! PlayerRb or DistanceJoint is null");
        }
    }
    
    public void DetachAndRetract()
    {
        isAttached = false;

        if (distanceJoint != null)
        {
            distanceJoint.enabled = false;
            distanceJoint.connectedBody = null;
        }
        StartRetract();
    }

    public void RetractManually()
    {
        if (isAttached)
        { 
            isAttached = false;

            if (distanceJoint != null)
            {
                distanceJoint.enabled = false;
                distanceJoint.connectedBody = null;
            }
        }

        StartRetract();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger with: " + collision.name);
        if (isRetracting || isAttached) return;

        if (collision.CompareTag("StickWall"))
        {
            Debug.Log("Attached to wall");
            AttachToPoint(collision.ClosestPoint(transform.position));
        }
    }
}
