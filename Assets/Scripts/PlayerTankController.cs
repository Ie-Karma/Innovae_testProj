using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerTankController : MonoBehaviour
{
	[SerializeField]
	private LayerMask groundMask;
	private NavMeshAgent tankNav;
	private float maxDistance = 12.50f;
	private Tank tankComponent;

	private void Awake()
	{
		tankNav = GetComponent<NavMeshAgent>();
		tankComponent = GetComponent<Tank>();
	}

	private void Update()
	{
		//get when player clicks using Right click
		if(Input.GetMouseButtonDown(1) && tankComponent.canMove) {

			SetNavDestination();

		}

		if (tankComponent.canAttack)
		{
			CalculateShot();
		}

		if (Input.GetMouseButtonDown(0) && tankComponent.canAttack)
		{

			Shot();

		}

	}

	private void Shot()
	{

		TurnsManager.instance.turnCount++;
		TurnsManager.instance.ContinueTurn();

	}

	private void CalculateShot()
	{
		


	}

	private void SetNavDestination()
	{
		//get the mouse position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100f, groundMask))
		{
			float distance = Vector3.Distance(hit.point, this.transform.position);

			if (distance <= maxDistance)
			{
				tankNav.SetDestination(hit.point);
				tankComponent.canMove = false;
				TurnsManager.instance.ContinueTurn();

			}

		}
	}

}
