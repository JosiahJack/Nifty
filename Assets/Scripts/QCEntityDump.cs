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
	[HideInInspector] public List<string> functionReferences;
	[HideInInspector] public List<string> entityReferences;

	// Redirect entities change minimal values that a mapper could do and then
	// use a standard entity found in the fgd.  Redirects aren't included in
	// fgd by default and excluded from fgd checks.
	[HideInInspector] public List<string> entityReferencesRedirects;

	// Legacy entities are entities that do nothing or are obsoleted by newer
	// engine features but included for posterity or full support.  Examples:
	// viewthing, 
	[HideInInspector] public List<string> entityReferencesLegacy;

	private StreamReader dataReader;
	private string readLine;
	private int currentLine;
	public Toggle useTags;

	public void QCFindEntitiesForList() {
		Log.a.WriteToLog("Finding all entities in .qc files...");
		functionReferences = new List<string>();
		entityReferences = new List<string>();
		entityReferencesRedirects = new List<string>();
		entityReferencesLegacy = new List<string>();

		FileData.a.PopulateFileNames();
		for (int i=0;i<FileData.a.qcFileNames.Length;i++) {
			List<QCFunction_struct> lst = new List<QCFunction_struct>();
			List<QCFunction_struct> lstR = new List<QCFunction_struct>();
			List<QCFunction_struct> lstL = new List<QCFunction_struct>();
			lst = Nifty.a.qcFuncParserRef.ParseQCFunctions(FileData.a.qcFileNames[i],false,false);
			lstR = Nifty.a.qcFuncParserRef.ParseQCFunctions(FileData.a.qcFileNames[i],true,false);
			lstL = Nifty.a.qcFuncParserRef.ParseQCFunctions(FileData.a.qcFileNames[i],false,true);
			ParseListForEntities(lst,0);
			ParseListForEntities(lstR,1);
			ParseListForEntities(lstL,2);
		}
	}

	private void ParseListForEntities(List<QCFunction_struct> lst, int list) {
		string s, args;
		long count;
		string voidstring = "void";
		string mainstring = "main";
		string worldstring = "worldspawn";
		string brackleft = "[";
		string brackright = "]";
		string caps = "[A-Z]"; // No capitalized letters (I hope)...
		string excArmor = "item_armorInv"; // Always exceptions aren't there :(.
		char[] textChars;
		int index = 0;
		int argind = 0;
		int foundents = 0;
		bool foundparenth = false;
		foundents = 0;
		count = lst.Count;
		for (int j=0;j<count;j++) {
			s = lst[j].functionName;
			if (s == null) continue;
			if (lst[j].functionType == QCFunctionParser.FunctionType.Function) {
				continue;
			}
			if (Regex.IsMatch(s,caps) && !s.Contains(excArmor)) continue;
			if (!s.Contains(voidstring)) continue;
			if (s.Contains(mainstring)) continue;
			if (s.Contains(worldstring)) continue;
			if (s.Contains(brackleft) || s.Contains(brackright)) continue;

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
			if (AbleToIgnore(s,lst[j].functionType)) continue;

			switch(list) {
				case 0:
					if (!entityReferences.Contains(s)) {
						entityReferences.Add(s);
					}
					break;
				case 1:
					if (!entityReferencesRedirects.Contains(s)) {
						entityReferencesRedirects.Add(s);
					}
					break;
				case 2:
					if (!entityReferencesLegacy.Contains(s)) {
						entityReferencesLegacy.Add(s);
					}
					break;
			}
			foundents++;
		}
	}

	private bool AbleToIgnore(string s, QCFunctionParser.FunctionType ft) {
		if (ft == QCFunctionParser.FunctionType.Entity) {
			return false;
		}
		if (s.Length < 3) return true;
		if (s.Contains("_use")) return true;
		if (s.Contains("think")) return true;
		if (s.Contains("touch")) return true;
		if (s.Contains("ai_")) return true;
		if (s.Contains("SUB_")) return true;
		if (s.Contains("BDW_")) return true;
		if (s.Contains("checkattack")) return true;
		if (s.Contains("die")) return true;
		if (s.Contains("run1")) return true;
		if (s.Contains("walk1")) return true;
		if (s.Contains("idle1")) return true;
		if (s.Contains("melee1")) return true;
		if (s.Contains("attack1")) return true;
		if (s.Contains("missile1")) return true;
		if (s.Contains("_setup")) return true;
		if (s.Contains("_awake")) return true;
		if (s.Contains("_blocked")) return true;
		if (s.Contains("create_attachment")) return true;
		if (s.Contains("update_attachment")) return true;
		if (s.Contains("walkframe")) return true;
		if (s.Contains("runframe")) return true;
		if (s.Contains("idleframe")) return true;
		if (s.Contains("standframe")) return true;
		if (s.Contains("spawntether")) return true;
		if (s.Contains("minionsetup")) return true;
		if (s.Contains("precache")) return true;
		return false;
	}

    public void QCEntityDumpAction()  {
		Log.a.WriteToLog("Dumping all entities in .qc files...");
		for (int k=0;k<entityReferences.Count;k++) {
			Log.a.WriteToLog(entityReferences[k]);
		}
		Log.a.WriteToLog("Done! Found " + entityReferences.Count.ToString()
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