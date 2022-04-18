using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Direction {
	UP, DOWN, LEFT, RIGHT
}
public class PlayerMovement : MonoBehaviour
{
	public float speed = 5f;
	public LayerMask colisionLayer;
	public LayerMask interactionLayer;
	private InputMaster controls;
	private Vector2 targetPosition;
	private Direction direction;
	private Vector2 AxisInput;

	void Awake() {
		controls = new InputMaster();
		controls.Player.Move.Enable();
		controls.Player.Interact.Enable();
	}

	void Start() {
		targetPosition = (Vector2)transform.position;
		direction = Direction.DOWN;
	}

	private void GetMovement() {
		AxisInput = controls.Player.Move.ReadValue<Vector2>();
		if (AxisInput != Vector2.zero && targetPosition == (Vector2)transform.position) {
			if (Mathf.Abs(AxisInput.x) > Mathf.Abs(AxisInput.y)) {
				if (AxisInput.x > 0) {
					direction = Direction.RIGHT;
					if (!GetCollision) {
						targetPosition += Vector2.right;
					}
				}
				else {
					direction = Direction.LEFT;
					if (!GetCollision) {
						targetPosition += Vector2.left;
					}
				}
			}
			else
			{
				if (AxisInput.y > 0)
				{
					direction = Direction.UP;
					if (!GetCollision)
					{
						targetPosition += Vector2.up;
					}
				}
				else
				{
					direction = Direction.DOWN;
					if (!GetCollision)
					{
						targetPosition += Vector2.down;
					}
				}
			}
		}
	}

	void Update() {
		//TODO: Update Animation

		if(Manager.manager.isPaused == false) {
			GetMovement();
			if(controls.Player.Interact.triggered) {
				Interact();
			}
		}
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
	}

	void Interact() {
		RaycastHit2D rh;

		Vector2 dir = Vector2.zero;

		if (direction == Direction.DOWN) {
			dir = Vector2.down;
		}
		if (direction == Direction.LEFT) {
			dir = Vector2.left;
		}
		if (direction == Direction.RIGHT) {
			dir = Vector2.right;
		}
		if (direction == Direction.UP) {
			dir = Vector2.up;
		}

		rh = Physics2D.Raycast(transform.position, dir, 1, interactionLayer);

		rh.collider?.gameObject.GetComponent<QuestNpc>()?.StartQuest();

		
	}


	bool GetCollision {
		get
		{
			RaycastHit2D rh;

			Vector2 dir = Vector2.zero;

			if (direction == Direction.DOWN) {
				dir = Vector2.down;
			}
			if (direction == Direction.LEFT) {
				dir = Vector2.left;
			}
			if (direction == Direction.RIGHT) {
				dir = Vector2.right;
			}
			if (direction == Direction.UP) {
				dir = Vector2.up;
			}

			rh = Physics2D.Raycast(transform.position, dir, 1, colisionLayer);

			return rh.collider != null;
		}
	}
}
