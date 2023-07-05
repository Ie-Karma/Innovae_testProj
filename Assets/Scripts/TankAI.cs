using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankAI : MonoBehaviour
{

	private  Tank tankComponent;
	private float movePrecision = 75,shotPrecision = 100;
	private float maxDistance = 13f;
	[SerializeField]
	private GameObject target;
	private Vector3 initialPosition;
	private NavMeshAgent navMeshAgent;
	private bool isMoving = false;
	private float shotSpeed;
	private Vector3 shotVelocity;

	private void Awake()
	{
		tankComponent = GetComponent<Tank>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (isMoving)
		{
			if (Vector3.Distance(transform.position, navMeshAgent.destination) < 0.5f)
			{
				isMoving = false;
				TurnsManager.instance.ContinueTurn();
			}
		}
	}

	public void Move()
	{
		if (tankComponent.canMove)
		{
			Debug.Log("NPC is moving");

			initialPosition = transform.position;

			SetDestination(GenerateNewPosition());
			isMoving = true;
			tankComponent.canMove = false;
		}
	}

	public void Attack()
	{
		if (tankComponent.canAttack)
		{
			Debug.Log("NPC is attacking");
			Shot();
			tankComponent.canAttack = false;
		}

	}

	private void SetDestination(Vector3 destination)
	{
		// Obtener una distancia de parada aleatoria entre 0 y maxDistance
/*
		if(Vector3.Distance(target.transform.position, destination) < maxDistance)
		{
			float stoppingDistance = Random.Range(0f, maxDistance);
			// Establecer la distancia de parada en el NavMeshAgent
			navMeshAgent.stoppingDistance = stoppingDistance;
		}
		else
		{
			navMeshAgent.stoppingDistance = 0;
		}
*/
		// Establecer el destino en el NavMeshAgent
		navMeshAgent.SetDestination(destination);
	}

	private Vector3 GenerateNewPosition()
	{
		// Obtener una posición aleatoria dentro de un radio máximo
		Vector3 randomPosition = Random.insideUnitSphere * maxDistance;
		randomPosition += initialPosition;

		// Calcular la dirección hacia el objetivo
		Vector3 direction = (target.transform.position - randomPosition).normalized;

		// Calcular el porcentaje de precisión
		float precisionPercentage = movePrecision / 100f;

		// Calcular la posición ajustada según la precisión
		Vector3 adjustedPosition = Vector3.Lerp(randomPosition, target.transform.position, precisionPercentage);

		// Verificar si la posición ajustada se encuentra dentro del radio máximo
		float adjustedDistance = Vector3.Distance(adjustedPosition, initialPosition);
		if (adjustedDistance <= maxDistance)
		{
			return adjustedPosition;
		}
		else
		{
			// Si la posición ajustada está fuera del radio máximo, devolver la posición aleatoria original
			return randomPosition;
		}
	}


	private void Shot()
	{
		CalculateShot();

		GameObject missile = Instantiate(tankComponent.missilePrefab, tankComponent.shootPoint.transform.position, tankComponent.shootPoint.parent.parent.rotation);
		missile.SetActive(true);

		missile.GetComponent<Missile>().Init(shotVelocity, false);

		tankComponent.canAttack = false;
		TurnsManager.instance.turnCount++;
	}

	private void CalculateShot()
	{
		GameObject tower = tankComponent.shootPoint.parent.parent.gameObject;

		// Calcular un punto aleatorio en una esfera unitaria
		Vector3 randomPoint = Random.insideUnitSphere;

		// Interpolar entre el punto aleatorio y el objetivo basado en la precisión
		Vector3 targetPoint = Vector3.Lerp(randomPoint, target.transform.position, shotPrecision / 100f);

		// Calcular la dirección hacia el punto de impacto
		Vector3 direction = targetPoint - tower.transform.position;
		direction.Normalize();

		// Calcular la rotación en el eje Y hacia el punto de impacto
		tower.transform.LookAt(targetPoint);

		// Calcular la rotación en el eje X en función de la distancia
		float distance = Vector3.Distance(tower.transform.position, targetPoint);
		float tiltAngle = Mathf.Clamp(distance * distance * -0.5f, -45f, -10);
		tower.transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);

		shotSpeed = Vector3.Distance(tankComponent.shootPoint.transform.position, targetPoint) * 1.1f;
		shotSpeed = Mathf.Clamp(shotSpeed, 4, 10);
		shotVelocity = tankComponent.shootPoint.parent.parent.forward * shotSpeed;
	}



}
