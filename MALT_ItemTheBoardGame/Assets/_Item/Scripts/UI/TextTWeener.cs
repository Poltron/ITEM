using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TextTWeener : MonoBehaviour {

	private string startString;
	public Text text;
 	// Use this for initialization
	void Start () {
		startString = text.text;
		string value = startString;

		Sequence sequence = DOTween.Sequence ();

		for (int i = 0; i < 3; i++) {
			value += ".";
			sequence.Append (text.DOText (value, 1f));
		}
		sequence.InsertCallback (4f, () => value = startString);
		sequence.SetLoops (-1);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
