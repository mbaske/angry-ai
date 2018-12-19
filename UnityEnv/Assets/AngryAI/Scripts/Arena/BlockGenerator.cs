using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    [SerializeField]
    private BlockSettings settings;
    [SerializeField]
    private Transform blockPrefab;
    private AudioPlayer audioPlayer;

    private void Start()
    {
        audioPlayer = Object.FindObjectOfType<AudioPlayer>();
    }

    private void OnValidate()
    {
        settings.MaxDropRadius = Mathf.Max(settings.MinDropRadius, settings.MaxDropRadius);

        settings.MaxSizeX = Mathf.Max(settings.MinSizeX, settings.MaxSizeX);
        settings.MaxSizeY = Mathf.Max(settings.MinSizeY, settings.MaxSizeY);
        settings.MaxSizeZ = Mathf.Max(settings.MinSizeZ, settings.MaxSizeZ);

        settings.MaxSize = new Vector3
        (
            settings.MaxSizeX,
            settings.MaxSizeY,
            settings.MaxSizeZ
        ).magnitude;
    }

    private void Update()
    {
        if (transform.childCount < settings.NumBlocks)
        {
            Block block = Instantiate(blockPrefab, transform).GetComponent<Block>();
            block.Initialize(settings);
            block.RaiseCollisionEvent += HandleCollisionEvent;
            block.Drop();
        }
    }

    private void HandleCollisionEvent(object sender, CollisionArgs e)
    {
        Block block = (Block)sender;

        if (e.Tag == Tags.CLEAR)
        {
            block.Drop();
        }
        else if (e.Tag == Tags.GROUND)
        {
            audioPlayer.Play(block.Audio, Sounds.BLOCK, 
                e.Collision.relativeVelocity.magnitude / 15f); // TBD: velocity -> volume
        }
    }
}