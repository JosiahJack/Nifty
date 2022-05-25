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
	private string[] modelIDFileNames;
	private string[] spriteIDFileNames;
	private string[] waveIDFileNames;
	private List<string> modelReferencesInQc;
	private List<string> spriteReferencesInQc;
	private List<string> waveReferencesInQc;
	private StreamReader dataReader;
	private string id1FolderPath;
	private string readLine;
	private int currentLine;
	private string errMsg;

    public void QCReferenceCheckAction()  {
		Log.a.WriteToLog("Checking references to models, sprites, and sounds in .qc files...");
		FileData.a.PopulateFileNames();
		string lastFolder = Path.GetDirectoryName(Nifty.a.modFolderPath);
		id1FolderPath = Path.GetDirectoryName(lastFolder);
		if (id1FolderPath == null) {
			errMsg = "ERROR: Could not find id1.  "
					 + "Ensure mod folder is a neighbor with id1.";
			Log.a.WriteToLog(errMsg);
			return;
		}
		modelIDFileNames = Directory.GetFiles(id1FolderPath+"progs/","*.mdl",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<modelIDFileNames.Length;i++) {
			modelIDFileNames[i] = modelIDFileNames[i].Remove(0,id1FolderPath.Length);
			modelIDFileNames[i] = modelIDFileNames[i].Replace("\\","/");
		}

		spriteIDFileNames = Directory.GetFiles(id1FolderPath+"progs/","*.spr",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<spriteIDFileNames.Length;i++) {
			spriteIDFileNames[i] = spriteIDFileNames[i].Remove(0,id1FolderPath.Length);
			spriteIDFileNames[i] = spriteIDFileNames[i].Replace("\\","/");
		}

		waveIDFileNames = Directory.GetFiles(id1FolderPath+"sound/","*.wav",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<waveIDFileNames.Length;i++) {
			waveIDFileNames[i] = waveIDFileNames[i].Remove(0,id1FolderPath.Length);
			waveIDFileNames[i] = waveIDFileNames[i].Remove(0,6); // remove 'sound/'
			waveIDFileNames[i] = waveIDFileNames[i].Replace("\\","/");
		}

		modelReferencesInQc = new List<string>();
		spriteReferencesInQc = new List<string>();
		waveReferencesInQc = new List<string>();
		int mdlrefTotal = 0;
		int sprrefTotal = 0;
		int wavrefTotal = 0;
		StringBuilder s1mdl;
		StringBuilder s2spr;
		StringBuilder s3wav;
		s1mdl = new StringBuilder();
		s2spr = new StringBuilder();
		s3wav = new StringBuilder();
		s1mdl.Append(".mdl");
		// s1mdl.Append((char)34);
		s2spr.Append(".spr");
		// s2spr.Append((char)34);
		s3wav.Append(".wav");
		// s3wav.Append((char)34);

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
							// Debug.Log("Entry: " + entries[j]);
							if (entries[j].Contains(s1mdl.ToString()) && entries[j].Contains((char)47) && !entries[j].Contains(':') && !entries[j].Contains("//") && !entries[j].Contains("[") && !entries[j].Contains("]")) {
								modelReferencesInQc.Add(entries[j]);
								mdlrefTotal++;
								// Debug.Log("Reference found (.mdl): " + entries[j]);
							} else if (entries[j].Contains(s2spr.ToString()) && entries[j].Contains((char)47) && !entries[j].Contains(':') && !entries[j].Contains("//") && !entries[j].Contains("[") && !entries[j].Contains("]")) {
								spriteReferencesInQc.Add(entries[j]);
								sprrefTotal++;
								// Debug.Log("Reference found (.spr): " + entries[j]);
							} else if (entries[j].Contains(s3wav.ToString()) && entries[j].Contains((char)47) && !entries[j].Contains(':') && !entries[j].Contains("//") && !entries[j].Contains("[") && !entries[j].Contains("]")) {
								waveReferencesInQc.Add(entries[j]);
								wavrefTotal++;
								// Debug.Log("Reference found (.wav): " + entries[j]);
							}
						}
					}
					currentLine++;
				} while (!dataReader.EndOfStream);
				dataReader.Close();
				// Debug.Log("File had " + currentLine.ToString() + " lines.");
			}
		}

		Log.a.WriteToLog("Models found (.mdl): " + mdlrefTotal.ToString()
						 + " and = " + modelReferencesInQc.Count.ToString());
		Log.a.WriteToLog("Sprites found (.spr): " + sprrefTotal.ToString()
						 + " and = " + spriteReferencesInQc.Count.ToString());
		Log.a.WriteToLog("Sounds found (.wav): " + wavrefTotal.ToString()
						 + " and = " + waveReferencesInQc.Count.ToString());
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
				for(int j=0;j<modelIDFileNames.Length;j++) {
					if (modelIDFileNames[j] == modelReferencesInQc[i]) {
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
			for(int j=0;j<FileData.a.sprFileNames.Length;j++) {
				if (FileData.a.sprFileNames[j] == spriteReferencesInQc[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<spriteIDFileNames.Length;j++) {
					if (spriteIDFileNames[j] == spriteReferencesInQc[i]) {
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
			for(int j=0;j<FileData.a.wavFileNames.Length;j++) {
				if (FileData.a.wavFileNames[j] == waveReferencesInQc[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<waveIDFileNames.Length;j++) {
					if (waveIDFileNames[j] == waveReferencesInQc[i]) {
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