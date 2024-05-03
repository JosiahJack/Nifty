using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class ImportMDL : MonoBehaviour {
	private Color col;
	private Color[] internalColormap;
	private Mesh mdlmesh;
	private Mesh[] mdlFrameMeshes;
	private Vector2[] uvmap;

	void Start () {
		internalColormap = Const.a.colormap;
	}

	public mdl_header_t ImportMDLFromFile (string fileNameAndPath) {
		mdl_header_t mdlHeader = new mdl_header_t();

		using (BinaryReader b = new BinaryReader(File.Open((fileNameAndPath),
														   FileMode.Open))) {
			mdlmesh = new Mesh();

			// 1. Read Header
			mdlHeader.ident = b.ReadInt32(); // Get the magic number
			if (mdlHeader.ident != 1330660425) {
				Log.a.WriteToLog("ERROR: Could not import mdl: "
								 + fileNameAndPath
								 + ", bad ident number was"
								 + mdlHeader.ident.ToString()
								 + ",(not IDPO(1330660425))");

				return mdlHeader;
			}

			mdlHeader.version = b.ReadInt32();
			if (mdlHeader.version != 6) {
				Log.a.WriteToLog("ERROR: Could not import mdl: "
								 + fileNameAndPath
								 + ", bad version number was"
								 + mdlHeader.version.ToString() +",(not 6)");

				return mdlHeader;
			}

			mdlHeader.scale.x = b.ReadSingle();
			mdlHeader.scale.y = b.ReadSingle();
			mdlHeader.scale.z = b.ReadSingle();
			mdlHeader.translate.x = b.ReadSingle();
			mdlHeader.translate.y = b.ReadSingle();
			mdlHeader.translate.z = b.ReadSingle();
			mdlHeader.boundingradius = b.ReadSingle();
			mdlHeader.eyeposition.x = b.ReadSingle();
			mdlHeader.eyeposition.y = b.ReadSingle();
			mdlHeader.eyeposition.z = b.ReadSingle();
			mdlHeader.num_skins = b.ReadInt32();
			mdlHeader.skinwidth = b.ReadInt32();
			mdlHeader.skinheight = b.ReadInt32();
			mdlHeader.num_verts = b.ReadInt32();
			mdlHeader.num_tris = b.ReadInt32();
			mdlHeader.num_frames = b.ReadInt32();
			mdlHeader.synctype = b.ReadInt32();
			mdlHeader.flags = b.ReadInt32();
			mdlHeader.size = b.ReadSingle();
		}
		return mdlHeader;
	}

	public struct mdl_header_t {
		public int ident;
		public int version;
		public Vector3 scale;
		public Vector3 translate;
		public float boundingradius;
		public Vector3 eyeposition;
		public int num_skins;
		public int skinwidth;
		public int skinheight;
		public int num_verts;
		public int num_tris;
		public int num_frames;
		public int synctype;
		public int flags;
		public float size;
	}

	public struct mdl_skin_t {
		public int group;
		public Byte data;
	}

	public struct mdl_groupskin_t {
		public int group;
		public int nb;
		public float time;
		public byte data;
	}

	public struct mdl_texcoord_t {
		public Int16 onseam;
		public Int16 s;
		public Int16 t;
	}

	public struct mdl_triangle_t {
		public int facesfront;
		public int[] vertex; // array length 3
	}

	public struct mdl_vertex_t {
		public Byte[] v; // array length 3
		public Byte normalIndex;
	}

	public struct mdl_simpleframe_t {
		public mdl_vertex_t bboxmin;
		public mdl_vertex_t bboxmax;
		public Byte[] name; // array length 16
		public mdl_vertex_t verts;
	}

	public struct mdl_frame_t {
		public int type;
		public mdl_simpleframe_t frame;
	}

	public struct mdl_groupframe_t {
		public int type;
		public mdl_vertex_t min;
		public mdl_vertex_t max;
		public float time;
		public mdl_simpleframe_t frames;
	}
}