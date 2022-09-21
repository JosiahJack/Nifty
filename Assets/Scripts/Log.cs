using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Globalization;
using System;

public class Log : MonoBehaviour {
	public Transform output1ContentContainer;
	public Toggle overwriteToggle;
	public Text[] logText;

	public static Log a;

	void Start() {
		a = this;
	}

	public void WriteToLog(string line) {
		if (line == null) return;

        bool append = !overwriteToggle.isOn;

		//#if DEBUG
	        Debug.Log(line);
		//#endif
		Console.WriteLine(line);
		logText[0].text = logText[1].text;
		logText[1].text = logText[2].text;
		logText[2].text = logText[3].text;
		logText[3].text = logText[4].text;
		logText[4].text = logText[5].text;
		logText[5].text = logText[6].text;
		logText[6].text = logText[7].text;
		logText[7].text = logText[8].text;
		logText[8].text = logText[9].text;
		logText[9].text = logText[10].text;
		logText[10].text = logText[11].text;
		logText[11].text = logText[12].text;
		logText[12].text = line;
		StreamWriter sw = new StreamWriter(Nifty.a.logFilePath, append,
                                           Encoding.ASCII);
		if (sw == null) return;

		using (sw) {
			sw.Write(line);
			sw.Write(Environment.NewLine);
			sw.Close();
		}
	}
}