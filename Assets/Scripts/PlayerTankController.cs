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
	private bool moving = false;
	private LineRenderer lineRenderer;

	private void Awake()
	{
		tankNav = GetComponent<NavMeshAgent>();
		tankComponent = GetComponent<Tank>();
		lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		//get when player clicks using Right click
		if(Input.GetMouseButtonDown(1) && tankComponent.canMove) {

			SetNavDestination();

		}

		if (tankComponent.canAttack && !moving)
		{
			CalculateShot();
		}

		if (Input.GetMouseButtonDown(0) && tankComponent.canAttack)
		{

			Shot();

		}

		//if this tank is not moving, then continue the turn
		if (Vector3.Distance(this.transform.position, tankNav.destination) < 0.1f && moving)
		{
			moving = false;
			TurnsManager.instance.ContinueTurn();
		}

	}

	private void Shot()
	{
		GameObject missile = Instantiate(tankComponent.missilePrefab, tankComponent.shootPoint.transform.position, tankComponent.shootPoint.parent.parent.rotation);
		missile.SetActive(true);

		GameObject tower = tankComponent.shootPoint.parent.parent.gameObject;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, 100f);

		Vector3 targetPosition = hit.point;
		Vector3 shootDirection = targetPosition - tankComponent.shootPoint.transform.position;

		float distance = Vector3.Distance(tankComponent.shootPoint.transform.position, targetPosition);
		float speed = distance * 1.5f;

		missile.GetComponent<Missile>().Init(shootDirection.normalized * speed, false);

		tankComponent.canAttack = false;
		TurnsManager.instance.turnCount++;
		TurnsManager.instance.ContinueTurn();
	}

	private void CalculateShot()
	{
		GameObject tower = tankComponent.shootPoint.parent.parent.gameObject;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, 100f);

		// Calcular la dirección hacia el punto de impacto
		Vector3 direction = hit.point - tower.transform.position;
		direction.Normalize();

		// Calcular la rotación en el eje Y hacia el punto de impacto
		tower.transform.LookAt(hit.point);

		// Calcular la rotación en el eje X en función de la distancia
		float distance = Vector3.Distance(tower.transform.position, hit.point);
		float tiltAngle = Mathf.Clamp(distance * distance * -0.5f, -45f, 45f);
		tower.transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);
		CalculateLine(hit.point);

	}

	private void CalculateLine(Vector3 targetPosition)
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
				moving = true;
				//TurnsManager.instance.ContinueTurn();

			}

		}
	}

}
