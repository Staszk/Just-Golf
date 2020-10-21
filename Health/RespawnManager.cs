using System.Collections;
using UnityEngine;


public class RespawnManager : MonoBehaviour
{
    public class SpawnLocation : System.IComparable<SpawnLocation>
    {
        public Transform location;
        public int score;

        public SpawnLocation(Transform t)
        {
            location = t;
            score = 0;
        }

        public int CompareTo(SpawnLocation other)
        {
            return this.score - other.score;
        }
    }

    public static RespawnManager instance; 

    [SerializeField] private Transform[] spawnPoints = null;
    private SpawnLocation[] locations;
    private int size;

    private void Awake()
    {
        instance = this;
        size = spawnPoints.Length;

        locations = new SpawnLocation[size];
        for (int i = 0; i < size; i++)
        {
            locations[i] = new SpawnLocation(spawnPoints[i]);
        }
    }

    public void Death(PlayerHealth health, float respawnTime)
    {
        StartCoroutine(Respawn(health, respawnTime));
    }

    public void Death(GameObject enemy, float respawnTime)
    {
        enemy.GetComponent<EnemyAnimation>().ToggleGameObject();
        StartCoroutine(Respawn(enemy, respawnTime));
    }

    public void Death(GameObject enemy)
    {
        enemy.GetComponent<EnemyAnimation>().ToggleGameObject();
    }

    public void Revive(GameObject enemy, Vector3 position)
    {
        enemy.GetComponent<EnemyAnimation>().ToggleGameObject();
        enemy.GetComponent<EnemyHealth>().ResetPlayer(position);
    }

    public void Revive(PlayerHealth health, Vector3 position)
    {
        EventController.FireEvent(new ClientRespawnMessage());
        health.ResetPlayer(position);
    }

    IEnumerator Respawn(PlayerHealth health, float respawnTime)
    {
        float timer = 0;

        while (timer != respawnTime)
        {
            timer = Mathf.Min(timer + Time.deltaTime, respawnTime);

            EventController.FireEvent(new ClientWaitToRespawnMessage(respawnTime - timer));

            yield return null;
        }
       
        if ((NetworkManager.instance && NetworkManager.instance.IsHost()) || !NetworkManager.instance)
        {
            Vector3 pos = EvaluateNextSpawnPoint(health.MyID);
            health.ResetPlayer(pos);

            if (NetworkManager.instance) { NetworkManager.instance.SendRespawnUpdate(pos, health.MyID);  }

            EventController.FireEvent(new ClientRespawnMessage());
        }
    }

    IEnumerator Respawn(GameObject obj, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        Vector3 pos = EvaluateNextSpawnPoint(NetworkManager.instance.GetIdOfPlayer(obj));
        Revive(obj, pos);
        NetworkManager.instance.SendRespawnUpdate(pos, obj.GetComponent<EnemyHealth>().MyID);
    }

    public Vector3 EvaluateNextSpawnPoint(int thisID)
    {
        if (!NetworkManager.instance)
            return GetRandomRespawnLocation();

        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].score = 0;
        }

        // Score Every spawn location
        for (int i = 0; i < locations.Length; i++)
        {
            // Add points for each enemy within range
            ScoreEnemyDistance(ref locations[i], thisID);

            // Add points for each ball
            //ScoreBallDistance(ref locations[i], thisID);
        }

        System.Array.Sort(locations, delegate (SpawnLocation a, SpawnLocation b) { return a.CompareTo(b); });

        //Debug.LogError("Spawn Score: " + locations[0].score);
        //Debug.LogError("Next Best: " + locations[1].score);

        return locations[0].location.position;
    }

    public Vector3 GetRandomRespawnLocation(){return spawnPoints[Random.Range(0, spawnPoints.Length)].position;}
    public Vector3 GetRandonBallRespawnLocation(){ return spawnPoints[Random.Range(0, 4)].position;  }

    public Vector3 GetRespawnLocationAtIndex(int index){return spawnPoints[index].position;}


    #region Spawn Location Utility AI

    private void ScoreEnemyDistance(ref SpawnLocation sl, int thisID)
    {
        int num = NetworkManager.instance ? NetworkManager.instance.NumOfConnectedPlayers : 1;

        for (int i = 0; i < num; i++) // Four, for player number
        {
            if (i == thisID)
                continue;

            Vector3 posOfPlayer = NetworkManager.instance.GetPlayerOfID(i).transform.position;

            if (Vector3.Distance(posOfPlayer, sl.location.position) <= 55f)
            {
                sl.score += 100;
            }
        }
    }

    private void ScoreBallDistance(ref SpawnLocation sl, int thisID)
    {
        int num = NetworkManager.instance ? NetworkManager.instance.NumOfConnectedPlayers : 1;

        for (int i = 0; i < num; i++) // Four, for player number
        {
            Vector3 posOfBall = NetworkManager.instance.GetBallOfID(i).transform.position;

            if (Vector3.Distance(posOfBall, sl.location.position) <= 20f)
            {
                sl.score += i != thisID ? 25 : -45;
            }
        }
    }

    #endregion
}
