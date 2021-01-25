using UnityEngine;
using System.Collections;

// This one isn't ready for prime time and is not in the menu. Feel free to modify or complete. :)
[RequireComponent (typeof (Detonator))]
public class DetonatorCloudRing : DetonatorComponent
{
	private float _baseSize = 1f;
	private float _baseDuration = 5f;
	private Vector3 _baseVelocity = new Vector3(155f, 5f, 155f);
	private Color _baseColor = Color.white;
	private Vector3 _baseForce = new Vector3(0.162f, 2.56f, 0f);
	
	private GameObject _cloudRing;
	private DetonatorBurstEmitter _cloudRingEmitter;
	public Material cloudRingMaterial;
	
	override public void Init()
	{
		//make sure there are materials at all
		FillMaterials(false);
		BuildCloudRing();
	}
	
	//if materials are empty fill them with defaults
	public void FillMaterials(bool wipe)
	{
		if (!cloudRingMaterial || wipe)
		{
			cloudRingMaterial = MyDetonator().smokeBMaterial;
		}
	}

	//Build these to look correct at the stock Detonator size of 10m... then let the size parameter
	//cascade through to the emitters and let them do the scaling work... keep these absolute.
    public void BuildCloudRing()
    {
		_cloudRing = new GameObject("CloudRing");
		_cloudRingEmitter = (DetonatorBurstEmitter)_cloudRing.AddComponent<DetonatorBurstEmitter>();
		_cloudRing.transform.parent = this.transform;
		_cloudRing.transform.localPosition = localPosition;
		_cloudRingEmitter.material = cloudRingMaterial;
		_cloudRingEmitter.useExplicitColorAnimation = true;
    }
	
	public void UpdateCloudRing()
	{
		_cloudRing.transform.localPosition = Vector3.Scale(localPosition,(new Vector3(size, size, size)));
		
		_cloudRingEmitter.color = color;
		_cloudRingEmitter.duration = duration;
		_cloudRingEmitter.durationVariation = duration/4f;
		_cloudRingEmitter.count = (int)(detail * 50f);
		_cloudRingEmitter.particleSize = 10f;
		_cloudRingEmitter.sizeVariation = 2f;
		_cloudRingEmitter.velocity = velocity;
		_cloudRingEmitter.startRadius = 3f;
		_cloudRingEmitter.size = size;
		_cloudRingEmitter.force = force;
		_cloudRingEmitter.explodeDelayMin = explodeDelayMin;
		_cloudRingEmitter.explodeDelayMax = explodeDelayMax;
		
		//make the starting colors more intense, towards white
		Color color1 = Color.Lerp(color, (new Color(.2f, .2f, .2f, .6f)), 0.5f);
		Color color2 = new Color(.2f, .2f, .2f, .5f);
		Color color3 = new Color(.2f, .2f, .2f, .3f);
		Color color4 = new Color(.2f, .2f, .2f, 0f);
		
		_cloudRingEmitter.colorAnimation[0] = color1;
		_cloudRingEmitter.colorAnimation[1] = color2;
		_cloudRingEmitter.colorAnimation[2] = color2;
		_cloudRingEmitter.colorAnimation[3] = color3;
		_cloudRingEmitter.colorAnimation[4] = color4;
	}

    public void Reset()
    {
		FillMaterials(true);
		on = true;
		size = _baseSize;
		duration = _baseDuration;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
		velocity = _baseVelocity;
		force = _baseForce;
    }

    override public void Explode()
    {
		if (on)
		{
			UpdateCloudRing();
			_cloudRingEmitter.Explode();
		}
    }

}


