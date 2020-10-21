using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrajectory : EventListener
{
    [SerializeField] private GameObject endPointPrefab; 
    [SerializeField] private Projector proj = null;
    [SerializeField] private Material[] projMats = null;

    private const int PUTTER_INDEX = 2;

    private LineRenderer line;

    private int currentClub = 0;
    private float power;

    private GolfBall currentBall;
    private float vectorYIncrease;

    private bool shouldTrackPosition = false;

    private PlayerController pc;
    private GameObject endPoint;
    private bool isHidden = true;

    //Putter
    private float initialOffset = -2f;
    private float currentOffset = 2f;
    private float precentage = 1.0f;
    private float maxOffSet = 4;
    private float maxPower;

    // Start is called before the first frame update
    private void OnEnable()
    {
        EventController.AddListener(typeof(NearbyGolfBallMessage), this);
        EventController.AddListener(typeof(ClubPowerChangedMessage), this);
        EventController.AddListener(typeof(ClubChangedMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(NearbyGolfBallMessage), this);
        EventController.RemoveListener(typeof(ClubPowerChangedMessage), this);
        EventController.RemoveListener(typeof(ClubChangedMessage), this);
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        pc = gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
        //proj.material = projMats[currentPower];
        endPoint = Instantiate(endPointPrefab);
        endPoint.SetActive(false);
        DrawTrajectory(0, false);


    }

    // Update is called once per frame
    void Update()
    {
        if (shouldTrackPosition)
        {
            gameObject.transform.position = currentBall.transform.position;
            if(currentClub == PUTTER_INDEX)
                SetUpProjector();
        }
    }

    public void LateUpdate()
    {
        if (pc.IsDead)
        {
            HideProjector();
        }
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is NearbyGolfBallMessage golfBallMessage)
        {
            currentBall = golfBallMessage.golfBall;
            DrawTrajectory(power, false);
        }
        else if(e is ClubPowerChangedMessage powerChangedMessage)
        {
            power = powerChangedMessage.power;
            //proj.material = projMats[currentPower];
            precentage = (powerChangedMessage.power / maxPower);
            currentOffset = initialOffset + precentage * maxOffSet;
            DrawTrajectory(powerChangedMessage.power, !powerChangedMessage.isInit);
           
        }
        else if (e is ClubChangedMessage clubChangedMessage)
        {
            maxPower = clubChangedMessage.stats.MaxDistance;
            precentage = (clubChangedMessage.stats.MinDistance / clubChangedMessage.stats.MaxDistance);
            currentOffset = initialOffset + precentage * maxOffSet;
            currentClub = clubChangedMessage.index;
            vectorYIncrease = clubChangedMessage.stats.VectorYIncrease;
            //proj.material = projMats[currentPower];
            endPoint.SetActive(false);
            DrawTrajectory(clubChangedMessage.stats.MinDistance, false);           
          
        }
      
    }

    private void DrawTrajectory(float power, bool showEnd)
    {
        if (currentBall == null) {
            line.enabled = false;
            shouldTrackPosition = false;
            HideProjector();
            endPoint.SetActive(false);
            return;
        }

        shouldTrackPosition = true;
        if (currentClub == PUTTER_INDEX)
        {
            SetUpProjector();
            line.enabled = false;
            endPoint.SetActive(false);
        }
        else
        {
            if (showEnd)
            {
                endPoint.SetActive(true);
            }
           
            HideProjector();
            line.enabled = true;
            Vector3 dir = Vector3.forward;
            dir = Vector3.RotateTowards(dir, new Vector3(dir.x, dir.y + vectorYIncrease, dir.z), 1, 0.0f);
            Vector3 velocity = dir.normalized * power;

            line.positionCount = 0;
            int counter = 0;
            float stepper = 0.1f;
            float currentStep = 0;

            while (counter < 20)
            {
                line.positionCount++;
                velocity = CalculatePoint(velocity, currentStep, stepper, counter, power);
                counter++;
                currentStep += stepper;

                if(counter > 1 && SetEndPoint(line.GetPosition(counter - 2), line.GetPosition(counter - 1)))
                {
                    break;
                }              
                
            }
        }   
    }

    private bool SetEndPoint(Vector3 lineP1, Vector3 lineP2)
    {
        RaycastHit hit;
        Vector3 p0 = transform.TransformPoint(lineP1);
        Vector3 p1 = transform.TransformPoint(lineP2);
        Vector3 rayDireciton = p1 - p0;
        bool hashit = Physics.Raycast(p0, rayDireciton, out hit, rayDireciton.magnitude);
        Debug.DrawRay(p0, rayDireciton, Color.red);
        if (hashit)
        {           
            endPoint.transform.position = hit.point;
           
            return true;
        }
        return false;
    }

    Vector3 CalculatePoint(Vector3 velocity, float currentTime, float stepper,  int point, float power)
    {
        Vector3 pos = Vector3.zero; 
        if (currentTime < (power * 0.0025f))
        {
            pos = velocity * currentTime;
        }
        else
        {
            pos = velocity * currentTime + 0.5f * Physics.gravity * stepper * stepper;
        }
        velocity += ((Vector3.up * Physics.gravity.y * 2f * stepper));
        line.SetPosition(point, pos);
        return velocity;
    }

    private void HideProjector()
    {
        if (isHidden)
            return;

        isHidden = true;
        proj.gameObject.SetActive(false);
    }

    private void SetUpProjector()
    {
        proj.transform.position = currentBall.gameObject.transform.position + Vector3.up * 5 + transform.forward * currentOffset;
        proj.transform.rotation = Quaternion.Euler(new Vector3(90, transform.rotation.eulerAngles.y, 0));
        proj.material.SetFloat("_Threshold", 1-precentage);

        if (isHidden)
        {
            proj.gameObject.SetActive(true);
            isHidden = false;
        }
    }
}
