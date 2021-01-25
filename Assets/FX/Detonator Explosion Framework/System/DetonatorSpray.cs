using UnityEngine;
using System.Collections;
/*
	Todo - set duration and color properly (actually, i'm not sure this is possible)
	calculate count based on detail
	inherit velocity
*/

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Object Spray")]
public class DetonatorSpray : DetonatorComponent {

	public GameObject sprayObject;
	public int count = 10;
	public float startingRadius = 0f;
	public float minScale = 1f;
	public float maxScale = 1f;
	
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	override public void Init()
	{
		//unused
	}

	void Update()
	{
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private Vector3 _explosionPosition;
	private float _tmpScale;
	override public void Explode()
	{
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
		}
		if (_explodeDelay <= 0) //if the delayTime is zero
		{
			int detailCount = (int)(detail * count);
			for (int i=0;i<detailCount;i++) 
			{
				Vector3 randVec = Random.onUnitSphere * (startingRadius * size);
				Vector3 velocityVec = new Vector3((velocity.x*size),(velocity.y*size),(velocity.z*size));
				GameObject chunk = Instantiate(sprayObject, (this.transform.position + randVec), this.transform.rotation) as GameObject;
				chunk.transform.parent = this.transform;
				
				//calculate scale for this piece
				_tmpScale = (minScale + (Random.value * (maxScale - minScale)));
				_tmpScale = _tmpScale * size;

				chunk.transform.localScale = new Vector3(_tmpScale,_tmpScale,_tmpScale);

				if (MyDetonator().upwardsBias > 0f) 
				{
					velocityVec = new Vector3(
						(velocityVec.x / Mathf.Log(MyDetonator().upwardsBias)),
						(velocityVec.y * Mathf.Log(MyDetonator().upwardsBias)),
						(velocityVec.z / Mathf.Log(MyDetonator().upwardsBias))
						);
				}

				chunk.GetComponent<Rigidbody>().velocity = Vector3.Scale(randVec.normalized,velocityVec);
				chunk.GetComponent<Rigidbody>().velocity = Vector3.Scale(randVec.normalized,velocityVec);
				Destroy(chunk, (duration * timeScale)); 

				_delayedExplosionStarted = false;
				_explodeDelay = 0f;
			}
		}
		else
		{
			//tell update to start reducing the start delay and call explode again when it's zero
			_delayedExplosionStarted = true;
		}
	}
	
	
	
	public void Reset()
	{
		velocity = new Vector3(15f,15f,15f);
	}
}


