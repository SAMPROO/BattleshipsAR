using UnityEngine;

public class TurretControllerV3 : MonoBehaviour
{
    public GameObject projectile, muzzleFlash;
    public Transform yAxisRotator, xAxisRotator;
    public float velocity, turnSpeed = 100f, muzzleFlashOffset;

    [Space]
    public Transform[] projectileSpawns;
    public Collider[] collidersToIgnore;

    [Space]
    public LaunchArcRenderer arc;

    [Header("For displaying accuracy and range")]
    [Range(1f, 100f)]
    public float accuracy;
    public Transform targetCircle;

    [Space]
    public Color color;
    public LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;

    float maxRange;
    bool movingToTargetRotation = false;
    Quaternion rotatorTarget, currentRotation;

    AudioSource audioSRC;

    private void Start()
    {
        float maxRange0 = velocity * velocity / Physics.gravity.magnitude; // Max range when target is at the same level
        
        maxRange = Mathf.Sqrt(1 + 2 * xAxisRotator.position.y / maxRange0) * maxRange0; // Max range with difference in height (turret height)
    
        currentRotation = Quaternion.LookRotation(xAxisRotator.forward);
        audioSRC = GetComponent<AudioSource>();

        lineRenderer.material.color = color;
        spriteRenderer.color = color;
    }

    private void Update()
    {
        if (movingToTargetRotation)
        {
            currentRotation = Quaternion.RotateTowards(currentRotation, rotatorTarget, Time.deltaTime * turnSpeed);
            yAxisRotator.eulerAngles = Vector3.up * currentRotation.eulerAngles.y;
            xAxisRotator.localEulerAngles = Vector3.right * currentRotation.eulerAngles.x;

            if (Approximately(currentRotation.eulerAngles, rotatorTarget.eulerAngles, 0.0001f))
            {
                Debug.Log(gameObject.name + ": Ready to shoot");                
                movingToTargetRotation = false;
            }
        }
    }

    public void TakeAim(Vector3 inputPosition)
    {
        Vector3 turretPosition = xAxisRotator.position + Vector3.down * xAxisRotator.position.y;
        float inputDistance = Vector3.Distance(turretPosition, inputPosition);
        float targetDistance = inputDistance < maxRange ? inputDistance : maxRange;

        Vector3 targetPosition = turretPosition + (inputPosition - turretPosition).normalized * targetDistance;

        // Chooce a random point to target within the accuracy
        float radius = 1 / accuracy * targetDistance;
        Vector2 point = Random.insideUnitCircle * radius;
        Vector3 aimPosition = targetPosition + new Vector3(point.x, 0, point.y);

        // Aim 
        Debug.Log(gameObject.name + ": Acquiring target..");
        movingToTargetRotation = true;
        Vector3 targetDir = AimAtTargetWithGivenVelocity(xAxisRotator.position, aimPosition, Physics.gravity, velocity);
        rotatorTarget = Quaternion.LookRotation(targetDir);

        // Draw predicted flight path (arc):
        float aimDistance = Vector3.Distance(turretPosition, aimPosition);
        arc.RenderArc(rotatorTarget.eulerAngles, aimDistance < maxRange ? aimDistance : maxRange);

        // Display accuracy and range:
        targetCircle.localScale = radius / transform.lossyScale.x * Vector3.one;
        targetCircle.position = targetPosition;
    }

    public void FireProjectile()
    {
        Debug.Log(gameObject.name + ": Firing..");

        foreach (var spawn in projectileSpawns)
        {
            GameObject projectileInstance = Instantiate(projectile, spawn.position, Quaternion.identity);
            Collider projectileCollider = projectileInstance.GetComponent<Collider>();

            foreach (Collider colliderToIgnore in collidersToIgnore)
            {
                Physics.IgnoreCollision(projectileCollider, colliderToIgnore, true);
            }

            projectileInstance.GetComponent<Rigidbody>().velocity = spawn.forward * velocity;

            Destroy(Instantiate(muzzleFlash, spawn.transform.position + spawn.transform.forward * muzzleFlashOffset, spawn.rotation), 3f);

            Destroy(projectileInstance, 5f);
        }

        audioSRC.Play();
    }

    public Vector3 AimAtTargetWithGivenVelocity(Vector3 start, Vector3 end, Vector3 gravity, float velocity)
    {
        Vector3 AtoB = end - start;

        Vector3 perpenticular = Vector3.Cross(AtoB, gravity);
        perpenticular = Vector3.Cross(gravity, perpenticular);
        Vector3 horizontal = Vector3.Project(AtoB, perpenticular);
        float horizontalDistance = horizontal.magnitude;

        float v2 = velocity * velocity;
        float v4 = v2 * v2;
        float gravMagn = gravity.magnitude;
        float d = 2 * - start.y * v2;

        float launchTest = v4 - (gravMagn * ((gravMagn * horizontalDistance * horizontalDistance) + d));

        if (launchTest < 0)
        {
            Debug.Log(gameObject.name + ": Target outside range");

            // Can't shoot at the target so target max range
            horizontalDistance = maxRange * 0.999f;
            launchTest = v4 - (gravMagn * ((gravMagn * horizontalDistance * horizontalDistance) + d));

            if (launchTest < 0)
            {
                // if the maxRange calculation is incorrect for some reason launch to 45 degree angle
                return (horizontal.normalized * Mathf.Cos(45 * Mathf.Deg2Rad) - gravity.normalized * Mathf.Sin(45 * Mathf.Deg2Rad));
            }
        }
        else
            Debug.Log(gameObject.name + ": Target in range");

        float angle = Mathf.Atan((v2 - Mathf.Sqrt(launchTest)) / (gravMagn * horizontalDistance));

        return (horizontal.normalized * Mathf.Cos(angle) - gravity.normalized * Mathf.Sin(angle));
    }

    public bool Approximately(Vector3 me, Vector3 other, float allowedDifference)
    {
        var dx = me.x - other.x;
        if (Mathf.Abs(dx) > allowedDifference)
            return false;

        var dy = me.y - other.y;
        if (Mathf.Abs(dy) > allowedDifference)
            return false;

        var dz = me.z - other.z;

        return Mathf.Abs(dz) <= allowedDifference;
    }
}
