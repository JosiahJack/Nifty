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
	private List<string> entityReferences;
	private int currentline;

    public void FGDEntityDumpAction()  {
		Log.a.WriteToLog("Dumping all FGD entity references...");
		entityReferences = new List<string>();
		int trimdex = 0;
		int trimdexEqualSign = 0;
		int lastGoodCharIndex = 0;
        StreamReader dataReader = new StreamReader(Nifty.a.fgdFilePath);
		currentline = 1;
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
				if (readline.Contains("=") && readline.Contains("[")) {
					trimdexEqualSign = readline.IndexOf("=");
					if (trimdexEqualSign < 0) continue;

					if (trimdexEqualSign < (readline.Length - 2)) s1 = readline.Remove(0,trimdexEqualSign + 1);
					s2 = s1.Trim();
					if (s2.Length <= 1) continue;

					trimdex = 0;
					// Find trimming index for rightmost non-whitespace or [ character.
					for (int i=0;i<s2.Length;i++) {
						Match match = Regex.Match(s2[i].ToString(),"^[a-z_][a-z0-9_]*$");
						trimdex = i;
						if (!match.Success) {
							lastGoodCharIndex = i-1;
							break;
						}
					}
					if (trimdex <= 0) continue;
					if (readline.Contains("item_armorInv")) {entityReferences.Add("item_armorInv"); continue; } // dumb dumb exceptions >(

					if (trimdex < (s2.Length - 1)) s3 = s2.Remove(trimdex,s2.Length - trimdex);
					if (readline.Length > 1 && s3 != "worldspawn") {
						entityReferences.Add(s3);
					}
				}
                currentline++;
            } while (!dataReader.EndOfStream);
            dataReader.Close();
        }
	

		StreamWriter sw = new StreamWriter(Nifty.a.outputFolderPath + Nifty.a.outputFileName + "fgd_ents.txt",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (int k=0;k<entityReferences.Count;k++) {
					sw.WriteLine(entityReferences[k]);
				}
				sw.Close();
			}
		}

		Log.a.WriteToLog("Found " + entityReferences.Count.ToString() + " entities.");
    }
}