using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GunSystem), typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{        
    public float tieSpeed = 20f;
    public float maxTieLength = 10f;
    public float extendSpeed = 25f;
    public float retractSpeed = 40f;
    public Transform tieLaunchTransform;

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerAnimationController animator;
    private GunSystem gunSystem;
    private DistanceJoint2D joint;
    private LineRenderer lineRenderer;
    private Vector2 currentAnchor;    
    private bool isExtending = false;
    private bool isRetracting = false;
    private bool isAttached = false;
    

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<PlayerAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        gunSystem = GetComponent<GunSystem>();
        joint = GetComponent<DistanceJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
        

        lineRenderer.positionCount = 0;
        joint.enabled = false;
        lineRenderer.widthCurve = new AnimationCurve(
                                        new Keyframe(0f, 0.55f),  // base width
                                        new Keyframe(1f, 0.40f)); // tip thinness
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) return;
        Vector2 tieLaunchSpot = tieLaunchTransform.position;
        
        HandleTie();

        if (joint.enabled && !isExtending && !isRetracting)
        {
            if (lineRenderer.positionCount < 2)
                lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, tieLaunchSpot);
            lineRenderer.SetPosition(1, currentAnchor);
            lineRenderer.textureMode = LineTextureMode.Tile;


            Color ropeColor = new Color
            (
            PlayerPrefs.GetFloat("TieColor_R", 1f),
            PlayerPrefs.GetFloat("TieColor_G", 1f),
            PlayerPrefs.GetFloat("TieColor_B", 1f),
            PlayerPrefs.GetFloat("TieColor_A", 1f)
            );

            lineRenderer.material.color = ropeColor;
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

        if (joint.enabled && Mathf.Abs(input.Horizontal) > 0.1f)
        {
            Vector2 swingForce = new Vector2(input.Horizontal * 10f, 0f);
            rb.AddForce(swingForce, ForceMode2D.Force);
        }        
    }   

    private void FireTie()
    {
        Vector2 launchPos = tieLaunchTransform.position;
        Vector2 target = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (target - launchPos).normalized;

        RaycastHit2D hit = Physics2D.Raycast(launchPos, direction, 100f, LayerMask.GetMask("StickWall"));

        if (hit.collider != null)
        {
            StopAllCoroutines(); // stop any previous rope
            StartCoroutine(ExtendRope(hit.point));
            //animator.SetTrigger("Launch");
        }
    }
    private void HandleTie()
    {
        if (input.TiePressed && !isExtending && !isAttached)
        {
            FireTie();
        }

        if (input.TieReleased && (isExtending || isAttached))
        {
            StopAllCoroutines();
            StartCoroutine(RetractRope());
        }
    }

   
    private void AttachRope(Vector2 anchorPoint)
    {
        Vector2 tieLaunchSpot = tieLaunchTransform.position;
        Debug.Log ("Tie Attached to" + anchorPoint);
        currentAnchor = anchorPoint;
        joint.autoConfigureConnectedAnchor = false;
        joint.autoConfigureDistance = false;
        joint.enabled = true;
        joint.maxDistanceOnly = true;
        joint.connectedBody = null;
        joint.connectedAnchor = anchorPoint;
        joint.distance = Vector2.Distance(tieLaunchSpot, anchorPoint);

    }
    private void RetractTie()
    {
        joint.enabled = false;
        lineRenderer.positionCount = 0;
        currentAnchor = Vector2.zero;

        isExtending = false;
        isRetracting = true;
        isAttached = false;

        //animator.SetTrigger("TieDone");
    }
    private IEnumerator ExtendRope(Vector2 anchorPoint)
    {
        isExtending = true;
        isRetracting = false;
        isAttached = false;

        Vector2 launchPos = tieLaunchTransform.position;
        float distance = Vector2.Distance(launchPos, anchorPoint);
        float traveled = 0f;

        if (lineRenderer.positionCount < 2)
            lineRenderer.positionCount = 2;

        while (traveled < distance)
        {
            // Smoothly extend the rope outward
            traveled += extendSpeed * Time.deltaTime;
            traveled = Mathf.Min(traveled, distance); // don’t overshoot

            Vector2 direction = (anchorPoint - launchPos).normalized;
            Vector2 currentPos = launchPos + direction * traveled;

            lineRenderer.SetPosition(0, launchPos);
            lineRenderer.SetPosition(1, currentPos);

            yield return null;
        }

        // Rope reached the anchor visually — now attach
        currentAnchor = anchorPoint;
        AttachRope(anchorPoint);

        isExtending = false;
        isAttached = true;
    }
    private IEnumerator RetractRope()
    {
        isRetracting = true;
        isExtending = false;
        isAttached = false;

        Vector2 launchPos = tieLaunchTransform.position;
        Vector2 endPos = currentAnchor;
        float traveled = 0f;
        float distance = Vector2.Distance(endPos, launchPos);

        if (lineRenderer.positionCount < 2)
            lineRenderer.positionCount = 2;

        while (traveled < distance)
        {
            traveled += retractSpeed * Time.deltaTime;
            traveled = Mathf.Min(traveled, distance);

            Vector2 currentPos = Vector2.Lerp(endPos , launchPos, traveled / distance);

            lineRenderer.SetPosition(0, launchPos);
            lineRenderer.SetPosition(1, currentPos);

            yield return null;
        }

        
        yield return new WaitForSeconds(0.05f); // slight pause to sell the ending
        RetractTie();
        isRetracting = false;
    }
}
