using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KernelExplorer : MonoBehaviour
{
    public RawImage kernelHeatmap;
    public Text metadataLabel;
    public Dropdown kernelTypeDropdown;
    public Dropdown colorMapDropdown;
    public Dropdown kernelSizeDropdown;
    public InputField labelOverrideInput;
    public Button importButton;

    public Dropdown noiseModelDropdown;
    public Slider noiseStrengthSlider;
    public Toggle smearingToggle;
    public Toggle previewNoiseToggle;
    public Text statusLabel;

    private Dictionary<string, float[,]> sampleKernels = new();
    private float[,] originalKernel;
    private string currentKernelName;
    private string currentColorMap = "BlueRed";

    void Start()
    {
        kernelTypeDropdown.onValueChanged.AddListener(delegate { VisualizeSelectedKernel(); });
        colorMapDropdown.onValueChanged.AddListener(delegate { VisualizeSelectedKernel(); });
        importButton.onClick.AddListener(() => ImportAndLabel());
        if (noiseModelDropdown) noiseModelDropdown.onValueChanged.AddListener(delegate { VisualizeSelectedKernel(); });
        if (noiseStrengthSlider) noiseStrengthSlider.onValueChanged.AddListener(delegate { VisualizeSelectedKernel(); });
        if (smearingToggle) smearingToggle.onValueChanged.AddListener(delegate { VisualizeSelectedKernel(); });
        if (previewNoiseToggle != null) previewNoiseToggle.onValueChanged.AddListener(delegate { ApplyNoise(); });

        LoadMockKernels();
        kernelTypeDropdown.ClearOptions();
        kernelTypeDropdown.AddOptions(new List<string>(sampleKernels.Keys));
        colorMapDropdown.AddOptions(new List<string> { "BlueRed", "Hot", "Cool", "Viridis" });

        if (noiseModelDropdown)
        {
            noiseModelDropdown.ClearOptions();
            noiseModelDropdown.AddOptions(new List<string> { "None", "Depolarizing", "GaussianBlur", "RandomCollapse" });
        }
    }

    void LoadMockKernels()
    {
        int size = GetKernelSize();
        sampleKernels.Clear();
        sampleKernels["QAOA"] = GenerateMockKernel(size, 0.9f);
        sampleKernels["VQE"] = GenerateMockKernel(size, 0.7f);
        sampleKernels["Classical"] = GenerateMockKernel(size, 0.3f);
        sampleKernels["Signal+BG"] = GenerateMockKernel(size, 0.6f);
        sampleKernels["DataEffective"] = GenerateMockKernel(size, 0.8f);
        sampleKernels["QKE"] = GenerateMockKernel(size, 0.85f);
        sampleKernels["HybridVQA"] = GenerateMockKernel(size, 0.78f);
    }

    float[,] GenerateMockKernel(int size, float intensity)
    {
        float[,] m = new float[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                m[i, j] = Mathf.Clamp01(Random.Range(0.0f, 1.0f) * intensity);
        return m;
    }

    int GetKernelSize()
    {
        if (kernelSizeDropdown != null && kernelSizeDropdown.options.Count > 0)
        {
            string selected = kernelSizeDropdown.options[kernelSizeDropdown.value].text;
            if (int.TryParse(selected, out int size)) return size;
        }
        return 8; // default
    }

    float[,] ApplyNoiseIfEnabled(float[,] input)
    {
        if (noiseModelDropdown == null || noiseModelDropdown.value == 0) return input;

        KernelNoiseSimulator.NoiseModel model = (KernelNoiseSimulator.NoiseModel)noiseModelDropdown.value;
        float strength = GetNoiseStrength();
        return KernelNoiseSimulator.ApplyNoise(input, model, strength);
    }

    float GetNoiseStrength() => noiseStrengthSlider ? noiseStrengthSlider.value : 0.1f;

    public void ApplyNoise()
    {
        VisualizeSelectedKernel();
    }

    public void VisualizeSelectedKernel()
    {
        if (kernelTypeDropdown.options.Count == 0) return;

        currentKernelName = kernelTypeDropdown.options[kernelTypeDropdown.value].text;
        float[,] baseKernel = sampleKernels[currentKernelName];
        originalKernel = baseKernel;

        float[,] kernelToRender = ApplyNoiseIfEnabled(baseKernel);

        int width = kernelToRender.GetLength(0);
        int height = kernelToRender.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float v = kernelToRender[x, y];
                texture.SetPixel(x, y, smearingToggle && smearingToggle.isOn ?
                    KernelNoiseSimulator.MapSmearingColor(v, GetNoiseStrength()) :
                    MapColor(v));
            }
        }

        texture.Apply();
        kernelHeatmap.texture = texture;
        metadataLabel.text = $"Kernel: {currentKernelName} | Qubits: {width} | Fidelity ≈ {ComputeFidelity(kernelToRender):F2}";
    }

    public void ImportAndLabel()
    {
        string label = string.IsNullOrEmpty(labelOverrideInput.text) ? "CustomKernel" : labelOverrideInput.text;
        float[,] matrix = KernelJsonLoader.LoadFromUserPath();
        if (matrix != null)
        {
            sampleKernels[label] = matrix;
            kernelTypeDropdown.AddOptions(new List<string> { label });
            currentKernelName = label;
            originalKernel = matrix;
            VisualizeSelectedKernel();

            if (previewNoiseToggle != null && previewNoiseToggle.isOn)
            {
                string model = noiseModelDropdown?.options[noiseModelDropdown.value].text ?? "None";
                float strength = GetNoiseStrength();
                statusLabel.text = $"✅ Imported with noise: {model} ({strength:F2})";
            }
            else
            {
                statusLabel.text = "📦 Loaded custom kernel without noise preview";
            }
        }
        else
        {
            statusLabel.text = "❌ Failed to load kernel";
        }
    }

    public void DisplayDeltaMatrix(float[,] delta)
    {
        int width = delta.GetLength(0);
        int height = delta.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.yellow, delta[x, y]));

        texture.Apply();
        kernelHeatmap.texture = texture;
        metadataLabel.text = $"Δ Kernel: {currentKernelName}";
    }

    public void RestoreOriginalKernel()
    {
        if (originalKernel != null)
            VisualizeKernel(currentKernelName);
    }

    void VisualizeKernel(string name)
    {
        if (!sampleKernels.ContainsKey(name)) return;
        currentKernelName = name;
        originalKernel = sampleKernels[name];
        VisualizeSelectedKernel();
    }

    public float[,] GetKernelMatrix(string name = null)
    {
        string key = name ?? currentKernelName;
        return sampleKernels.ContainsKey(key) ? sampleKernels[key] : null;
    }

    public List<string> GetAvailableKernelNames() => new(sampleKernels.Keys);

    float ComputeFidelity(float[,] m)
    {
        float total = 0f;
        foreach (float v in m)
            total += v;
        return total / (m.GetLength(0) * m.GetLength(1));
    }

    Color MapColor(float v)
    {
        return currentColorMap switch
        {
            "Hot" => new Color(v, v * 0.5f, 0),
            "Cool" => new Color(0, v, 1 - v),
            "Viridis" => new Color(0.2f + v * 0.6f, v, 0.4f + 0.2f * (1 - v)),
            _ => new Color(v, 0, 1 - v),
        };
    }

    public void SetKernel(float[,] kernel)
    {
        if (kernel == null) return;

        string label = "ImportedKernel_" + sampleKernels.Count;
        sampleKernels[label] = kernel;
        currentKernelName = label;
        originalKernel = kernel;

        if (kernelTypeDropdown != null)
            kernelTypeDropdown.AddOptions(new List<string> { label });

        VisualizeSelectedKernel();

        if (statusLabel != null)
            statusLabel.text = $"✅ Kernel manually set: {label}";
    }

}