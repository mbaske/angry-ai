using UnityEngine;
using System.Collections;

/*
	DetonatorBurstEmitter is an interface for DetonatorComponents to use to create particles
	
	- Handles common tasks for Detonator... almost every DetonatorComponent uses this for particles
	- Builds the gameobject with emitter, animator, renderer
	- Everything incoming is automatically scaled by size, timeScale, color
	- Enable oneShot functionality

	You probably don't want to use this directly... though you certainly can.
*/

public class DetonatorBurstEmitter : DetonatorComponent
{

	private ParticleSystem _particleSystem;
	private ParticleSystemRenderer _psRenderer;
	private ParticleSystem.EmissionModule _psEmission;
	private ParticleSystem.EmitParams _psEmitParams;
	private ParticleSystem.ShapeModule _psShape;
	private ParticleSystem.MainModule _psMain;
	private ParticleSystem.ColorOverLifetimeModule _psColorOverLifetime;
	private Gradient _psCoLGradient = new Gradient();
	private GradientAlphaKey[] _psCoLGradientAlpha;
	private GradientColorKey[] _psCoLGradientColor;
	private ParticleSystem.LimitVelocityOverLifetimeModule _psVelocityOverLifetime;
	private ParticleSystem.SizeOverLifetimeModule _psSizeOverLifetime;
	[Tooltip ("If left as false, SizeOverLifetieme will be set to a linear line")]
	public bool useExplicitSizeCurve = false;
	public AnimationCurve SizeOverLifetimeCurve = new AnimationCurve();
	private ParticleSystem.MinMaxCurve _psSoLMMCurve;
	ParticleSystemRenderMode _psRenderMode = ParticleSystemRenderMode.Billboard;

	private float _baseDamping = 0.1300004f;
	private float _baseSize = 1f;
	private Color _baseColor = Color.white;
	
	public float damping = 1f;
	public float startRadius = 1f;
	public float maxScreenSize = 2f;
	public bool explodeOnAwake = false;
	public bool oneShot = true;
	public float sizeVariation = 0f;
	public float particleSize = 1f;
	public float count = 1;
	public float sizeGrow = 20f;
	public bool exponentialGrowth = true;
	public float durationVariation = 0f;
	public bool useWorldSpace = true;
	public float upwardsBias = 0f;
	public float angularVelocity = 20f;
	public bool randomRotation = true;
	
	public bool useExplicitColorAnimation = false;
	public Color[] colorAnimation = new Color[5];
	
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	public Material material;
	
	//unused
	override public void Init() 
	{
		print ("UNUSED");
	} 
	
    public void Awake()
    {
		_particleSystem = (gameObject.AddComponent<ParticleSystem>()) as ParticleSystem;		

		if (gameObject.GetComponent<ParticleSystemRenderer>())
			_psRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
		else
			_psRenderer = (gameObject.AddComponent<ParticleSystemRenderer>()) as ParticleSystemRenderer;

		_psEmitParams = new ParticleSystem.EmitParams();
		_psEmission = _particleSystem.emission;
		_psMain = _particleSystem.main;
		_psVelocityOverLifetime = _particleSystem.limitVelocityOverLifetime;
		_psSizeOverLifetime = _particleSystem.sizeOverLifetime;
		_psColorOverLifetime = _particleSystem.colorOverLifetime;
		_psShape = _particleSystem.shape;

		_psMain.loop = false;
		_psMain.startSpeed = 0f;
		_psMain.flipRotation = .5f;

		_psEmission.rateOverTime = 0;
		_psEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 1, 1)});   

		_psVelocityOverLifetime.enabled = true;
		_psVelocityOverLifetime.separateAxes = true;
		_psVelocityOverLifetime.limitX = .1f;
		_psVelocityOverLifetime.limitY = .1f;
		_psVelocityOverLifetime.limitZ = .1f;
		_psVelocityOverLifetime.space = ParticleSystemSimulationSpace.World;

		_psShape.enabled = true;
		_psShape.shapeType = ParticleSystemShapeType.Sphere;
		_psShape.radius = 0.1f;

		_psRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		_psRenderer.receiveShadows = true;

		//Just to start and make sure there is no interference
		_psEmission.enabled = false;

		_particleSystem.hideFlags = HideFlags.HideAndDontSave;
		
		_psVelocityOverLifetime.dampen = _baseDamping;

		_psRenderer.maxParticleSize = maxScreenSize;
		_psRenderer.material = material;
		_psRenderer.material.color = Color.white;

		//Defaulting to a linear curve
		if (!useExplicitSizeCurve)
		{
			SizeOverLifetimeCurve.AddKey(0.0f, 0.1f);
			SizeOverLifetimeCurve.AddKey(1.0f, 1.0f);
		}	

		_psSoLMMCurve = new ParticleSystem.MinMaxCurve(2, SizeOverLifetimeCurve);
		_psSizeOverLifetime.size = _psSoLMMCurve;
		
		if (explodeOnAwake)
		{
			Explode();
		}
    }
	
	private float _emitTime;
	private float speed = 3.0f;
	private float initFraction = 0.1f;
	static float epsilon = 0.01f;
	
	void Update () 
	{		
		//delayed explosion
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private float SizeFunction (float elapsedTime) 
	{
		float divided = 1 - (1 / (1 + elapsedTime * speed));
		return initFraction + (1 - initFraction) * divided;
	}
	
    public void Reset()
    {
		size = _baseSize;
		color = _baseColor;
		damping = _baseDamping;
    }

	
	private float _tmpParticleSize; //calculated particle size... particleSize * randomized size (by sizeVariation)
	private Vector3 _tmpPos; //calculated position... randomized inside sphere of incoming radius * size
	private Vector3 _tmpDir; //calculated velocity - randomized inside sphere - incoming velocity * size
	private Vector3 _thisPos; //handle on this gameobject's position, set inside
	private float _tmpDuration; //calculated duration... incoming duration * incoming timescale
	private float _tmpCount; //calculated count... incoming count * incoming detail
	private float _scaledDuration; //calculated duration... duration * timescale
	private float _scaledDurationVariation; 
	private float _scaledStartRadius; 
	private float _scaledColor; //color with alpha adjusted according to detail and duration
	private float _randomizedRotation;
	private float _tmpAngularVelocity; //random angular velocity from -angularVelocity to +angularVelocity, if randomRotation is true;
	
    override public void Explode()
    {
		if (on)
		{			
			if (useWorldSpace)
				_psMain.simulationSpace = ParticleSystemSimulationSpace.World; 
			else			
				_psMain.simulationSpace = ParticleSystemSimulationSpace.Local; 
			
			_scaledDuration = timeScale * duration;
			_scaledDurationVariation = timeScale * durationVariation;
			_scaledStartRadius = size * startRadius;
			
			if (!_delayedExplosionStarted)
			{
				_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
			}
			if (_explodeDelay <= 0) 
			{				
				if (useExplicitColorAnimation)
				{
					float timeDivision = 1.0f / colorAnimation.Length;
					_psCoLGradientColor = new GradientColorKey[colorAnimation.Length]; 
					_psCoLGradientAlpha = new GradientAlphaKey[colorAnimation.Length]; 

					for (int i = 0; i < colorAnimation.Length; i++)
					{
						_psCoLGradientColor[i] = new GradientColorKey(colorAnimation[i], (timeDivision * i) + timeDivision);
						_psCoLGradientAlpha[i] = new GradientAlphaKey(colorAnimation[i].a, (timeDivision * i) + timeDivision);
					}

					_psCoLGradient.SetKeys(_psCoLGradientColor, _psCoLGradientAlpha);
					_psColorOverLifetime.color = _psCoLGradient;
				}
				else //auto fade
				{
					float timeDivision = 1 / colorAnimation.Length;
					_psCoLGradientColor = new GradientColorKey[colorAnimation.Length]; 
					_psCoLGradientAlpha = new GradientAlphaKey[colorAnimation.Length]; 

					for (int i = 0; i < colorAnimation.Length; i++)
					{
						_psCoLGradientColor[i] = new GradientColorKey(color, timeDivision * i);
					}
					_psCoLGradientAlpha = new GradientAlphaKey[] {new GradientAlphaKey(color.a * .7f, timeDivision * 0), 
																  new GradientAlphaKey(color.a * 1f, timeDivision * 1), 
																  new GradientAlphaKey(color.a * .5f, timeDivision * 2), 
																  new GradientAlphaKey(color.a * .3f, timeDivision * 3), 
																  new GradientAlphaKey(color.a * 0f, timeDivision * 4), };
					_psCoLGradient.SetKeys(_psCoLGradientColor, _psCoLGradientAlpha);
					_psColorOverLifetime.color = _psCoLGradient;
				}
				
				_tmpCount = count * detail;
				if (_tmpCount < 1) _tmpCount = 1;
				
				if (useWorldSpace)
					_thisPos = this.gameObject.transform.position;
				else
					_thisPos = new Vector3(0,0,0);

				for (int i = 1; i <= _tmpCount; i++)
				{
					_tmpPos =  Vector3.Scale(Random.insideUnitSphere, new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius)); 
					_tmpPos = _thisPos + _tmpPos;
									
					_tmpDir = Vector3.Scale(Random.insideUnitSphere, new Vector3(velocity.x, velocity.y, velocity.z)); 
					_tmpDir.y = (_tmpDir.y + (2 * (Mathf.Abs(_tmpDir.y) * upwardsBias)));
					
					if (randomRotation == true)
					{
						_randomizedRotation = Random.Range(-1f,1f);
						_tmpAngularVelocity = Random.Range(-1f,1f) * angularVelocity;
						
					}
					else
					{
						_randomizedRotation = 0f;
						_tmpAngularVelocity = angularVelocity;
					}
					
					_tmpDir = Vector3.Scale(_tmpDir, new Vector3(size, size, size));
					
					 _tmpParticleSize = size * (particleSize + (Random.value * sizeVariation));
					
					_tmpDuration = _scaledDuration + (Random.value * _scaledDurationVariation);

					_psEmitParams.position = _tmpPos;
					_psEmitParams.velocity = _tmpDir;
					_psEmitParams.startSize = _tmpParticleSize;
					_psEmitParams.startLifetime = _tmpDuration;
					_psEmitParams.startColor = color;
					_psEmitParams.rotation = _randomizedRotation;
					_psEmitParams.angularVelocity = _tmpAngularVelocity;
					_particleSystem.Emit(_psEmitParams, 1);
				}
				
				_psMain.startLifetime = _tmpDuration;
				_psRenderer.material = material;
					
				_emitTime = Time.time;
				_delayedExplosionStarted = false;
				_explodeDelay = 0f;

				//End, so emit
				_psEmission.enabled = true;
				_psColorOverLifetime.enabled = true;
				_psSizeOverLifetime.enabled = true;
				_psVelocityOverLifetime.enabled = true;

				_psEmission.enabled = false;
			}
			else
			{
				//tell update to start reducing the start delay and call explode again when it's zero
				_delayedExplosionStarted = true;
			}
		}
    }

}
