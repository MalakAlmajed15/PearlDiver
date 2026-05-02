using UnityEngine;

public class JellyfishAI : MonoBehaviour
{
    private enum JellyfishState
    {
        Idle,
        Chase,
        ReturnHome
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSfx;

    [Header("Idle Movement")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.4f;
    [SerializeField] private float idleLerpSpeed = 2f;
    [SerializeField] private float horizontalDriftRadius = 0.5f;
    [SerializeField] private float horizontalDriftSpeed = 0.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseRange = 6.5f;
    [SerializeField] private float chaseSpeed = 1.6f;
    [SerializeField] private float maxChaseDistanceFromHome = 7f;
    [SerializeField] private float returnHomeReengageDelay = 0.75f;

    [Header("Chase Contact")]
    [SerializeField] private float stopDistance = 0.9f;
    [SerializeField] private float damageRange = 1.0f;

    [Header("Rotation")]
    [SerializeField] private bool keepUpright = true;
    [SerializeField] private float modelYawOffset = 0f;
    [SerializeField] private float rotationSmoothSpeed = 6f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1.5f;

    [Header("Return Home Failsafe")]
    [SerializeField] private float maxReturnTime = 3f;

    private JellyfishState currentState = JellyfishState.Idle;

    private Vector3 homePosition;
    private float baseY;
    private bool canDamage = true;
    private float floatTimer = 0f;
    private float returnTimer = 0f;
    private float returnHomeLockTimer = 0f;

    private PlayerHealth playerHealth;

    private float originalX;
    private float originalZ;

    private void Start()
    {
        homePosition = transform.position;
        baseY = transform.position.y;

        Vector3 startEuler = transform.rotation.eulerAngles;
        originalX = startEuler.x;
        originalZ = startEuler.z;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInParent<PlayerHealth>();
            }
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (returnHomeLockTimer > 0f)
        {
            returnHomeLockTimer -= Time.deltaTime;
        }

        if (playerHealth != null && playerHealth.IsDead())
        {
            if (currentState != JellyfishState.ReturnHome)
            {
                currentState = JellyfishState.ReturnHome;
                returnTimer = 0f;
                returnHomeLockTimer = returnHomeReengageDelay;
            }

            HandleReturnHome();
            ForceUprightRotation();
            return;
        }

        if (player == null)
        {
            HandleIdle();
            ForceUprightRotation();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromHome = Vector3.Distance(transform.position, homePosition);

        switch (currentState)
        {
            case JellyfishState.Idle:
                HandleIdle();

                if (distanceToPlayer <= detectionRange)
                {
                    currentState = JellyfishState.Chase;
                }
                break;

            case JellyfishState.Chase:
                HandleChase();
                TryDealDamageByDistance();

                if (distanceToPlayer > loseRange || distanceFromHome > maxChaseDistanceFromHome)
                {
                    currentState = JellyfishState.ReturnHome;
                    returnTimer = 0f;
                    returnHomeLockTimer = returnHomeReengageDelay;
                }
                break;

            case JellyfishState.ReturnHome:
                HandleReturnHome();

                if (returnHomeLockTimer <= 0f &&
                    playerHealth != null &&
                    !playerHealth.IsDead() &&
                    distanceToPlayer <= detectionRange)
                {
                    currentState = JellyfishState.Chase;
                    returnTimer = 0f;
                }
                break;
        }

        ForceUprightRotation();
    }

    private void HandleIdle()
    {
        floatTimer += Time.deltaTime * floatSpeed;

        float yOffset = 0f;
        if (floatHeight > 0.001f)
        {
            yOffset = Mathf.Sin(floatTimer) * floatHeight;
        }

        float xOffset = 0f;
        float zOffset = 0f;

        if (horizontalDriftRadius > 0.001f)
        {
            xOffset = Mathf.Sin(floatTimer * horizontalDriftSpeed) * horizontalDriftRadius;
            zOffset = Mathf.Cos(floatTimer * horizontalDriftSpeed) * horizontalDriftRadius * 0.5f;
        }

        Vector3 targetPos = new Vector3(
            homePosition.x + xOffset,
            baseY + yOffset,
            homePosition.z + zOffset
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * idleLerpSpeed);
    }

    private void HandleChase()
    {
        // Keep the jellyfish chasing on its own swim plane so it does not climb
        // above the player's head while pursuing.
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 toTarget = targetPos - transform.position;

        if (toTarget.sqrMagnitude <= 0.0001f)
            return;

        Vector3 direction = toTarget.normalized;

        // Stop slightly before the player's center so the jellyfish feels like it
        // collides/hits instead of pushing inside the player model.
        float distanceToTarget = toTarget.magnitude;
        if (distanceToTarget > stopDistance)
        {
            float moveDistance = Mathf.Min(chaseSpeed * Time.deltaTime, distanceToTarget - stopDistance);
            transform.position += direction * moveDistance;
        }

        SmoothFaceDirection(direction);
    }

    private void HandleReturnHome()
    {
        returnTimer += Time.deltaTime;

        Vector3 target = new Vector3(homePosition.x, baseY, homePosition.z);
        Vector3 toTarget = target - transform.position;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Vector3 direction = toTarget.normalized;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                chaseSpeed * 0.8f * Time.deltaTime
            );

            SmoothFaceDirection(direction);
        }

        if (Vector3.Distance(transform.position, target) <= 0.2f)
        {
            currentState = JellyfishState.Idle;
            floatTimer = 0f;
            returnTimer = 0f;
            returnHomeLockTimer = 0f;
            transform.position = target;
            return;
        }

        if (returnTimer >= maxReturnTime)
        {
            homePosition = new Vector3(transform.position.x, baseY, transform.position.z);
            currentState = JellyfishState.Idle;
            floatTimer = 0f;
            returnTimer = 0f;
            returnHomeLockTimer = 0f;
        }
    }

    private void SmoothFaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.Normalize();

        float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float finalYaw = targetYaw + modelYawOffset;

        Quaternion targetRotation = Quaternion.Euler(originalX, finalYaw, originalZ);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothSpeed
        );
    }

    private void ForceUprightRotation()
    {
        if (!keepUpright)
            return;

        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(originalX, euler.y, originalZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDealDamage(other);
    }

    private void TryDealDamage(Collider other)
    {
        if (!canDamage)
            return;

        PlayerHealth hitPlayerHealth = other.GetComponent<PlayerHealth>();
        if (hitPlayerHealth == null)
        {
            hitPlayerHealth = other.GetComponentInParent<PlayerHealth>();
        }

        if (hitPlayerHealth != null && !hitPlayerHealth.IsDead())
        {
            DealDamage(hitPlayerHealth);
        }
    }

    private void TryDealDamageByDistance()
    {
        if (!canDamage || playerHealth == null || playerHealth.IsDead())
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= damageRange)
        {
            DealDamage(playerHealth);
        }
    }

    private void DealDamage(PlayerHealth targetHealth)
    {
        targetHealth.TakeDamage(damage);

        if (audioSource != null && hitSfx != null)
        {
            audioSource.PlayOneShot(hitSfx);
        }

        canDamage = false;
        Invoke(nameof(ResetDamage), damageCooldown);
    }

    private void ResetDamage()
    {
        canDamage = true;
    }
}