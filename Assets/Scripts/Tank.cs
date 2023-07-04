using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
	NPC,
	Player
}

public class Tank : MonoBehaviour
{
	public TankType tankType;

	[Space,Header("Tank Stats")]
	public float HP = 100;
	public float damage = 20;

	[Space, Header("Turn manager")]
	public bool canMove = false;
	public bool canAttack = false;

	[Space, Header("Other")]
	public GameObject missilePrefab;
	public Transform shootPoint;


	public void TakeDamage(float damage)
	{
		if (HP <= 0) return;
		HP -= damage;
		if (HP <= 0)
		{
			Die();
		}
	}

	private void Die()
	{

		Debug.Log("Tank Die");
		//Destroy(gameObject);

	}

}
