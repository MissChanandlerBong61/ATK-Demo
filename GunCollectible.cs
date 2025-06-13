using TMPro;
using UnityEngine;
public enum FireMode
{
    Single,
    SemiAuto,
    Auto
}
public class GunCollectible : MonoBehaviour
{
    public GunData gunData;
    public TextMeshPro gunNameText;
    

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50);
        if (gunNameText != null)
        {
            gunNameText.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GunSystem gunSystem = collision.GetComponent<GunSystem>();

            if (gunSystem != null)
            {
                gunSystem.nearGunCollectible = true;
                gunSystem.nearbyGun = this;

                if (gunNameText != null)
                {
                    gunNameText.text = $"Press E to Pick Up " + gunData.gunName;
                    gunNameText.gameObject.SetActive(true);
                }
            }                
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GunSystem gunSystem = collision.GetComponent<GunSystem>();
            if (gunSystem != null)
            {
                gunSystem.nearGunCollectible = false;
                gunSystem.nearbyGun = null;
            }

            if (gunNameText != null)
            {
                gunNameText.gameObject.SetActive(false);
            }
        }
    }
}
