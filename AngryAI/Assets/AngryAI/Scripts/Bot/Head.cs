using UnityEngine;

public class Head : MonoBehaviour
{
    [SerializeField]
    private Vector3 constraints;
    [SerializeField]
    private Transform gun;
    [SerializeField]
    private Bullet bulletPrefab;
    private const float bulletOffset = 0.5f;

    private int lmTarget;
    private Transform[] bones;
    private Vector3[] eulers;
    RaycastResult result;

    private bool isDemo;
    private Transform[] casing;
    private Vector3[] csPos;
    private Vector3[] csVelo;
    private bool casingIsOpen;
    private const float csRange = 0.06f;
    private const float csDamp = 0.075f;

    public void Initialize()
    {
        lmTarget = 1 << Layers.BODY;
        result = new RaycastResult();

        bones = new Transform[5];
        eulers = new Vector3[5];
        Transform bone = transform;
        for (int i = 0; i < 5; i++)
        {
            bones[i] = bone;
            eulers[i] = bone.localEulerAngles;
            bone = bone.parent;
        }

        isDemo = FindObjectOfType<DemoAcademy>() != null;

        if (isDemo)
        {
            casing = new Transform[2] { transform.GetChild(0), transform.GetChild(1) };
            csPos = new Vector3[2] { casing[0].localPosition, casing[1].localPosition };
            csVelo = new Vector3[2];
        }
    }

    public Bullet ShootBullet()
    {
        Bullet bullet = bulletPrefab.Spawn(gun.position - transform.right * bulletOffset);
        bullet.Shoot(-transform.right); // should be forward, bone rotations are weird
        return bullet;
    }

    public RaycastResult CastRay(float distance = 20f)
    {
        result.HasHit = Physics.SphereCast(
            transform.position, 0.5f, -transform.right, out RaycastHit hit, distance, lmTarget);

        if (result.HasHit)
        {
            result.HitPoint = hit.point;
            result.NormDistance = (hit.distance / distance) * 2f - 1f;
            result.OpponentBody = hit.collider.gameObject.GetComponent<BodyFighter>();

            if (isDemo)
            {
                casingIsOpen = true;
                CancelInvoke();
            }
        }
        else if (isDemo)
        {
            Invoke("CloseCasing", 1f);
        }

        return result;
    }

    public void StepUpdate(float x, float y, float z)
    {
        for (int i = 0; i < 5; i++)
        {
            // Update all neck bones.
            bones[i].localRotation = Quaternion.Euler(
                eulers[i].x + x * constraints.x,
                eulers[i].y + y * constraints.y,
                eulers[i].z + z * constraints.z
            );
        }

        if (isDemo)
        {
            UpdateCasing();
        }
    }

    private void CloseCasing()
    {
        casingIsOpen = false;
    }

    private void UpdateCasing()
    {
        Vector3 pos = csPos[0];
        pos.z -= (casingIsOpen ? csRange : 0);
        casing[0].localPosition = Vector3.SmoothDamp(
            casing[0].localPosition, pos, ref csVelo[0], csDamp);
        pos = csPos[1];
        pos.z += (casingIsOpen ? csRange : 0);
        casing[1].localPosition = Vector3.SmoothDamp(
            casing[1].localPosition, pos, ref csVelo[1], csDamp);
    }
}

public class RaycastResult
{
    public bool HasHit;
    public float NormDistance;
    public Vector3 HitPoint;
    public BodyFighter OpponentBody;
}
