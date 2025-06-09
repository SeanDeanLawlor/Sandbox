using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class OpenAIKeyPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent)
    {
        GameObject panel = new GameObject("OpenAIKeyPanel");
        panel.transform.SetParent(parent);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(320, 220);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // API Key input
        InputField keyInput = CreateInputField(panel.transform, new Vector2(0, 60), font, "Enter OpenAI API Key...");
        Toggle useOpenAI = CreateToggle(panel.transform, new Vector2(-80, 20), font, "Use OpenAI");

        // Save button
        Button saveBtn = CreateButton(panel.transform, new Vector2(-60, -30), font, "Save");
        saveBtn.onClick.AddListener(() => {
            File.WriteAllText(Application.dataPath + "/../Python/.env", $"OPENAI_API_KEY={keyInput.text}");
            PlayerPrefs.SetInt("UseOpenAI", useOpenAI.isOn ? 1 : 0);
            Debug.Log("✅ API key saved.");
        });

        // Clear button
        Button clearBtn = CreateButton(panel.transform, new Vector2(60, -30), font, "Clear");
        clearBtn.onClick.AddListener(() => {
            if (File.Exists(Application.dataPath + "/../Python/.env"))
                File.Delete(Application.dataPath + "/../Python/.env");
            keyInput.text = "";
            PlayerPrefs.SetInt("UseOpenAI", 0);
            useOpenAI.isOn = false;
            Debug.Log("🧼 API key cleared.");
        });

        return panel;
    }

    private InputField CreateInputField(Transform parent, Vector2 pos, Font font, string placeholder)
    {
        GameObject go = new GameObject("InputField");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(260, 30);

        InputField field = go.AddComponent<InputField>();

        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(go.transform);
        text.font = font;
        text.color = Color.white;
        field.textComponent = text;

        Text ph = new GameObject("Placeholder").AddComponent<Text>();
        ph.transform.SetParent(go.transform);
        ph.font = font;
        ph.text = placeholder;
        ph.color = Color.gray;
        field.placeholder = ph;

        return field;
    }

    private Toggle CreateToggle(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Toggle");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200, 30);

        Toggle toggle = go.AddComponent<Toggle>();
        Text lbl = new GameObject("Label").AddComponent<Text>();
        lbl.transform.SetParent(go.transform);
        lbl.font = font;
        lbl.text = label;
        lbl.color = Color.white;
        lbl.alignment = TextAnchor.MiddleLeft;

        return toggle;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(100, 30);

        Button btn = go.AddComponent<Button>();
        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(go.transform);
        btnText.font = font;
        btnText.text = label;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;

        return btn;
    }
}