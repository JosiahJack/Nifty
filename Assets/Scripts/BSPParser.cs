using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Diagnostics;

public class BSPParser : MonoBehaviour {
    private string utilPath = "/StreamingAssets/bsputil";
	public static CultureInfo en_US_Culture = new CultureInfo("en-US");
    public static BSPParser a;

    void Awake() {
        a = this;
    }

    public void CheckBSPs() {
        Log.a.WriteToLog("Checking all bsp files...");
        for (int i=0;i<FileData.a.bspFileNames.Length;i++) {
            CheckBSPForErrors(FileData.a.bspFileNames[i]);
        }
        Log.a.WriteToLog("Done checking bsp files!");
    }

    private void CheckBSPForErrors(string path) {
        int i = 0;
        int numIssues = 0;
        string whole;
        string entFile = path;
        bool entFileGenerated = false;
        bool foundref = false;
		entFile = entFile.Remove(entFile.Length - 3,3);
        entFile = entFile + "ent";
        if (File.Exists(entFile)) {
            whole = File.ReadAllText(entFile);
            if (!(whole.Contains("\"classname\" \"info_player_start\""))) {
                return;
            }

            //Log.a.WriteToLog("Checking extent " + entFile);
        } else {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Application.dataPath + utilPath;
            p.StartInfo.Arguments = "--extract-entities " + path;
            p.Start(); // Execute bsputil to get entity data as an .ent file.
            p.WaitForExit(); // e.g. e1m1.bsp -> e1m1.ent [NEW]
            if (File.Exists(entFile)) {
                entFileGenerated = true;
                whole = File.ReadAllText(entFile);
                if (!(whole.Contains("\"classname\" \"info_player_start\""))) {
                    File.Delete(entFile);
                    return;
                }

                //Log.a.WriteToLog("Checking generated " + entFile);
            } else {
                Log.a.WriteToLog("Failed to generate " + entFile);
                return;
            }
        }

        List<string> targets = new List<string>();
        List<string> targetnames = new List<string>();
        List<string> soundNames = new List<string>();
        List<string> modelNames = new List<string>();
        List<string> spriteNames = new List<string>();
        string skyname = "";
        string[] lines = File.ReadAllLines(entFile);
        for (i=0;i<lines.Length;i++) {
            string[] entries = lines[i].Split('"');
            if (entries.Length > 2) {
                if (entries[1] == "target") targets.Add(entries[3]);
                else if (entries[1] == "target2") targets.Add(entries[3]);
                else if (entries[1] == "target3") targets.Add(entries[3]);
                else if (entries[1] == "target4") targets.Add(entries[3]);
                else if (entries[1] == "targetback") targets.Add(entries[3]);
                else if (entries[1] == "killtarget") targets.Add(entries[3]);
                else if (entries[1] == "killtarget2") targets.Add(entries[3]);
                else if (entries[1] == "deathtarget") targets.Add(entries[3]);
                else if (entries[1] == "angletarget") targets.Add(entries[3]);
                else if (entries[1] == "angrytarget") targets.Add(entries[3]);
                else if (entries[1] == "sighttarget") targets.Add(entries[3]);
                else if (entries[1] == "turrettarget") targets.Add(entries[3]);
                else if (entries[1] == "turretclosing") targets.Add(entries[3]);
                else if (entries[1] == "turretopening") targets.Add(entries[3]);
                else if (entries[1] == "event") targets.Add(entries[3]);
                else if (entries[1] == "event2") targets.Add(entries[3]);
                else if (entries[1] == "message" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);
                else if (entries[1] == "message2" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);
                else if (entries[1] == "noise1" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);
                else if (entries[1] == "noise2" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);
                else if (entries[1] == "noise3" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);
                else if (entries[1] == "noise4" && !entries[3].Contains(" ") && !entries[3].Contains(".")) targets.Add(entries[3]);

                else if (entries[1] == "targetname") targetnames.Add(entries[3]);
                else if (entries[1] == "sky") skyname = entries[3];

                if (lines.Contains(".wav")) soundNames.Add(entries[3]);
                if (lines.Contains(".mp3")) soundNames.Add(entries[3]);
                if (lines.Contains(".ogg")) soundNames.Add(entries[3]);
                if (lines.Contains(".mdl")) modelNames.Add(entries[3]);
                if (lines.Contains(".spr")) spriteNames.Add(entries[3]);
                if (lines.Contains("sounds")) {
                    int val = GetIntFromString(entries[3]);
                    if (val >= 0) {
                        string trackname = "track" + entries[3];
                        spriteNames.Add(trackname + ".mp3");
                        spriteNames.Add(trackname + ".ogg");
                        spriteNames.Add(trackname + ".wav");
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(skyname)) {
            bool foundSky = false;
            for (int sk=0;sk<FileData.a.skyNames.Length;sk++) {
                if (skyname == FileData.a.skyNames[sk]) foundSky = true;
            }
            if (!foundSky) {
                Log.a.WriteToLog(path + ":: Failed to find sky: " + skyname);
                numIssues++;
            }
        }

        bool foundTarg = false;
        for (i=0;i<targets.Count;i++) {
            foundTarg = false;
            for (int j=0;j<targetnames.Count;j++) {
                if (targets[i] == targetnames[j]) foundTarg = true;
            }
            if (!foundTarg) {
                Log.a.WriteToLog(path + ":: No matching targetname for target value of "
                                 + targets[i]);
                numIssues++;
            }
        }

        bool foundTargname = false;
        for (i=0;i<targetnames.Count;i++) {
            foundTargname = false;
            for (int j=0;j<targets.Count;j++) {
                if (targetnames[i] == targets[j]) foundTargname = true;
            }
            if (!foundTargname) {
                Log.a.WriteToLog(path + ":: No activator targetting targetname value of "
                                 + targetnames[i]);
                numIssues++;
            }
        }

		for (i=0;i<modelNames.Count;i++) {
			foundref = false;
			for(int j=0;j<FileData.a.mdlMapEditorNames.Length;j++) {
				if (FileData.a.mdlMapEditorNames[j] == modelNames[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.modelIDFiles.Length;j++) {
					if (FileData.a.modelIDFiles[j] == modelNames[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.mdl): " + modelNames[i]);
					numIssues++;
				}
			}
		}

		for (i=0;i<spriteNames.Count;i++) {
			for(int j=0;j<FileData.a.sprMapEditorNames.Length;j++) {
				if (FileData.a.sprMapEditorNames[j] == spriteNames[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.spriteIDFiles.Length;j++) {
					if (FileData.a.spriteIDFiles[j] == spriteNames[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.spr): " + spriteNames[i]);
					numIssues++;
				}
			}
		}

		for (i=0;i<soundNames.Count;i++) {
			foundref = false;
			for(int j=0;j<FileData.a.wavMapEditorNames.Length;j++) {
				if (FileData.a.wavMapEditorNames[j] == soundNames[i]) {
					foundref = true;
				}
			}
			if (!foundref) {
				for(int j=0;j<FileData.a.waveIDFiles.Length;j++) {
					if (FileData.a.waveIDFiles[j] == soundNames[i]) {
						foundref = true;
					}
				}
				if (!foundref) {
					Log.a.WriteToLog("Missing file (.wav): " + soundNames[i]);
					numIssues++;
				}
			}
		}

        if (numIssues > 0) Log.a.WriteToLog(path + ":: " + numIssues.ToString()
                                            + " issues found");
        if (entFileGenerated) File.Delete(entFile);
    }

	public int GetIntFromString(string val) {
		if (val == "0") return 0;

        int getValreadInt = -1;
		bool getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture,
                                      out getValreadInt);
		if (!getValparsed) return -1;
		return getValreadInt;
	}
}