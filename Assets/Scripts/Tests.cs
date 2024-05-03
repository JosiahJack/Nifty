using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Tests : MonoBehaviour {
	[HideInInspector] public string buttonLabel = "Run Tests";

	public void RunUnits() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();

		testTimer.Stop();
		UnityEngine.Debug.Log("All unit tests completed in "
							  + testTimer.Elapsed.ToString());
	}

	public void Run() {
		#if UNITY_EDITOR
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();
		//for (int k=0;k<Nifty.a.fgdentRef.entityReferences.Count;k++) {
			Process noteWriter = new Process();
			string arg = Nifty.a.qcentRef.entityReferences[0];//k];
			noteWriter.StartInfo.FileName = "/bin/bash";
			noteWriter.StartInfo.Arguments = "cd /home/qmaster/Github/KeepModReadme; "
											 + "npx dendron note write --fname \""
											 + arg + "\"";
			noteWriter.StartInfo.UseShellExecute = false;
			noteWriter.StartInfo.RedirectStandardOutput = true;
			noteWriter.StartInfo.RedirectStandardError = true;
			noteWriter.Start();

			//if (noteWriter.ExitCode != 0) {
			//	UnityEngine.Debug.Log("Failed to execute the command with the "
			//						  + "following error: "
			//						  + noteWriter.StandardError.ReadToEnd());
			//}
		//}

		testTimer.Stop();
		UnityEngine.Debug.Log("All tests completed in " + testTimer.Elapsed.ToString());
		buttonLabel = "Run Tests (Last was: " + testTimer.Elapsed.ToString() + ")";
		#endif
	}
}

