using UnityEngine;

public class EntityResourceGather : MonoBehaviour
{
	public DNA<Vector2> DNA;
	public float DirectionChangeTime;
	public float DistToTarget;
	public float DistToStart;
	public float Speed;
	public int ResourceCurrent { get; private set; }
	public int ResourceGathered { get; private set; }
	public int ResourceDelivered { get; private set; }

	private float lastDirectionChangeTime;
	private int geneIndex;
	private Transform _transform;
	private Rigidbody2D rigidBody;

	private const string targetTag = "Target Point";
	private const string startingTag = "Starting Point";

	void Start()
	{
		_transform = this.transform;
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (Time.time - lastDirectionChangeTime >= DirectionChangeTime)
		{
			lastDirectionChangeTime = Time.time;

			geneIndex++;

			if (geneIndex >= DNA.Genes.Length)
			{
				Debug.LogError("Trying to access gene beyond array size");
			}
		}

		Vector2 pos = _transform.localPosition;
		rigidBody.MovePosition(pos + DNA.Genes[geneIndex] * Speed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(targetTag) && ResourceCurrent == 0)
		{
			ResourceCurrent = 1;
			ResourceGathered += ResourceCurrent;
			other.GetComponent<ResourceBank>().Resource -= ResourceCurrent;
		}
		else if (other.CompareTag(startingTag) && ResourceCurrent > 0)
		{
			other.GetComponent<ResourceBank>().Resource += ResourceCurrent;
			ResourceDelivered += ResourceCurrent;
			ResourceCurrent = 0;
		}
	}

	public void Reset()
	{
		geneIndex = 0;
		DistToTarget = 1000;
		DistToStart = 1000;
		lastDirectionChangeTime = Time.time;

		ResourceCurrent = 0;
		ResourceGathered = 0;
		ResourceDelivered = 0;
	}
}
