namespace CardGameArchive
{
    using UnityEngine;

    /// <summary>
    /// Handles all code related to managing Game settings
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;
        }
    }

}