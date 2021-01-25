using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Heatwave")]
public class DetonatorHeatwave : DetonatorComponent {
	
	private GameObject _heatwave;
	
	private float s;
	private float _startSize;
	private float _maxSize;
	private float _baseDuration = .25f;
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	public float zOffset = .5f;
	public float distortion = 64;
	
	private float _elapsedTime = 0f;
	private float _normalizedTime;
	
	public Material heatwaveMaterial;
	private Material _material; //tmp material we alter at runtime;
	
	override public void Init()
	{
		//we don't want to do anything until we explode
	}
	
	void Update () 
	{
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
		
        //_heatwave doesn't get defined unless SystemInfo.supportsImageEffects is true, checked in Explode()
		if (_heatwave)
		{
			// billboard it so it always faces the camera - can't use regular lookat because the built in Unity plane is lame
			_heatwave.transform.rotation = Quaternion.FromToRotation(Vector3.up, Camera.main.transform.position - _heatwave.transform.position); 
			_heatwave.transform.localPosition = localPosition + (Vector3.forward * zOffset);
			
			_elapsedTime = _elapsedTime + Time.deltaTime;
			_normalizedTime = _elapsedTime/duration;
			
			//thought about this, and really, the wave would move linearly, fading in amplitude. 
			s = Mathf.Lerp(_startSize,_maxSize,_normalizedTime);
			
			_heatwave.GetComponent<Renderer>().material.SetFloat("_BumpAmt", ((1-_normalizedTime) * distortion));
			
			_heatwave.gameObject.transform.localScale = new Vector3(s,s,s);
			if (_elapsedTime > duration) Destroy(_heatwave.gameObject);
		}
	}
	
	override public void Explode()
	{
        //try to early out if we can't draw this (not sure if this also gets us out of Unity Indie)
        if (SystemInfo.supportsImageEffects)
        {
            if ((detailThreshold > detail) || !on) return;

            if (!_delayedExplosionStarted)
            {
                _explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
            }
            if (_explodeDelay <= 0)
            {
                //incoming size is based on 1, so we multiply here
                _startSize = 0f;
                _maxSize = size * 10f;

                _material = new Material(Shader.Find("HeatDistort"));
                _heatwave = GameObject.CreatePrimitive(PrimitiveType.Plane);
				_heatwave.name = "Heatwave";
				_heatwave.transform.parent = this.transform;
				Destroy(_heatwave.GetComponent(typeof(MeshCollider)));

                if (!heatwaveMaterial) heatwaveMaterial = MyDetonator().heatwaveMaterial;
                _material.CopyPropertiesFromMaterial(heatwaveMaterial);
                _heatwave.GetComponent<Renderer>().material = _material;
                _heatwave.transform.parent = this.transform;

                _delayedExplosionStarted = false;
                _explodeDelay = 0f;
            }
            else
            {
                _delayedExplosionStarted = true;
            }
        }
	}
	
	public void Reset()
	{
		duration = _baseDuration;
	}
}