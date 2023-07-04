using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickMovement : MonoBehaviour
{
	[SerializeField]
	private LayerMask groundMask;
	private NavMeshAgent tank;
	private float maxDistance = 12.50f;

	private void Awake()
	{
		tank = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		//get when player clicks using Right click
		if(Input.GetMouseButtonDown(1)) { 
		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 100f, groundMask))
			{
				float distance = Vector3.Distance(hit.point, this.transform.position);

                if (distance<=maxDistance)
                {
					tank.SetDestination(hit.point);

				}

			}

		}
	}


}
