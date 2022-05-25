using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Globalization;
using System;

public class Log : MonoBehaviour {
	public Transform output1ContentContainer;
    public GameObject logUILine;
	private bool startedLogging;

	public static Log a;

	void Start() {
		a = this;
		a.startedLogging = false;
	}

	public void WriteToLog(string line) {
        bool append = false;
        if (startedLogging) append = true;

        Debug.Log(line);
        if (logUILine == null) {
            GameObject lineUI = (GameObject) Instantiate(logUILine, Vector3.zero, 
                                 Quaternion.identity, output1ContentContainer);
            Text t = lineUI.GetComponent<Text>();
            t.text = line;
        }

		StreamWriter sw = new StreamWriter(Nifty.a.logFilePath, append,
                                           Encoding.ASCII);
		if (sw == null) return;

		startedLogging = true;
		using (sw) {
			sw.Write(line);
			sw.Close();
		}
	}
}