using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankAI : MonoBehaviour
{

	private  Tank tankComponent;
	public float movePrecision = 100,shotPrecision = 80;
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

	internal void SetPrecision(Vector2 npcPrecision)
	{

		movePrecision = npcPrecision.x;
		shotPrecision = npcPrecision.y;

	}
	internal Vector2 GetPrecision()
	{
		return new Vector2(movePrecision, shotPrecision);
	}

	private void Update()
	{
		CheckMoving();
	}

	private void CheckMoving()
	{
		if (!isMoving) return;
		if (Vector3.Distance(transform.position, navMeshAgent.destination) - navMeshAgent.stoppingDistance >= 0.5f) return;
		isMoving = false;
		TurnsManager.instance.ContinueTurn();
	}
	public void UpdatePrecision(bool more)
	{
		var movePrecisionAmount = more ? 10f : -7f;
		movePrecision = Mathf.Clamp(movePrecision + movePrecisionAmount, 0f, 100f);
		var shotPrecisionAmount = more ? 5f : -2f;
		shotPrecision = Mathf.Clamp(shotPrecision + shotPrecisionAmount, 0f, 100f);
	}

	public void Move()
	{
		if (!tankComponent.canMove) return;
		initialPosition = transform.position;

		GenerateNewPosition();
		isMoving = true;
		tankComponent.canMove = false;
	}
	public void Attack()
	{
		if (!tankComponent.canAttack) return;
		Shot();
		tankComponent.canAttack = false;
	}

	private void GenerateNewPosition()
	{
		var direction = (target.transform.position - initialPosition);
		var movePrecisionValue = (100f - movePrecision) / 100f;
		var deviationVector = new Vector3(
			Random.Range(-movePrecisionValue, movePrecisionValue) * 90,
			0f,
			Random.Range(-movePrecisionValue, movePrecisionValue) * 90
		);
		direction += deviationVector;
		direction.Normalize();

		//Calculates a random distance within maxDistance
		var maxDistanceMultiplicator = Mathf.Clamp(Vector3.Distance(target.transform.position, transform.position) / maxDistance, 0f, 1f);
		var distance = Mathf.Lerp(0f, maxDistance, maxDistanceMultiplicator);

		// Calculates the new position by adding the deviation vector to the direction vector and multiplying the distance
		var newPosition = initialPosition + Vector3.Scale(direction, new Vector3(1f, 0f, 1f)).normalized * distance;

		navMeshAgent.SetDestination(newPosition);
		navMeshAgent.stoppingDistance = (1f - maxDistanceMultiplicator) * 3f;
	}

	private void Shot()
    {
        CalculateShot();

        var missile = Instantiate(tankComponent.missilePrefab, tankComponent.shootPoint.transform.position, tankComponent.shootPoint.parent.parent.rotation);
        missile.SetActive(true);

        missile.GetComponent<Missile>().Init(shotVelocity, false);

        tankComponent.canAttack = false;
    }

	private void CalculateShot()
	{
		GameObject tower = tankComponent.shootPoint.parent.parent.gameObject;
		float maxHeight = -70;
		float minHeight = -45;
		float height = -45;

		// Calcular un punto aleatorio en una esfera unitaria
		Vector3 randomPoint = Random.insideUnitSphere;

		// Interpolar entre el punto aleatorio y el objetivo basado en la precisión
		Vector3 targetPoint = Vector3.Lerp(randomPoint, target.transform.position, shotPrecision / 100f);

		// Calcular la dirección hacia el punto de impacto
		Vector3 direction = targetPoint - tower.transform.position;

		float precision = 100 - shotPrecision;
		direction.x += Random.Range(-precision, precision) / 100;
		direction.z += Random.Range(-precision, precision) / 100;
		direction.Normalize();

		// Calcular la rotación en el eje Y hacia el punto de impacto
		tower.transform.LookAt(targetPoint);

		//calcular inclinacion
		if (Vector3.Distance(target.transform.position, this.transform.position) < maxDistance + 1)
		{
			//lanzar rayo hacia el target para ver si colisiona con el o con un obstaculo

			RaycastHit hit;
			hit = new RaycastHit();
			//debug draw this ray

			Debug.DrawRay(tower.transform.position, direction * maxDistance, Color.red, 1f);


			if (Physics.Raycast(tower.transform.position, direction, out hit, maxDistance))
			{
				if (hit.collider.gameObject.tag == "Player")
				{
					height = minHeight;
				}
				else
				{
					//aumentar height entre maxHeight y minHeight segun lo lejos que este el target

					height = Mathf.Lerp(maxHeight, minHeight, Vector3.Distance(target.transform.position, this.transform.position) / maxDistance);

				}
			}
			else
			{
				height = minHeight;
			}

		}

		// Calcular la rotación en el eje X en función de la distancia
		float distance = Vector3.Distance(tower.transform.position, targetPoint);
		float tiltAngle = Mathf.Clamp(distance * distance * -0.5f, height, -10);
		tower.transform.rotation *= Quaternion.Euler(tiltAngle, 0f, 0f);

		shotSpeed = Vector3.Distance(tankComponent.shootPoint.transform.position, targetPoint) * 1.1f;
		shotSpeed = Mathf.Clamp(shotSpeed, 4, 10);
		shotVelocity = tankComponent.shootPoint.parent.parent.forward * shotSpeed;
	}

}
