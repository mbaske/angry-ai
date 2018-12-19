using UnityEngine;
using MLAgents;

public class FighterBot : Robot
{
    public TargetingSensor Sensor { get; private set; }

    private NavigatorAgent navigator;
    private BulletPool bulletPool;
    private Transform eye;
    private Transform cam;
    private Texture2D camTexture;
    private SFX sfx;

    public override void Initialize()
    {
        base.Initialize();

        Sensor = new TargetingSensor(body, 20f, 30f);
        bulletPool = Object.FindObjectOfType<BulletPool>();
        eye = movingParts.Find("Eye");
        cam = transform.Find("Cam");

        navigator = GetComponent<NavigatorAgent>();
        resolution res = navigator.brain.brainParameters.cameraResolutions[0];
        // See changes in ML-Agents/Scripts/Agent.cs line 548 ff.
        camTexture = new Texture2D(res.width, res.height, TextureFormat.RGB24, false)
        {
            filterMode = FilterMode.Point
        };
        // See /DepthCam/Shader/DepthAndMotion.cginc
        GetCam().Initialize(ref camTexture);
        navigator.SetCustonTexture(ref camTexture);

        sfx = GetComponentInChildren<SFX>();
        sfx.Initialize(this);
    }

    public override void ReSet()
    {
        base.ReSet();
        navigator.OnWalkerReset();
    }

    public void UpdateSensor()
    {
        Sensor.Sweep();
        sfx.UpdateLaser(Sensor);
    }

    public Bullet Shoot()
    {
        return bulletPool.Shoot(Sensor);
    }

    public DepthCam GetCam()
    {
        return cam.GetComponent<DepthCam>();
    }

    public Color GetAvgTextureColor()
    {
        Color32[] c = camTexture.GetPixels32();
        int n = c.Length;
        int r = 0, g = 0, b = 0;
        for (int i = 0; i < n; i++)
        {
            r += c[i].r;
            g += c[i].g;
            b += c[i].b;
        }
        return new Color(
            r / (float)n / 255f,
            g / (float)n / 255f,
            b / (float)n / 255f
        );
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        cam.position = position;
        // Reduce camera shakiness.
        cam.rotation = Quaternion.Lerp(cam.rotation, body.rotation, 0.25f);

        movingParts.position = position;
        movingParts.rotation = Quaternion.Lerp(movingParts.rotation, Sensor.Rotation, 0.25f);

        // Rotating the eye has no function, just looks better.
        eye.localRotation = Quaternion.Euler(0f, 0f,
            Util.ClampAngle(movingParts.localEulerAngles.y) / Sensor.MaxAngle * 113f);

        sfx.transform.position = position;
    }

    private void Update()
    {
        OnUpdate();
    }
}