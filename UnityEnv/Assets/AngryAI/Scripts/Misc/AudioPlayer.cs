using UnityEngine;
using System.Collections.Generic;

public class Sounds
{
    public const int STEP = 1;
    public const int SERVO = 2;
    public const int BLOCK = 4;
    public const int SHOT = 8;
    public const int STRAY = 16;
    public const int GROUND_IMPACT = 32;
    public const int LIGHT_IMPACT = 64;
    public const int MEDIUM_IMPACT = 128;
    public const int HARD_IMPACT = 256;
}

public enum Filter : int
{
    Step = Sounds.STEP,
    Servo = Sounds.SERVO,
    Block = Sounds.BLOCK,
    Shot = Sounds.SHOT,
    Stray = Sounds.STRAY,
    GroundImpact = Sounds.GROUND_IMPACT,
    LightImpact = Sounds.LIGHT_IMPACT,
    MediumImpact = Sounds.MEDIUM_IMPACT,
    HardImpact = Sounds.HARD_IMPACT
}

public class AudioPlayer : MonoBehaviour
{
    public bool IsEnabled => filter != 0;

    [SerializeField]
    private Transform listener;
    [SerializeField]
    [Range(0f, 100f)]
    public float cutoffDistance = 100f;
    [SerializeField]
    [Range(0f, 10f)]
    private float expRollOff = 1f;
    [SerializeField]
    [BitMask(typeof(Filter))]
    private Filter filter;
    private Dictionary<int, AudioClip[]> clips;

    private void Start()
    {
        clips = new Dictionary<int, AudioClip[]>();

        Load(Sounds.STEP, new string[]
        {
            "move/step/step1_short",
            "move/step/step2_short",
            "move/step/step3_short",
            "move/step/step4_short"
        });
        Load(Sounds.SERVO, new string[]
        {
            "move/servo/servo2",
            "move/servo/servo3",
            "move/servo/servo4",
            "move/servo/servo5"
        });
        Load(Sounds.BLOCK, new string[]
        {
            "block/rumble_short",
            "block/block1",
            "block/block3",
            "block/block4"
        });
        Load(Sounds.SHOT, new string[]
        {
            "shot/short"
        });
        Load(Sounds.STRAY, new string[]
        {
            "stray/stray1",
            "stray/stray2",
            "stray/stray3",
            "stray/stray4",
            "stray/stray5",
            "stray/stray6",
            "stray/stray7"
        });
        Load(Sounds.GROUND_IMPACT, new string[]
        {
            "impact/ground/ground1",
            "impact/ground/ground2",
            "impact/ground/ground3",
            "impact/ground/ground4",
            "impact/thud/thud1",
            "impact/thud/thud2",
            "impact/thud/thud3"
        });
        Load(Sounds.LIGHT_IMPACT, new string[]
        {
            "impact/light/light1",
            "impact/light/light2",
            "impact/light/light3",
            "impact/light/light4",
            "impact/light/light6"
        });
        Load(Sounds.MEDIUM_IMPACT, new string[]
        {
            "impact/medium/medium1",
            "impact/medium/medium2",
            "impact/medium/medium3",
            "impact/medium/medium4",
            "impact/medium/medium5"
        });
        Load(Sounds.HARD_IMPACT, new string[]
        {
            "impact/hard/hard1",
            "impact/hard/hard2",
            "impact/hard/hard3",
            "impact/hard/hard4",
            "impact/hard/hard5"
        });
    }

    private void Load(int id, string[] paths)
    {
        int n = paths.Length;
        AudioClip[] c = new AudioClip[n];
        for (int i = 0; i < n; i++)
        {
            c[i] = Resources.Load<AudioClip>("Audio/" + paths[i]);
        }
        clips.Add(id, c);
    }

    public void Play(AudioSource source, int id, float volume = 1f, float pitch = 1f)
    {
        if (IsEnabled)
        {
            float d = Vector3.Distance(source.transform.position, listener.position);
            if (d < cutoffDistance && filter.HasFlag((Filter)id))
            {
                if (pitch < 0f)
                {
                    // e.g. pitch = -0.25f
                    // -> random from +0.75f to +1.25f
                    pitch = 1f + pitch + Random.value * -pitch * 2f;
                }
                volume *= Mathf.Pow(1f - d / cutoffDistance, expRollOff);
                source.pitch = pitch;
                source.PlayOneShot(Util.RndItem(clips[id]), volume);
            }
        }
    }
}