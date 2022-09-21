using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

public class QCReferenceCheck : MonoBehaviour {
	private List<string> modelReferencesInQc;
	private List<string> spriteReferencesInQc;
	private List<string> waveReferencesInQc;
	private StreamReader dataReader;
	private string readLine;
	private int currentLine;
	private string errMsg;
	private string exmdl = ".mdl";
	private string exspr = ".spr";
	private string exwav = ".wav";

	// Expects the " delineated string from a line of a QC file.
	// Example expects "misc/null.wav" without the quotes.
	// Can also accept "null.wav" which would be in "mod/sound/null.wav".
	private bool LineCheck(string line, string extNeeded) { 
		if (line.Contains(':')) return false; // Paths don't have colons.
		if (line.Contains("//")) return false; // Commented out.
		if (line.Contains("[")) return false; // Paths don't have bracket
		if (line.Contains("]")) return false; // Paths don't have bracket
		if (extNeeded == exwav) return line.Contains(extNeeded); 
		else return line.Contains(extNeeded) //.mdl or .spr or .wav
					&& line.Contains((char)47); // / needed for models which
												// should always be in "progs/"
												// or in "maps/".
	}

    public void QCReferenceCheckAction()  {
		Log.a.WriteToLog("Checking references to models, sprites, and "
						 + "sounds in .qc files...");
		FileData.a.PopulateFileNames();
		modelReferencesInQc = new List<string>();
		spriteReferencesInQc = new List<string>();
		waveReferencesInQc = new List<string>();
		int mdlrefTotal = 0;
		int sprrefTotal = 0;
		int wavrefTotal = 0;
		for (int i=0;i<FileData.a.qcFileNames.Length;i++) {
			dataReader = new StreamReader(FileData.a.qcFileNames[i], Encoding.ASCII);
			currentLine = 0;
			using (dataReader) {
				do {
					// Read the next line
					readLine = dataReader.ReadLine();
					if (readLine == null) continue;
					var reg = new Regex("\".*?\"");
					// MatchCollection mc = reg.Matches(readLine);
					// string[] entries = new string[mc.Count];
					// for (int ij=0;ij<mc.Count;ij++) entries[ij] = mc[ij].Groups[0].Value;
					string[] entries = readLine.Split('"');
					if (entries.Length > 1) {
						for (int j=0;j<entries.Length;j++) {
							if (LineCheck(entries[j],exmdl.ToString())) {
								modelReferencesInQc.Add(entries[j]);
								mdlrefTotal++;
							} else if (LineCheck(entries[j],exspr.ToString())) {
								spriteReferencesInQc.Add(entries[j]);
								sprrefTotal++;
							} else if (LineCheck(entries[j],exwav.ToString())) {
								waveReferencesInQc.Add(entries[j]);
								wavrefTotal++;
							}
						}
					}
					currentLine++;
				} while (!dataReader.EndOfStream);
				dataReader.Close();
			}
		}

		Log.a.WriteToLog("Models references found (.mdl): " + mdlrefTotal.ToString()
						 + ", Sprites references found (.spr): " + sprrefTotal.ToString()
						 + ", Sounds references found (.wav): " + wavrefTotal.ToString());
		int badmdl = 0;
		int badspr = 0;
		int badwav = 0;
		errMsg = string.Empty;
		for (int i=0;i<modelReferencesInQc.Count;i++) {
			bool foundref = false;
			for(int j=0;j<FileData.a.mdlMapEditorNames.Length;j++) {
				if (FileData.a.mdlMapEditorNames[j] == modelReferencesInQc[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.modelIDFiles.Length;j++) {
					if (FileData.a.modelIDFiles[j] == modelReferencesInQc[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.mdl): "
									 + modelReferencesInQc[i]);
					badmdl++;
				}
			}
		}

		for (int i=0;i<spriteReferencesInQc.Count;i++) {
			bool foundref = false;
			for(int j=0;j<FileData.a.sprMapEditorNames.Length;j++) {
				if (FileData.a.sprMapEditorNames[j] == spriteReferencesInQc[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.spriteIDFiles.Length;j++) {
					if (FileData.a.spriteIDFiles[j] == spriteReferencesInQc[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.spr): "
									 + spriteReferencesInQc[i]);
					badspr++;
				}
			}
		}

		for (int i=0;i<waveReferencesInQc.Count;i++) {
			bool foundref = false;
			for(int j=0;j<FileData.a.wavMapEditorNames.Length;j++) {
				if (FileData.a.wavMapEditorNames[j] == waveReferencesInQc[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.waveIDFiles.Length;j++) {
					if (FileData.a.waveIDFiles[j] == waveReferencesInQc[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.wav): "
									 + waveReferencesInQc[i]);
					badwav++;
				}
			}
		}

		Log.a.WriteToLog("Found " + badmdl.ToString() + " missing .mdl references.");
		Log.a.WriteToLog("Found " + badspr.ToString() + " missing .spr references.");
		Log.a.WriteToLog("Found " + badwav.ToString() + " missing .wav references.");
    }
}