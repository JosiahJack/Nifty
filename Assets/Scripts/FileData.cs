using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;

public class FileData : MonoBehaviour {
	public string[] qcFileNames;         // C:/QUAKE/mod/modsrc/code.qc
    public string[] csqcFileNames;       // C:/QUAKE/mod/modsrc/csqc/code.csqc
    public string progsSrcFileName;      // C:/QUAKE/mod/modsrc/progs.src
    public string csprogsSrcFileName;    // C:/QUAKE/mod/modsrc/csprogs.src
    public string[] mdlFileNames;        // C:/QUAKE/mod/progs/model.mdl
    public string[] mdlMapEditorNames;   // progs/model.mdl
    public string[] sprFileNames;        // C:/QUAKE/mod/progs/sprite.spr
    public string[] sprMapEditorNames;   // progs/sprite.spr
    public string[] wavFileNames;        // C:/QUAKE/mod/sound/folder/sound.wav
    public string[] wavMapEditorNames;   // folder/sound.wav
	public string[] modelIDFiles;       // models in id1
	public string[] spriteIDFiles;      // sprites in id1
	public string[] waveIDFiles;        // sounds in id1
    public string[] bspFileNames;        // C:/QUAKE/mod/maps/map.bsp
    public string[] skyNames;            // skyname_ stripped of up, lf, rt etc.
    private string lastUsedModdir;
    private string lastPWD;
	private string id1FolderPath;

    public static FileData a;

    void Awake() {
        a = this;
        a.lastUsedModdir = string.Empty;
    }

    public void PopulateFileNames() {
        mdlFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.mdl",
                            System.IO.SearchOption.AllDirectories);
        mdlMapEditorNames = new string[mdlFileNames.Length];
        for (int i=0;i<mdlFileNames.Length;i++) {
            mdlFileNames[i] = mdlFileNames[i].Replace("\\","/");
            mdlMapEditorNames[i] =
                mdlFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
        }

        sprFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.spr",
                            System.IO.SearchOption.AllDirectories);
        sprMapEditorNames = new string[sprFileNames.Length];
        for (int i=0;i<sprFileNames.Length;i++) {
            sprFileNames[i] = sprFileNames[i].Replace("\\","/");
            sprMapEditorNames[i] =
                sprFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
        }

        qcFileNames = Directory.GetFiles(Nifty.a.modFolderPath + "keepsrc/","*.qc",
                        System.IO.SearchOption.AllDirectories);
        csqcFileNames = Directory.GetFiles(Nifty.a.modFolderPath + "keepsrc/csqc/","*.csqc",
                            System.IO.SearchOption.AllDirectories);
        string[] srcFiles = Directory.GetFiles(Nifty.a.modFolderPath,
                            "*.src",System.IO.SearchOption.AllDirectories);
        progsSrcFileName = csprogsSrcFileName = string.Empty;
        for (int i=0;i<srcFiles.Length; i++) {
            if (srcFiles[i].Contains("progs.src")) {
                progsSrcFileName = srcFiles[i];
            } else if (srcFiles[i].Contains("csprogs.src")) {
                csprogsSrcFileName = srcFiles[i];
            }
        }

        wavFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.wav",
                            System.IO.SearchOption.AllDirectories);
        wavMapEditorNames = new string[wavFileNames.Length];
        for (int i=0;i<wavFileNames.Length;i++) {
            wavFileNames[i] = wavFileNames[i].Replace("\\","/");
            // Remove the path and 6 more for 'sound'.
            wavMapEditorNames[i] =
                wavFileNames[i].Remove(0,Nifty.a.modFolderPath.Length + 6);
        }

        bspFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.bsp",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<bspFileNames.Length;i++) {
            bspFileNames[i] = bspFileNames[i].Replace("\\","/");
        }

        string[] skyNamesAll = Directory.GetFiles(Nifty.a.modFolderPath
                                 + "gfx/env/","*.tga",
                                 System.IO.SearchOption.AllDirectories);
        List<string> skyNamesStripped = new List<string>();
        for (int i=0;i<skyNamesAll.Length;i++) {
            skyNamesAll[i] = skyNamesAll[i].Replace("\\","/");
            // Make sure that there are 6 characters to remove, direction plus
            // the file extension, e.g. up.tga or dn.tga
            string s = skyNamesAll[i];
            s = s.Substring(0,(s.Length - 6));
            s = s.Remove(0,Nifty.a.modFolderPath.Length + 8);
            if (!String.IsNullOrWhiteSpace(s) && !skyNamesStripped.Contains(s)) {
                skyNamesStripped.Add(s);
                //Debug.Log("sky added: " + s);
            }
        }
        skyNames = skyNamesStripped.ToArray();



		string lastFolder = Path.GetDirectoryName(Nifty.a.modFolderPath);
		id1FolderPath = "/home/qmaster/QUAKE/id1/";//Path.GetDirectoryName(lastFolder) + "/id1/";
		if (id1FolderPath == null) {
			string errMsg = "ERROR: Could not find id1.  "
					       + "Ensure mod folder is a neighbor with id1.";
			Log.a.WriteToLog(errMsg);
			return;
		}

		modelIDFiles = Directory.GetFiles(id1FolderPath + "progs/","*.mdl",
						 System.IO.SearchOption.AllDirectories);
		// Strip from "/home/qmaster/QUAKE/id1/progs/model.mdl"
		// down to "progs/model.mdl"
		for (int i=0;i<modelIDFiles.Length;i++) {
			modelIDFiles[i] = modelIDFiles[i].Remove(0,id1FolderPath.Length);
			modelIDFiles[i] = modelIDFiles[i].Replace("\\","/");
		}

		spriteIDFiles = Directory.GetFiles(id1FolderPath + "progs/","*.spr",
						  System.IO.SearchOption.AllDirectories);
		// Strip from "/home/qmaster/QUAKE/id1/progs/sprite.spr"
		// down to "progs/sprite.spr"
		for (int i=0;i<spriteIDFiles.Length;i++) {
			spriteIDFiles[i] = spriteIDFiles[i].Remove(0,id1FolderPath.Length);
			spriteIDFiles[i] = spriteIDFiles[i].Replace("\\","/");
		}

		waveIDFiles = Directory.GetFiles(id1FolderPath + "sound/","*.wav",
					    System.IO.SearchOption.AllDirectories);
		// Strip from "/home/qmaster/QUAKE/id1/sound/folder/something.wav"
		// down to "folder/something.wav"
		// These are the odd one out as 'sound/' also needs removed.
		for (int i=0;i<waveIDFiles.Length;i++) {
			waveIDFiles[i] = waveIDFiles[i].Remove(0,id1FolderPath.Length);
			waveIDFiles[i] = waveIDFiles[i].Remove(0,6); // Remove 'sound/'.
			waveIDFiles[i] = waveIDFiles[i].Replace("\\","/");
		}
    }

    public void ReportCapsInFileNames() {
        string[] badMDLFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.MDL",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badMDLFileNames.Length;i++) {
            badMDLFileNames[i] = badMDLFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badMDLFileNames[i]);
        }

        string[] badSPRFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.SPR",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badSPRFileNames.Length;i++) {
            badSPRFileNames[i] = badSPRFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badSPRFileNames[i]);
        }

        string[] badWAVFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.WAV",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badWAVFileNames.Length;i++) {
            badWAVFileNames[i] = badWAVFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badWAVFileNames[i]);
        }

        string[] badMAPFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.MAP",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badMAPFileNames.Length;i++) {
            badMAPFileNames[i] = badMAPFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badMAPFileNames[i]);
        }

        string[] badTGAFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.TGA",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badTGAFileNames.Length;i++) {
            badTGAFileNames[i] = badTGAFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badTGAFileNames[i]);
        }

        string[] badBSPFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.BSP",
                            System.IO.SearchOption.AllDirectories);
        for (int i=0;i<badBSPFileNames.Length;i++) {
            badBSPFileNames[i] = badBSPFileNames[i].Replace("\\","/");
            Log.a.WriteToLog("Found file with bad capitalization: " + badBSPFileNames[i]);
        }
    }
}