using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredBall : GolfBall
{
    private MeshRenderer meshRend;

    public enum BallColor { Red, Blue, Inactive, Count };

    public BallColor BCol { get; private set; }

    public override void Initialize(GolfBallManager gbm)
    {
        base.Initialize(gbm);

        meshRend = GetComponent<MeshRenderer>();
    }

    public void BecomeColor(BallColor c, Material mat)
    {
        //Debug.Log(c.ToString());

        BCol = c;
        meshRend.material = mat;
    }
}
