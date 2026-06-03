using UnityEngine;

public class BowlingMachine : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform releasePoint;

    // ── Randomisation ranges ────────────────────────────────────
    [Header("Speed (km/h)")]
    public float speedMin = 85f;
    public float speedMax = 150f;

    [Header("Pitch angle (degrees)")]
    public float pitchAngleMin = -15f;  // yorker
    public float pitchAngleMax = 8f;   // bouncer

    [Header("Swing force (N) — positive = outswing")]
    public float swingMin = -0.5f;
    public float swingMax = 0.5f;

    [Header("Spin (rad/s) — Y axis, positive = off spin")]
    public float spinMin = -50f;
    public float spinMax = 50f;

    [Header("Line offset — left/right of stumps (metres)")]
    public float lineMin = -0.25f;
    public float lineMax = 0.25f;

    // ── Bowl ─────────────────────────────────────────────────────
    public void Bowl()
    {
        // 1. Randomise every parameter independently
        float speed = Random.Range(speedMin, speedMax) / 3.6f;
        float pitch = Random.Range(pitchAngleMin, pitchAngleMax);
        float swing = Random.Range(swingMin, swingMax);
        float spin = Random.Range(spinMin, spinMax);
        float line = Random.Range(lineMin, lineMax);

        // 2. Spawn ball
        GameObject ball = Instantiate(
            ballPrefab,
            releasePoint.position,
            Quaternion.identity
        );

        // 3. Build launch direction from pitch angle + line offset
        Vector3 forward = Quaternion.Euler(pitch, 0f, 0f) * transform.forward;
        forward += transform.right * line;
        forward.Normalize();

        // 4. Apply velocity
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = forward * speed;
        rb.angularVelocity = new Vector3(0f, spin, 0f);

        // 5. Pass swing to ball physics
        BallPhysics bp = ball.GetComponent<BallPhysics>();
        bp.swingStrength = swing;
        bp.SetSeam(Random.insideUnitSphere);

        // 6. Auto-destroy so the scene stays clean
        Destroy(ball, 6f);
    }

    // Call this from a VR button, a timer, or any external trigger
    public void BowlOnTimer(float interval)
    {
        InvokeRepeating(nameof(Bowl), 1f, interval);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Bowl();
    }

    public void StopBowling() => CancelInvoke(nameof(Bowl));
}