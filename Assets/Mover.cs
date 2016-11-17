using UnityEngine;
using System.Collections;
using System.IO;

public class Mover : MonoBehaviour {

	float elapsedTime = 0;

	// Use this for initialization
	void Start () {
		// Register for the Everyplay ReadyForRecording event
		Log ("Start() begins");
		Log ("is supported: " + Everyplay.IsSupported() );
		Log ("is recording supported: " + Everyplay.IsRecordingSupported ());

		Everyplay.ReadyForRecording += OnReadyForRecording;
		Everyplay.RecordingStarted += OnRecordingStarted;
		Everyplay.RecordingStopped += OnRecordingStopped;
		Log ("Start() ends");

		StartCoroutine (CheckIsSupported());
	}

	IEnumerator CheckIsSupported() {
		yield return new WaitForSeconds(2f);
		Log ("is supported: " + Everyplay.IsSupported() );
		Log ("is recording supported: " + Everyplay.IsRecordingSupported ());
	}

	void Destroy() {
		Everyplay.ReadyForRecording -= OnReadyForRecording;
		Everyplay.RecordingStarted -= OnRecordingStarted;
		Everyplay.RecordingStopped -= OnRecordingStopped;
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		transform.position = Vector3.Lerp (
			Vector3.zero, new Vector3 (0, 1, 0), elapsedTime % 1);
	}

	public void OnReadyForRecording(bool enabled) {
		if (enabled) {
			Log ("is supported: " + Everyplay.IsSupported() );
			Log ("is recording supported: " + Everyplay.IsRecordingSupported ());
			StartCoroutine (Record ());
		} else {
			Log ("OnReadyForRecording with false!!!");
		}
	}

	public void OnRecordingStarted() {
		Log ("recording started!!!");
	}

	public void OnRecordingStopped() {
		Log ("recording stopped!!!");
	}

	IEnumerator Record() {
		Everyplay.StartRecording();
		yield return new WaitForSeconds(3f);
		Everyplay.StopRecording ();
		Log ("find video at " + GetVideoPath ());

	}


	// ref: http://answers.unity3d.com/questions/1089052/can-i-use-everyplay-to-record-without-upload.html
	static string GetVideoPath ()
	{
		#if UNITY_IOS

		var root = new DirectoryInfo(Application.persistentDataPath).Parent.FullName;
		var everyplayDir = root + "/tmp/Everyplay/session";

		#elif UNITY_ANDROID

		var root = new DirectoryInfo(Application.temporaryCachePath).FullName;
		var everyplayDir = root + "/sessions";

		#else

		var root = new DirectoryInfo(Application.temporaryCachePath).FullName;
		var everyplayDir = root + "/sessions";

		#endif

		var files = new DirectoryInfo(everyplayDir).GetFiles("*.mp4", SearchOption.AllDirectories);
		var videoLocation = "";

		// Should only be one video, if there is one at all
		foreach (var file in files) {
			#if UNITY_ANDROID
			videoLocation = "file://" + file.FullName;
			#else
			videoLocation = file.FullName;
			#endif
			break;
		}

		return videoLocation;
	}

	private static void Log(string msg) {
		Debug.Log(msg);

	}


}
