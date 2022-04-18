using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : MonoBehaviour
{
	public Quest quest;
	public bool questStarted = false;

	public void StartQuest() {
		Debug.Log("Starting Quest");
		if(questStarted == false) {
			questStarted = true;

			Ui_Manager uiManager = FindObjectOfType<Ui_Manager>();

			if (uiManager != null) {
				uiManager.StartDialogue(quest?.startDialog, quest?.questPortrait, quest, true);
			}
			else {
				Debug.LogError("No Ui_Manager found");
			}
		}
	}
	
}
