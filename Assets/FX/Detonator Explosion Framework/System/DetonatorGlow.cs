using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Glow")]
public class DetonatorGlow : DetonatorComponent
{
	private float _baseSize = 1f;
	private float _baseDuration = 1f;
	private Vector3 _baseVelocity = new Vector3(0f, 0f, 0f);
	private Color _baseColor = Color.black;
	private float _scaledDuration;
	
	private GameObject _glow;
	private DetonatorBurstEmitter _glowEmitter;
	public Material glowMaterial;
		
	override public void Init()
	{
		//make sure there are materials at all
		FillMaterials(false);
		BuildGlow();
	}
	
	//if materials are empty fill them with defaults
	public void FillMaterials(bool wipe)
	{
		if (!glowMaterial || wipe)
		{
			glowMaterial = MyDetonator().glowMaterial;
		}
	}
	
	//Build these to look correct at the stock Detonator size of 10m... then let the size parameter
	//cascade through to the emitters and let them do the scaling work... keep these absolute.
    public void BuildGlow()
    {
		_glow = new GameObject("Glow");
		_glowEmitter = (DetonatorBurstEmitter)_glow.AddComponent<DetonatorBurstEmitter>();
		_glow.transform.parent = this.transform;
		_glow.transform.localPosition = localPosition;
		_glowEmitter.material = glowMaterial;
		_glowEmitter.exponentialGrowth = false;
		_glowEmitter.useExplicitColorAnimation = true;
		_glowEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		
    }
	
	public void UpdateGlow()
	{
		//this needs
		_glow.transform.localPosition = Vector3.Scale(localPosition,(new Vector3(size, size, size)));
		
		_glowEmitter.color = color;
		_glowEmitter.duration = duration;
		_glowEmitter.timeScale = timeScale;
		_glowEmitter.count = 1;
		_glowEmitter.particleSize = 65f;
		_glowEmitter.randomRotation = false;
		_glowEmitter.sizeVariation = 0f;
		_glowEmitter.velocity = new Vector3(0f, 0f, 0f);
		_glowEmitter.startRadius = 0f;
		_glowEmitter.sizeGrow = 0;
		_glowEmitter.size = size;		
		_glowEmitter.explodeDelayMin = explodeDelayMin;
		_glowEmitter.explodeDelayMax = explodeDelayMax;

		Color stage1 = Color.Lerp(color, (new Color(.5f, .1f, .1f, 1f)),.5f);
		stage1.a = .9f;
		
		Color stage2 = Color.Lerp(color, (new Color(.6f, .3f, .3f, 1f)),.5f);
		stage2.a = .8f;
		
		Color stage3 = Color.Lerp(color, (new Color(.7f, .3f, .3f, 1f)),.5f);
		stage3.a = .5f;
		
		Color stage4 = Color.Lerp(color, (new Color(.4f, .3f, .4f, 1f)),.5f);
		stage4.a = .2f;
		
		Color stage5 = new Color(.1f, .1f, .4f, 0f);
		
		_glowEmitter.colorAnimation[0] = stage1;
		_glowEmitter.colorAnimation[1] = stage2;
		_glowEmitter.colorAnimation[2] = stage3;
		_glowEmitter.colorAnimation[3] = stage4;
		_glowEmitter.colorAnimation[4] = stage5;
	}

	void Update () 
	{
		//others might be able to do this too... only update themselves before exploding?
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
			UpdateGlow();
			_glowEmitter.Explode();
		}
    }

}
