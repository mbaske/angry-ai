using UnityEngine;
using System.Collections;

/*
	All pieces of Detonator inherit from this. 
*/
public abstract class DetonatorComponent : MonoBehaviour
{
	public bool on = true;
	public bool detonatorControlled = true;
	
	[HideInInspector] 
	public  float startSize = 1f;
	public  float size = 1f;
	
	public  float explodeDelayMin = 0f;
	public  float explodeDelayMax = 0f;
	
	[HideInInspector]  
	public  float startDuration = 2f;
	public  float duration = 2f;
	
	[HideInInspector]
	public  float timeScale = 1f;
	
	[HideInInspector] 
	public  float startDetail = 1f;
	public  float detail = 1f;
	
	[HideInInspector] 
	public  Color startColor = Color.white;
	public  Color color = Color.white;
	
	[HideInInspector]
	public  Vector3 startLocalPosition = Vector3.zero;
	public  Vector3 localPosition = Vector3.zero;
	
	[HideInInspector]
	public  Vector3 startForce = Vector3.zero;
	public  Vector3 force = Vector3.zero;
	
	[HideInInspector]
	public  Vector3 startVelocity = Vector3.zero;
	public  Vector3 velocity = Vector3.zero;
	
	public abstract void Explode();
	
	//The main Detonator calls this instead of using Awake() or Start() on subcomponents
	//which ensures it happens when we want.
	public abstract void Init();
	
	public  float detailThreshold;
	
	/*
		This exists because Detonator makes relative changes
		to set values once the game is running, so we need to store their beginning
		values somewhere to calculate against. An improved design could probably
		avoid this.
	*/
	public void SetStartValues()
	{
		startSize = size;
		startForce = force;
		startVelocity = velocity;
		startDuration = duration;
		startDetail = detail;
		startColor = color;
		startLocalPosition = localPosition;
	}
	
	//implement functions to find the Detonator on this GO and get materials if they are defined
	public Detonator MyDetonator()
	{
		Detonator _myDetonator = GetComponent("Detonator") as Detonator;
		return _myDetonator;
	}
}