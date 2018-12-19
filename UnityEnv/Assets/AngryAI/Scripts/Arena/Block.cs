using UnityEngine;

public class Block : PhysicsBody
{
    public AudioSource Audio { get; private set; }

    private BlockSettings settings;
    private Material mat;
    private Color col;

    public void Initialize(BlockSettings settings)
    {
        base.Initialize();
        this.settings = settings;
        Audio = GetComponent<AudioSource>();
        mat = GetComponent<Renderer>().material;
        col = mat.color;
    }

    public void Drop()
    {
        float radius = Random.Range(settings.MinDropRadius, settings.MaxDropRadius);
        float angle = Random.Range(-Mathf.PI, Mathf.PI);

        transform.position = new Vector3
        (
            radius * Mathf.Cos(angle),
            settings.DropHeight,
            radius * Mathf.Sin(angle)
        );
        transform.localScale = new Vector3
        (
            Random.Range(settings.MinSizeX, settings.MaxSizeX),
            Random.Range(settings.MinSizeY, settings.MaxSizeY),
            Random.Range(settings.MinSizeZ, settings.MaxSizeZ)
        );
        transform.rotation = UnityEngine.Random.rotation;

        float size = transform.localScale.magnitude;
        rb.mass = size;
        mat.color = col * (1f - size / settings.MaxSize);
    }
}