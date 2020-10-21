using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Head Placement", menuName = "Head Placement")]
public class HeadPlacement : ScriptableObject
{
    public Vector3 Offset = Vector3.zero;
    public Vector3 ScaleValues = Vector3.zero;
}
