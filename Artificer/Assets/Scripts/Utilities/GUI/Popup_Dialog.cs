using UnityEngine;
using System;
using System.Collections;

public class Popup_Dialog : MonoBehaviour
{
	static public Popup_Dialog instance;

	public Texture2D emptyProgressBar; 				// Set this in inspector.
	public Texture2D fullProgressBar; 				// Set this in inspector.

	public static float loadProgress;

	// Event functions return result
	public delegate void DialogEvent (DialogResult returnResult);
	public static DialogEvent OnDialogEvent;

	private Rect popup_rect = new Rect(350, 250, 300, 150);

	//dialog result 
	public enum DialogResult{NULL = 0,
		OK, CANCEL, YES, NO}

	public enum DialogType{OK = 0,
		YESNO, YESNOTIMER, LOADDIALOG};
	
	public static DialogType thisType = DialogType.OK;
	public static string _messageBody;
	public static string _messageHead;
	public static bool _show = false;
	public static int _timer;

	// Use this for initialization
	void Awake(){ //called when an instance awakes in the game
		instance = this; //set our static reference to our newly initialized instance
	}

	public static void ShowPopup
		(string head, string body, DialogType type)
	{
		_messageHead = head;
		_messageBody = body;
		
		thisType = type;

		//if (type == DialogType.YESNOTIMER) {
			instance.StartCoroutine ("TickPopup");
		//}

		Show = true;
	}
	
	public IEnumerator TickPopup()
	{
		if (thisType == DialogType.YESNOTIMER)
			_timer = 10;

		while (true) {
			bool end = false;
			switch (thisType)
			{
			case DialogType.YESNOTIMER:
				_timer--;
				if(_timer == 0)
				{
					OnDialogEvent(DialogResult.NULL);
					StopCoroutine ("TickPopup");
					Show = false;
					end = true;
					break;
				}
				yield return new WaitForSeconds(1f);
				break;
			default: 
				yield return new WaitForSeconds(1f);
				break;
			}

			if(end)
				break;
		}
		yield return null;
	}

	public static bool Show
	{
		get { return _show; }
		set	{ _show = value; }
	}


	void OnGUI()
	{
		if(_show)
			popup_rect = GUI.Window(2, popup_rect, PopupBox, _messageHead);
	}

	public void PopupBox(int gui)
	{
		GUILayout.BeginArea(new Rect(0, 0 ,300, 150));

		GUI.Label (new Rect (50, 20, 150, 30), _messageBody);

		switch(thisType)
		{
		case DialogType.OK:
			break;
		case DialogType.YESNOTIMER:
			GUI.Label(new Rect(120, 50, 150, 30), "Reverting in: " + _timer.ToString());

			if (GUI.Button (new Rect (30, 100, 100, 30), "Yes")) 
			{
				OnDialogEvent (DialogResult.YES);
				Show = false;
				StopCoroutine ("TickPopup");
			}
			
			if (GUI.Button (new Rect (160, 100, 100, 30), "No")) 
			{
				OnDialogEvent (DialogResult.NO);
				Show = false;
				StopCoroutine ("TickPopup");
			}
			break;
		case DialogType.YESNO:
			if (GUI.Button (new Rect (30, 100, 100, 30), "Yes")) {
				OnDialogEvent (DialogResult.YES);
				Show = false;
				StopCoroutine ("TickPopup");
			}
			
			if (GUI.Button (new Rect (160, 100, 100, 30), "No")) {
				OnDialogEvent (DialogResult.NO);
				Show = false;
				StopCoroutine ("TickPopup");
			}
			break;

		case DialogType.LOADDIALOG:
				GUI.DrawTexture(new Rect(10, 65, 280, 30), emptyProgressBar);
				GUI.DrawTexture(new Rect(10, 65, 280 * loadProgress, 30), fullProgressBar);
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(10, 65, 280, 30), String.Format("{0:N0}%", loadProgress * 100f));
			break;
		}

		GUILayout.EndArea();
	}
}

