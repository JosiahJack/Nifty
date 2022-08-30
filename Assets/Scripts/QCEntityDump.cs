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

	public void QCFindEntitiesForList() {
		Log.a.WriteToLog("Finding all entities in .qc files...");
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
		FileData.a.PopulateFileNames();
		for (int i=0;i<FileData.a.qcFileNames.Length;i++) {
			List<QCFunction_struct> lst = new List<QCFunction_struct>();;
			lst = QCFunctionParser.ParseQCFunctions(FileData.a.qcFileNames[i]);
			count = lst.Count;
			foundents = 0;
			for (int j=0;j<count;j++) {
				s = lst[j].functionName;
				if (s == null) continue;

				if (s.Contains(voidstring) && (!Regex.IsMatch(s,"[A-Z]")
					|| s.Contains("item_armorInv")) && !s.Contains("main")
					&& !s.Contains("worldspawn")) {
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
						if (foundparenth && index > 0 && index < s.Length) {
							s = s.Remove(index,(s.Length - index));
						}

						s = s.Trim();
						if (s.Contains("_use") || s.Contains("think")
							|| s.Contains("touch") || s.Contains("ai_")
							|| s.Contains("th_") || s.Contains("SUB_")
							|| s.Contains("BDW_")
							|| s.Contains("checkattack")) continue;

						functionReferences.Add(s);
						foundents++;
					}
				}
			}
			Log.a.WriteToLog(count.ToString() + " functions total and "
							 + foundents.ToString() + " entities within "
							 + FileData.a.qcFileNames[i]);
		}
		Log.a.WriteToLog("Found " + functionReferences.Count.ToString()
						 + " entities.");
	}

    public void QCEntityDumpAction()  {
		Log.a.WriteToLog("Dumping all entities in .qc files...");

		StreamWriter sw = new StreamWriter(Nifty.a.outputFolderPath
										   + Nifty.a.outputFileName
										   + "_all_functions.txt",
										   false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (int k=0;k<functionReferences.Count;k++) {
					sw.WriteLine(functionReferences[k]);
				}
			}
		}

		Log.a.WriteToLog("Done! Found " + functionReferences.Count.ToString()
						 + " entities.");
    }

	long CountLinesReader(string fileNameAndPath) {
		long lineCounter = 0L;
		using (var reader = new StreamReader(fileNameAndPath)) {
			while(reader.ReadLine() !=null) lineCounter++;
			return lineCounter;
		}
	}


}