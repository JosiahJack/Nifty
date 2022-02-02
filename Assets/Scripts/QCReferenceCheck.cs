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
	private string[] modelFileNames;
	private string[] spriteFileNames;
	private string[] waveFileNames;
	private string[] modelIDFileNames;
	private string[] spriteIDFileNames;
	private string[] waveIDFileNames;
	private string[] qcFileNames;
	private List<string> modelReferences;
	private List<string> spriteReferences;
	private List<string> waveReferences;
	private StreamReader dataReader;
	public string id1FolderPath;
	private string readLine;
	private int currentLine;

    public string QCReferenceCheckAction()  {
		StringBuilder outputLogs = new StringBuilder();
		outputLogs.Append("Checking references to models, sprites, and sounds in .qc files...");
		Debug.Log("Checking references to models, sprites, and sounds in .qc files...");
		outputLogs.AppendLine();

		modelFileNames = Directory.GetFiles(Nifty.a.modFolderPath+"progs/","*.mdl",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<modelFileNames.Length;i++) {
			modelFileNames[i] = modelFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
			modelFileNames[i] = modelFileNames[i].Replace("\\","/");
		}

		modelIDFileNames = Directory.GetFiles(id1FolderPath+"progs/","*.mdl",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<modelIDFileNames.Length;i++) {
			modelIDFileNames[i] = modelIDFileNames[i].Remove(0,id1FolderPath.Length);
			modelIDFileNames[i] = modelIDFileNames[i].Replace("\\","/");
		}

		spriteFileNames = Directory.GetFiles(Nifty.a.modFolderPath+"progs/","*.spr",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<spriteFileNames.Length;i++) {
			spriteFileNames[i] = spriteFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
			spriteFileNames[i] = spriteFileNames[i].Replace("\\","/");
		}

		spriteIDFileNames = Directory.GetFiles(id1FolderPath+"progs/","*.spr",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<spriteIDFileNames.Length;i++) {
			spriteIDFileNames[i] = spriteIDFileNames[i].Remove(0,id1FolderPath.Length);
			spriteIDFileNames[i] = spriteIDFileNames[i].Replace("\\","/");
		}

		waveFileNames = Directory.GetFiles(Nifty.a.modFolderPath+"sound/","*.wav",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<waveFileNames.Length;i++) {
			waveFileNames[i] = waveFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
			waveFileNames[i] = waveFileNames[i].Remove(0,6); // remove 'sound/'
			waveFileNames[i] = waveFileNames[i].Replace("\\","/");
		}

		waveIDFileNames = Directory.GetFiles(id1FolderPath+"sound/","*.wav",System.IO.SearchOption.AllDirectories);
		for (int i=0;i<waveIDFileNames.Length;i++) {
			waveIDFileNames[i] = waveIDFileNames[i].Remove(0,id1FolderPath.Length);
			waveIDFileNames[i] = waveIDFileNames[i].Remove(0,6); // remove 'sound/'
			waveIDFileNames[i] = waveIDFileNames[i].Replace("\\","/");
		}

		qcFileNames = Directory.GetFiles(Nifty.a.modFolderPath+"keepsrc/","*.qc");

		modelReferences = new List<string>();
		spriteReferences = new List<string>();
		waveReferences = new List<string>();
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

		for (int i=0;i<qcFileNames.Length;i++) {
			dataReader = new StreamReader(qcFileNames[i], Encoding.ASCII);
			// Debug.Log("Check: " + qcFileNames[i]);
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
								modelReferences.Add(entries[j]);
								mdlrefTotal++;
								// Debug.Log("Reference found (.mdl): " + entries[j]);
							} else if (entries[j].Contains(s2spr.ToString()) && entries[j].Contains((char)47) && !entries[j].Contains(':') && !entries[j].Contains("//") && !entries[j].Contains("[") && !entries[j].Contains("]")) {
								spriteReferences.Add(entries[j]);
								sprrefTotal++;
								// Debug.Log("Reference found (.spr): " + entries[j]);
							} else if (entries[j].Contains(s3wav.ToString()) && entries[j].Contains((char)47) && !entries[j].Contains(':') && !entries[j].Contains("//") && !entries[j].Contains("[") && !entries[j].Contains("]")) {
								waveReferences.Add(entries[j]);
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

		outputLogs.AppendLine();
		outputLogs.Append("Models found (.mdl): " + mdlrefTotal.ToString() + " and = " + modelReferences.Count.ToString());
		Debug.Log("Models found (.mdl): " + mdlrefTotal.ToString() + " and = " + modelReferences.Count.ToString());
		outputLogs.AppendLine();
		outputLogs.Append("Sprites found (.spr): " + sprrefTotal.ToString() + " and = " + spriteReferences.Count.ToString());
		Debug.Log("Sprites found (.spr): " + sprrefTotal.ToString() + " and = " + spriteReferences.Count.ToString());
		outputLogs.AppendLine();
		outputLogs.Append("Sounds found (.wav): " + wavrefTotal.ToString() + " and = " + waveReferences.Count.ToString());
		Debug.Log("Sounds found (.wav): " + wavrefTotal.ToString() + " and = " + waveReferences.Count.ToString());

		int badmdl = 0;
		int badspr = 0;
		int badwav = 0;

		for (int i=0;i<modelReferences.Count;i++) {
			bool foundref = false;
			for(int j=0;j<modelFileNames.Length;j++) {
				if (modelFileNames[j] == modelReferences[i]) foundref = true;
			}
			if (!foundref) {
				for(int j=0;j<modelIDFileNames.Length;j++) {
					if (modelIDFileNames[j] == modelReferences[i]) foundref = true;
				}
				if (!foundref) {
					outputLogs.AppendLine();
					outputLogs.Append("Missing file (.mdl): " + modelReferences[i]);
					Debug.Log("Missing file (.mdl): " + modelReferences[i]);
					badmdl++;
				}
			}
		}

		for (int i=0;i<spriteReferences.Count;i++) {
			bool foundref = false;
			for(int j=0;j<spriteFileNames.Length;j++) {
				if (spriteFileNames[j] == spriteReferences[i]) foundref = true;
				// Debug.Log("Ref: " + spriteReferences[i] + " vs. " + spriteFileNames[j]);
			}
			if (!foundref) {
				for(int j=0;j<spriteIDFileNames.Length;j++) {
					if (spriteIDFileNames[j] == spriteReferences[i]) foundref = true;
				}
				if (!foundref) {
					outputLogs.AppendLine();
					outputLogs.Append("Missing file (.spr): " + spriteReferences[i]);
					Debug.Log("Missing file (.spr): " + spriteReferences[i]);
					badspr++;
				}
			}
		}

		for (int i=0;i<waveReferences.Count;i++) {
			bool foundref = false;
			for(int j=0;j<waveFileNames.Length;j++) {
				if (waveFileNames[j] == waveReferences[i]) foundref = true;
				// Debug.Log("Ref: " + waveReferences[i] + " vs. " + waveFileNames[j]);
			}
			if (!foundref) {
				for(int j=0;j<waveIDFileNames.Length;j++) {
					if (waveIDFileNames[j] == waveReferences[i]) foundref = true;
				}
				if (!foundref) {
					outputLogs.AppendLine();
					outputLogs.Append("Missing file (.wav): " + waveReferences[i]);
					Debug.Log("Missing file (.wav): " + waveReferences[i]);
					badwav++;
				}
			}
		}

		outputLogs.AppendLine();
		outputLogs.Append("Found " + badmdl.ToString() + " missing .mdl references.");
		Debug.Log("Found " + badmdl.ToString() + " missing .mdl references.");
		outputLogs.AppendLine();
		outputLogs.Append("Found " + badspr.ToString() + " missing .spr references.");
		Debug.Log("Found " + badspr.ToString() + " missing .spr references.");
		outputLogs.AppendLine();
		outputLogs.Append("Found " + badwav.ToString() + " missing .wav references.");
		Debug.Log("Found " + badwav.ToString() + " missing .wav references.");

		return outputLogs.ToString();
		// #if UNITY_EDITOR_WIN
			// UnityEditor.EditorApplication.isPlaying = false;
		// #endif
    }
}