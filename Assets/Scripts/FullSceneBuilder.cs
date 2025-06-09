// FullSceneBuilder.cs (Updated to fix BuildPanel signature and match KernelLoaderUI)
using UnityEngine;
using UnityEngine.UI;

public class FullSceneBuilder : MonoBehaviour
{
    void Start()
    {
        // === Create or Find Canvas ===
        Canvas canvas = FindObjectOfType<Canvas>();
        if (!canvas)
        {
            GameObject canvasGO = new GameObject("MainCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === Ensure Required Core Components ===
        KernelLoaderUI kernelLoaderUI = CreateOrGet<KernelLoaderUI>("KernelLoaderUI");
        ZXDiagramJsonLoader zxLoader = CreateOrGet<ZXDiagramJsonLoader>("ZXDiagramJsonLoader");
        ZXRewriter zxRewriter = CreateOrGet<ZXRewriter>("ZXRewriter");

        // === Kernel Panel ===
        var kernelLoaderGO = new GameObject("KernelLoaderPanelBuilder");
        var kernelLoaderBuilder = kernelLoaderGO.AddComponent<KernelLoaderPanelBuilder>();
        var kernelPanel = kernelLoaderBuilder.BuildPanel(canvas.transform, kernelLoaderUI);
        kernelPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 250);

        // === ZX Diagram Panel ===
        var zxBuilderGO = new GameObject("ZXDiagramPanelBuilder");
        var zxBuilder = zxBuilderGO.AddComponent<ZXDiagramPanelBuilder>();
        var zxPanel = zxBuilder.BuildPanel(canvas.transform, zxLoader);
        zxPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, -150);

        // === ZX Rewriter Panel ===
        var rewriterGO = new GameObject("ZXRewriterPanelBuilder");
        var rewriterBuilder = rewriterGO.AddComponent<ZXRewriterPanelBuilder>();
        var rewriterPanel = rewriterBuilder.BuildPanel(canvas.transform, zxRewriter);
        rewriterPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, 250);

        // === Snapshot Timeline Panel ===
        var timelineGO = new GameObject("SnapshotTimelinePanelBuilder");
        var timelineBuilder = timelineGO.AddComponent<SnapshotTimelinePanelBuilder>();
        var timelinePanel = timelineBuilder.BuildPanel(canvas.transform);
        timelinePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, -150);

        Debug.Log("✅ Full scene setup complete.");
    }

    T CreateOrGet<T>(string name) where T : Component
    {
        T existing = FindObjectOfType<T>();
        if (existing != null) return existing;

        GameObject go = new GameObject(name);
        return go.AddComponent<T>();
    }
}