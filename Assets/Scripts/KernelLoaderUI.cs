using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class KernelLoaderUI : MonoBehaviour, IDropHandler
{
    public InputField manualPathInput;
    public Text feedbackText;
    public Dropdown colorMapDropdown;
    public KernelExplorer kernelExplorer;
    public Text statusLabel;

    void Start()
    {
        colorMapDropdown.onValueChanged.AddListener(OnColorMapChanged);
    }

    public void LoadFromManualPath()
    {
        if (manualPathInput == null || string.IsNullOrEmpty(manualPathInput.text))
        {
            SetFeedback("⚠️ No path provided.");
            return;
        }

        string path = manualPathInput.text;
        if (!File.Exists(path))
        {
            SetFeedback("❌ File does not exist: " + path);
            return;
        }

        float[,] kernel = KernelJsonLoader.LoadFromFile(path);
        if (kernel != null)
        {
            kernelExplorer.SetKernel(kernel);
            SetFeedback("📂 Loaded from: " + path);
        }
    }

    public void LoadBundledSample()
    {
        string path = Application.dataPath + "/ExampleKernels/example_kernel.json";
        if (!File.Exists(path))
        {
            SetFeedback("⚠️ Sample kernel not found in: " + path);
            return;
        }

        float[,] kernel = KernelJsonLoader.LoadFromFile(path);
        if (kernel != null)
        {
            manualPathInput.text = path;
            kernelExplorer.SetKernel(kernel);
            SetFeedback("📦 Loaded sample kernel from project!");
        }
    }

    public void LoadFromFileDialog()
    {
        float[,] kernel = KernelJsonLoader.LoadFromUserPath();
        if (kernel != null)
        {
            kernelExplorer.SetKernel(kernel);
            SetFeedback("📁 Loaded kernel from file dialog.");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            string droppedPath = eventData.pointerDrag.name;
            if (!File.Exists(droppedPath)) return;

            float[,] kernel = KernelJsonLoader.LoadFromFile(droppedPath);
            if (kernel != null)
            {
                manualPathInput.text = droppedPath;
                kernelExplorer.SetKernel(kernel);
                SetFeedback("🖱️ Dropped and loaded: " + droppedPath);
            }
        }
    }

    public void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    void OnColorMapChanged(int index)
    {
        if (kernelExplorer != null)
        {
            kernelExplorer.colorMapDropdown.value = index;
            kernelExplorer.VisualizeSelectedKernel();
        }
    }
}