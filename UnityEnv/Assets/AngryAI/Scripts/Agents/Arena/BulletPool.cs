using UnityEngine;

// TODO Redesign pool.
public class BulletPool : MonoBehaviour
{
    [SerializeField]
    private int size = 100;
    private int index;
    [SerializeField]
    private Bullet prefab;
    private Bullet[] bullets;
    private AudioPlayer audioPlayer;

    public Bullet Shoot(TargetingSensor sensor)
    {
         // TODO MissingReferenceException ?
        Bullet bullet = bullets[--index] ?? CreateBullet(index);
        index = index == 0 ? size : index;

        bullet.Shoot(sensor);
        audioPlayer.Play(bullet.Audio, Sounds.SHOT, 1f, -0.5f);

        return bullet;
    }

    private Bullet CreateBullet(int index)
    {
        Bullet bullet = Instantiate(prefab, transform).GetComponent<Bullet>();
        bullet.Initialize();
        bullet.RaiseCollisionEvent += HandleCollisionEvent;
        bullets[index] = bullet;
        return bullet;
    }

    private void Start()
    {
        audioPlayer = UnityEngine.Object.FindObjectOfType<AudioPlayer>();

        index = size;
        bullets = new Bullet[size];
        for (int i = 0; i < size; i++)
        {
            CreateBullet(i);
        }
    }

    private void HandleCollisionEvent(object sender, CollisionArgs e)
    {
        Bullet bullet = (Bullet)sender;

        switch (e.Tag)
        {
            case Tags.NONE:
                audioPlayer.Play(bullet.Audio, Sounds.STRAY, 1f, -0.25f);
                break;
            case Tags.GROUND:
                audioPlayer.Play(bullet.Audio, Sounds.GROUND_IMPACT, 1f, -0.25f);
                break;
            case Tags.ROBOT:
                bullet.Sparks1.Play();
                audioPlayer.Play(bullet.Audio, Sounds.LIGHT_IMPACT, 1f, -0.25f);
                break;
            case Tags.OBSTACLE:
            case Tags.WALL:
                bullet.Sparks2.Play();
                audioPlayer.Play(bullet.Audio, Sounds.GROUND_IMPACT, 1f, -0.25f);
                break;
        }
    }
}
