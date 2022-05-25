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

public struct QCFunction_struct {
    public string functionName;
    public string content;
    public string fullPath;
	public string sourceFilename;
    public int startLine;
    public int endLine;
	public QCFunctionParser.FunctionType functionType;
}