using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ui_Manager : MonoBehaviour {
	public Queue<string> dialogText;
	public Image dialogPortrait;
	public Text dialogTextBox;
	public Animator dialogAnimator;
	public Quest quest = null;
	bool isStart = false;

	void Start() {
		dialogText = new Queue<string>();
	}

	public void StartDialogue(List<string> dialogue, Sprite portrait, Quest quest, bool isStart) {
		Manager.manager.isPaused = true;
		dialogPortrait.sprite = portrait;
		this.quest = quest;
		dialogAnimator.SetBool("isOpen", true);

		dialogText.Clear();
		foreach (string line in dialogue) {
			dialogText.Enqueue(line);
		}
		this.isStart = isStart;
		DisplayNextLine();
	}

	public void DisplayNextLine() {
		if (dialogText.Count == 0) {
			EndDialogue();
			return;
		}

		string line = dialogText.Dequeue();
		dialogTextBox.text = line;
	}

	void EndDialogue() {
		Debug.Log("End of conversation");
		dialogAnimator.SetBool("isOpen", false);

		if(this.isStart) {
			StartCoroutine(StartQuestCouritine());
			return;
		}

		this.quest = null;
		Manager.manager.isPaused = false;
	}

	private IEnumerator StartQuestCouritine() {
		yield return new WaitForSeconds(1);

		Manager.manager.isPaused = true;

		Manager.manager.currentQuest = quest;
		Manager.manager.StartQuest();
		this.quest = null;

	} 

}
