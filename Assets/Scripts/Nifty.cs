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
using UnityEngine.UI;

public class Nifty : MonoBehaviour {
	public string modFolderPath;
	public string qcFolderPath;
	public string[] qcFileNames;
	public string fgdFolderPath;
	public string outputFolderPath;
	public string outputFileName = "nifty_output";
	public string logFileName = "nifty_log.txt";
	public InputField tinputModPath;
	public InputField tinputOutputPath;
	public InputField tinputOutputFilename;
	public InputField tinputQCPath;
	public InputField tinputFGDPath;
	public InputField tinputLogPath;
	private QCEntityDump qcentRef;
	private QCReferenceCheck qcrefRef;
	private FGDEntityDump fgdentRef;
	private OutputAllMiscModels outputmodelsRef;
	private DocQC docQC;
	public Text logtext;

	public static Nifty a;

	void Start() {
		a = this;
		a.qcentRef = GetComponent<QCEntityDump>();
		a.qcrefRef = GetComponent<QCReferenceCheck>();
		a.fgdentRef = GetComponent<FGDEntityDump>();
		a.docQC = GetComponent<DocQC>();
		a.outputmodelsRef = GetComponent<OutputAllMiscModels>();
		a.ReadLastUserSettingsFromFile();
	}

	public void ModPathEntry() {
		modFolderPath = tinputModPath.text;
		WriteLastUserSettingsToFile();
	}

	public void OutputPathEntry() {
		outputFolderPath = tinputOutputPath.text;
		WriteLastUserSettingsToFile();
	}

	public void OutputFilenameEntry() {
		outputFileName = tinputOutputFilename.text;
		WriteLastUserSettingsToFile();
	}

	public void QCPathEntry() {
		qcFolderPath = tinputQCPath.text;
		WriteLastUserSettingsToFile();
		qcFileNames = Directory.GetFiles(qcFolderPath,"*.qc");
	}

	public void FGDPathEntry() {
		fgdFolderPath = tinputFGDPath.text;
		WriteLastUserSettingsToFile();
	}

	public void LogPathEntry() {
		logFileName = tinputLogPath.text;
		WriteLastUserSettingsToFile();
	}

	public void ButtonModFilenameCheck() {
		if (!string.IsNullOrWhiteSpace(modFolderPath)) {
			// string outputLogs = 
			// logtext.text += outputLogs;
			string outputLogs = "Error! Function to check file names does not exist yet.";
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! Mod folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonQCEntityDump() {
		if (!string.IsNullOrWhiteSpace(qcFolderPath) && qcentRef != null) {
			string outputLogs = qcentRef.QCFindEntitiesForList();
			outputLogs += qcentRef.QCEntityDumpAction();
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! QC folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonQCReferenceCheck() {
		if (!string.IsNullOrWhiteSpace(qcFolderPath) && qcrefRef != null) {
			string outputLogs = qcrefRef.QCReferenceCheckAction();
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! Mod folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonFGDEntityDump() {
		if (!string.IsNullOrWhiteSpace(outputFolderPath) && fgdentRef != null) {
			string outputLogs = fgdentRef.FGDEntityDumpAction();
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! FGD folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonDocQC() {
		if (!string.IsNullOrWhiteSpace(outputFolderPath) && docQC != null) {
			string outputLogs = docQC.DocQCAction();
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! DocQC folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonOutputAllModelsToMap() {
		if (!string.IsNullOrWhiteSpace(modFolderPath) && !string.IsNullOrWhiteSpace(outputFolderPath) && outputmodelsRef != null) {
			string outputLogs = outputmodelsRef.OutputAllMiscModelsAction();
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! Mod folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	public void ButtonCompareQCtoFGDEntities() {
		if (!string.IsNullOrWhiteSpace(qcFolderPath) && qcentRef != null) {
			qcentRef.QCFindEntitiesForList();
			string outputLogs = "Done.";
			WriteToLog(outputLogs);
		} else {
			string outputLogs = "Error! QC folder path not specified.";
			WriteToLog(outputLogs);
		}
	}

	void WriteToLog(string entries) {
		//if (entries.Length < 10000) logtext.text = entries;
		//else logtext.text = "Text too large to display.  Logged to logfile.";
		StreamWriter sw = new StreamWriter(logFileName,true,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				sw.Write(entries);
				sw.Close();
			}
		}
	}

	void WriteLastUserSettingsToFile() {
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/nifty_settings.dat",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				sw.WriteLine(modFolderPath);
				sw.WriteLine(qcFolderPath);
				sw.WriteLine(fgdFolderPath);
				sw.WriteLine(outputFolderPath);
				sw.WriteLine(outputFileName);
				sw.WriteLine(logFileName);
				// OutputAllMiscModels xoffset
				// OutputAllMiscModels yoffset
				// OutputAllMiscModels xwidth
				// OutputAllMiscModels ywidth
				sw.Close();
			}
		}
	}

	void ReadLastUserSettingsFromFile() {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		StreamReader dataReader = new StreamReader(Application.streamingAssetsPath + "/nifty_settings.dat",Encoding.ASCII);
		if (dataReader != null) {
			using (dataReader) {
				do {
					// Read the next line
					readline = dataReader.ReadLine();
					if (currentline == 0) modFolderPath = readline;
					else if (currentline == 1) qcFolderPath = readline;
					else if (currentline == 2) fgdFolderPath = readline;
					else if (currentline == 3) outputFolderPath = readline;
					else if (currentline == 4) outputFileName = readline;
					else if (currentline == 5) logFileName = readline;
					// else if (currentline == 6) modFolderPath = readline;
					// else if (currentline == 7) modFolderPath = readline;
					// else if (currentline == 8) modFolderPath = readline;
					// else if (currentline == 9) modFolderPath = readline;
					currentline++;
				} while (!dataReader.EndOfStream);
				dataReader.Close();
			}
		}

		tinputModPath.text = modFolderPath;
		tinputOutputPath.text = outputFolderPath;
		tinputOutputFilename.text = outputFileName;
		tinputQCPath.text = qcFolderPath;
		tinputFGDPath.text = fgdFolderPath;
		tinputLogPath.text = logFileName;
		if (!string.IsNullOrWhiteSpace(qcFolderPath))qcFileNames = Directory.GetFiles(qcFolderPath,"*.qc");
	}
}

public struct QCFunction {
    public string functionName;
    public string content;
    public string fullPath;
	public string sourceFilename;
    public int startLine;
    public int endLine;
	public QCFunctionParser.FunctionType functionType;
}

public static class QCFunctionParser {
	public enum FunctionType {Function,Entity,Unknown};
	private static string[] RemoveComment(IEnumerable<string> loc) {
		string[] line = loc.ToArray();
		bool startComment = false;
		int startComPos=0;
		int endComPos=-1;
		int count = line.Length;
		string comment;
		bool mistakeComment;
		int multiCommentStart, multiCommentEnd;
		for(int i=0;i<count;i++) {
			if (string.IsNullOrWhiteSpace(line[i])) continue;
			// if (line[i].Contains("[ENTITY]")) functionTaggedAsEntity[i] = IsAnEntity;
			// else if (line[i].Contains("[FUNCTION]")) functionTaggedAsEntity[i] = IsAFunction;
			// else functionTaggedAsEntity[i] = IsUnknown;

			if (line[i].Contains("//")) {
				mistakeComment = false;
				if(line[i].Contains("*//*")) { //Case mistake /**//**/ with //
					if ((line[i].IndexOf("//") - line[i].IndexOf("*//*")) == 1) mistakeComment = true;
				}

				if(!mistakeComment) {
					comment = line[i].Substring(line[i].IndexOf("//"));
					line[i] = line[i].Replace(comment, string.Empty);
				}
			}
			if(line[i].Contains("/*")) {
				startComment = true;
				startComPos = line[i].IndexOf("/*");
				endComPos = -1;
			} else {
				startComPos = 0;
			}

			if (startComment) {
				if(!string.IsNullOrEmpty(line[i])) {
					if (line[i].Contains("*/")) {
						startComment = false;
						endComPos = line[i].IndexOf("*/", startComPos);
					} else endComPos = -1;

					if (endComPos == -1) {
						comment = line[i].Substring(startComPos);
						line[i] = line[i].Replace(comment, string.Empty);
					} else {
						comment = line[i].Substring(startComPos, endComPos - startComPos + 2);
						line[i] = line[i].Replace(comment, string.Empty);
					}
				}
			}

			if (line[i].Contains("/*"))
			while((multiCommentStart = line[i].IndexOf("/*")) >= 0 && (multiCommentEnd = line[i].IndexOf("*/")) >= 0 && multiCommentEnd > multiCommentStart) {
				comment = line[i].Substring(multiCommentStart, multiCommentEnd - multiCommentStart + 2);
				line[i] = line[i].Replace(comment, string.Empty);
			}
		}
		return line;
	}

	public static List<QCFunction> ParseQCFunctions(string path) {
		List<QCFunction> lstCppFunc = new List<QCFunction>();

		IEnumerable<string> listOfFunctions = File.ReadLines(path, System.Text.Encoding.ASCII);
		string[] listOfFunctionsNoComments = RemoveComment(listOfFunctions);
		int level = 0;
		QCFunction crtFunc = new QCFunction();
		int lineCount = 0;
		StringBuilder builder = new StringBuilder();
		bool startName = false;
		string builderToString;
		string lastLine = "";
		for (int i=0;i<listOfFunctionsNoComments.Length;i++) {
			lineCount++; // start at 1 and increment thereafter
			if (string.IsNullOrWhiteSpace(listOfFunctionsNoComments[i])) { lastLine = listOfFunctionsNoComments[i]; continue; }

			if (level <= 0) {
				if (listOfFunctionsNoComments[i].Contains('(')) {
					if (listOfFunctionsNoComments[i].Trim().IndexOf('(') == 0) builder.Append(lastLine);
					builder.AppendLine(listOfFunctionsNoComments[i]);
					crtFunc.startLine = lineCount;
					crtFunc.fullPath = path;
					crtFunc.sourceFilename = Path.GetFileName(path);
					startName = true;
				}
				if (startName) {
					builderToString = builder.ToString();
					if (listOfFunctionsNoComments[i] != builderToString.Replace("\r\n",string.Empty)) builder.AppendLine(listOfFunctionsNoComments[i]);
					if (listOfFunctionsNoComments[i].Contains(')')) {
						startName = false;
						crtFunc.functionName = builder.ToString();
						builder.Clear();
					}
				}
			}

			if(listOfFunctionsNoComments[i].Contains('{')) {
				foreach(char c in listOfFunctionsNoComments[i]) {
					if (c == '{') level++;
				}
			}
			if(listOfFunctionsNoComments[i].Contains('}')) {
				foreach (char c in listOfFunctionsNoComments[i]) {
					if (c == '}') level--;
				}
				if (level <= 0) {
					crtFunc.endLine = lineCount;
					lstCppFunc.Add(crtFunc);
					level = 0;
				}
			}
			lastLine = listOfFunctionsNoComments[i];
		}
		return lstCppFunc;
	}
}