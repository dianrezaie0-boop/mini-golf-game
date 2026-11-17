using UnityEngine;

public class AboutPanel : MonoBehaviour
{
    public GameObject aboutPanel;

    public void OpenAbout()
    {
        aboutPanel.SetActive(true);
    }

    public void CloseAbout()
    {
        aboutPanel.SetActive(false);
    }
}