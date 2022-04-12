using System.Collections.Generic;
using UnityEngine;
using System.Data;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class Quest : ScriptableObject
{
	public string state;
	public int difficulty = 0;
	public List<Question> questions = new List<Question>();
	public List<string> instructions = new List<string>();

	public int maxQuestions = 10;
	public int currentQuestions = 0;

	public Question generateQuestion() {
		Debug.Log("Generating Question");

		Question template = questions[Random.Range(0, questions.Count)];
		Question output = ScriptableObject.CreateInstance<Question>();
		output.maxTime = template.maxTime;

		parseQuestion(template, output);
		Util.Shuffle(output.answers);

		currentQuestions++;

		return output;
	}

	public void parseQuestion(Question template, Question q) {
		string[] splitText = template.text.Split(' ');
		List<string> tokens = new List<string>();

		try {
			for (int i = 0; i < splitText.Length; i++)	{
				if (splitText[i].StartsWith("$")) {
					splitText[i] = splitText[i].Replace("$", "");
					splitText[i] = splitText[i].Replace("{", "");
					splitText[i] = splitText[i].Replace("}", "");

					splitText[i] = generateNumber(splitText[i]);
					tokens.Add(splitText[i]);
				}			
			}

			q.text = string.Join(" ", splitText);
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Error parsing Question Template [{template.text}]");
			Debug.LogError(e);
			throw;
		}

		Debug.Log("Finished Parsing Question");


		try	{
			splitText = template.answerFormula.Split(' ');
			for (int i = 0; i < splitText.Length; i++)
			{
				if (splitText[i].StartsWith("$"))
				{
					splitText[i] = splitText[i].Replace("$", "");
					splitText[i] = splitText[i].Replace("{", "");
					splitText[i] = splitText[i].Replace("}", "");

					splitText[i] = tokens[int.Parse(splitText[i])];
				}
			}

			q.answerFormula = string.Join(" ", splitText);

			//Generate the Answer
			//No eval so we gotta use something else
			//https://stackoverflow.com/questions/333737/evaluating-string-342-yield-int-18
			DataTable dt = new DataTable();
			q.answer = dt.Compute(q.answerFormula, "").ToString();

			q.answers = new List<string>();
			q.answers.Add(q.answer);

			//Generates 2 more answers
			for (int i = 0; i < 2; i++) {
				float tempAnswer = Mathf.Abs(float.Parse(q.answer) * Random.Range(-2.0f, 2.0f));
				if(q.answer.Contains("-")) {
					tempAnswer *= -1.0f;
				}

				if(q.answer.Contains(".")) {
					//TODO: Normalize floats
					q.answers.Add(tempAnswer.ToString());
				} else {
					q.answers.Add(((int)tempAnswer).ToString());
				}
			}	
		}
		catch (System.Exception e) {
			Debug.LogError($"Error parsing Answer Formula [{template.answerFormula}]");
			Debug.LogError(e);
			throw;
		}

		Debug.Log("Finished Parsing answerFormula");
	}

	//Generates a random number given a token
	//Tokens are string with the following format:
	//${[-]nd}: A random int between 0 and 10^n. If there's a - in front, it can be negative.
	//${[-]n[.p]f}: A random number between 0 and 10^n. With a presision of p digits
	//If there's a - in front, it can be negative.
	public string generateNumber(string token) {
		try {
			if (token.Contains("d")) {
				int digits = int.Parse(token.Replace("d", "").Replace("-", ""));
				if (token.Contains("-")) {
					return Random.Range(-(int)Mathf.Pow(10, digits), (int)Mathf.Pow(10, digits)).ToString();
				}
				return Random.Range(0, (int)Mathf.Pow(10, digits)).ToString();

			}
			if (token.Contains("f")) {
				string newToken = token.Replace("f", "").Replace("-", "");
				int real = (int)float.Parse(newToken);
				int dec = 0;
				if (token.Contains(".")) {
					dec = (int)int.Parse(newToken.Substring(newToken.IndexOf(".") + 1));
				}

				float temp;
				
				if (token.Contains("-")) {
					temp = Random.Range(-(int)Mathf.Pow(10, real + dec), (int)Mathf.Pow(10, real + dec));
					return (temp / (float)Mathf.Pow(10, dec)).ToString();
				}
				temp = Random.Range(0, (int)Mathf.Pow(10, real + dec));
				return (temp / (float)Mathf.Pow(10, dec)).ToString();
			}


		}
		catch (System.Exception e) {
			Debug.LogError($"Error parsing token [{token}]");
			Debug.LogError(e);
			throw;
		}
		Debug.LogError($"generateNumber Case Not Specified for token [{token}]");
		return "";
	}
}
