using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//https://answers.unity.com/questions/323195/how-can-i-have-a-static-class-i-can-access-from-an.html
public class Manager : MonoBehaviour 
{
	public static Manager manager;
	public Question currentQuestion = null;
	public int maxQuestionIndex = 0;
	public int currentQuestionIndex = 0;
	public Quest currentQuest = null;
	public int Score = 0;
	public float timeLeft = 0;

	private Image timerBar;

	public bool isPaused = false;
	public GameObject questionCanvas;

	public AudioManager CityMusic;
	public AudioManager BattleMusic;


	//For Quest 3:
	public Dictionary<string, int> quest3Values = new Dictionary<string, int>();

	void Awake() {
		if (manager != null) {
			GameObject.Destroy(manager);
		} else {
			manager = this;
		}

		DontDestroyOnLoad(this);
	}

	void Start() {
		quest3Values.Add("Male", 0);
		quest3Values.Add("Female", 0);

		quest3Values.Add("Red", 0);
		quest3Values.Add("Green", 0);
		quest3Values.Add("Blue", 0);

		quest3Values.Add("Square", 0);
		quest3Values.Add("Circle", 0);
		quest3Values.Add("Triangle", 0);

		Debug.Log(SceneManager.GetActiveScene().name);

		if(SceneManager.GetActiveScene().name == "Game") {
			StartCoroutine(CityMusic.FadeIn(2f));
		}
	}

	void Update() {
		if (currentQuestion != null) {
			if (timeLeft <= 0 && currentQuest != null) {
				//Debug.Log("Time's up!");
				Instantiate(Resources.Load("WrongSound"));
				DisplayQuestion(currentQuest.generateQuestion());
			} else {
				timeLeft -= Time.deltaTime;
				if(timerBar != null) {
					timerBar.fillAmount = timeLeft / currentQuestion.maxTime;
				}
			}
		}
	}

	public void StartQuest() {
		questionCanvas.SetActive(true);
		if(currentQuest == null) {
			return;
		}
		Question q = currentQuest.generateQuestion();
		timerBar = GameObject.Find("TimerForeground").GetComponent<Image>();
		maxQuestionIndex = currentQuest.maxQuestions;
		DisplayQuestion(q);
		StartCoroutine(Manager.manager.CityMusic.FadeOut(2f));
		StartCoroutine(Manager.manager.BattleMusic.FadeIn(2f));
		this.isPaused = true;
	}

	void DisplayQuestion(Question question) {
		currentQuestionIndex++;

		if(currentQuestionIndex >= maxQuestionIndex) {
			Debug.Log("Quest is complete!");
			//TODO: Finish Quest)

			StartCoroutine(Manager.manager.CityMusic.FadeIn(2f));
			StartCoroutine(Manager.manager.BattleMusic.FadeOut(2f));

			Ui_Manager uiManager = FindObjectOfType<Ui_Manager>();
			if (uiManager != null) {
				uiManager.StartDialogue(currentQuest?.endDialog, currentQuest?.questPortrait, currentQuest, false);
			}
			else {
				Debug.LogError("No Ui_Manager found");
			}

			currentQuest = null;
			currentQuestion = null;
			questionCanvas.SetActive(false);
			currentQuestionIndex = 0;

			GameObject.Find("UICamera/CommonCanvas/LocatorArrow").SendMessage("FindNPCs");

			if (GameObject.FindGameObjectsWithTag("Exclamation").Length == 0) {
				//Win Game
				SceneManager.LoadScene("Menu");
			}

			return;
		}

		currentQuestion = question;
		timeLeft = currentQuestion.maxTime;

		questionCanvas.transform.Find("QuestionText").GetComponent<Text>().text = question.text;

		for(int i = 0; i < question.answers.Count; i++) {
			GameObject answerButton = questionCanvas.transform.Find("Answers/Answer" + i).gameObject;
			if(answerButton == null) {
				Debug.LogError("AnswerButton" + i + " not found");
				continue;
			}

			answerButton.GetComponentInChildren<Text>().text = question.answers[i];
			answerButton.GetComponent<Button>().onClick.RemoveAllListeners();
			int x = i;
			answerButton.GetComponent<Button>().onClick.AddListener(delegate { AnswerQuestion(question.answers[x]); });
		}
	}

	void AnswerQuestion(string answer) {
		if(answer == currentQuestion.answer) {
			Debug.Log("Correct answer");
			//TODO: Score
			Instantiate(Resources.Load("CorrectSound"));
			Score += 50 + (int)(timeLeft * 5);
			UpdateScore();
		} else {
			Instantiate(Resources.Load("WrongSound"));
			Debug.Log("Wrong answer");
		}

		if(currentQuest != null) {
			DisplayQuestion(currentQuest.generateQuestion());
		}
	}

	void UpdateScore() {
		GameObject commonCanvas = GameObject.Find("CommonCanvas");

		if(commonCanvas == null) {
			Debug.LogError("CommonCanvas not found");
			return;
		}

		commonCanvas.transform.Find("ScoreText").GetComponent<Text>().text = Score.ToString().PadLeft(4, '0');
	}
}
