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

	private void OnCollisionEnter(Collision collision)
	{
		var explosion = Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.Euler(collision.contacts[0].normal));
		explosion.SetActive(true);
		if (damagedTanks.Count > 0)
		{
			foreach (Tank tank in damagedTanks)
			{
				tank.TakeDamage(20);
			}
		}

		TurnsManager.instance.ContinueTurn();

		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Tank tank) && !damagedTanks.Contains(tank))
		{
			damagedTanks.Add(tank);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Tank tank) && damagedTanks.Contains(tank))
		{
			damagedTanks.Remove(tank);
		}
	}

}