using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CamTarget : MonoBehaviour
{
    [SerializeField]
    private bool autoCycle = true;
    [SerializeField]
    private float followDuration = 5f; // if autoCycle
    [SerializeField]
    private float interpolation = 0.05f;

    private int outerBound = 50 * 50;
    private float lowVeloThresh = 9f; // squared

    private FighterBot[] robots;
    private FighterBot selected;
    private RawImage uiImage;
    private Texture2D demoTexture;
    private Coroutine crFollow;
    private float time;
    private bool hasUI;
    private bool showTexture;

    private void Start()
    {
        uiImage = Object.FindObjectOfType<RawImage>();
        hasUI = uiImage != null;
        if (hasUI)
        {
            uiImage.enabled = false;
        }

        Transform agents = GameObject.Find("Agents").transform;
        int n = agents.childCount;
        robots = new FighterBot[n];
        for (int i = 0; i < n; i++)
        {
            robots[i] = agents.GetChild(i).GetComponent<FighterBot>();
        }

        Follow(robots[0]);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StopFollowCR();
                SelectNext();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                StopFollowCR();
                Follow(Util.RndItem(robots));
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                autoCycle = !autoCycle;
            }
            else if (Input.GetKeyDown(KeyCode.T) && hasUI)
            {
                ToggleTexture(selected, !showTexture);
            }
        }

        transform.position = Vector3.Lerp(transform.position, selected.GetPosition(), interpolation);
    }

    private void Follow(FighterBot robot)
    {
        if (robot != selected)
        {
            if (showTexture)
            {
                ToggleTexture(selected, false);
                ToggleTexture(robot, true);
            }
            selected = robot;
            time = Time.time;
        }
        crFollow = StartCoroutine(FollowCR());
    }

    private void SelectNext()
    {
        List<FighterBot> candidates = new List<FighterBot>();
        List<FighterBot> preferedCandidates = new List<FighterBot>();

        for (int i = 0; i < robots.Length; i++)
        {
            FighterBot robot = robots[i];
            if (robot != selected && robot.GetPosition().sqrMagnitude < outerBound 
                && IsUpright(robot) && HasMatchingVelocity(robot))
            {
                candidates.Add(robot);
                if (robot.Sensor.IsLocked)
                {
                    preferedCandidates.Add(robot);
                }
            }
        }

        if (preferedCandidates.Count > 0)
        {
            candidates = preferedCandidates;
        }

        float minDist = Mathf.Infinity;
        FighterBot closest = selected;
        foreach (FighterBot robot in candidates)
        {
            float d = Vector3.Distance(transform.position, robot.GetPosition());
            if (d < minDist)
            {
                closest = robot;
                minDist = d;
            }
        }
        Follow(closest);
    }

    private bool NeedToSwitch()
    {
        return Time.time - time > followDuration || !HasMatchingVelocity(selected);
    }

    private bool HasMatchingVelocity(Robot robot)
    {
        return robot.GetVelocity(false).sqrMagnitude > lowVeloThresh;
    }

    private bool IsUpright(Robot robot)
    {
        return robot.GetTilt() > 0.5f;
    }

    private void StopFollowCR()
    {
        if (crFollow != null)
        {
            StopCoroutine(crFollow);
        }
    }

    private IEnumerator FollowCR()
    {
        yield return new WaitForSeconds(1f);

        if (autoCycle && NeedToSwitch())
        {
            SelectNext();
        }
        else
        {
            Follow(selected);
        }
    }

    private void ToggleTexture(FighterBot robot, bool show)
    {
        if (robot != null)
        {
            uiImage.texture = robot.GetCam().Texture;
            uiImage.enabled = show;
            showTexture = show;
        }
    }
}