// QECController.cs (Updated to use QubitVisual.cs and fixed QubitType.Anon support)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QECController : MonoBehaviour
{
    public GameObject latticeCellPrefab;
    public Transform latticeParent;
    public int gridSize = 5;
    public float flashInterval = 0.6f;
    public Text fidelityLabel;

    private QubitVisual[,] lattice;
    private List<Vector2Int> activeErrors = new();

    void Start()
    {
        GenerateLattice();
        InvokeRepeating("SimulateErrors", 1f, 5f);
    }

    void GenerateLattice()
    {
        lattice = new QubitVisual[gridSize, gridSize];
        float spacing = 40f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cell = Instantiate(latticeCellPrefab, latticeParent);
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * spacing, -y * spacing);

                QubitVisual qv = cell.GetComponent<QubitVisual>();
                qv.Init(x, y);
                lattice[x, y] = qv;
            }
        }
    }

    void SimulateErrors()
    {
        ClearErrors();
        activeErrors.Clear();

        QubitType qt = GlobalSettings.SelectedQubitType;

        int errorCount = qt switch
        {
            QubitType.Anon => Random.Range(2, 5),
            QubitType.Ion => gridSize,
            QubitType.Superconducting => Random.Range(6, 10),
            QubitType.Photonic => Random.Range(0, 2),
            _ => 3
        };

        for (int i = 0; i < errorCount; i++)
        {
            int x = Random.Range(0, gridSize);
            int y = Random.Range(0, gridSize);
            activeErrors.Add(new Vector2Int(x, y));
            StartCoroutine(FlashError(x, y));
        }

        StartCoroutine(ApplyCorrection());

        foreach (var err in activeErrors)
        {
            // For demonstration, mark neighboring cell as a syndrome
            int sx = Mathf.Clamp(err.x + 1, 0, gridSize - 1);
            int sy = err.y;
            lattice[sx, sy].MarkSyndrome(true);
        }
    }

    IEnumerator FlashError(int x, int y)
    {
        lattice[x, y].SetError(true);
        yield return new WaitForSeconds(flashInterval);
        lattice[x, y].SetError(false);
    }

    IEnumerator ApplyCorrection()
    {
        yield return new WaitForSeconds(flashInterval * 2);
        foreach (var err in activeErrors)
        {
            lattice[err.x, err.y].FlashCorrected();
            yield return new WaitForSeconds(flashInterval);
        }

        float fidelity = GlobalSettings.SelectedQubitType switch
        {
            QubitType.Anon => 0.91f + Random.Range(0f, 0.05f),
            QubitType.Ion => 0.89f + Random.Range(0f, 0.07f),
            QubitType.Superconducting => 0.85f + Random.Range(0f, 0.10f),
            QubitType.Photonic => 0.97f + Random.Range(0f, 0.02f),
            _ => 0.90f
        };

        fidelityLabel.text = $"Fidelity: {fidelity:F2}";
        SnapshotUtility.TakeSnapshot(
            circuit: "QEC Snapshot",
            kernelMatrix: null,
            kernelLabel: "N/A",
            qubitLabels: new string[0],
            qubitType: GlobalSettings.SelectedQubitType.ToString(),
            fidelity: float.Parse(fidelityLabel.text.Replace("Fidelity: ", "")),
            colorMap: "Default",
            errorPositions: GetActiveErrors(),
            syndromePositions: GetSyndromePositions(),
            zxRules: new string[0],
            zxRewrittenStr: "",
            zxDiagramName: "",
            zxDiagram: null
        );
    }

    void ClearErrors()
    {
        foreach (var qv in lattice)
        {
            qv.SetError(false);
        }
    }

    public Vector2Int[] GetActiveErrors()
    {
        List<Vector2Int> errors = new();
        foreach (var qv in lattice)
        {
            if (qv.HasError)
                errors.Add(new Vector2Int(qv.X, qv.Y));
        }
        return errors.ToArray();
    }

    public Vector2Int[] GetSyndromePositions()
    {
        List<Vector2Int> result = new();
        foreach (QubitVisual q in lattice)
        {
            if (q.IsSyndrome())
                result.Add(new Vector2Int(q.X, q.Y));
        }
        return result.ToArray();
    }

    private bool hideSyndromes = false;

    public bool ToggleSyndromesHidden()
    {
        hideSyndromes = !hideSyndromes;
        foreach (QubitVisual q in lattice)
        {
            if (q.IsSyndrome())
                q.SetColor(hideSyndromes ? Color.white : new Color(1f, 0.5f, 0f));
        }
        return hideSyndromes;
    }

    public void LoadSnapshot(Snapshot snap)
    {
        if (snap.activeErrorPositions != null)
        {
            ClearErrors();
            foreach (Vector2Int err in snap.activeErrorPositions)
            {
                if (IsValid(err))
                    lattice[err.x, err.y].SetError(true);
            }
        }

        if (snap.qecErrors != null)
        {
            foreach (Vector2Int s in snap.qecErrors)
            {
                if (IsValid(s))
                    lattice[s.x, s.y].MarkSyndrome(true);
            }
        }

        fidelityLabel.text = $"Snapshot Fidelity: {snap.fidelity:F2}";
    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridSize && pos.y < gridSize;
    }
}
