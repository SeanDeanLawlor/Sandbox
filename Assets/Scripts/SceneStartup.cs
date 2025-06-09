// SceneStartup.cs (Auto-instantiating version, no inspector linking needed)
using UnityEngine;
using UnityEngine.UI;

public class SceneStartup : MonoBehaviour
{
    void Start()
    {
        // === Create Canvas ===
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        Transform parent = canvasGO.transform;

        // === Kernel Loader UI ===
        GameObject kernelHost = new GameObject("KernelLoaderUIHost");
        kernelHost.transform.SetParent(parent);
        var kernelUI = kernelHost.AddComponent<KernelLoaderUI>();

        GameObject kernelBuilderGO = new GameObject("KernelLoaderPanelBuilder");
        var kernelBuilder = kernelBuilderGO.AddComponent<KernelLoaderPanelBuilder>();
        var kernelPanel = kernelBuilder.BuildPanel(parent, kernelUI);
        kernelPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 250);

        // === ZX Diagram Viewer ===
        GameObject zxLoaderGO = new GameObject("ZXDiagramJsonLoader");
        zxLoaderGO.transform.SetParent(parent);
        var zxLoader = zxLoaderGO.AddComponent<ZXDiagramJsonLoader>();

        GameObject zxViewerGO = new GameObject("ZXDiagramViewer");
        zxViewerGO.transform.SetParent(parent);
        var zxViewer = zxViewerGO.AddComponent<ZXDiagramViewer>();
        zxLoader.viewer = zxViewer; // ✅ REQUIRED LINE

        GameObject zxBuilderGO = new GameObject("ZXDiagramPanelBuilder");
        var zxBuilder = zxBuilderGO.AddComponent<ZXDiagramPanelBuilder>();
        var zxPanel = zxBuilder.BuildPanel(parent, zxLoader);
        zxPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, -150);

        // === ZX Rewriter Panel ===
        GameObject rewriterGO = new GameObject("ZXRewriter");
        rewriterGO.transform.SetParent(parent);
        var zxRewriter = rewriterGO.AddComponent<ZXRewriter>();

        GameObject rwBuilderGO = new GameObject("ZXRewriterPanelBuilder");
        var rwBuilder = rwBuilderGO.AddComponent<ZXRewriterPanelBuilder>();
        var rwPanel = rwBuilder.BuildPanel(parent, zxRewriter);
        rwPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, 250);

        // === Snapshot Timeline Panel ===
        GameObject snapBuilderGO = new GameObject("SnapshotTimelinePanelBuilder");
        var snapBuilder = snapBuilderGO.AddComponent<SnapshotTimelinePanelBuilder>();
        var snapPanel = snapBuilder.BuildPanel(parent);
        snapPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, -150);

        Debug.Log("✅ All UI components created and positioned programmatically.");
    }
}
