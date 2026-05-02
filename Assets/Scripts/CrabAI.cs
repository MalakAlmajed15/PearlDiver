using UnityEngine;

public class CrabAI : MonoBehaviour
{
    private enum CrabState
    {
        Patrol,
        Chase,
        AttackWindup,
        Recover,
        ReturnHome
    }

    [Header("References")]
    [Tooltip("Player transform. If empty, the script will try to find the object tagged Player.")]
    [SerializeField] private Transform player;

    [Tooltip("First patrol point.")]
    [SerializeField] private Transform pointA;

    [Tooltip("Second patrol point.")]
    [SerializeField] private Transform pointB;

    [Tooltip("AudioSource used for crab sounds.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Sound played when the crab first detects the player.")]
    [SerializeField] private AudioClip detectSfx;

    [Tooltip("Sound played when the crab begins its attack windup.")]
    [SerializeField] private AudioClip attackWindupSfx;

    [Tooltip("Sound played when the crab successfully hits the player.")]
    [SerializeField] private AudioClip hitSfx;

    [Header("Base Movement")]
    [Tooltip("Base patrol speed before any runtime variation or multipliers are applied.")]
    [SerializeField] private float patrolSpeed = 1.5f;

    [Tooltip("Base chase speed before any runtime variation or multipliers are applied.")]
    [SerializeField] private float chaseSpeed = 2.2f;

    [Tooltip("How quickly the crab rotates toward its movement direction.")]
    [SerializeField] private float rotationSmoothSpeed = 8f;

    [Tooltip("How close the crab must get to a patrol point before switching.")]
    [SerializeField] private float reachDistance = 0.2f;

    [Tooltip("Locked Y position for movement.")]
    [SerializeField] private float groundY;

    [Header("Movement Feel")]
    [Tooltip("Adds a short hesitation before movement when the crab changes direction sharply.")]
    [SerializeField] private bool useMovementStartDelay = true;

    [Tooltip("How long the crab waits before moving after a sharp turn.")]
    [SerializeField] private float movementStartDelay = 0.08f;

    [Tooltip("Minimum direction change, in degrees, required to trigger the start delay.")]
    [SerializeField] private float movementDelayTurnThreshold = 30f;

    [Header("Movement Tuning")]
    [Tooltip("If enabled, patrol/chase speeds get a small random offset per crab on Start.")]
    [SerializeField] private bool randomizeMovementStats = true;

    [Tooltip("If randomization is disabled, these multipliers are used instead.")]
    [SerializeField] private bool useManualMovementMultipliers = false;

    [Tooltip("Manual multiplier for patrol speed when randomization is disabled.")]
    [SerializeField] private float patrolSpeedMultiplier = 1f;

    [Tooltip("Manual multiplier for chase speed when randomization is disabled.")]
    [SerializeField] private float chaseSpeedMultiplier = 1f;

    [Tooltip("Maximum random +/- offset applied to patrol speed.")]
    [SerializeField] private float patrolSpeedVariance = 0.15f;

    [Tooltip("Maximum random +/- offset applied to chase speed.")]
    [SerializeField] private float chaseSpeedVariance = 0.2f;

    [Tooltip("Return-home speed is derived from patrol speed using this multiplier.")]
    [SerializeField] private float returnHomeSpeedMultiplier = 0.85f;

    [Header("Detection")]
    [Tooltip("Range where the crab fully detects and chases the player.")]
    [SerializeField] private float detectionRange = 4f;

    [Tooltip("Range where the crab only looks toward the player without fully chasing yet.")]
    [SerializeField] private float alertRange = 5.2f;

    [Tooltip("Distance after which the crab gives up chasing.")]
    [SerializeField] private float loseRange = 5.5f;

    [Tooltip("Maximum distance the crab may chase away from its home position.")]
    [SerializeField] private float maxChaseDistanceFromHome = 6f;

    [Tooltip("Delay before the crab can re-detect after returning home.")]
    [SerializeField] private float returnHomeReengageDelay = 0.75f;

    [Header("Base Attack")]
    [Tooltip("Distance at which the crab starts attacking.")]
    [SerializeField] private float attackRange = 1.2f;

    [Tooltip("Base pause before the crab lunges.")]
    [SerializeField] private float attackWindupTime = 0.4f;

    [Tooltip("Time before the crab may attack again.")]
    [SerializeField] private float attackCooldown = 1f;

    [Tooltip("Base lunge distance.")]
    [SerializeField] private float lungeDistance = 0.6f;

    [Tooltip("How fast the crab lunges.")]
    [SerializeField] private float lungeSpeed = 6f;

    [Header("Attack Tuning")]
    [Tooltip("If enabled, windup and lunge distance get a small random offset per crab on Start.")]
    [SerializeField] private bool randomizeAttackStats = true;

    [Tooltip("If randomization is disabled, these multipliers are used instead.")]
    [SerializeField] private bool useManualAttackMultipliers = false;

    [Tooltip("Manual multiplier for attack windup time when randomization is disabled.")]
    [SerializeField] private float attackWindupMultiplier = 1f;

    [Tooltip("Manual multiplier for lunge distance when randomization is disabled.")]
    [SerializeField] private float lungeDistanceMultiplier = 1f;

    [Tooltip("Maximum random +/- offset applied to windup time.")]
    [SerializeField] private float attackWindupVariance = 0.08f;

    [Tooltip("Maximum random +/- offset applied to lunge distance.")]
    [SerializeField] private float lungeDistanceVariance = 0.08f;

    [Header("Recovery")]
    [Tooltip("How far the crab steps back after a close hit.")]
    [SerializeField] private float backstepDistance = 0.5f;

    [Tooltip("How fast the crab steps back.")]
    [SerializeField] private float backstepSpeed = 4f;

    [Tooltip("How long recovery lasts if the player stays close.")]
    [SerializeField] private float recoverTime = 0.4f;

    [Tooltip("Recovery starts only if the player is still within this range after the lunge.")]
    [SerializeField] private float startRecoverIfPlayerWithinRange = 1.0f;

    [Tooltip("Recovery is cancelled if the player moves farther than this.")]
    [SerializeField] private float cancelRecoverIfPlayerBeyondRange = 1.6f;

    [Header("Damage")]
    [Tooltip("Damage dealt per successful hit.")]
    [SerializeField] private int damage = 1;

    [Tooltip("Spacing between allowed hits.")]
    [SerializeField] private float damageCooldown = 1.5f;

    [Header("Patrol")]
    [Tooltip("Pause duration at each patrol point.")]
    [SerializeField] private float patrolPauseTime = 0.5f;

    [Tooltip("If enabled, the crab turns toward the next patrol point before moving.")]
    [SerializeField] private bool turnBeforePatrolMove = true;

    [Tooltip("If the crab is still more misaligned than this angle, it keeps turning before moving.")]
    [SerializeField] private float patrolTurnThreshold = 10f;

    [Header("Audio Polish")]
    [Tooltip("Adds slight pitch variation so repeated sounds feel less identical.")]
    [SerializeField] private bool randomizeAudioPitch = true;

    [Tooltip("Minimum random pitch applied before PlayOneShot.")]
    [SerializeField] private float minAudioPitch = 0.96f;

    [Tooltip("Maximum random pitch applied before PlayOneShot.")]
    [SerializeField] private float maxAudioPitch = 1.04f;

    [Header("Model Fix")]
    [Tooltip("Use this if the model faces the wrong direction. Usually 0, 90, -90, or 180.")]
    [SerializeField] private float modelYawOffset = 0f;

    [Tooltip("Keeps the crab upright by preserving original X/Z rotation.")]
    [SerializeField] private bool keepUpright = true;

    private CrabState currentState = CrabState.Patrol;

    private PlayerHealth playerHealth;
    private PlayerHitFeedback playerHitFeedback;
    private Transform currentPatrolTarget;
    private Vector3 homePosition;

    private bool canDamage = true;
    private float returnHomeLockTimer = 0f;
    private float patrolPauseTimer = 0f;

    private float attackWindupTimer = 0f;
    private float attackCooldownTimer = 0f;

    private bool isLunging = false;
    private Vector3 lungeTarget;

    private bool isRecovering = false;
    private Vector3 backstepTarget;
    private float recoverTimer;

    private float originalX;
    private float originalZ;

    private bool hasPlayedDetectSfx = false;
    private bool hasPlayedWindupSfx = false;

    // Runtime-tuned values. The base inspector values remain intact so designers can
    // either use them directly, randomize them, or scale them with multipliers.
    private float patrolSpeedRuntime;
    private float chaseSpeedRuntime;
    private float returnHomeSpeedRuntime;
    private float attackWindupTimeRuntime;
    private float lungeDistanceRuntime;

    // Used to add a small sense of weight when movement direction changes sharply.
    private Vector3 lastMoveDirection;
    private float movementStartDelayTimer = 0f;

    private void Start()
    {
        homePosition = transform.position;
        groundY = transform.position.y;

        Vector3 startEuler = transform.rotation.eulerAngles;
        originalX = startEuler.x;
        originalZ = startEuler.z;

        ResolvePlayerReferences();
        ResolveAudioSource();
        ResolvePatrolPoints();
        BuildRuntimeStats();
    }

    private void Update()
    {
        if (returnHomeLockTimer > 0f)
            returnHomeLockTimer -= Time.deltaTime;

        if (patrolPauseTimer > 0f)
            patrolPauseTimer -= Time.deltaTime;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (movementStartDelayTimer > 0f)
            movementStartDelayTimer -= Time.deltaTime;

        if (playerHealth != null && playerHealth.IsDead())
        {
            currentState = CrabState.ReturnHome;
            ResetAttackFlags();
            hasPlayedDetectSfx = false;
            hasPlayedWindupSfx = false;
            HandleReturnHome();
            ForceUprightRotation();
            return;
        }

        if (player == null || pointA == null || pointB == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromHome = Vector3.Distance(transform.position, homePosition);

        switch (currentState)
        {
            case CrabState.Patrol:
                HandlePatrol();

                if (distanceToPlayer <= detectionRange)
                {
                    if (!hasPlayedDetectSfx)
                    {
                        PlaySound(detectSfx);
                        hasPlayedDetectSfx = true;
                    }

                    currentState = CrabState.Chase;
                    movementStartDelayTimer = movementStartDelay;
                }
                else if (distanceToPlayer <= alertRange)
                {
                    // Alert behavior is intentionally lightweight. The crab acknowledges
                    // the player before committing to a full chase.
                    Vector3 alertDirection = player.position - transform.position;
                    alertDirection.y = 0f;

                    if (alertDirection.sqrMagnitude > 0.0001f)
                    {
                        alertDirection.Normalize();
                        SmoothFaceDirection(alertDirection);
                    }
                }
                break;

            case CrabState.Chase:
                HandleChase();

                if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0f)
                {
                    currentState = CrabState.AttackWindup;
                    attackWindupTimer = attackWindupTimeRuntime;
                    isLunging = false;
                    hasPlayedWindupSfx = false;
                    movementStartDelayTimer = 0f;
                }
                else if (distanceToPlayer > loseRange || distanceFromHome > maxChaseDistanceFromHome)
                {
                    currentState = CrabState.ReturnHome;
                    returnHomeLockTimer = returnHomeReengageDelay;
                    ResetAttackFlags();
                    hasPlayedDetectSfx = false;
                    hasPlayedWindupSfx = false;
                    movementStartDelayTimer = movementStartDelay;
                }
                break;

            case CrabState.AttackWindup:
                HandleAttackWindup();

                if (distanceToPlayer > loseRange || distanceFromHome > maxChaseDistanceFromHome)
                {
                    currentState = CrabState.ReturnHome;
                    returnHomeLockTimer = returnHomeReengageDelay;
                    ResetAttackFlags();
                    hasPlayedDetectSfx = false;
                    hasPlayedWindupSfx = false;
                    movementStartDelayTimer = movementStartDelay;
                }
                break;

            case CrabState.Recover:
                HandleRecover();
                break;

            case CrabState.ReturnHome:
                HandleReturnHome();

                if (returnHomeLockTimer <= 0f &&
                    playerHealth != null &&
                    !playerHealth.IsDead() &&
                    distanceToPlayer <= detectionRange)
                {
                    if (!hasPlayedDetectSfx)
                    {
                        PlaySound(detectSfx);
                        hasPlayedDetectSfx = true;
                    }

                    currentState = CrabState.Chase;
                    movementStartDelayTimer = movementStartDelay;
                }
                break;
        }

        ForceUprightRotation();
    }

    private void ResolvePlayerReferences()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
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
    }

    private void ResolveAudioSource()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void ResolvePatrolPoints()
    {
        if (pointA != null && pointB != null)
            currentPatrolTarget = pointB;
    }

    private void BuildRuntimeStats()
    {
        patrolSpeedRuntime = patrolSpeed;
        chaseSpeedRuntime = chaseSpeed;
        attackWindupTimeRuntime = attackWindupTime;
        lungeDistanceRuntime = lungeDistance;

        if (randomizeMovementStats)
        {
            patrolSpeedRuntime += Random.Range(-patrolSpeedVariance, patrolSpeedVariance);
            chaseSpeedRuntime += Random.Range(-chaseSpeedVariance, chaseSpeedVariance);
        }
        else if (useManualMovementMultipliers)
        {
            patrolSpeedRuntime *= patrolSpeedMultiplier;
            chaseSpeedRuntime *= chaseSpeedMultiplier;
        }

        if (randomizeAttackStats)
        {
            attackWindupTimeRuntime += Random.Range(-attackWindupVariance, attackWindupVariance);
            lungeDistanceRuntime += Random.Range(-lungeDistanceVariance, lungeDistanceVariance);
        }
        else if (useManualAttackMultipliers)
        {
            attackWindupTimeRuntime *= attackWindupMultiplier;
            lungeDistanceRuntime *= lungeDistanceMultiplier;
        }

        patrolSpeedRuntime = Mathf.Max(0.05f, patrolSpeedRuntime);
        chaseSpeedRuntime = Mathf.Max(0.05f, chaseSpeedRuntime);
        attackWindupTimeRuntime = Mathf.Max(0.05f, attackWindupTimeRuntime);
        lungeDistanceRuntime = Mathf.Max(0.05f, lungeDistanceRuntime);
        returnHomeSpeedRuntime = Mathf.Max(0.05f, patrolSpeedRuntime * returnHomeSpeedMultiplier);
    }

    private void HandleAttackWindup()
    {
        if (!hasPlayedWindupSfx)
        {
            PlaySound(attackWindupSfx);
            hasPlayedWindupSfx = true;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            direction.Normalize();
            SmoothFaceDirection(direction);
        }

        if (!isLunging)
        {
            attackWindupTimer -= Time.deltaTime;

            if (attackWindupTimer <= 0f)
            {
                Vector3 lungeDir = direction.normalized;

                if (lungeDir.sqrMagnitude <= 0.0001f)
                {
                    lungeDir = transform.forward;
                    lungeDir.y = 0f;
                    lungeDir.Normalize();
                }

                lungeTarget = transform.position + lungeDir * lungeDistanceRuntime;
                lungeTarget.y = groundY;

                isLunging = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                lungeTarget,
                lungeSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, lungeTarget) <= 0.05f)
            {
                TryDamagePlayerDirect();
                attackCooldownTimer = attackCooldown;
                hasPlayedWindupSfx = false;

                Vector2 crabPos2D = new Vector2(transform.position.x, transform.position.z);
                Vector2 playerPos2D = new Vector2(player.position.x, player.position.z);
                float horizontalDistanceToPlayerAfterLunge = Vector2.Distance(crabPos2D, playerPos2D);

                if (horizontalDistanceToPlayerAfterLunge <= startRecoverIfPlayerWithinRange)
                {
                    Vector3 backDir = transform.position - player.position;
                    backDir.y = 0f;

                    if (backDir.sqrMagnitude <= 0.0001f)
                    {
                        backDir = -transform.forward;
                        backDir.y = 0f;
                    }

                    backDir.Normalize();

                    backstepTarget = transform.position + backDir * backstepDistance;
                    backstepTarget.y = groundY;

                    isRecovering = true;
                    recoverTimer = recoverTime;
                    currentState = CrabState.Recover;
                }
                else
                {
                    isRecovering = false;
                    currentState = CrabState.Chase;
                    movementStartDelayTimer = movementStartDelay;
                }

                ResetAttackFlags();
            }
        }
    }

    private void HandleRecover()
    {
        if (!isRecovering) return;

        if (player == null)
        {
            isRecovering = false;
            currentState = CrabState.ReturnHome;
            movementStartDelayTimer = movementStartDelay;
            return;
        }

        Vector2 crabPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos2D = new Vector2(player.position.x, player.position.z);
        float horizontalDistanceToPlayer = Vector2.Distance(crabPos2D, playerPos2D);

        if (horizontalDistanceToPlayer > cancelRecoverIfPlayerBeyondRange)
        {
            isRecovering = false;
            currentState = CrabState.Chase;
            movementStartDelayTimer = movementStartDelay;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            backstepTarget,
            backstepSpeed * Time.deltaTime
        );

        recoverTimer -= Time.deltaTime;

        if (recoverTimer <= 0f)
        {
            isRecovering = false;
            currentState = CrabState.Chase;
            movementStartDelayTimer = movementStartDelay;
        }
    }

    private void HandlePatrol()
    {
        if (currentPatrolTarget == null) return;
        if (patrolPauseTimer > 0f) return;

        Vector3 target = new Vector3(
            currentPatrolTarget.position.x,
            groundY,
            currentPatrolTarget.position.z
        );

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f) return;

        if (turnBeforePatrolMove)
        {
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + modelYawOffset;
            float currentYaw = transform.eulerAngles.y;
            float angleDelta = Mathf.Abs(Mathf.DeltaAngle(currentYaw, targetYaw));

            SmoothFaceDirection(direction);

            if (angleDelta > patrolTurnThreshold)
                return;
        }

        MoveTowards(target, patrolSpeedRuntime);

        if (Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(target.x, target.z)) <= reachDistance)
        {
            currentPatrolTarget = currentPatrolTarget == pointA ? pointB : pointA;
            patrolPauseTimer = patrolPauseTime;
            movementStartDelayTimer = movementStartDelay;
        }
    }

    private void HandleChase()
    {
        Vector3 target = new Vector3(player.position.x, groundY, player.position.z);
        MoveTowards(target, chaseSpeedRuntime);
    }

    private void HandleReturnHome()
    {
        Transform nearestPoint = GetNearestPatrolPoint();
        if (nearestPoint == null) return;

        Vector3 target = new Vector3(nearestPoint.position.x, groundY, nearestPoint.position.z);
        MoveTowards(target, returnHomeSpeedRuntime);

        if (Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(target.x, target.z)) <= reachDistance)
        {
            currentPatrolTarget = nearestPoint == pointA ? pointB : pointA;
            currentState = CrabState.Patrol;
            patrolPauseTimer = patrolPauseTime;
            movementStartDelayTimer = movementStartDelay;
        }
    }

    private Transform GetNearestPatrolPoint()
    {
        float distA = Vector3.Distance(transform.position, pointA.position);
        float distB = Vector3.Distance(transform.position, pointB.position);
        return distA <= distB ? pointA : pointB;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.Normalize();

        // Only gate movement when the crab is changing course noticeably. This keeps
        // the delay feeling intentional instead of making all movement feel sluggish.
        if (useMovementStartDelay && lastMoveDirection.sqrMagnitude > 0.0001f)
        {
            float angleDelta = Vector3.Angle(lastMoveDirection, direction);

            if (angleDelta >= movementDelayTurnThreshold && movementStartDelayTimer <= 0f)
                movementStartDelayTimer = movementStartDelay;
        }

        SmoothFaceDirection(direction);
        lastMoveDirection = direction;

        if (useMovementStartDelay && movementStartDelayTimer > 0f)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    private void SmoothFaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

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

    private void TryDamagePlayerDirect()
    {
        if (!canDamage || playerHealth == null || playerHealth.IsDead())
            return;

        Vector2 crabPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos2D = new Vector2(player.position.x, player.position.z);
        float horizontalDistance = Vector2.Distance(crabPos2D, playerPos2D);

        if (horizontalDistance > attackRange + 0.25f)
            return;

        playerHealth.TakeDamage(damage);

        PlaySound(hitSfx);

        if (playerHitFeedback != null)
            playerHitFeedback.PlayHitFeedback();

        canDamage = false;
        Invoke(nameof(ResetDamage), damageCooldown);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.pitch = randomizeAudioPitch
            ? Random.Range(minAudioPitch, maxAudioPitch)
            : 1f;

        audioSource.PlayOneShot(clip);
    }

    private void ResetDamage()
    {
        canDamage = true;
    }

    private void ResetAttackFlags()
    {
        isLunging = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.2f);
        }

        if (pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 home = Application.isPlaying ? homePosition : transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(home, maxChaseDistanceFromHome);

        if (Application.isPlaying && currentPatrolTarget != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, currentPatrolTarget.position);
            Gizmos.DrawSphere(currentPatrolTarget.position, 0.1f);
        }
    }
}