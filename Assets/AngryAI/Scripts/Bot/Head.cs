using UnityEngine;

namespace MBaske.AngryAI
{
    public class Head : MonoBehaviour
    {
        [SerializeField]
        private Vector3 constraints;
        [SerializeField]
        private Transform gun;
        [SerializeField]
        private float bulletOffset = 0.5f;
        private BulletPool bulletPool;

        private const int nBones = 5;
        private Transform[] bones;
        private Vector3[] eulers;
        RaycastResult rcResult;
        private const float detectionRange = 20;
        private readonly int lmTarget = 1 << Layers.BODY;
        // Should be forward, bone rotations are weird.
        private Vector3 direction => -transform.right;

        private bool isDemo;
        // Animate gun casing in demo.
        private Transform[] casing;
        private Vector3[] csPos;
        private Vector3[] csVelo;
        private bool casingIsOpen;
        private const float csRange = 0.06f;
        private const float csDamp = 0.075f;

        public void Initialize()
        {
            rcResult = new RaycastResult();

            // Store all neck bones and their rotations.
            bones = new Transform[nBones];
            eulers = new Vector3[nBones];
            Transform bone = transform;
            for (int i = 0; i < nBones; i++)
            {
                bones[i] = bone;
                eulers[i] = bone.localEulerAngles;
                bone = bone.parent;
            }

            bulletPool = FindObjectOfType<BulletPool>();
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
            return bulletPool.Shoot(gun.position + direction * bulletOffset, direction);
        }

        public RaycastResult CastRay()
        {
            rcResult.HasHit = Physics.SphereCast(
                transform.position, 0.5f, direction, out RaycastHit hit, detectionRange, lmTarget);

            if (rcResult.HasHit)
            {
                rcResult.HitPoint = hit.point;
                rcResult.NormDistance = (hit.distance / detectionRange) * 2f - 1f;
                rcResult.OpponentBody = hit.collider.gameObject.GetComponent<BodyFighter>();

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

            return rcResult;
        }

        public void StepUpdate(float x, float y, float z)
        {
            for (int i = 0; i < nBones; i++)
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
}