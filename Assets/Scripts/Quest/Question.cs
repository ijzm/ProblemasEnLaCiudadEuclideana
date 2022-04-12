using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "ScriptableObjects/Question")]
public class Question : ScriptableObject
{
	public string text;
	public string answerFormula;
	public List<string> answers;
	public string answer;
	public float maxTime = 10f;

	public Question(string text, float maxTime) {
		this.text = text;
		this.maxTime = maxTime;
	}
}
