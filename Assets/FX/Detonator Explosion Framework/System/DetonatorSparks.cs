using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Sparks")]
public class DetonatorSparks : DetonatorComponent
{
	private float _baseSize = 1f;
	private float _baseDuration = 4f;
	private Vector3 _baseVelocity = new Vector3(400f, 400f, 400f);
	private Color _baseColor = Color.white;
//	private float _baseDamping = 0.185f;
	private Vector3 _baseForce = Physics.gravity;
	private float _scaledDuration;
	
	private GameObject _sparks;
	private DetonatorBurstEmitter _sparksEmitter;
	public Material sparksMaterial;
		
	override public void Init()
	{
		//make sure there are materials at all
		FillMaterials(false);
		BuildSparks();
	}
	
	//if materials are empty fill them with defaults
	public void FillMaterials(bool wipe)
	{
		if (!sparksMaterial || wipe)
		{
			sparksMaterial = MyDetonator().sparksMaterial;
		}
	}

	//Build these to look correct at the stock Detonator size of 10m... then let the size parameter
	//cascade through to the emitters and let them do the scaling work... keep these absolute.
    public void BuildSparks()
    {
		_sparks = new GameObject("Sparks");
		_sparksEmitter = (DetonatorBurstEmitter)_sparks.AddComponent<DetonatorBurstEmitter>();
		_sparks.transform.parent = this.transform;
		_sparks.transform.localPosition = localPosition;
		_sparks.transform.localRotation = Quaternion.identity;
		_sparksEmitter.material = sparksMaterial;
		_sparksEmitter.force = Physics.gravity / 3; //don't fall fast - these are sparks
		_sparksEmitter.useExplicitColorAnimation = false;
		_sparksEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_sparksEmitter.upwardsBias = MyDetonator().upwardsBias;
    }
	
	public void UpdateSparks()
	{
		_scaledDuration = (duration * timeScale);
		_sparksEmitter.color = color;
		_sparksEmitter.duration = _scaledDuration/4;
		_sparksEmitter.durationVariation = _scaledDuration;
		_sparksEmitter.count = (int)(detail * 50f);
		_sparksEmitter.particleSize = .5f;
		_sparksEmitter.sizeVariation = .25f;
		
		//get wider as upwardsBias goes up - counterintuitive, but right in this case?
		if (_sparksEmitter.upwardsBias > 0f) 
		{
			_sparksEmitter.velocity = new Vector3(
			(velocity.x / Mathf.Log(_sparksEmitter.upwardsBias)),
			(velocity.y * Mathf.Log(_sparksEmitter.upwardsBias)),
			(velocity.z / Mathf.Log(_sparksEmitter.upwardsBias)));
		}
		else
		{
			_sparksEmitter.velocity = this.velocity;
		}
		
		_sparksEmitter.startRadius = 0f;
		_sparksEmitter.size = size;		
		_sparksEmitter.explodeDelayMin = explodeDelayMin;
		_sparksEmitter.explodeDelayMax = explodeDelayMax;
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
			UpdateSparks();
			_sparksEmitter.Explode();
		}
    }

}
