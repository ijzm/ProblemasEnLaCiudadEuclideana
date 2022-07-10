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
	public Animator animator;
	private InputMaster controls;
	private Vector2 targetPosition;
	private Direction direction;
	private Vector2 AxisInput;

	public Sprite portrait;

	float interactionTime = 0;

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

		//Animations:
		if(animator.GetInteger("Direction") != (int)direction) {
			animator.SetInteger("Direction", (int)direction);
		}

		if (targetPosition == (Vector2)transform.position && AxisInput == Vector2.zero) {
			animator.speed = 0;
			animator.Play(animator.GetAnimatorTransitionInfo(0).nameHash, 0, 0f);
		} else {
			animator.speed = 1;
		}
	}

	void Update() {
		if(Manager.manager.isPaused == false) {
			GetMovement();
			if(controls.Player.Interact.triggered && Time.time > interactionTime) {
				interactionTime = Time.time + 1f;
				Interact();
			}
		}
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
	}

	void Interact() {
		RaycastHit2D rh;
		NetworkManager networkManager = FindObjectOfType<NetworkManager>();

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

		QuestNpc questNpc = null;
		
		if(rh.collider != null) {
			questNpc = rh.collider.gameObject.GetComponent<QuestNpc>();
		} 

		if(questNpc != null) {
			if(questNpc.isBoss) {
				Ui_Manager uiManager = FindObjectOfType<Ui_Manager>();
				List<string> dialog = new List<string>();
				if (GameObject.FindGameObjectsWithTag("Exclamation").Length > 1)
				{
					foreach (var item in networkManager.dialogues) {
						if ((string)item["quest_id"] == "quest_boss" && (string)item["dialog_type"] == "not_enough"){
							dialog.AddRange(item["es"].ToString().Split(';'));
							Debug.Log(item["es"].ToString());
							break;
						}
					}

					animator.speed = 0;
					animator.Play(animator.GetAnimatorTransitionInfo(0).nameHash, 0, 0f);

					uiManager.StartDialogue(dialog, questNpc.quest.questPortrait);
				} else {
					questNpc.StartQuest();
				}
			} else {
				questNpc.StartQuest();
			}

			return;
		}



		if (rh.collider == null) {
			Ui_Manager uiManager = FindObjectOfType<Ui_Manager>();
			List<string> dialog = new List<string>();

			foreach (var item in networkManager.dialogues){
				Debug.Log((string)item["quest_id"]);
				if ((string)item["quest_id"] == "dialogue_interact") {
					dialog.AddRange(item["es"].ToString().Split(';'));
				}
			}
			animator.speed = 0;
			animator.Play(animator.GetAnimatorTransitionInfo(0).nameHash, 0, 0f);

			uiManager.StartDialogue(dialog, portrait);
		}
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
