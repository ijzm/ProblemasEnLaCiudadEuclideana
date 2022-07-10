using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class Test : MonoBehaviour {
	void Start() {
		int TotalQuestions = 100;
		int CurrentQuestion = 0;

		Quest quest = ScriptableObject.CreateInstance<Quest>();

		Question question = ScriptableObject.CreateInstance<Question>();
		question.text = "${5d}";
		question.answerFormula = "${0}";
		quest.questions.Add(question);

		for (int i = 0; i < 100; i++) {
			Question q = quest.generateQuestion();
			Assert.IsTrue(int.Parse(q.answer) >= 0 && int.Parse(q.answer) <= 100000);
			CurrentQuestion++;
		}
		Debug.Log($"Test 1: {CurrentQuestion} / {TotalQuestions} tests passed");

		quest.questions.Clear();
		question.text = "${-5d}";
		question.answerFormula = "${0}";
		quest.questions.Add(question);
		CurrentQuestion = 0;

		for (int i = 0; i < 100; i++) {
			Question q = quest.generateQuestion();
			Assert.IsTrue(int.Parse(q.answer) >= -100000 && int.Parse(q.answer) <= 100000);
			CurrentQuestion++;
		}
		Debug.Log($"Test 2: {CurrentQuestion} / {TotalQuestions} tests passed");

		quest.questions.Clear();
		question.text = "${5.2f}";
		question.answerFormula = "${0}";
		quest.questions.Add(question);
		CurrentQuestion = 0;

		for (int i = 0; i < 100; i++) {
			Question q = quest.generateQuestion();
			Assert.IsTrue(float.Parse(q.answer) >= 0f && float.Parse(q.answer) <= 100000f);
			CurrentQuestion++;
		}
		Debug.Log($"Test 3: {CurrentQuestion} / {TotalQuestions} tests passed");

		quest.questions.Clear();
		question.text = "${-5.2f}";
		question.answerFormula = "${0}";
		quest.questions.Add(question);
		CurrentQuestion = 0;

		for (int i = 0; i < 100; i++) {
			Question q = quest.generateQuestion();
			Assert.IsTrue(float.Parse(q.answer) >= -100000f && float.Parse(q.answer) <= 100000f);
			CurrentQuestion++;
		}
		Debug.Log($"Test 4: {CurrentQuestion} / {TotalQuestions} tests passed");
	}
}
