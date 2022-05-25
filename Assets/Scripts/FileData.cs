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
    public string[] spr32FileNames;      // C:/QUAKE/mod/progs/sprite.spr32
    public string[] wavFileNames;        // C:/QUAKE/mod/sound/sound.wav
    private bool qcInitialized;
    private bool mdlInitialized;
    private bool sprInitialized;
    private bool spr32Initialized;
    private bool wavInitialized;
    public bool initialized = false;
    private string lastUsedModdir;
    private string lastPWD;

    public static FileData a;

    void Awake() {
        a = this;
        a.qcInitialized = a.mdlInitialized = false;
        a.sprInitialized = a.spr32Initialized = false;
        a.wavInitialized = false;
        a.initialized = false;
        a.lastUsedModdir = string.Empty;
    }

    public void PopulateFileNames() {
        if (initialized) return;

        if (!mdlInitialized) {
            mdlFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.mdl",
                                              System.IO.SearchOption.AllDirectories);
            mdlMapEditorNames = new string[mdlFileNames.Length];
            for (int i=0;i<mdlFileNames.Length;i++) {
                mdlFileNames[i] = mdlFileNames[i].Replace("\\","/");
                mdlMapEditorNames[i] =
                    mdlFileNames[i].Remove(0,Nifty.a.modFolderPath.Length);
            }
            mdlInitialized = true;
        }
        if (!sprInitialized) {
            sprFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.spr",
                                              System.IO.SearchOption.AllDirectories);
            sprInitialized = true;
        }
        if (!spr32Initialized) { 
            spr32FileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.spr32",
                                                System.IO.SearchOption.AllDirectories);
            spr32Initialized = true;
        }
        if (!qcInitialized) {
            qcFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.qc",
                                             System.IO.SearchOption.AllDirectories);
            csqcFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.csqc",
                                               System.IO.SearchOption.AllDirectories);
            string[] srcFiles = Directory.GetFiles(Nifty.a.modFolderPath,"*.src",
                                                   System.IO.SearchOption.AllDirectories);
            progsSrcFileName = csprogsSrcFileName = string.Empty;
            for (int i=0;i<srcFiles.Length; i++) {
                if (srcFiles[i].Contains("progs.src")) {
                    progsSrcFileName = srcFiles[i];
                } else if (srcFiles[i].Contains("csprogs.src")) {
                    csprogsSrcFileName = srcFiles[i];
                }
            }
            qcInitialized = true;
        }
        if (!wavInitialized) { 
            wavFileNames = Directory.GetFiles(Nifty.a.modFolderPath,"*.wav",
                                              System.IO.SearchOption.AllDirectories);
            wavInitialized = true;
        }
        initialized = qcInitialized && mdlInitialized && sprInitialized
                      && spr32Initialized && wavInitialized;
    }
}