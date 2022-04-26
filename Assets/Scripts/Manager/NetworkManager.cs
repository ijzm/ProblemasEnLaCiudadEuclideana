using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class NetworkManager : MonoBehaviour
{
	public TextAsset FallbackConfig;
	public TextAsset DialoguesCSV;
	public TextAsset QuestionsCSV;
	public List<string> QuestNames = new List<string>();
	public List<Quest> Quests = new List<Quest>();
	private string ConfigFile;

    // Start is called before the first frame update
    void Start() {
		GameObject.DontDestroyOnLoad(this.gameObject);
		//Try to get json from server

		//If fails, open local json

		if (!System.IO.File.Exists(Application.persistentDataPath + "/config.json")) {
			System.IO.File.WriteAllText(Application.persistentDataPath + "/config.json", FallbackConfig.text);
		}

		ConfigFile = System.IO.File.ReadAllText(Application.persistentDataPath + "/config.json");

		//Parse json
		ParseConfig(ConfigFile);


    }

	void ParseConfig(string json) {
		JSONNode jsonData = JSON.Parse(json);

		foreach (JSONNode quest  in jsonData["dgbl_features"]["ilos"]) {
			
			int currentIndex = QuestNames.FindIndex(x => x == quest["label"].Value);

			if (currentIndex == -1) {
				Debug.LogError("Couldn't find index for: " + quest["label"].Value);
			}

			Quests[currentIndex].questions.Clear();
			Quests[currentIndex].allowedQuests.Clear();

			Quests[currentIndex].startDialog = GetDialog("start_dialog", quest["label"].Value, "es");
			Quests[currentIndex].endDialog = GetDialog("end_dialog", quest["label"].Value, "es");

			foreach(JSONNode item in quest["ilo_parameters"]) {
				if(item["label"] == "time") {
					//TODO: REVISAR
					Quests[currentIndex].maxTime = item["default_value"].AsInt;
				} else if(item["label"] == "num_questions") {
					//TODO: REVISAR
					Quests[currentIndex].maxQuestions = item["default_value"].AsInt;
				} else {
					//TODO: REVISAR
					if(item["default_value"].AsBool) {
						Quests[currentIndex].allowedQuests.Add(item["label"]);
					}
				}
			}

			GetQuestions(quest["label"].Value, "es");
		}
	}

	List<string> GetDialog(string dialogType, string questName, string language) {
		List<string> dialog = new List<string>();

		List<Dictionary<string, object>> data = CSVReader.Read(DialoguesCSV.text);
		foreach (var item in data)
		{
			if ((string)item["quest_id"] == questName) {
				if((string)item["dialog_type"] == dialogType) {
					dialog.AddRange(item[language].ToString().Split(';'));
				}
			}
		}

		return dialog;
	}
	
	void GetQuestions(string questName, string language) {
		List<Dictionary<string, object>> data = CSVReader.Read(QuestionsCSV.text);
		int currentIndex = QuestNames.FindIndex(x => x == questName);

		Quests[currentIndex].questions.Clear();

		foreach (var item in data) {
			if((string)item["quest_id"] == questName) {
				if(Quests[currentIndex].allowedQuests.Contains(((string)item["achivement_id"]))) {
					Question newQuestion = ScriptableObject.CreateInstance<Question>();
					if(language == "es") {
						newQuestion.text = (string)item["question_text_es"];
						newQuestion.answerFormula = (string)item["answer_formula_es"];
					} else if(language == "en") {
						newQuestion.text = (string)item["question_text_en"];
						newQuestion.answerFormula = (string)item["answer_formula_en"];
					}

					newQuestion.maxTime = Quests[currentIndex].maxTime;
					
					Quests[currentIndex].questions.Add(newQuestion);
				}
			}
		}
	}
}
