using UnityEngine;

public class EelAI : MonoBehaviour
{
    private enum EelState
    {
        Patrol,
        Alert,
        Chase,
        ReturnHome
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip detectSfx;
    [SerializeField] private AudioClip hitSfx;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.6f;
    [SerializeField] private float chaseSpeed = 3.2f;
    [SerializeField] private float returnSpeed = 2.2f;
    [SerializeField] private float rotationSmoothSpeed = 4f;

    [Header("Steering")]
    [SerializeField] private float turnSpeed = 3.5f;

    [Header("Patrol")]
    [SerializeField] private float reachDistance = 1.25f;
    [SerializeField] private float roamRadius = 2.5f;
    [SerializeField] private float verticalRange = 1.2f;
    [SerializeField] private float baseY;

    [Header("Vertical Motion")]
    [SerializeField] private float verticalLerpSpeed = 2.5f;
    [SerializeField] private float chaseVerticalLerpSpeed = 3.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float alertRange = 8f;
    [SerializeField] private float loseRange = 9f;
    [SerializeField] private float maxChaseDistanceFromHome = 10f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1.5f;
    [SerializeField] private float damageRange = 1.3f;

    [Header("Model Fix")]
    [SerializeField] private float modelYawOffset = 0f;

    private EelState currentState = EelState.Patrol;

    private PlayerHealth playerHealth;
    private PlayerHitFeedback playerHitFeedback;

    private Vector3 homePosition;
    private Vector3 patrolTarget;

    private bool canDamage = true;

    private float originalX;
    private float originalZ;

    // Horizontal movement is maintained separately so the eel never "stalls"
    // just because its target height changes.
    private Vector3 velocityXZ;

    // Persistent target height prevents sudden height snaps.
    private float targetY;

    private void Start()
    {
        homePosition = transform.position;
        baseY = transform.position.y;
        targetY = transform.position.y;

        Vector3 startEuler = transform.rotation.eulerAngles;
        originalX = startEuler.x;
        originalZ = startEuler.z;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = player.GetComponentInParent<PlayerHealth>();

            playerHitFeedback = player.GetComponent<PlayerHitFeedback>();
            if (playerHitFeedback == null)
                playerHitFeedback = player.GetComponentInParent<PlayerHitFeedback>();
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        patrolTarget = GetRandomPoint();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distFromHome = Vector3.Distance(transform.position, homePosition);

        switch (currentState)
        {
            case EelState.Patrol:
                HandlePatrol();

                if (distToPlayer <= detectionRange)
                {
                    PlaySound(detectSfx);
                    currentState = EelState.Chase;
                }
                else if (distToPlayer <= alertRange)
                {
                    currentState = EelState.Alert;
                }
                break;

            case EelState.Alert:
                // Keep the eel alive during alert instead of freezing in place.
                DriftForward(patrolSpeed * 0.75f, verticalLerpSpeed);
                FacePlayer();

                if (distToPlayer <= detectionRange)
                {
                    PlaySound(detectSfx);
                    currentState = EelState.Chase;
                }
                else if (distToPlayer > alertRange)
                {
                    currentState = EelState.Patrol;
                }
                break;

            case EelState.Chase:
                MoveSteering(player.position, chaseSpeed, chaseVerticalLerpSpeed);
                TryDamagePlayerDistance();

                if (distToPlayer > loseRange || distFromHome > maxChaseDistanceFromHome)
                    currentState = EelState.ReturnHome;
                break;

            case EelState.ReturnHome:
                if (distToPlayer <= detectionRange)
                {
                    PlaySound(detectSfx);
                    currentState = EelState.Chase;
                    break;
                }

                MoveSteering(homePosition, returnSpeed, verticalLerpSpeed);

                if (Vector3.Distance(transform.position, homePosition) < reachDistance)
                {
                    patrolTarget = GetRandomPoint();
                    currentState = EelState.Patrol;
                }
                break;
        }
    }

    private void HandlePatrol()
    {
        float horizontalDistanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(patrolTarget.x, patrolTarget.z)
        );

        // Retarget early so the eel never reaches a dead stop.
        if (horizontalDistanceToTarget < reachDistance)
        {
            patrolTarget = GetRandomPoint();
        }

        MoveSteering(patrolTarget, patrolSpeed, verticalLerpSpeed);
    }

    private Vector3 GetRandomPoint()
    {
        if (pointA == null || pointB == null)
            return homePosition;

        float t = Random.Range(0f, 1f);
        Vector3 basePos = Vector3.Lerp(pointA.position, pointB.position, t);

        return new Vector3(
            basePos.x + Random.Range(-roamRadius, roamRadius),
            baseY + Random.Range(-verticalRange, verticalRange),
            basePos.z + Random.Range(-roamRadius, roamRadius)
        );
    }

    private void MoveSteering(Vector3 target, float speed, float yLerpSpeed)
    {
        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetXZ = new Vector3(target.x, 0f, target.z);
        Vector3 toTargetXZ = targetXZ - currentXZ;

        Vector3 currentDirection;

        if (velocityXZ.sqrMagnitude > 0.0001f)
            currentDirection = velocityXZ.normalized;
        else
            currentDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        // If the eel is almost directly above/below its target, keep moving forward
        // instead of allowing the horizontal speed to collapse.
        Vector3 desiredDirection = toTargetXZ.sqrMagnitude > 0.0001f
            ? toTargetXZ.normalized
            : currentDirection;

        float currentTurnSpeed = currentState == EelState.Chase
            ? turnSpeed * 2.2f
            : turnSpeed;

        Vector3 newDirection = Vector3.Slerp(
            currentDirection,
            desiredDirection,
            Time.deltaTime * currentTurnSpeed
        ).normalized;

        velocityXZ = newDirection * speed;

        transform.position += new Vector3(
            velocityXZ.x * Time.deltaTime,
            0f,
            velocityXZ.z * Time.deltaTime
        );

        targetY = target.y;
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * yLerpSpeed),
            transform.position.z
        );

        SmoothFaceDirection(newDirection);
    }

    private void DriftForward(float speed, float yLerpSpeed)
    {
        Vector3 currentDirection;

        if (velocityXZ.sqrMagnitude > 0.0001f)
            currentDirection = velocityXZ.normalized;
        else
            currentDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        velocityXZ = currentDirection * speed;

        transform.position += new Vector3(
            velocityXZ.x * Time.deltaTime,
            0f,
            velocityXZ.z * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * yLerpSpeed),
            transform.position.z
        );
    }

    private void SmoothFaceDirection(Vector3 flatDirection)
    {
        if (flatDirection.sqrMagnitude <= 0.0001f)
            return;

        float yaw = Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg + modelYawOffset;

        float rotSpeed = currentState == EelState.Chase
            ? rotationSmoothSpeed * 2f
            : rotationSmoothSpeed;

        Quaternion targetRot = Quaternion.Euler(originalX, yaw, originalZ);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * rotSpeed
        );
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        SmoothFaceDirection(dir.normalized);
    }

    private void TryDamagePlayerDistance()
    {
        if (!canDamage || playerHealth == null || playerHealth.IsDead())
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > damageRange)
            return;

        playerHealth.TakeDamage(damage);
        PlaySound(hitSfx);

        if (playerHitFeedback != null)
            playerHitFeedback.PlayHitFeedback();

        canDamage = false;
        Invoke(nameof(ResetDamage), damageCooldown);
    }

    private void ResetDamage()
    {
        canDamage = true;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}