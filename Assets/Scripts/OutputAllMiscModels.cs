using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;

public class OutputAllMiscModels : MonoBehaviour {
	private List<int> modelFrameCountReferences;
	private List<int> modelSkinCountReferences;
	private List<float> modelBoundsReferences;
	private string readLine;
	private int currentLine;
	public float xOffset = 64;
	public float yOffset = 64;
	public float xWidth = 4096;
	public float yWidth = 4096;
	private ImportMDL.mdl_header_t head;
	private float maxRadiusThisRow;
	private float lastRadius;
	public ImportMDL importer;

    public void OutputAllMiscModelsAction()  {
		Log.a.WriteToLog("Dumping all models into the mod folder into a .map...");
		FileData.a.PopulateFileNames();
		maxRadiusThisRow = 8f;
		lastRadius = maxRadiusThisRow;
		modelFrameCountReferences = new List<int>();
		modelSkinCountReferences = new List<int>();
		modelBoundsReferences = new List<float>();
		for (int i=0;i<FileData.a.mdlFileNames.Length;i++) {
			head = importer.ImportMDLFromFile(FileData.a.mdlFileNames[i]); // Expensive!
			modelFrameCountReferences.Add(head.num_frames);
			modelSkinCountReferences.Add(head.num_skins);
			modelBoundsReferences.Add(head.boundingradius);
		}

		StreamWriter sw = new StreamWriter(Nifty.a.outputFolderPath + Nifty.a.outputFileName + "_miscmodels.map",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				// Write Worldspawn header using Keep template map
				sw.WriteLine("// Game: Quake");
				sw.WriteLine("// Format: Valve");
				sw.WriteLine("// entity 0");
				sw.WriteLine("{");
				sw.WriteLine("\"classname\" \"worldspawn\"");
				sw.WriteLine("\"mapversion\" \"220\"");
				sw.WriteLine("\"wad\" \"C:/Users/Josiah/Dropbox/QUAKE/Other/Compiling/quake.wad\"");
				sw.WriteLine("\"_tb_def\" \"external:C:/Users/Josiah/Dropbox/QUAKE/Other/Compiling/Keep.fgd\"");
				sw.WriteLine("\"_tb_mod\" \"keep\"");
				sw.WriteLine("// brush 0");
				sw.WriteLine("{");
				sw.WriteLine("( -1024 -1024 -544 ) ( -1024 -1023 -544 ) ( -1024 -1024 -543 ) city4_44 [ 0 -1 0 0 ] [ 0 0 -1 0 ] 0 1 1");
				sw.WriteLine("( -1024 -1024 -544 ) ( -1024 -1024 -543 ) ( -1023 -1024 -544 ) city4_44 [ 1 0 0 0 ] [ 0 0 -1 0 ] 0 1 1");
				sw.WriteLine("( -1024 -1024 -544 ) ( -1023 -1024 -544 ) ( -1024 -1023 -544 ) city4_44 [ -1 0 0 0 ] [ 0 -1 0 0 ] 0 1 1");
				sw.WriteLine("( 1152 1008 -512 ) ( 1152 1009 -512 ) ( 1153 1008 -512 ) city4_44 [ 1 0 0 0 ] [ 0 -1 0 0 ] 0 1 1");
				sw.WriteLine("( 1152 1024 -512 ) ( 1153 1024 -512 ) ( 1152 1024 -511 ) city4_44 [ -1 0 0 0 ] [ 0 0 -1 0 ] 0 1 1");
				sw.WriteLine("( 1024 1008 -512 ) ( 1024 1008 -511 ) ( 1024 1009 -512 ) city4_44 [ 0 1 0 0 ] [ 0 0 -1 0 ] 0 1 1");
				sw.WriteLine("}");
				sw.WriteLine("}");

				float x = -1f * (xWidth/2f);
				float y = -1f * (yWidth/2f);
				float z = 0f;
				float rad = maxRadiusThisRow;
				for (int j=0;j<FileData.a.mdlMapEditorNames.Length;j++) {
					if (string.IsNullOrWhiteSpace(FileData.a.mdlMapEditorNames[j])) continue;
					sw.WriteLine("// entity " + (j+1).ToString());
					sw.WriteLine("{");
					sw.WriteLine("\"classname\" \"misc_model\"");
					sw.WriteLine("\"origin\" \"" + x.ToString("N4") + " " + y.ToString("N4") + " " + z.ToString("N4") + "\"");
					sw.WriteLine("\"mdl\" \"" + FileData.a.mdlMapEditorNames[j] + "\"");
					if (modelSkinCountReferences[j] > 1) {
						sw.WriteLine("\"pos2\" \"0 " + modelSkinCountReferences[j].ToString() + " -1\"");
					}
					if (modelFrameCountReferences[j] > 1) {
						sw.WriteLine("\"pos1\" \"0 " + (modelFrameCountReferences[j]-1).ToString() + " -1\"");
					} else {
						if (modelSkinCountReferences[j] <= 1) sw.WriteLine("\"spawnflags\" \"32\"");
					}
					sw.WriteLine("}");
					rad = modelBoundsReferences[j];
					if (rad > maxRadiusThisRow) maxRadiusThisRow = rad;
					// if (rad > 256f) rad = 256f; // Cap it for sanity
					if (rad < xOffset) rad = xOffset;
					x += xOffset;
					lastRadius = rad;
					if (x > (xWidth/2f)) {
						x = -1f * (xWidth/2f);
						y += maxRadiusThisRow; // Move over by an amount equal to largest radius dist
						// maxRadiusThisRow = 8f; // Reset for next row
						lastRadius = 8f;
						if (y > (yWidth/2f)) {
							y = -1f * (yWidth/2f);
							z += 256f;
						}
					}
				}
				sw.Close();
			}
		}
		Log.a.WriteToLog("Success!");
    }
}