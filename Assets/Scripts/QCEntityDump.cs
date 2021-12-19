using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

public class QCEntityDump : MonoBehaviour {
	private List<string> functionReferences;
	private List<string> entityReferences;
	private StreamReader dataReader;
	private string readLine;
	private int currentLine;
	public Toggle useTags;

	public string QCFindEntitiesForList() {
		StringBuilder outputLogs = new StringBuilder();
		outputLogs.Append("Finding all entities in .qc files...");
		Debug.Log("Finding all entities in .qc files...");

		functionReferences = new List<string>();
		entityReferences = new List<string>();
		string s, args;
		long count;
		string voidstring = "void";
		char[] textChars;
		int index = 0;
		int argind = 0;
		int foundents = 0;
		bool foundparenth = false;

		for (int i=0;i<Nifty.a.qcFileNames.Length;i++) {
			List<QCFunction> lst = new List<QCFunction>();;
			lst = QCFunctionParser.ParseQCFunctions(Nifty.a.qcFileNames[i]);
			count = lst.Count;
			foundents = 0;
			for (int j=0;j<count;j++) {
				s = lst[j].functionName;
				if (s.Contains(voidstring) && (!Regex.IsMatch(s,"[A-Z]") || s.Contains("item_armorInv")) && !s.Contains("main") && !s.Contains("worldspawn")) {
					if (!s.Contains("[") && !s.Contains("]")) {
						textChars = s.ToCharArray();
						index = argind = 0;
						for (int k=0;k<textChars.Length;k++) {
							if (textChars[k] == ')') {
								index = k;
								foundparenth = true;
								break;
							}
						}
						if (foundparenth && index > 0) {
							for (int k=0;k<textChars.Length;k++) {
								if (textChars[k] == '(') {
									argind = k;
									break;
								}
							}
							args = s;
							if (argind > 0) args = args.Remove(0,argind);
							textChars = args.ToCharArray();
							for (int k=0;k<textChars.Length;k++) {
								if (textChars[k] == ')') {
									argind = k;
									break;
								}
							}
							s = s.Remove(0,index+1);
						}
						if (argind > 1) continue;

						index = 0;
						textChars = s.ToCharArray();
						for (int k=0;k<textChars.Length;k++) {
							if (textChars[k] == '=') {
								index = k;
								foundparenth = true;
								break;
							}
						}
						if (foundparenth && index > 0 && index < s.Length) s = s.Remove(index,(s.Length - index));

						s = s.Trim();

						if (s.Contains("_use") || s.Contains("think") || s.Contains("touch") || s.Contains("ai_") || s.Contains("checkattack")) continue;
						functionReferences.Add(s);
						foundents++;
					}
				}
			}
			outputLogs.AppendLine();
			outputLogs.Append(count.ToString() + " functions total and " + foundents.ToString() + " entities within " + Nifty.a.qcFileNames[i]);
			Debug.Log(count.ToString() + " functions total and " + foundents.ToString() + " entities within " + Nifty.a.qcFileNames[i]);
		}
		outputLogs.AppendLine();
		outputLogs.Append("Found " + functionReferences.Count.ToString() + " entities.");
		Debug.Log("Found " + functionReferences.Count.ToString() + " entities.");
		return outputLogs.ToString();
	}

    public string QCEntityDumpAction()  {
		StringBuilder outputLogs = new StringBuilder();
		outputLogs.Append("Dumping all entities in .qc files...");
		Debug.Log("Dumping all entities in .qc files...");

		StreamWriter sw = new StreamWriter(Nifty.a.outputFolderPath + Nifty.a.outputFileName + "_all_functions.txt",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (int k=0;k<functionReferences.Count;k++) {
					sw.WriteLine(functionReferences[k]);
				}
			}
		}

		outputLogs.AppendLine();
		outputLogs.Append("Done! Found " + functionReferences.Count.ToString() + " entities.");
		Debug.Log("Done! Found " + functionReferences.Count.ToString() + " entities.");

		// #if UNITY_EDITOR_WIN
			// UnityEditor.EditorApplication.isPlaying = false;
		// #endif
		return outputLogs.ToString();
    }

	long CountLinesReader(string fileNameAndPath) {
		long lineCounter = 0L;
		using (var reader = new StreamReader(fileNameAndPath)) {
			while(reader.ReadLine() !=null) lineCounter++;
			return lineCounter;
		}
	}


}