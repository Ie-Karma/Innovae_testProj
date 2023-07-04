using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAI : MonoBehaviour
{

	private  Tank tank;

	private void Awake()
	{
		tank = GetComponent<Tank>();
	}

	public void Move()
	{
		if (tank.canMove)
		{
			Debug.Log("NPC is moving");
			TurnsManager.instance.ContinueTurn();
			tank.canMove = false;
		}
	}

	public void Attack()
	{
		if (tank.canAttack)
		{
			Debug.Log("NPC is attacking");
			TurnsManager.instance.ContinueTurn();
			tank.canAttack = false;
		}

	}
}
