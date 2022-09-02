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

public class Nifty : MonoBehaviour {
	public string modFolderPath;
	public string fgdFilePath;
	public string outputFolderPath;
	public string outputFileName = "nifty_output";
	public string logFilePath = "nifty_log.txt";
	public InputField tinputModPath;
	public InputField tinputOutputPath;
	public InputField tinputOutputFilename;
	public InputField tinputFGDPath;
	public InputField tinputLogPath;
	private QCEntityDump qcentRef;
	public QCFunctionParser qcFuncParserRef;
	private QCReferenceCheck qcrefRef;
	private FGDEntityDump fgdentRef;
	private OutputAllMiscModels outputmodelsRef;
	private DocQC docQC;
	private string lastFgdFilePath;
	public bool onLinux;
	public bool onWindows;
	public bool onMac;
	public bool useUnixPaths;

	public static Nifty a;

	void Start() {
		a = this;
		a.qcentRef = GetComponent<QCEntityDump>();
		a.qcrefRef = GetComponent<QCReferenceCheck>();
		a.fgdentRef = GetComponent<FGDEntityDump>();
		a.qcFuncParserRef = GetComponent<QCFunctionParser>();
		a.docQC = GetComponent<DocQC>();
		a.outputmodelsRef = GetComponent<OutputAllMiscModels>();
		a.ReadLastUserSettingsFromFile();
		OperatingSystem os = Environment.OSVersion;
		PlatformID pid = os.Platform;
		a.onWindows = a.onLinux = a.onMac = a.useUnixPaths = false;
		if (pid == PlatformID.Win32NT || pid == PlatformID.Win32S
			|| pid == PlatformID.Win32Windows || pid == PlatformID.WinCE) {
			a.onWindows = true; // Ironically, the odd duck here.
		} else if (pid == PlatformID.Unix) {
			a.onLinux = a.useUnixPaths = true;
		} else if (pid == PlatformID.MacOSX) {
			a.onMac = a.useUnixPaths = true;
		}
		a.lastFgdFilePath = "/home/qmaster/QUAKE/null.fgd";
	}

	// Add trailing slash.
	// Example: C:/Program Files/QUAKE/mod
	//          becomes
	//          C:/Program Files/QUAKE/mod/
	// This is necessary for adding subpaths.
	public string FixSlashEnd(string str) {
		if (string.IsNullOrWhiteSpace(str)) return str;

		if (!(str.EndsWith("/") || str.EndsWith("\\"))) str = str + "/";
		return str;
	}

	// Add beginning slash for unix filesystems only (Linux, MAC, etc.).
	// Example:  home/username/QUAKE/mod/
	//           becomes:
	//          /home/username/QUAKE/mod/
	// This is necessary for Directory.GetFiles to use absolute pathing.
	public string FixSlashBeginning(string str) {
		if (string.IsNullOrWhiteSpace(str)) return str;
		if (!useUnixPaths) return str; // Don't change it for Windows.

		if (str[0] != '/') str = "/" + str;
		return str;
	}

	public string FixExtensionMissing(string str, string ext) {
		if (string.IsNullOrWhiteSpace(str)) return str;

		string extCap = ext.ToUpper();
		if (!str.EndsWith(ext) && !str.EndsWith(extCap)) return (str + ext);
		return str;
	}

	public void ModPathEntry() {
		// Fix both ends for path that ends in folder.
		tinputModPath.text = FixSlashEnd(tinputModPath.text);
		tinputModPath.text = FixSlashBeginning(tinputModPath.text);
		modFolderPath = tinputModPath.text;
		WriteLastUserSettingsToFile();
	}

	public void OutputPathEntry() {
		// Fix both ends for path that ends in folder.
		tinputOutputPath.text = FixSlashEnd(tinputOutputPath.text);
		tinputOutputPath.text = FixSlashBeginning(tinputOutputPath.text);
		outputFolderPath = tinputOutputPath.text;
		WriteLastUserSettingsToFile();
	}

	public void OutputFilenameEntry() {
		// No fixing, this is a name with no path or filetype extension.
		outputFileName = tinputOutputFilename.text;
		WriteLastUserSettingsToFile();
	}

	public void FGDPathEntry() {
		if (string.IsNullOrWhiteSpace(tinputFGDPath.text)) return;

		// Fix beginning only since this ends with the file name and extension.
		tinputFGDPath.text = FixSlashBeginning(tinputFGDPath.text); 		
		tinputFGDPath.text = FixExtensionMissing(tinputFGDPath.text,".fgd");
		fgdFilePath = tinputFGDPath.text;
		if (tinputFGDPath.text != lastFgdFilePath) WriteLastUserSettingsToFile();
	}

	public void LogPathEntry() {
		// Fix beginning only since this ends with the file name and extension.
		tinputLogPath.text = FixSlashBeginning(tinputLogPath.text);
		logFilePath = tinputLogPath.text;
		WriteLastUserSettingsToFile();
	}

	public void ButtonModFileCheck() {
		if (!string.IsNullOrWhiteSpace(modFolderPath)) {
			Log.a.WriteToLog("Error! Function does not exist yet.");
		} else {
			Log.a.WriteToLog("Error! Mod folder path not specified.");
		}
	}

	public void ButtonQCEntityDump() {
		if (!string.IsNullOrWhiteSpace(modFolderPath) && qcentRef != null) {
			qcentRef.QCFindEntitiesForList();
			qcentRef.QCEntityDumpAction();
		} else {
			Log.a.WriteToLog("Error! QC folder path not specified.");
		}
	}

	public void ButtonQCReferenceCheck() {
		if (!string.IsNullOrWhiteSpace(modFolderPath) && qcrefRef != null) {
			qcrefRef.QCReferenceCheckAction();
		} else {
			Log.a.WriteToLog("Error! Mod folder path not specified.");
		}
	}

	public void ButtonFGDEntityDump() {
		if (!string.IsNullOrWhiteSpace(outputFolderPath) && fgdentRef != null) {
			fgdentRef.FGDFindEntitiesForList();
			fgdentRef.FGDEntityDumpAction();
		} else {
			Log.a.WriteToLog("Error! FGD folder path not specified.");
		}
	}

	public void ButtonDocQC() {
		if (!string.IsNullOrWhiteSpace(outputFolderPath) && docQC != null) {
			docQC.DocQCAction();
		} else {
			Log.a.WriteToLog("Error! DocQC folder path not specified.");
		}
	}

	public void ButtonOutputAllModelsToMap() {
		if (!string.IsNullOrWhiteSpace(modFolderPath)
            && !string.IsNullOrWhiteSpace(outputFolderPath)
            && outputmodelsRef != null) {
			outputmodelsRef.OutputAllMiscModelsAction();
		} else {
			Log.a.WriteToLog("Error! Mod folder path not specified.");
		}
	}

	public void ButtonCompareQCtoFGDEntities() {
		if (!string.IsNullOrWhiteSpace(modFolderPath) && qcentRef != null) {
			Log.a.WriteToLog("Comparing .fgd entities to .qc entities...");
			fgdentRef.FGDFindEntitiesForList();
			qcentRef.QCFindEntitiesForList();
			CompareFGDToQcEntities();
			Log.a.WriteToLog("Done.  Comparison complete.");
		} else {
			Log.a.WriteToLog("Error! QC folder path not specified.");
		}
	}

	void WriteLastUserSettingsToFile() {
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath +
						      "/nifty_settings.dat",false,Encoding.ASCII);
		if (sw == null) return;

		using (sw) {
			sw.WriteLine(modFolderPath);
			sw.WriteLine(fgdFilePath);
			sw.WriteLine(outputFolderPath);
			sw.WriteLine(outputFileName);
			sw.WriteLine(logFilePath);
			// OutputAllMiscModels xoffset
			// OutputAllMiscModels yoffset
			// OutputAllMiscModels xwidth
			// OutputAllMiscModels ywidth
			sw.Close();
		}
	}

	void ReadLastUserSettingsFromFile() {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		StreamReader dataReader = new StreamReader(Application.streamingAssetsPath
									  + "/nifty_settings.dat",Encoding.ASCII);
		if (dataReader != null) {
			using (dataReader) {
				do {
					// Read the next line
					readline = dataReader.ReadLine();
					if (currentline == 0) modFolderPath = readline;
					else if (currentline == 1) fgdFilePath = readline;
					else if (currentline == 2) outputFolderPath = readline;
					else if (currentline == 3) outputFileName = readline;
					else if (currentline == 4) logFilePath = readline;
					currentline++;
				} while (!dataReader.EndOfStream);
				dataReader.Close();
			}
		}

		tinputModPath.text = modFolderPath;
		tinputOutputPath.text = outputFolderPath;
		tinputOutputFilename.text = outputFileName;
		tinputFGDPath.text = fgdFilePath;
		tinputLogPath.text = logFilePath;
	}

	private void CompareFGDToQcEntities() {
		int i, j, missingEntitiesInQC, missingEntitiesInFGD;
		missingEntitiesInQC = missingEntitiesInFGD = 0;
		for (i = 0; i < qcentRef.entityReferences.Count; i++) {
			bool foundMatch = false;
			for (j = 0; j < fgdentRef.entityReferences.Count; j++) {
				if (fgdentRef.entityReferences[j] == qcentRef.entityReferences[i]) {
					foundMatch = true;
				}
			}
			if (!foundMatch) {
				Log.a.WriteToLog("Could not find qc entity `" 
								 + qcentRef.entityReferences[i] + "` in fgd");
				missingEntitiesInFGD++;
			}
		}

		for (i = 0; i < fgdentRef.entityReferences.Count; i++) {
			bool foundMatch = false;
			for (j = 0; j < qcentRef.entityReferences.Count; j++) {
				if (fgdentRef.entityReferences[i] == qcentRef.entityReferences[j]) {
					foundMatch = true;
				}
			}
			if (!foundMatch) {
				Log.a.WriteToLog("Could not find fgd entity `" 
								 + qcentRef.entityReferences[i]
                                 + "` in qc files");
				missingEntitiesInQC++;
			}
		}
		string message = "Comparison results: ";
		if (missingEntitiesInFGD > 0) {
			message = message + " ...Missing " + missingEntitiesInFGD
                      + " entities in the .fgd file";
		}

		if (missingEntitiesInQC > 0) {
			message = message + " ...Missing " + missingEntitiesInQC
                      + " entities in the .qc files";
		}

		if (missingEntitiesInFGD + missingEntitiesInQC == 0) {
			message = "No missing entities found.  QC and FGD correlate!";
		}
		Log.a.WriteToLog(message);
	}
}