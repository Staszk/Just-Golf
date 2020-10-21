using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour
{
    public void Swing()
    {
        EventController.FireEvent(new SwingAnimationMessage());
    }
}
