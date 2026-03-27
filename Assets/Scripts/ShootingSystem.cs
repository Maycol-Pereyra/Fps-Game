using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingSystem : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float maxHitDistance = 200f;

    [Header("Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Header("Shell Ejection")]
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private Transform ejectionPort;
    [SerializeField] private float casingEjectForce = 3f;

    [Header("Animation")]
    [SerializeField] private Animator weaponAnimator;

    private float nextFireTime;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (firePoint == null && playerCamera != null)
            firePoint = playerCamera.transform;

        if (weaponAnimator == null)
            weaponAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance != null && GameManager.Instance.IsReloading) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Manual reload with R key
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null && GameManager.Instance.CanReload())
            {
                GameManager.Instance.StartReload();
                if (weaponAnimator != null)
                    weaponAnimator.SetTrigger("Reload");
                return;
            }
        }

        if (mouse.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (GameManager.Instance != null && !GameManager.Instance.TryUseAmmo())
            {
                if (GameAudio.Instance != null)
                    GameAudio.Instance.PlayEmptyGun();
                return;
            }

            Fire();
        }
    }

    private void Fire()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        // Play gunshot sound
        if (GameAudio.Instance != null)
            GameAudio.Instance.PlayGunshot(origin);

        // Fire animation
        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Fire");

        // Raycast for instant hit detection
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxHitDistance))
        {
            ProcessHit(hit);
        }

        // Visual bullet tracer
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : origin;
            GameObject bullet = Instantiate(projectilePrefab, spawnPos + direction * 0.5f, playerCamera.transform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }

        // Muzzle flash
        if (muzzleFlashPrefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.5f);
        }

        // Shell ejection
        EjectCasing();
    }

    private void ProcessHit(RaycastHit hit)
    {
        GameObject hitObj = hit.collider.gameObject;

        // Gas tank
        var gasTank = hitObj.GetComponentInParent<GasTankScript>();
        if (gasTank != null)
        {
            gasTank.isHit = true;
            return;
        }

        // Explosive barrel
        var barrel = hitObj.GetComponentInParent<ExplosiveBarrelScript>();
        if (barrel != null)
        {
            barrel.explode = true;
            return;
        }

        // Pack target
        var targetScript = hitObj.GetComponentInParent<TargetScript>();
        if (targetScript != null)
        {
            targetScript.isHit = true;
            if (GameManager.Instance != null) GameManager.Instance.AddScore(10);
            if (GameAudio.Instance != null) GameAudio.Instance.PlayTargetHit(hit.point);
            return;
        }

        // Custom target
        Target target = hitObj.GetComponentInParent<Target>();
        if (target != null)
        {
            target.OnHit();
            return;
        }

        // Enemy
        EnemyController enemy = hitObj.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.OnHit();
            return;
        }

        // Impact effect on non-target surfaces
        if (GameAudio.Instance != null)
            GameAudio.Instance.PlayImpact(hit.point);
    }

    private void EjectCasing()
    {
        Transform spawnPoint = ejectionPort != null ? ejectionPort : firePoint;
        if (spawnPoint == null) return;

        Vector3 spawnPos = spawnPoint.position;
        GameObject casing;

        if (casingPrefab != null)
        {
            casing = Instantiate(casingPrefab, spawnPos, spawnPoint.rotation);
        }
        else
        {
            // Procedural casing fallback
            casing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            casing.transform.position = spawnPos;
            casing.transform.localScale = new Vector3(0.02f, 0.04f, 0.02f);
            var mr = casing.GetComponent<Renderer>();
            if (mr != null) mr.material.color = new Color(0.85f, 0.65f, 0.13f);
        }

        Rigidbody rb = casing.GetComponent<Rigidbody>();
        if (rb == null) rb = casing.AddComponent<Rigidbody>();

        rb.mass = 0.01f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Vector3 ejectDir = (spawnPoint.right + spawnPoint.up * 0.5f).normalized;
        rb.AddForce(ejectDir * casingEjectForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        Destroy(casing, 5f);
    }
}
