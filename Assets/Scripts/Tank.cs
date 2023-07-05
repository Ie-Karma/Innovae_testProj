using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tank : MonoBehaviour
{
	public enum TankType
	{
		NPC,
		Player
	}

	public TankType tankType;

	[Space,Header("Tank Stats")]
	public float HP = 100;
	public bool alive = true;

	[Space, Header("Turn manager")]
	public bool canMove = false;
	public bool canAttack = false;
	private Vector3 initialPosition;

	[Space, Header("Other")]
	public GameObject missilePrefab;
	public Transform shootPoint;


	private void Start()
	{
		alive = true;
		HP = 100;
		initialPosition = transform.position;

	}

	public void ResetTank()
	{
		alive = true;
		HP = 100;
		canAttack = false;
		canMove = false;
		transform.position = initialPosition;
		this.gameObject.SetActive(true);
	}

	public void TakeDamage(float damage)
	{

		if (HP <= 0) return;
		HP -= damage;
		if (HP <= 0)
		{
			Die();
		}

		if(this.TryGetComponent(out TankAI tankAI)){

			tankAI.UpdatePrecision(true);

		}
		else
		{
			TurnsManager.instance.npcAI.UpdatePrecision(false);
		}

	}

	private void Die()
	{

		alive = false;

	}

}
