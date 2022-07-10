using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : MonoBehaviour
{
	public Quest quest;
	public bool questStarted = false;
	public bool isBoss = false;

	public void Start() {
		if(quest.questions.Count <= 0) {
			Destroy(this.gameObject);
		}
	}

	public void StartQuest() {
		Debug.Log("Starting Quest");
		if(isBoss) {
			if(GameObject.FindGameObjectsWithTag("Exclamation").Length > 1) {
				return;
			}
		}

		if(questStarted == false) {
			questStarted = true;

			Ui_Manager uiManager = FindObjectOfType<Ui_Manager>();

			Destroy(transform.Find("Exclamation").gameObject);

			if (uiManager != null) {
				uiManager.StartDialogue(quest?.startDialog, quest?.questPortrait, quest, true);
			}
			else {
				Debug.LogError("No Ui_Manager found");
			}
		}
	}
	
}
