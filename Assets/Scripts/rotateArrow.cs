using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateArrow : MonoBehaviour
{
    public List<GameObject> QuestNPCs = new List<GameObject>();
	public GameObject BossNPC;
	GameObject Player;
	public float offset = -90f;

	void FindNPCs() {
		QuestNPCs.Clear();

		foreach(GameObject exclamation in GameObject.FindGameObjectsWithTag("Exclamation")) {
			if(exclamation.transform.parent.GetComponent<QuestNpc>().isBoss) {
				BossNPC = exclamation.transform.parent.gameObject;
			} else {
				QuestNPCs.Add(exclamation.transform.parent.gameObject);
			}
		}
	}
    void Start() {
		Player = GameObject.Find("Player");
        FindNPCs();
    }

    // Update is called once per frame
    void Update() {

		Vector3 Target = Vector3.zero;

        if(QuestNPCs.Count > 0) {
			Target = QuestNPCs[0].transform.position;
		}
		if(QuestNPCs.Count == 0) {
			if(BossNPC != null) {
				Target = BossNPC.transform.position;
			}
		}

		Vector3 dir = Target - Player.transform.position;
		Debug.DrawRay(Player.transform.position, dir, Color.green, 1f);
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - offset, Vector3.forward);
    }
}
