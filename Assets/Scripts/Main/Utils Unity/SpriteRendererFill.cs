using UnityEngine;

public enum Direction
{
	TopToBottom,
	BottomToTop,
	LeftToRight,
	RightToLeft,
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererFill : MonoBehaviour
{
	[SerializeField] Direction Direction;
	[SerializeField] Vector2 Offset = Vector2.zero;
	[SerializeField] Vector2 Scale = Vector2.one;

	[Range(0, 1)]
	[SerializeField] float _value;
	public float FillValue {
		get { return _value; }
		set {
			_value = value;
			SetFill();
		}
	}

	private SpriteRenderer spriteRenderer;
	private Vector3 Size {
		get {
			spriteRenderer = GetComponent<SpriteRenderer>();
			return spriteRenderer.sprite.bounds.size;
		}
	}

	void Awake()
	{
		SetFill();
	}

	void OnValidate()
	{
		FillValue = _value;
	}

	private void SetFill()
	{
		float offset, value;
		transform.localPosition = Offset;
		transform.SetLocalScale(Scale.x, Scale.y, 1);

		switch (Direction)
		{
			case Direction.TopToBottom:
				value = _value * Scale.y;
				transform.SetLocalScale(y: value);

				offset = (value / 2) * Size.y - (Size .y/ 2);
				transform.SetLocalPos(y: -offset + Offset.y);
				break;
			case Direction.BottomToTop:
				value = _value * Scale.y;
				transform.SetLocalScale(y: value);

				offset = (value / 2) * Size.y - (Size .y/ 2);
				transform.SetLocalPos(y: offset + Offset.y);
				break;
			case Direction.LeftToRight:
				value = _value * Scale.x;
				transform.SetLocalScale(x: value);

				offset = (value / 2) * Size.x - (Size .x/ 2);
				transform.SetLocalPos(x: offset + Offset.x);
				break;
			case Direction.RightToLeft:
				value = _value * Scale.x;
				transform.SetLocalScale(x: value);

				offset = (value / 2) * Size.x - (Size .x/ 2);
				transform.SetLocalPos(x: -offset + Offset.x);
				break;
		}
	}
}
