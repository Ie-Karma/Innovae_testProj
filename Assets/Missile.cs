using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
	public GameObject explosionPrefab;
	private Rigidbody rb;
	private List<Tank> damagedTanks = new List<Tank>();

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Init(Vector3 velocity, bool _isGhost)
	{
		rb.AddForce(velocity, ForceMode.Impulse);
	}

	public void OnCollisionEnter(Collision col)
	{
		//Instantiate(poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
		var explosion = Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
		explosion.SetActive(true);
		if (damagedTanks.Count>0)
		{
			foreach(Tank tank in damagedTanks)
			{
				tank.TakeDamage(20);
			}
		}

		TurnsManager.instance.ContinueTurn();

		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent<Tank>(out Tank tankCol)) {

			if(!damagedTanks.Contains(tankCol))
			{
				damagedTanks.Add(tankCol);
			}

		}
	}

	private void OnTriggerExit(Collider other)
	{

		if (other.TryGetComponent<Tank>(out Tank tankCol))
		{

			if (damagedTanks.Contains(tankCol))
			{
				damagedTanks.Remove(tankCol);
			}

		}

	}

}