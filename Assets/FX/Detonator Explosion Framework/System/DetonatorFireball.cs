using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Fireball")]
public class DetonatorFireball : DetonatorComponent
{
	private float _baseSize = 1f;
	private float _baseDuration = 3f;
	private Color _baseColor = new Color(1f, .423f, 0f, .5f);
	private Vector3 _baseVelocity = new Vector3(60f, 60f, 60f);
//	private float _baseDamping = 0.1300004f;
	private float _scaledDuration;

	private GameObject _fireballA;
	private DetonatorBurstEmitter _fireballAEmitter;
	public Material fireballAMaterial;
	
	private GameObject _fireballB;
	private DetonatorBurstEmitter _fireballBEmitter;
	public Material fireballBMaterial;
	
	private GameObject _fireShadow;
	private DetonatorBurstEmitter _fireShadowEmitter;
	public Material fireShadowMaterial;
	
	public bool drawFireballA = true;
	public bool drawFireballB = true;
	public bool drawFireShadow = true;
	
	override public void Init()
	{
		//make sure there are materials at all
		FillMaterials(false);
		BuildFireballA();
		BuildFireballB();
		BuildFireShadow();
	}
	
	//if materials are empty fill them with defaults
	public void FillMaterials(bool wipe)
	{
		if (!fireballAMaterial || wipe)
		{
			fireballAMaterial = MyDetonator().fireballAMaterial;
		}
		if (!fireballBMaterial || wipe)
		{
			fireballBMaterial = MyDetonator().fireballBMaterial;
		}
		if (!fireShadowMaterial || wipe)
		{
			if (Random.value > 0.5)
			{
				fireShadowMaterial = MyDetonator().smokeAMaterial;
			}
			else
			{
				fireShadowMaterial = MyDetonator().smokeBMaterial;
			}
		}
	}
	
	private Color _detailAdjustedColor;
	
	//Build these to look correct at the stock Detonator size of 10m... then let the size parameter
	//cascade through to the emitters and let them do the scaling work... keep these absolute.
    public void BuildFireballA()
    {
		_fireballA = new GameObject("FireballA");
		_fireballAEmitter = (DetonatorBurstEmitter)_fireballA.AddComponent<DetonatorBurstEmitter>();
		_fireballA.transform.parent = this.transform;
		_fireballA.transform.localRotation = Quaternion.identity;
		_fireballAEmitter.material = fireballAMaterial;
		_fireballAEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireballAEmitter.upwardsBias = MyDetonator().upwardsBias;
    }
	
	public void UpdateFireballA()
	{
		_fireballA.transform.localPosition = Vector3.Scale(localPosition,(new Vector3(size, size, size)));
		_fireballAEmitter.color = color;
		_fireballAEmitter.duration =  duration * .5f;
		_fireballAEmitter.durationVariation =  duration * .5f;
		_fireballAEmitter.count = 2f;
		_fireballAEmitter.timeScale = timeScale;
		_fireballAEmitter.detail = detail;
		_fireballAEmitter.particleSize = 14f;
		_fireballAEmitter.sizeVariation = 3f;
		_fireballAEmitter.velocity = velocity;
		_fireballAEmitter.startRadius = 4f;
		_fireballAEmitter.size = size;		
		_fireballAEmitter.useExplicitColorAnimation = true;

		//make the starting colors more intense, towards white
		Color fadeWhite = new Color(1f, 1f, 1f, .5f);
		Color fadeRed = new Color(.6f, .15f, .15f, .3f);
		Color fadeBlue = new Color(.1f, .2f, .45f, 0f);
		
		_fireballAEmitter.colorAnimation[0] = Color.Lerp(color, fadeWhite, .8f);
		_fireballAEmitter.colorAnimation[1] = Color.Lerp(color, fadeWhite, .5f);
		_fireballAEmitter.colorAnimation[2] = color;
		_fireballAEmitter.colorAnimation[3] = Color.Lerp(color, fadeRed, .7f);
		_fireballAEmitter.colorAnimation[4] = fadeBlue;
		
		_fireballAEmitter.explodeDelayMin = explodeDelayMin;
		_fireballAEmitter.explodeDelayMax = explodeDelayMax;
	}
	
	public void BuildFireballB()
    {
		_fireballB = new GameObject("FireballB");
		_fireballBEmitter = (DetonatorBurstEmitter)_fireballB.AddComponent<DetonatorBurstEmitter>();
		_fireballB.transform.parent = this.transform;
		_fireballB.transform.localRotation = Quaternion.identity;
		_fireballBEmitter.material = fireballBMaterial;
		_fireballBEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireballBEmitter.upwardsBias = MyDetonator().upwardsBias;
    }
	
	public void UpdateFireballB()
{
		_fireballB.transform.localPosition = Vector3.Scale(localPosition,(new Vector3(size, size, size)));
		_fireballBEmitter.color = color;
		_fireballBEmitter.duration =  duration * .5f;
		_fireballBEmitter.durationVariation = duration * .5f;
		_fireballBEmitter.count = 2f;
		_fireballBEmitter.timeScale = timeScale;
		_fireballBEmitter.detail = detail;
		_fireballBEmitter.particleSize = 10f;
		_fireballBEmitter.sizeVariation = 6f;
		_fireballBEmitter.velocity = velocity;
		_fireballBEmitter.startRadius = 4f;
		_fireballBEmitter.size = size;
		_fireballBEmitter.useExplicitColorAnimation = true;

		//make the starting colors more intense, towards white
		Color fadeWhite = new Color(1f, 1f, 1f, .5f);
		Color fadeRed = new Color(.6f, .15f, .15f, .3f);
		Color fadeBlue = new Color(.1f, .2f, .45f, 0f);
		
		_fireballBEmitter.colorAnimation[0] = Color.Lerp(color, fadeWhite, .8f);
		_fireballBEmitter.colorAnimation[1] = Color.Lerp(color, fadeWhite, .5f);
		_fireballBEmitter.colorAnimation[2] = color;
		_fireballBEmitter.colorAnimation[3] = Color.Lerp(color, fadeRed, .7f);
		_fireballBEmitter.colorAnimation[4] = fadeBlue;
		
		_fireballBEmitter.explodeDelayMin = explodeDelayMin;
		_fireballBEmitter.explodeDelayMax = explodeDelayMax;
	}
	
	public void BuildFireShadow()
    {
		_fireShadow = new GameObject("FireShadow");
		_fireShadowEmitter = (DetonatorBurstEmitter)_fireShadow.AddComponent<DetonatorBurstEmitter>();
		_fireShadow.transform.parent = this.transform;
		_fireShadow.transform.localRotation = Quaternion.identity;
		_fireShadowEmitter.material = fireShadowMaterial;
		_fireShadowEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireShadowEmitter.upwardsBias = MyDetonator().upwardsBias;
    }
	
	public void UpdateFireShadow()
	{
		_fireShadow.transform.localPosition = Vector3.Scale(localPosition,(new Vector3(size, size, size)));
		
			//move slightly towards the main camera so it sorts properly
		_fireShadow.transform.LookAt(Camera.main.transform);
		_fireShadow.transform.localPosition = -(Vector3.forward * 1f);
		
		_fireShadowEmitter.color = new Color(.1f, .1f, .1f, .6f);
		_fireShadowEmitter.duration = duration * .5f;
		_fireShadowEmitter.durationVariation = duration * .5f;
		_fireShadowEmitter.timeScale = timeScale;
		_fireShadowEmitter.detail = 1; //don't scale up count
		_fireShadowEmitter.particleSize = 13f;
		_fireShadowEmitter.velocity = velocity;
		_fireShadowEmitter.sizeVariation = 1f;
		_fireShadowEmitter.count = 4;
		_fireShadowEmitter.startRadius = 6f;
		_fireShadowEmitter.size = size;
		_fireShadowEmitter.explodeDelayMin = explodeDelayMin;
		_fireShadowEmitter.explodeDelayMax = explodeDelayMax;
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
    }

    override public void Explode()
    {
		if (detailThreshold > detail) return;
		
		if (on)
		{
			UpdateFireballA();
			UpdateFireballB();
			UpdateFireShadow();
			if (drawFireballA)	_fireballAEmitter.Explode();
			if (drawFireballB) _fireballBEmitter.Explode();
			if (drawFireShadow)	_fireShadowEmitter.Explode();
		}
    }

}
