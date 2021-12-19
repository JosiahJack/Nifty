using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Const : MonoBehaviour {
	public Color[] colormap;
	public float doubleClickTime = 0.500f;
    public static Const a;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {a = this;}
}
