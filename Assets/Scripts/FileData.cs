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
    private string lastUsedModdir;
    private string lastPWD;

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

        qcFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.qc",
                        System.IO.SearchOption.AllDirectories);
        csqcFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.csqc",
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
    }
}