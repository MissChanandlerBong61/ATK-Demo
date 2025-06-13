using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

public class GunSystem : MonoBehaviour
{
    public Transform firePoint;

    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private int maxBurstShots = 3;
    [SerializeField] private float burstCooldown = 2f;
    [SerializeField] private float shotCooldown = 0.7f;
    [SerializeField] private GameObject dropPrefab;

    private GunData currentGunData;
    public bool nearGunCollectible = false;
    public GunCollectible nearbyGun = null;

    private bool inBurstCooldown = false;

    private GameObject bulletPrefab;
    private int currentAmmo;
    private int maxAmmo;
    private int clipSize;
    private int clipsRemaining;
    private float bulletSpeed;
    private int damage;
    private FireMode fireMode;
    private Label ammoLabel;
    private VisualElement bulletCount;
    private UIDocument uiDocument;



    private bool isFiring;


    public float fireCooldown = 0.2f;
    private bool isReloading = false;
    private bool canShoot => !isReloading && currentAmmo > 0;


    void Start()
    {
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");
        uiDocument = FindFirstObjectByType<UIDocument>();
        if (uiDocument != null)
        {
            bulletCount = uiDocument.rootVisualElement.Q<VisualElement>("BulletCount");
            ammoLabel = uiDocument.rootVisualElement.Q<Label>("AmmoLabel");
            
        }
        if (bulletCount != null)
            bulletCount.style.display = DisplayStyle.None;
        if (bulletCount == null)
            Debug.LogWarning("BulletCount label not found in UI!");
        UpdateAmmoUI();
    }

    public void EquipGun(GunData data)
    {
        currentGunData = data;

        bulletPrefab = data.bulletPrefab;
        clipSize = data.clipSize;
        maxAmmo = clipSize;
        currentAmmo = maxAmmo;
        clipsRemaining = data.clips;
        bulletSpeed = data.bulletSpeed;
        damage = data.damage;
        fireMode = data.fireMode;

        if (bulletCount != null)
            bulletCount.style.display = DisplayStyle.Flex;

        UpdateAmmoUI();
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) return;

        switch (fireMode)
        {
            case FireMode.Single:
                if (Input.GetMouseButtonDown(0) && !isFiring)
                {
                    TryShoot();
                    StartCoroutine(ShotCooldown());
                }
                break;

            case FireMode.SemiAuto:
                if (Input.GetMouseButtonDown(0) && !inBurstCooldown)
                {
                    StartCoroutine(FireBurst());
                }
                break;

            case FireMode.Auto:
                if (Input.GetMouseButton(0) && !isFiring)
                {
                    StartCoroutine(AutoFire());
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < clipSize)
        {
            StartCoroutine(Reload());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearGunCollectible && nearbyGun != null && currentGunData == null)
            {
                EquipGun(nearbyGun.gunData);
                Destroy(nearbyGun.gameObject);
                nearbyGun = null;
                nearGunCollectible = false;
            }
            else if (currentGunData != null)
            {
                DropGun();
            }
        }

    }

    public void DropGun()
    {
        if (dropPrefab !=null || currentGunData == null) return;

        GameObject droppedGun = Instantiate(currentGunData.dropPrefab, transform.position, Quaternion.identity);
        GunCollectible collectible = droppedGun.GetComponent<GunCollectible>();
       
        if (collectible != null)
        {
            collectible.gunData = currentGunData;
        }

        bulletPrefab = null;
        currentAmmo = 0;
        clipsRemaining = 0;
        currentGunData = null;

        UpdateAmmoUI();
    }

    private IEnumerator AutoFire()
    {
        isFiring = true;
        while (Input.GetMouseButton(0))
        {
            TryShoot();
            yield return new WaitForSeconds(fireCooldown);
        }
        isFiring = false;
    }


    public void TryShoot()
    {
        Debug.Log("Shot fired. Ammo = " + currentAmmo);
        if (currentAmmo <= 0 || bulletPrefab == null) return;
        if (!canShoot || bulletPrefab == null) return;


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
        }
        currentAmmo--;        
        UpdateAmmoUI();

        if (currentAmmo <= 0 && clipsRemaining > 0 && !isReloading)
        {
            
            StartCoroutine(Reload());
            
        }
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
    private void UpdateAmmoUI()
    {
        if (ammoLabel != null)
        {
            ammoLabel.text = $"{currentAmmo} / {clipSize} (x{clipsRemaining})";

            bulletCount.style.display = currentAmmo < 1 && clipsRemaining < 1 ? DisplayStyle.None: DisplayStyle.Flex;
        }                
    }

    private IEnumerator Reload()
    {
        if (currentAmmo >= maxAmmo || clipsRemaining <= 0)
        {
            Debug.Log("No clips left!");
            yield break;
        }

        isReloading = true;

        if (ammoLabel != null)
            ammoLabel.text = "Reloading...";

        yield return new WaitForSeconds(reloadTime);

        clipsRemaining--;
        currentAmmo = clipSize;
        isReloading = false;


        UpdateAmmoUI();
    }
    private IEnumerator FireBurst()
    {
        inBurstCooldown = true;


        int shots = Mathf.Min(maxBurstShots, currentAmmo);
        for (int i = 0; i < shots; i++)
        {
            TryShoot();
            yield return new WaitForSeconds(0.1f); 
        }
        yield return new WaitForSeconds(burstCooldown);        
        inBurstCooldown = false;
    }
    private IEnumerator ShotCooldown()
    {
        isFiring = true;
        yield return new WaitForSeconds(shotCooldown);
        isFiring = false;
    }

}
