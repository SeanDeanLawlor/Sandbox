// LicenseKeyManager: license activation logic
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LicenseKeyManager : MonoBehaviour
{
    public InputField licenseInput;
    public Text statusText;

    // Pro-only UI or features that will be toggled
    public GameObject[] proFeatureObjects;

    // Valid license keys
    public string[] acceptedKeys = { "SANDBOX-MASTER-KEY", "SANDBOX-PRO-2025" };

    public bool IsProActivated { get; private set; } = false;

    public void CheckLicense()
    {
        string entered = licenseInput.text.Trim();

        foreach (string key in acceptedKeys)
        {
            if (entered == key)
            {
                IsProActivated = true;
                statusText.text = "✅ Pro Features Activated";
                statusText.color = Color.green;
                ApplyProFeatureAccess();
                return;
            }
        }

        IsProActivated = false;
        statusText.text = "❌ Invalid License Key";
        statusText.color = Color.red;
        ApplyProFeatureAccess();
    }

    public void ApplyProFeatureAccess()
    {
        foreach (GameObject obj in proFeatureObjects)
        {
            if (obj != null)
                obj.SetActive(IsProActivated);
        }
    }

    public void AutoLoadLicenseFromFile(string path = "license.key")
    {
        if (File.Exists(path))
        {
            licenseInput.text = File.ReadAllText(path).Trim();
            CheckLicense();
        }
    }
}