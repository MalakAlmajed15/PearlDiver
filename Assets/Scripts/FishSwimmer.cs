using UnityEngine;

public class FishSwimmer : MonoBehaviour
{
    [Header("Swimming Speed")]
    public float swimSpeed = 2f;          
    public float turnSpeed = 90f;         

    [Header("Body Wobble")]
    public float wobbleAmount = 15f;     
    public float wobbleSpeed = 2f;        

    [Header("Up/Down Drift")]
    public float driftAmount = 0.3f;      
    public float driftSpeed = 0.8f;     

    [Header("Roaming Boundary")]
    public Vector3 centerPoint = Vector3.zero; 
    public float roamRadius = 10f;             

    [Header("Turn Behaviour")]
    public float minTimeBetweenTurns = 2f;  
    public float maxTimeBetweenTurns = 6f;   


    private Vector3    _startPosition;
    private Quaternion _targetRotation;
    private float      _turnTimer;
    private float      _wobbleTime;
    private float      _driftTime;

    void Start()
    {
        _startPosition   = transform.position;
        centerPoint      = transform.position;   // roam around wherever the fish starts
        _targetRotation  = transform.rotation;
        PickNewDirection();
    }

    void Update()
    {
        Swim();
        Wobble();
        Drift();
        HandleTurnTimer();
        StayInBounds();
    }

    // ── Move forward in current facing direction ─────────────────────────────
    void Swim()
    {
        transform.position += transform.forward * swimSpeed * Time.deltaTime;

        // Smoothly rotate toward the chosen target direction
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    // ── Side-to-side body sway ───────────────────────────────────────────────
    void Wobble()
    {
        _wobbleTime += Time.deltaTime * wobbleSpeed;
        float sway = Mathf.Sin(_wobbleTime) * wobbleAmount;

        // Apply as a local Y-axis rotation on top of the current rotation
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y,
            sway
        );
    }

    // ── Gentle vertical bob ──────────────────────────────────────────────────
    void Drift()
    {
        _driftTime += Time.deltaTime * driftSpeed;
        float bob = Mathf.Sin(_driftTime) * driftAmount;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + bob * Time.deltaTime,
            transform.position.z
        );
    }

    // ── Count down and pick a new random direction ───────────────────────────
    void HandleTurnTimer()
    {
        _turnTimer -= Time.deltaTime;
        if (_turnTimer <= 0f)
            PickNewDirection();
    }

    // ── Choose a random horizontal direction to swim toward ──────────────────
    void PickNewDirection()
    {
        float randomYaw = Random.Range(0f, 360f);
        _targetRotation  = Quaternion.Euler(0f, randomYaw, 0f);
        _turnTimer       = Random.Range(minTimeBetweenTurns, maxTimeBetweenTurns);
    }

    // ── Turn around if the fish wanders outside the roam radius ─────────────
    void StayInBounds()
    {
        Vector3 flatPos    = new Vector3(transform.position.x, centerPoint.y, transform.position.z);
        Vector3 flatCenter = new Vector3(centerPoint.x,        centerPoint.y, centerPoint.z);

        if (Vector3.Distance(flatPos, flatCenter) > roamRadius)
        {
            // Point back toward the centre and immediately pick a new turn timer
            Vector3 dirToCenter = (flatCenter - flatPos).normalized;
            _targetRotation     = Quaternion.LookRotation(dirToCenter);
            _turnTimer          = Random.Range(minTimeBetweenTurns, maxTimeBetweenTurns);
        }
    }

    // ── Draw the roam boundary in the Scene view for easy tuning ─────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? centerPoint : transform.position, roamRadius);
    }
}
