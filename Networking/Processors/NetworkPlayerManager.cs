using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProcessorManager")]
public class NetworkPlayerManager : MonoBehaviour
{
    private Vector3 PlayerGravity;
    private int _myID;
    private NetworkManager network = null;

    private void Start()
    {
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();
        PlayerGravity = new Vector3(0, network.GetLocalPlayer().GetComponent<PlayerMovement>().Gravity, 0);
    }

    /// <summary>
    /// Updates the transform a specific object in a specific list.
    /// </summary>
    /// <param name="id"> The id in the list </param>
    /// <param name="mat"> The matrix holding the transform data </param>
    /// <param name="itemNum"> The code to determine which part of the transform to update </param>
    /// <param name="obj"> The where the object to update is contained in </param>
    /// <param name="interpolate"> Should the object snap to the position or interpolate. </param>
    internal void UpdateTransform(GameObject obj, Matrix4x4 mat, string itemNum, bool interpolate = false)
    {
        if (obj == null)
            return;

        int size = itemNum.Length;

        for (int i = 0; i < size; i++)
        {
            int num = int.Parse(itemNum[i].ToString());

            switch (num)
            {
                case 1:

                    if (interpolate)
                        obj.transform.position += ((Vector3)mat.GetRow(0) - obj.transform.position) / 4.0f;
                    else
                        obj.transform.position = mat.GetRow(0);

                    break;
                case 2:
                    Vector4 rot = mat.GetRow(1);
                    obj.transform.rotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                    break;
                case 3:
                    obj.transform.localScale = mat.GetRow(2);
                    break;
            }
        }

    }

    /// <summary>
    /// Updates the current pose of a specific player.
    /// </summary>
    /// <param name="splitCode"> The message data from the packets </param>
    internal void UpdatePoses(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        //network.GetPlayerOfID(id).GetComponent<EnemyAnimation>().ToggleMesh();
    }

    /// <summary>
    /// Deadreckon and update the velocity of a specific player 
    /// </summary>
    /// <param name="code">The message data from the packets</param>
    /// <param name="delay"> The delay between sending and recieving packets </param>
    internal void UpdatePlayerMovement(string code, int delay)
    {
        string[] splitCode = code.Split(':');
        int id = int.Parse(splitCode[1]);
        float delayInSeconds = ((float)delay / 1000.0f);

        if (id == _myID || network.GetPlayerOfID(id) == null)
            return;

        Vector3 movement = SerializationScript.StringToVector3(splitCode[2]);
        float x = movement.x * delayInSeconds;
        float z = movement.z * delayInSeconds;
        float y = movement.y * delayInSeconds + 0.5f * PlayerGravity.y * delayInSeconds * delayInSeconds;

        network.GetPlayerOfID(id).transform.position += new Vector3(x, y, z);
        network.GetPlayerOfID(id).GetComponent<Rigidbody>().velocity = movement;

    }

    internal void UpdateHealth(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        int shooterID = int.Parse(splitCode[3]);

        float currentHealth = float.Parse(splitCode[2]);
        IHaveHealth target = network.GetPlayerOfID(id).GetComponent(typeof(IHaveHealth)) as IHaveHealth;
        float damage = target.GetHealth() - currentHealth;
        target?.TakeDamage(damage, shooterID);

        if (shooterID*-1 == _myID) { target?.SimulateDamage(network.GetPlayerOfID(id).transform.position + Vector3.up, damage); }

    }

    internal void TurnOnShield(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        EntityHealth target = network.GetPlayerOfID(id).GetComponent<EntityHealth>();
        target.TurnOnShield();
    }
}
