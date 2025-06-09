// Snapshot.cs (Revised with qecErrors fix, Group 2)
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Snapshot
{
    // Core fields
    public string experimentId;         // Unique ID for the snapshot
    public string timestamp;            // Timestamp of when the snapshot was taken
    public string circuit;              // Representation of the quantum circuit, if applicable
    public float[,] kernelMatrix;       // The kernel matrix data
    public string kernelLabel;          // A descriptive label for the kernel (e.g., "Gaussian RBF k=0.5")
    public string[] qubitLabels;        // Labels for qubits/dimensions in the kernel matrix
    public string qubitType;            // Type of qubits (e.g., "Physical", "Logical")
    public float fidelity;              // Fidelity score or other performance metric
    public string colorMap;             // Identifier for a colormap used in visualization

    // QEC related fields
    public Vector2Int[] activeErrorPositions; // Positions of applied errors
    public Vector2Int[] syndromePositions;    // Positions of detected syndromes
    public Vector2Int[] qecErrors;            // For legacy compatibility (mirrors activeErrorPositions)

    // ZX Diagram related fields
    public string[] zxRewriteRules;     // List of ZX rewrite rules applied
    public string zxRewritten;          // Representation of the rewritten ZX diagram (e.g., as a string)
    public string zxDiagramName;        // Name for the ZX Diagram
    public ZXDiagramViewer.ZXDiagram zxDiagram; // The ZX Diagram data structure itself

    // Parameterless constructor for object initializers and deserialization
    public Snapshot() 
    {
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        this.qubitLabels = new string[0];
        this.zxRewriteRules = new string[0];
        this.activeErrorPositions = new Vector2Int[0];
        this.syndromePositions = new Vector2Int[0];
        this.qecErrors = new Vector2Int[0]; // Default legacy field

        // Initialize other fields to sensible defaults if necessary
        this.experimentId = string.Empty;
        this.circuit = string.Empty;
        this.kernelLabel = string.Empty;
        this.qubitType = string.Empty;
        this.colorMap = string.Empty;
        this.zxRewritten = string.Empty;
        this.zxDiagramName = string.Empty;
    }

    // Parameterized constructor for direct instantiation
    public Snapshot(
        string id,
        string circ,                // circuit string
        float[,] kernel,            // kernel matrix
        string kLabel,              // kernel label
        string[] qLabels,           // qubit labels
        string qType,               // qubit type
        float fidel,                // fidelity
        string cMap,                // color map
        Vector2Int[] errPos,        // active error positions
        Vector2Int[] syndPos,       // syndrome positions
        string[] zxRules,           // zx rewrite rules applied
        string zxRewrittenStr,      // zx diagram as string after rewrites
        string zxName = "",         // zx diagram name
        ZXDiagramViewer.ZXDiagram zxDiag = null // zx diagram object
    )
    {
        this.experimentId = id;
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        this.circuit = circ;
        this.kernelMatrix = kernel;
        this.kernelLabel = kLabel;
        this.qubitLabels = qLabels ?? new string[0];
        this.qubitType = qType;
        this.fidelity = fidel;
        this.colorMap = cMap;
        this.activeErrorPositions = errPos ?? new Vector2Int[0];
        this.syndromePositions = syndPos ?? new Vector2Int[0];
        this.qecErrors = this.activeErrorPositions; // Set legacy field
        this.zxRewriteRules = zxRules ?? new string[0];
        this.zxRewritten = zxRewrittenStr;
        this.zxDiagramName = zxName;
        this.zxDiagram = zxDiag;
    }
}


