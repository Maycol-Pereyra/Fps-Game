using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject impactEffectPrefab;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Visual impact only - hit detection is handled by raycast in ShootingSystem
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(impact, 2f);
            }
            else
            {
                CreateImpactSpark(contact.point, contact.normal);
            }
        }

        Destroy(gameObject);
    }

    private void CreateImpactSpark(Vector3 position, Vector3 normal)
    {
        GameObject sparkObj = new GameObject("ImpactSpark");
        sparkObj.transform.position = position;
        sparkObj.transform.rotation = Quaternion.LookRotation(normal);

        ParticleSystem ps = sparkObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 3f;
        main.startSize = 0.05f;
        main.startColor = new Color(1f, 0.8f, 0.3f);
        main.maxParticles = 10;
        main.duration = 0.1f;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 8) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;

        var renderer = sparkObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.color = new Color(1f, 0.8f, 0.3f);

        Destroy(sparkObj, 1f);
    }
}
