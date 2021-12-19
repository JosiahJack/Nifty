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

public class DocQC : MonoBehaviour {
	private List<string> functionReferences;
	private List<string> entityReferences;
	private StreamReader dataReader;
	private string readLine;
	private int currentLine;
	public Toggle useTags;

	public string DocQCAction() {
		StringBuilder outputLogs = new StringBuilder();
		outputLogs.Append("Documenting all entities in .qc files...");
		Debug.Log("Documenting all entities in .qc files...");

		// Parse QC functions into a list of QCFunction data structs.

		return outputLogs.ToString();
	}
}