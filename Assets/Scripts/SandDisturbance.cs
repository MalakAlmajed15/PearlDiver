using UnityEngine;

public class SandDisturbance : MonoBehaviour
{
    [Header("Effect Settings")]
    public ParticleSystem sandParticles;
    public float raycastDistance = 1.0f;
    public LayerMask seabedLayer;

    void Update()
    {
        CheckSeabed();
    }

    void CheckSeabed()
    {
        float swimInput = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
        bool isSwimming = swimInput > 0.1f;

        // Create a variable to store the exact collision data from the laser
        RaycastHit hit;

        // Shoot the raycast down and output the data to 'hit'
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, seabedLayer))
        {
            if (isSwimming)
            {
                // Snap the particle effect to the terrain height.
                // add a tiny 0.05f offset to the Y value so the particles don't clip inside the floor.
                sandParticles.transform.position = new Vector3(transform.position.x, hit.point.y + 0.05f, transform.position.z);

                if (!sandParticles.isPlaying)
                {
                    sandParticles.Play();
                }
            }
            else
            {
                if (sandParticles.isPlaying) sandParticles.Stop();
            }
        }
        else
        {
            // swam up too high off the floor
            if (sandParticles.isPlaying)
            {
                sandParticles.Stop();
            }
        }
    }
}