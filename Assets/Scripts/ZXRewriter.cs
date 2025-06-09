// ZXRewriter.cs (with LLM provider + auto snapshot fill)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ZXRewriter : MonoBehaviour
{
    public InputField circuitInput;
    public Text zxCircuitLabel;
    public Button rewriteButton;
    public Text resultLabel;

    public Dropdown providerDropdown;
    public InputField endpointField;

    public string[] lastRules;
    public string lastRewritten;
    public bool useAI = true;

    void Start()
    {
        if (rewriteButton != null)
            rewriteButton.onClick.AddListener(RewriteCircuit);

        UpdateCircuitDisplay("H - CNOT - T - H");
    }

    public void RewriteCircuit()
    {
        string circuit = circuitInput.text;
        string provider = providerDropdown.options[providerDropdown.value].text.ToLower();
        string endpoint = endpointField.text;

        StartCoroutine(SendRewriteRequest(circuit, provider, endpoint));
    }

    IEnumerator SendRewriteRequest(string circuit, string provider, string endpoint)
    {
        resultLabel.text = "🔄 Contacting LLM server...";

        string json = JsonUtility.ToJson(new CircuitData
        {
            circuit = circuit,
            mode = useAI ? "rewrite" : "explain"
        });

        UnityWebRequest req = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            resultLabel.text = "❌ Error: " + req.error;
        }
        else
        {
            RewriteResponse resp = JsonUtility.FromJson<RewriteResponse>(req.downloadHandler.text);
            lastRules = resp.rules_applied;
            lastRewritten = resp.rewritten;

            zxCircuitLabel.text = $"Original: {resp.original}\nRewritten: {resp.rewritten}";
            resultLabel.text = $"✅ Rules applied: {string.Join(", ", resp.rules_applied)}\n\n{resp.reasoning}";

            // ✅ Auto-fill snapshot metadata
            if (SnapshotManager.Instance != null)
            {
                var snap = SnapshotManager.Instance.GetLatestSnapshot();
                if (snap != null)
                {
                    snap.circuit = resp.original;
                    snap.zxRewritten = resp.rewritten;
                    snap.zxRewriteRules = resp.rules_applied;
                    Debug.Log("📎 Snapshot updated with ZX rewrite results.");
                }
            }
        }
    }

    public void UpdateCircuitDisplay(string circuit)
    {
        if (circuitInput != null)
            circuitInput.text = circuit;

        if (zxCircuitLabel != null)
            zxCircuitLabel.text = "Current Circuit: " + circuit;
    }

    [System.Serializable]
    public class CircuitData
    {
        public string circuit;
        public string mode;
    }

    [System.Serializable]
    public class RewriteResponse
    {
        public string original;
        public string rewritten;
        public string[] rules_applied;
        public string reasoning;
    }
}
