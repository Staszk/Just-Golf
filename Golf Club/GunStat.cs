using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Stat", menuName = "Stat Object/Gun")]
public class GunStat : ScriptableObject
{
    [SerializeField] private string gunName = "New Gun";
    [SerializeField] private float damage = 0;
    [SerializeField] private float range = 0;
    [Tooltip("Time between shots (in seconds).")][SerializeField] private float fireRate = 0;
    [SerializeField] private float spread = 0;

    [Header("Camera/UI")]
    [SerializeField] private float zoomSpeed = 0.25f;
    [Range(1, 2)][SerializeField] private int zoomDepth = 1;

    public string GunName { get { return gunName; } }
    public float Damage { get { return damage; } }
    public float Range { get { return range; } }
    public float FireRate { get { return fireRate; } }
    public float Spread { get { return spread; } }
    public float ZoomSpeed { get { return zoomSpeed; } }
    public int ZoomDepth { get { return zoomDepth; } }
}
