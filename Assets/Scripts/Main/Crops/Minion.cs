using System;
using UnityEngine;
using DG.Tweening;

public enum State
{
	MovingToStart,
	MovingToPlant,
	Delivering,
	Gathering,
	Finished
}

public enum Shape
{
	Triangle,
	Square,
	Hexagon,
	Circle
}

public class Minion : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Transform bodyTransform;
	[SerializeField] SpriteRenderer bodyRenderer;
	[SerializeField] SpriteRendererFill fill;

	[Header("Shapes (Order: Triangle, Square, Hexagon, Circle)")]
	[SerializeField] Sprite[] shapes;

	[Header("Farm")]
	public FarmArea FarmArea;

	public Shape Shape { get; private set; }
	public int SpotIndex { get; private set; }
	public float MoveSpeed { get; private set; }
	public float ActionSpeed { get; private set; }
	public float RotationSpeed { get; private set; }

	public State State { get; private set; }
	public int CarryCapacity { get; private set; }
	public int ResourceGathered { get; private set; }
	public float TimeToCompletion { get; private set; }
	public float CurrentActionTime { get; private set; }

	private int _resourceCarried;
	public int ResourceCarried {
		get { return _resourceCarried; }
		set {
			_resourceCarried = value;
			DOTween.To(() => fill.FillValue, x => fill.FillValue = x, (float)_resourceCarried / CarryCapacity, ActionSpeed / 2);
		}
	}

	public event Action<int> ResourceDeliveredEvent;

	private float scale;
	private float startTime;
	private float targetAngle;
	private Vector3 NextTargetPos { get { return FarmArea.FarmSpots[SpotIndex].transform.position; } }
	private Vector3 SeedBasketPos { get { return FarmArea.FarmSpots[0].transform.position; } }

	private Vector3 trianglePos = new Vector3(0, -0.165f, 0);
	private Vector3 triangleScale = new Vector3(1, 1.33f, 1);

	private static readonly int numShapes = Enum.GetValues(typeof(Shape)).Length;

	public void Init(FarmArea farmArea)
	{
		FarmArea = farmArea;
		transform.position = SeedBasketPos;
	}

	public void Reset(float[] genes, float moveSpeed, float actionSpeed, float rotationSpeed, float timeForAnimation)
	{
		//Gene 0
		CarryCapacity = Mathf.CeilToInt(genes[0] * 8);
		float size = (float)CarryCapacity / 8;
		MoveSpeed = (1 / size * 0.5f + 0.5f) * moveSpeed;
		scale = size * 0.75f + 0.25f;

		//Gene 1
		int shapeIndex = Mathf.FloorToInt(genes[1] * numShapes);
		Shape = (Shape)shapeIndex;
		if (Shape == Shape.Triangle) {
			bodyTransform.localPosition = trianglePos;
			bodyTransform.localScale = triangleScale;
		} else {
			bodyTransform.localPosition = Vector3.zero;
			bodyTransform.localScale = Vector3.one;
		}
		bodyRenderer.sprite = shapes[shapeIndex];

		// TODO: better logic for rotation speed depending on shape
		float rotSpeed = (float)(shapeIndex + 1) / numShapes;
		RotationSpeed = rotSpeed * 900 * rotationSpeed;
		ActionSpeed = rotSpeed * 0.4f / actionSpeed;

		SpotIndex = 1;
		transform.position = SeedBasketPos;

		ResourceGathered = 0;
		ResourceCarried = 0;

		CurrentActionTime = 0;
		TimeToCompletion = 1000;
		startTime = -1;

		State = State.Delivering;

		DOTween.Sequence() // Evolution animation
			.Append(transform.DOScale(0.1f, timeForAnimation / 2))
			.Append(transform.DOScale(scale, timeForAnimation / 2))
			.OnComplete(() => transform.localScale = new Vector3(scale, scale, 1));
	}

	public void MyUpdate()
	{
		if (startTime < 0) startTime = Time.time;

		State startState;

		if (CarryCapacity == 0 || ActionSpeed == 0 || MoveSpeed == 0 || RotationSpeed == 0)
		{
			Debug.LogError("Minion at " + FarmArea.name + " has attributes with value 0. Did Reset() run??");
			return;
		}

		int maxLoops = 100;

		do {
			maxLoops--;
			if (maxLoops <= 0) {
				throw new Exception();
			}

			startState = State;

			switch (State)
			{
				case State.Delivering:
					
					if (CurrentActionTime <= 0)
					{
						if (ResourceDeliveredEvent != null) ResourceDeliveredEvent(ResourceCarried);
						ResourceCarried = 0;

						if (SpotIndex == FarmArea.FarmSpots.Length)
						{
							State = State.Finished;
							transform.localEulerAngles = Vector3.zero;
							TimeToCompletion = Time.time - startTime;
						}
						else
						{
							State = State.MovingToPlant;
							targetAngle = CalculateTargetAngle(NextTargetPos);
						}
					}
					else
					{
						CurrentActionTime -= Time.deltaTime;
					}
					break;

				case State.Gathering:

					if (CurrentActionTime <= 0)
					{
						if (SpotIndex == FarmArea.FarmSpots.Length) {
							State = State.MovingToStart;
							targetAngle = CalculateTargetAngle(SeedBasketPos);
						} else {
							State = State.MovingToPlant;
							targetAngle = CalculateTargetAngle(NextTargetPos);
						}
						ResourceCarried++;
						ResourceGathered++;
						FarmArea.FarmSpots[SpotIndex-1].HasSeeds = false;
					}
					else
					{
						CurrentActionTime -= Time.deltaTime;
					}
					break;

				case State.MovingToPlant:

					if (ResourceCarried == CarryCapacity)
					{
						State = State.MovingToStart;
						targetAngle = CalculateTargetAngle(SeedBasketPos);
					}
					else if (transform.position.IsApproximatelyEqualTo(NextTargetPos))
					{
						State = State.Gathering;
						CurrentActionTime = ActionSpeed;
						SpotIndex++;
						PlayActionAnimation();
					}
					else
					{
						MoveTo(NextTargetPos);
					}
					break;

				case State.MovingToStart:

					if (transform.position.IsApproximatelyEqualTo(SeedBasketPos))
					{
						State = State.Delivering;
						CurrentActionTime = ActionSpeed;
						PlayActionAnimation();
					}
					else
					{
						MoveTo(SeedBasketPos);
					}
					break;

				case State.Finished:
					break;
			}
		} while (startState != State);
	}

	private void MoveTo(Vector3 point)
	{
		float angleZ = transform.localEulerAngles.z;

		if (!angleZ.IsApproximately(targetAngle))
		{
			transform.localEulerAngles = RotateTo_Z(angleZ, targetAngle, RotationSpeed * Time.deltaTime);
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, point, MoveSpeed * Time.deltaTime);
		}
	}

	private Vector3 RotateTo_Z(float current, float target, float step)
	{
		float finalAngleZ;
		float delta = Mathf.DeltaAngle(current, target);

		if (delta < 0)
		{
			finalAngleZ = current - step;

			if (finalAngleZ <= current + delta)
			{
				finalAngleZ = target;
			}
		}
		else
		{
			finalAngleZ = current + step;

			if (finalAngleZ >= current + delta)
			{
				finalAngleZ = target;
			}
		}

		return new Vector3(0, 0, finalAngleZ);
	}

	private void PlayActionAnimation()
	{
		transform.localScale = new Vector3(scale, scale, 1);
		transform.DOScaleY(scale / 2, ActionSpeed / 2).SetLoops(2, LoopType.Yoyo);
	}

	private float CalculateTargetAngle(Vector3 point)
	{
		if (Shape == Shape.Circle) {
			return 0;
		}

		float angleFromPoint = Mathf.Round(transform.position.CalcAngle2D(point)).NormalizeDegrees();

		int numSides = 0;
		float startingAngle = 0;
		float targetAngle = 0;

		if (Shape == Shape.Triangle)
		{
			targetAngle = angleFromPoint - 90;
		}
		else
		{
			if (Shape == Shape.Square) {
				numSides = 4;
				startingAngle = 0;
			} else if (Shape == Shape.Hexagon) {
				numSides = 6;
				startingAngle = 0;
			}

			float angleStep = 360 / numSides;
			float closestDiff = 360;
			float bestAngle = 0;

			for (int i = 0; i < numSides; i++)
			{
				float angle = startingAngle + i * angleStep;
				float diff = Mathf.Abs(angleFromPoint - angle);

				if (diff >= closestDiff) break;
				closestDiff = diff;
				bestAngle = angle;
			}

			targetAngle = angleFromPoint - bestAngle;
		}

		targetAngle = targetAngle.NormalizeDegrees();

		return targetAngle;
	}
}
