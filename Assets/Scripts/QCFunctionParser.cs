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

public class QCFunctionParser : MonoBehaviour {
	public enum FunctionType {Function,Entity,Unknown};
	private FunctionType[] funcTypeArray;

	private string[] RemoveComment(IEnumerable<string> loc) {
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
			if (line[i].Contains("[ENTITY]")) {
				funcTypeArray[i] = FunctionType.Entity;
			} else if (line[i].Contains("[FUNCTION]")) {
				funcTypeArray[i] = FunctionType.Function;
			} else {
				funcTypeArray[i] = FunctionType.Unknown;
			}

			if (line[i].Contains("dcrossbow_checkmelee")) {
				Debug.Log("line[" + i.ToString() + "] funcType: " + funcTypeArray[i].ToString() + ":: " + line[i]);
			}

			if (line[i].Contains("//")) {
				mistakeComment = false;
				if(line[i].Contains("*//*")) { //Case mistake /**//**/ with //
					if ((line[i].IndexOf("//") - line[i].IndexOf("*//*")) == 1) {
						mistakeComment = true;
					}
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
						comment = line[i].Substring(startComPos,
								    endComPos - startComPos + 2);
						line[i] = line[i].Replace(comment, string.Empty);
					}
				}
			}

			if (line[i].Contains("/*"))
			while((multiCommentStart = line[i].IndexOf("/*")) >= 0
				  && (multiCommentEnd = line[i].IndexOf("*/")) >= 0
                  && multiCommentEnd > multiCommentStart) {
				comment = line[i].Substring(multiCommentStart,
						    multiCommentEnd - multiCommentStart + 2);
				line[i] = line[i].Replace(comment, string.Empty);
			}
		}
		return line;
	}

	public List<QCFunction_struct> ParseQCFunctions(string path) {
		List<QCFunction_struct> lstCppFunc = new List<QCFunction_struct>();

		IEnumerable<string> listOfFunctions = new string[] {};
		foreach (string ln in File.ReadLines(path,System.Text.Encoding.ASCII)) {
			listOfFunctions = listOfFunctions.Concat(new[] { ln });
		}
		funcTypeArray = new FunctionType[listOfFunctions.Count()];
		string[] listOfFunctionsNoComments = RemoveComment(listOfFunctions);
		int level = 0;
		QCFunction_struct crtFunc = new QCFunction_struct();
		int lineCount = 0;
		StringBuilder builder = new StringBuilder();
		bool startName = false;
		string builderToString;
		string lastLine = "";
		for (int i=0;i<listOfFunctionsNoComments.Length;i++) {
			//Debug.Log("funcTypeArray[2047]: " + funcTypeArray[2047].ToString());
			lineCount++; // start at 1 and increment thereafter
			if (string.IsNullOrWhiteSpace(listOfFunctionsNoComments[i])) {
				lastLine = listOfFunctionsNoComments[i];
				continue;
			}

			if (i < funcTypeArray.Length) crtFunc.functionType=funcTypeArray[i];
			if (level <= 0) {
				if (listOfFunctionsNoComments[i].Contains('(')) {
					if (listOfFunctionsNoComments[i].Trim().IndexOf('(') == 0) {
						builder.Append(lastLine);
					}
					builder.AppendLine(listOfFunctionsNoComments[i]);
					crtFunc.startLine = lineCount;
					crtFunc.fullPath = path;
					crtFunc.sourceFilename = Path.GetFileName(path);
					startName = true;
				}
				if (startName) {
					builderToString = builder.ToString();
					if (listOfFunctionsNoComments[i] != 
						builderToString.Replace("\r\n",string.Empty)) {
						builder.AppendLine(listOfFunctionsNoComments[i]);
					}
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