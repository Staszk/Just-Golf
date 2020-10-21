using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBallSpawnerScript : MonoBehaviour
{
   [Header("keep these at 0 unless you want to spawn in different location")]
   [Header("Y at 100 is good though")]
   public Vector3 center;
   [Header("area the balls spawn")]
   public Vector3 size;
   public GameObject golfBall;
   public GameObject spawnPos;
   [Header("Amount of balls that will spawn")]
   public int spawnAmount;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      center = new Vector3(spawnPos.transform.position.x, spawnPos.transform.position.y, spawnPos.transform.position.z);
      if (Input.GetKeyDown(KeyCode.P))
      {
         SpawnBall();
      }
    }
   void SpawnBall()
   {
    
      for (int i = spawnAmount; i > 0; i--)
      {
         Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
         Instantiate(golfBall, pos, Quaternion.identity);
      }
      
   }

   void OnDrawGizmosSelected()
   {
      Gizmos.color = new Color(1, 0, 0, 0.5f);
      Gizmos.DrawCube(center, size);
   }
}
