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

public class FGDEntityDump : MonoBehaviour {
	[HideInInspector] public List<string> entityReferences;

    public void FGDFindEntitiesForList()  {
		Log.a.WriteToLog("Finding all entities in .fgd file...");
		entityReferences = new List<string>();
		int trimdex = 0;
		int trimdexEqualSign = 0;
		int lastGoodCharIndex = 0;
        StreamReader dataReader = new StreamReader(Nifty.a.fgdFilePath);
		string s1 = "";
		string s2 = "";
		string s3 = "";
        using (dataReader) {
            do {
                // Read the next line
                string readline = dataReader.ReadLine();
				if (readline == null) continue;
				if (readline.Length < 4) continue;
				if (readline[0] == '/' && readline[1] == '/') continue;
				if (!readline.Contains("=")) continue;
				//if (!readline.Contains("[")) continue;

				trimdexEqualSign = readline.IndexOf("=");
				if (trimdexEqualSign < 0) continue;

				if (trimdexEqualSign < (readline.Length - 2)) {
					s1 = readline.Remove(0,trimdexEqualSign + 1);
				}
				s2 = s1.Trim();
				if (s2.Length <= 1) continue;

				trimdex = 0;
				// Find trimming index for rightmost non-whitespace 
				// or [ character.
				Match match;
				for (int i=0;i<s2.Length;i++) {
					if (i == 0) {
						match = Regex.Match(s2[i].ToString(),"[a-z]");
					} else {
						match = Regex.Match(s2[i].ToString(),"[a-z_0-9]");
					}
					trimdex = i;
					if (!match.Success) {
						lastGoodCharIndex = i-1;
						break;
					}
				}
				if (trimdex <= 0) continue;
				if (readline.Contains("item_armorInv")) {
					entityReferences.Add("item_armorInv");
					continue; // dumb dumb exceptions >(
				}

				if (trimdex < (s2.Length - 1)) {
					s3 = s2.Remove(trimdex,s2.Length - trimdex);
				}

				if (readline.Length > 1 && s3 != "worldspawn") {
					entityReferences.Add(s3);
				}
            } while (!dataReader.EndOfStream);
            dataReader.Close();
        }
	}
	
    public void FGDEntityDumpAction()  {
		Log.a.WriteToLog("Dumping all entities in .fgd to log...");
		for (int k=0;k<entityReferences.Count;k++) {
			Log.a.WriteToLog(entityReferences[k]);
		}
		Log.a.WriteToLog("Done! Found " + entityReferences.Count.ToString()
						 + " entities.");
    }
}