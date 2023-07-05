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
	private float maxDistance = 13f;
	private Tank tankComponent;
	private bool moving = false;
	private LineRenderer lineRenderer;

	//line display
	private int lineSegments = 25;
	private float timeBetweenPoints = 0.1f;
	private float shotSpeed = 0;
	private Vector3 shotVelocity;
	[SerializeField]
	private GameObject shotLocation;

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

		missile.GetComponent<Missile>().Init(shotVelocity, false);

		tankComponent.canAttack = false;
		lineRenderer.enabled = false;
		shotLocation.SetActive(false);
		TurnsManager.instance.npcAI.UpdatePrecision(false);

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
		float tiltAngle = Mathf.Clamp(distance * distance * -0.5f, -70, -10);
		tower.transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);

		shotSpeed = Vector3.Distance(tankComponent.shootPoint.transform.position, hit.point) * 1.1f;
		shotSpeed = Mathf.Clamp(shotSpeed, 4, 10);
		shotVelocity = tankComponent.shootPoint.parent.parent.forward * shotSpeed;


		CalculateLine();

	}

	private void CalculateLine()
	{
		lineRenderer.enabled = true;
		shotLocation.SetActive(true);
		lineRenderer.positionCount = Mathf.CeilToInt(lineSegments/timeBetweenPoints)+1;

		Vector3 startPos = tankComponent.shootPoint.position;
		int i = 0;
		Vector3 pos = Vector3.zero;
		lineRenderer.SetPosition(i, startPos);

		for (float t = 0; t < lineSegments; t += timeBetweenPoints)
		{
			i++;
			pos = startPos + t * shotVelocity;
			pos.y = startPos.y + shotVelocity.y * t + 0.5f * Physics.gravity.y * t * t;
			lineRenderer.SetPosition(i, pos);

			Vector3 lastPosition = lineRenderer.GetPosition(i - 1);

			if (Physics.Raycast(lastPosition, (pos - lastPosition).normalized, out RaycastHit hit, (pos - lastPosition).magnitude))
			{
				lineRenderer.SetPosition(i, hit.point);
				lineRenderer.positionCount = i + 1;
				shotLocation.transform.position = hit.point;
				return;
			}

		}

	}

	private void SetNavDestination()
	{
		//get the mouse position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 5f);

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
