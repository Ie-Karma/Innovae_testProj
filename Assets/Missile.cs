using UnityEngine;

public class Missile : MonoBehaviour
{
	public GameObject explosionPrefab;
	private Rigidbody rb;
	//[SerializeField] private GameObject poofPrefab;
	private bool isGhost;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Init(Vector3 velocity, bool _isGhost)
	{
		isGhost = _isGhost;
		rb.AddForce(velocity, ForceMode.Impulse);
	}

	public void OnCollisionEnter(Collision col)
	{
		if (isGhost) return;
		//Instantiate(poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
		var explosion = Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
		explosion.SetActive(true);
		Destroy(gameObject);
	}
}