using System.Collections;
using UnityEngine;

public class SFX : MonoBehaviour
{
    private Color red = new Color(1f, 0f, 0f, 1f);
    private Color blue = new Color(0f, 0.2f, 1f, 1f);

    private AudioPlayer audioPlayer;
    private AudioSource audioSource;
    private LineRenderer laser;
    private Rigidbody rb;
    private float veloSqrMag;
    private bool isPlayingStepSound;
    private bool isPlayingServoSound;

    public void Initialize(FighterBot robot)
    {
        for (int i = 0; i < 4; i++)
        {
            robot.GetLeg(i).GetOuterLeg().RaiseCollisionEvent += HandleCollisionEvent;
        }
        rb = robot.GetBody().GetRigidbody();
        audioPlayer = Object.FindObjectOfType<AudioPlayer>();
        audioSource = GetComponent<AudioSource>();
        laser = GetComponent<LineRenderer>();
        laser.startWidth = 0.075f;
        laser.endWidth = 0.075f;
        laser.enabled = true;
    }

    private void Update()
    {
        float vsm = rb.velocity.sqrMagnitude;
        float change = Mathf.Abs(veloSqrMag - vsm);

        if (change > 64f)
        {
            audioPlayer.Play(audioSource, Sounds.HARD_IMPACT, 0.75f);
        }
        else if (change > 36f)
        {
            audioPlayer.Play(audioSource, Sounds.MEDIUM_IMPACT, 0.75f);
        }

        if (!isPlayingServoSound && vsm < 16f)
        {
            StartCoroutine(PlayServoSoundCR());
        }

        veloSqrMag = vsm;
    }

    private void HandleCollisionEvent(object sender, CollisionArgs e)
    {
        if (!isPlayingStepSound
            && veloSqrMag > 16f
            && (e.Tag == Tags.GROUND || e.Tag == Tags.OBSTACLE))
        {
            StartCoroutine(PlayStepSoundCR());
        }
    }

    private void SetLineColor(Color color)
    {
        laser.startColor = color;
        laser.endColor = color;
    }

    public void UpdateLaser(TargetingSensor sensor)
    {
        laser.enabled = sensor.IsLocked || sensor.HasObstacle;

        if (laser.enabled)
        {
            SetLineColor(sensor.HasObstacle ? blue : red);
            laser.SetPosition(0, sensor.StartPosition);
            laser.SetPosition(1, sensor.EndPosition);
        }
    }

    private IEnumerator PlayStepSoundCR()
    {
        audioPlayer.Play(audioSource, Sounds.STEP);
        isPlayingStepSound = true;
        yield return new WaitForSeconds(0.25f);
        isPlayingStepSound = false;
    }

    private IEnumerator PlayServoSoundCR()
    {
        audioPlayer.Play(audioSource, Sounds.SERVO, 0.25f, 0.5f);
        isPlayingServoSound = true;
        yield return new WaitForSeconds(1f);
        isPlayingServoSound = false;
    }
}