using UnityEngine;	
public class LanguageMenuController : MonoBehaviour
{
	[Header("UI Panel")]
	public GameObject languageMenuPanel;
	
	private bool isOpen = false;
	
	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        ToggleMenu();
	    }
	}
	
	public void ToggleMenu()
	{
	    isOpen = !isOpen;
	    languageMenuPanel.SetActive(isOpen);
	    PauseGame(isOpen);
	}
	
	public void CloseMenu()
	{
	    isOpen = false;
	    languageMenuPanel.SetActive(false);
	    PauseGame(false);
	}
	
	private void PauseGame(bool pause)
	{
	    Time.timeScale = pause ? 0f : 1f;
	}
}