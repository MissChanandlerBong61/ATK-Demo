using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Guns/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject bulletPrefab;
    public int clipSize = 10;       
    public int clips = 2;            
    public float bulletSpeed = 10f;
    public int damage = 1;
    public FireMode fireMode = FireMode.SemiAuto;
    public GameObject dropPrefab;
}
