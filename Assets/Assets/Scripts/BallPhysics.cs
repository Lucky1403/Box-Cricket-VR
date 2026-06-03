using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    // Set by BowlingMachine before bowl
    [HideInInspector] public float   swingStrength;
    [HideInInspector] public Vector3 seamDirection;

    private Rigidbody rb;
    private bool      hasBounced;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass               = 0.156f;
        rb.drag               = 0f;
        rb.angularDrag        = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation      = RigidbodyInterpolation.Interpolate;
    }

    public void SetSeam(Vector3 dir) => seamDirection = dir.normalized;

    // ── In-flight swing ──────────────────────────────────────────
    void FixedUpdate()
    {
        if (!hasBounced)
            rb.AddForce(transform.right * swingStrength, ForceMode.Force);
    }

    // ── Pitch contact ────────────────────────────────────────────
    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag("Pitch")) return;
        if (hasBounced) return;  // only react to first bounce

        hasBounced = true;

        ApplySpinDeviation();
        ApplySeamDeviation();

        // Reverse swing kicks in after ball hits the pitch
        swingStrength *= 2f;
    }

    void ApplySpinDeviation()
    {
        Vector3 av = rb.angularVelocity;

        if      (av.y >  10f)  rb.AddForce( transform.right   * 2f,  ForceMode.Impulse); // off spin
        else if (av.y < -10f)  rb.AddForce(-transform.right   * 2f,  ForceMode.Impulse); // leg spin
        else if (av.x >  20f)  rb.AddForce( transform.forward * 1.5f, ForceMode.Impulse); // top spinner
        else if (av.x < -20f)  rb.AddForce(-transform.forward * 1.5f, ForceMode.Impulse); // flipper
    }

    void ApplySeamDeviation()
    {
        float dot       = Vector3.Dot(seamDirection, Vector3.up);
        float deviation = dot * Random.Range(-2f, 2f);
        rb.AddForce(transform.right * deviation, ForceMode.Impulse);
    }
}