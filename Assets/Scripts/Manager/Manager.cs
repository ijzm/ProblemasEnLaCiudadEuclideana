using UnityEngine;
using UnityEngine.UI;

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

	void Awake() {
		if (manager != null) {
			GameObject.Destroy(manager);
		} else {
			manager = this;
		}

		DontDestroyOnLoad(this);
	}

	//TODO:
	void Start() {
		Question q = currentQuest.generateQuestion();
		//Debug.Log($"Generated Question {q.text}");

		timerBar = GameObject.Find("TimerForeground").GetComponent<Image>();
		maxQuestionIndex = currentQuest.maxQuestions;
		DisplayQuestion(q);
	}

	void Update() {
		if (currentQuestion != null) {
			if (timeLeft <= 0) {
				//Debug.Log("Time's up!");
				//TODO: Sound Effect
				DisplayQuestion(currentQuest.generateQuestion());
			} else {
				timeLeft -= Time.deltaTime;
				if(timerBar != null) {
					timerBar.fillAmount = timeLeft / currentQuestion.maxTime;
				}
			}
		}
	}

	void DisplayQuestion(Question question) {
		maxQuestionIndex++;

		if(currentQuestionIndex >= maxQuestionIndex) {
			Debug.Log("Quest is complete!");
			//TODO: Finish Quest
		}

		currentQuestion = question;
		timeLeft = currentQuestion.maxTime;

		GameObject questionCanvas = GameObject.Find("QuestionCanvas");

		if(questionCanvas == null) {
			Debug.LogError("QuestionCanvas not found");
			return;
		}

		questionCanvas.GetComponent<Canvas>().enabled = true;

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
			Score += 50;
			UpdateScore();
		} else {
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