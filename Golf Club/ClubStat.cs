using UnityEngine;

[CreateAssetMenu(fileName = "New Club Stat", menuName = "Stat Object/Club")]
public class ClubStat : ScriptableObject
{
    [SerializeField] private string clubName = "";
    [SerializeField] private float minDistance = 0;
    [SerializeField] private float maxDistance = 0;
    [SerializeField] private float timeForMaxDistance = 1.5f;
    [SerializeField] private float vectorYIncrease = 0;
    [Header("Deprecated")][SerializeField] private float[] distances = { 50, 50, 50};

    private float originalMaxDistance;


    public string ClubName { get { return clubName; } }
    public float MinDistance { get { return minDistance; } }
    public float MaxDistance { get { return maxDistance; } }
    public float TimeForMaxDistance { get { return timeForMaxDistance; } }
    //public float[] Distances { get { return distances; } }
    public float VectorYIncrease { get { return vectorYIncrease; } }

    public void IncreaseMaxDistance(float multiplier)
    {
        originalMaxDistance = maxDistance;
        maxDistance *= multiplier;
    }

    public void ChangeMaxDistanceBackToOriginal()
    {
        maxDistance = originalMaxDistance;
    }
}
