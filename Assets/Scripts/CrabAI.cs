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
    [SerializeField] private Transform player;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Terrain")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private float terrainOffset = 0.35f;

    [Header("Water Limits")]
    [SerializeField] private float waterSurfaceY = 0f;
    [SerializeField] private float surfaceOffset = 1f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 2.2f;
    [SerializeField] private float returnHomeSpeed = 1.2f;
    [SerializeField] private float rotationSmoothSpeed = 8f;
    [SerializeField] private float reachDistance = 0.2f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 4f;
    [SerializeField] private float loseRange = 5.5f;
    [SerializeField] private float maxChaseDistanceFromHome = 6f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackWindupTime = 0.4f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float lungeDistance = 0.6f;
    [SerializeField] private float lungeSpeed = 6f;

    [Header("Recovery")]
    [SerializeField] private float backstepDistance = 0.5f;
    [SerializeField] private float backstepSpeed = 4f;
    [SerializeField] private float recoverTime = 0.4f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1.5f;

    [Header("Patrol")]
    [SerializeField] private float patrolPauseTime = 0.5f;

    [Header("Rotation")]
    [SerializeField] private float modelYawOffset = 0f;
    [SerializeField] private bool keepUpright = true;

    private CrabState currentState = CrabState.Patrol;

    private PlayerHealth playerHealth;
    private PlayerHitFeedback playerHitFeedback;

    private Transform currentPatrolTarget;

    private Vector3 homePosition;

    private bool canDamage = true;

    private bool isLunging = false;
    private Vector3 lungeTarget;

    private bool isRecovering = false;
    private Vector3 backstepTarget;
    private float recoverTimer;

    private float patrolPauseTimer;
    private float attackCooldownTimer;
    private float attackWindupTimer;

    private float originalX;
    private float originalZ;

    private void Start()
    {
        homePosition = transform.position;

        Vector3 startEuler = transform.rotation.eulerAngles;

        originalX = startEuler.x;
        originalZ = startEuler.z;

        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth =
                    player.GetComponentInParent<PlayerHealth>();
            }

            playerHitFeedback =
                player.GetComponent<PlayerHitFeedback>();

            if (playerHitFeedback == null)
            {
                playerHitFeedback =
                    player.GetComponentInParent<PlayerHitFeedback>();
            }
        }

        currentPatrolTarget = pointB;
    }

    private void Update()
    {
        if (player == null || pointA == null || pointB == null)
        {
            return;
        }

        if (patrolPauseTimer > 0f)
        {
            patrolPauseTimer -= Time.deltaTime;
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        float distanceFromHome =
            Vector3.Distance(transform.position, homePosition);

        switch (currentState)
        {
            case CrabState.Patrol:

                HandlePatrol();

                if (distanceToPlayer <= detectionRange)
                {
                    currentState = CrabState.Chase;
                }

                break;

            case CrabState.Chase:

                HandleChase();

                if (distanceToPlayer <= attackRange &&
                    attackCooldownTimer <= 0f)
                {
                    currentState = CrabState.AttackWindup;
                    attackWindupTimer = attackWindupTime;
                }

                if (distanceToPlayer > loseRange ||
                    distanceFromHome > maxChaseDistanceFromHome)
                {
                    currentState = CrabState.ReturnHome;
                }

                break;

            case CrabState.AttackWindup:

                HandleAttack();

                break;

            case CrabState.Recover:

                HandleRecover();

                break;

            case CrabState.ReturnHome:

                HandleReturnHome();

                break;
        }

        ForceUprightRotation();
    }

    private void HandlePatrol()
    {
        if (currentPatrolTarget == null)
        {
            return;
        }

        if (patrolPauseTimer > 0f)
        {
            return;
        }

        Vector3 target =
            currentPatrolTarget.position;

        target.y = GetTerrainHeight(target);

        MoveTowards(target, patrolSpeed);

        float distance =
            Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(target.x, target.z));

        if (distance <= reachDistance)
        {
            currentPatrolTarget =
                currentPatrolTarget == pointA
                ? pointB
                : pointA;

            patrolPauseTimer = patrolPauseTime;
        }
    }

    private void HandleChase()
    {
        Vector3 target =
            player.position;

        target.y = GetTerrainHeight(target);

        MoveTowards(target, chaseSpeed);
    }

    private void HandleReturnHome()
    {
        Transform nearestPoint = GetNearestPatrolPoint();

        if (nearestPoint == null)
        {
            return;
        }

        Vector3 target =
            nearestPoint.position;

        target.y = GetTerrainHeight(target);

        MoveTowards(target, returnHomeSpeed);

        float distance =
            Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(target.x, target.z));

        if (distance <= reachDistance)
        {
            currentPatrolTarget =
                nearestPoint == pointA
                ? pointB
                : pointA;

            currentState = CrabState.Patrol;
        }
    }

    private void HandleAttack()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        direction.Normalize();

        SmoothFaceDirection(direction);

        if (!isLunging)
        {
            attackWindupTimer -= Time.deltaTime;

            if (attackWindupTimer <= 0f)
            {
                lungeTarget =
                    transform.position +
                    direction * lungeDistance;

                lungeTarget.y =
                    GetTerrainHeight(lungeTarget);

                isLunging = true;
            }
        }
        else
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    lungeTarget,
                    lungeSpeed * Time.deltaTime);

            ClampToTerrain();

            if (Vector3.Distance(
                transform.position,
                lungeTarget) <= 0.05f)
            {
                TryDamagePlayer();

                attackCooldownTimer = attackCooldown;

                Vector3 backDir =
                    transform.position - player.position;

                backDir.y = 0f;

                backDir.Normalize();

                backstepTarget =
                    transform.position +
                    backDir * backstepDistance;

                backstepTarget.y =
                    GetTerrainHeight(backstepTarget);

                recoverTimer = recoverTime;

                isRecovering = true;
                isLunging = false;

                currentState = CrabState.Recover;
            }
        }
    }

    private void HandleRecover()
    {
        if (!isRecovering)
        {
            return;
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                backstepTarget,
                backstepSpeed * Time.deltaTime);

        ClampToTerrain();

        recoverTimer -= Time.deltaTime;

        if (recoverTimer <= 0f)
        {
            isRecovering = false;
            currentState = CrabState.Chase;
        }
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction =
            target - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        direction.Normalize();

        SmoothFaceDirection(direction);

        Vector3 newPosition =
            Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime);

        float terrainHeight =
            GetTerrainHeight(newPosition);

        newPosition.y = terrainHeight;

        // Prevent crab from reaching water surface
        float maxY = waterSurfaceY - surfaceOffset;

        newPosition.y =
            Mathf.Min(newPosition.y, maxY);

        transform.position = newPosition;
    }

    private void ClampToTerrain()
    {
        Vector3 pos = transform.position;

        float terrainHeight =
            GetTerrainHeight(pos);

        pos.y = terrainHeight;

        float maxY =
            waterSurfaceY - surfaceOffset;

        pos.y = Mathf.Min(pos.y, maxY);

        transform.position = pos;
    }

    private float GetTerrainHeight(Vector3 worldPosition)
    {
        if (terrain == null)
        {
            return worldPosition.y;
        }

        float height =
            terrain.SampleHeight(worldPosition) +
            terrain.transform.position.y;

        return height + terrainOffset;
    }

    private void SmoothFaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float targetYaw =
            Mathf.Atan2(direction.x, direction.z) *
            Mathf.Rad2Deg;

        float finalYaw =
            targetYaw + modelYawOffset;

        Quaternion targetRotation =
            Quaternion.Euler(
                originalX,
                finalYaw,
                originalZ);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSmoothSpeed);
    }

    private void ForceUprightRotation()
    {
        if (!keepUpright)
        {
            return;
        }

        Vector3 euler =
            transform.rotation.eulerAngles;

        transform.rotation =
            Quaternion.Euler(
                originalX,
                euler.y,
                originalZ);
    }

    private void TryDamagePlayer()
    {
        if (!canDamage ||
            playerHealth == null ||
            playerHealth.IsDead())
        {
            return;
        }

        float distance =
            Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(player.position.x, player.position.z));

        if (distance > attackRange + 0.25f)
        {
            return;
        }

        playerHealth.TakeDamage(damage);

        if (playerHitFeedback != null)
        {
            playerHitFeedback.PlayHitFeedback();
        }

        canDamage = false;

        Invoke(nameof(ResetDamage), damageCooldown);
    }

    private void ResetDamage()
    {
        canDamage = true;
    }

    private Transform GetNearestPatrolPoint()
    {
        float distA =
            Vector3.Distance(transform.position, pointA.position);

        float distB =
            Vector3.Distance(transform.position, pointB.position);

        return distA <= distB
            ? pointA
            : pointB;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistanceFromHome);
    }
}