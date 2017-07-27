using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// 
/// Administration manager.
/// 
/// Useful to manage users accounts
/// 
/// </summary>
public class AdministrationManager : ServerManager
{
	// UI components
	public UnityEngine.Object reportRowPrefab = null;
	private GameObject reportsGrid = null;
	// MESSAGE
	private GameObject displayMessage = null;
	private InputField displayMessageField = null;
	private RectTransform displayMessageRect = null;
	// SCREENSHOT
	private GameObject displayWindow = null;
	private RawImage displayWindowImage = null;
	private RectTransform displayWindowRect = null;
	// BAN
	private InputField usernameToBanField = null;

	// List of reports
	private List<Report> reports { get; set; }

	// Menus
	public string AdminLoginScene = "AdminLogin";

	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Verify we did logged in
		if(!UserSession.loggedIn)
		{
			UtilsProSecure.Load(AdminLoginScene);	// The connection is not established, go back to login menu
		}

		// Get UI components
		if (reportRowPrefab == null)
			reportRowPrefab = Resources.Load ("ReportRow");
		reportsGrid = GameObject.Find("ReportsGrid");
		// Message
		displayMessage = GameObject.Find ("DisplayMessage");
		displayMessageField = displayMessage.GetComponentInChildren<InputField>();
		displayMessageRect = displayMessage.GetComponent<RectTransform>();
		// Screenshot
		displayWindow = GameObject.Find ("DisplayWindow");
		displayWindowImage = displayWindow.GetComponentInChildren<RawImage>();
		displayWindowRect = displayWindow.GetComponent<RectTransform>();
		// Ban
		usernameToBanField = GameObject.Find ("UsernameToBan").GetComponentInChildren<InputField>();

		// Load screen data
		RefreshReports();
	}

	private void ShowScreenshotWindow()
	{
		displayWindowRect.localScale = Vector3.one;
	}
	private void ShowMessageWindow()
	{
		displayMessageRect.localScale = Vector3.one;
	}


	///////////////////////////////// - CALLBACKS EVENTS on UI components - ///////////////////////////////////////////
	public void RefreshReportsLaunched()
	{
		RefreshReports();
	}
	public void HideScreenshotWindow()
	{
		displayWindowRect.localScale = Vector3.zero;
	}
	public void HideMessageWindow()
	{
		displayMessageRect.localScale = Vector3.zero;
	}
	public void ShowScreenshot(string reportId)
	{
		GetScreenshot (reportId);
	}
	private void DisplayScreenshot(string screenshotString)
	{
		byte[] screenshotBytes = Convert.FromBase64String(screenshotString);
		Texture2D tex = new Texture2D(862,415);
		tex.LoadImage(screenshotBytes);
		displayWindowImage.texture = tex;
		ShowScreenshotWindow ();
	}

	public void ShowMessage(string message)
	{
		displayMessageField.text = message;
		ShowMessageWindow ();
	}

	public void SaveLaunched()
	{
		SaveReports ();
	}

	public void SetRemoveFlag(string id, Image reportRowImage)
	{
		Color red = new Color (1, 0, 0.16f);
		Color grey = new Color (1, 1, 1, 0.3921f);

		foreach(Report r in reports)
		{
			if(r.Id == id)
			{
				r.ToBeRemoved = !r.ToBeRemoved;
				reportRowImage.color = r.ToBeRemoved ? red : grey;
				break;
			}
		}
	}

	public void BanLaunched()
	{
		BanUser ();
	}
	
	
	
	///////////////////////////////// - Refresh reports - ///////////////////////////////////////////
	private void RefreshReports()
	{
		// Clear the reports list
		reports = new List<Report>();

		// Clear the report list
		Transform reportsGridTransform = reportsGrid.transform;
		foreach(Transform child in reportsGridTransform)
		{
			Destroy(child.gameObject);
		}

		// Get report list from server
		Send("Administration", handleAdministrationSuccess, handleAdministrationError);
	}
	private void handleAdministrationSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];		// Show server message

		int numberOfDataReceivedPerReport = 4;	// A report is constituted of 4 datas

		int dataLength = serverDatas.Length;	// Get the length of the array received
		for(int i=1; i+numberOfDataReceivedPerReport < dataLength; i += (numberOfDataReceivedPerReport+1))	// As long as there is datas -> create a report row
		{
			string reportId = serverDatas [i];
			string reportDate = serverDatas [i+1];
			string reporterUsername = serverDatas [i+2];
			string message = serverDatas [i+3];
			//string screenshot = serverDatas [i+4];
			string isDone = serverDatas [i+4];

			// Create the report row
			GameObject reportRow = (GameObject)Instantiate(reportRowPrefab);

			// Get buttons of the row to display information
			GameObject ReportId = reportRow.transform.Find("ReportId").gameObject;
			GameObject ReportDate = reportRow.transform.Find("ReportDate").gameObject;
			GameObject Reporter = reportRow.transform.Find("Reporter").gameObject;
			GameObject ShowMessage = reportRow.transform.Find("ShowMessage").gameObject;
			GameObject ShowScreenshot = reportRow.transform.Find("ShowScreenshot").gameObject;
			GameObject IsDone = reportRow.transform.Find("IsDone").gameObject;
			GameObject Remove = reportRow.transform.Find("Remove").gameObject;

			// Check if all gameObject are found in the prefab : otherwise show some errors
			if(ReportId == null) Debug.LogError("The ReportRow prefab must contain a ReportId child.");
			if(ReportDate == null) Debug.LogError("The ReportRow prefab must contain a ReportDate child.");
			if(Reporter == null) Debug.LogError("The ReportRow prefab must contain a Reporter child.");
			if(ShowMessage == null) Debug.LogError("The ReportRow prefab must contain a ShowMessage child.");
			if(ShowScreenshot == null) Debug.LogError("The ReportRow prefab must contain a ShowScreenshot child.");
			if(IsDone == null) Debug.LogError("The ReportRow prefab must contain a IsDone child.");

			// Attach action on button clicks
			Button ShowMessageButton = ShowMessage.GetComponent<Button>();
			ShowMessageButton.onClick.AddListener (() => this.ShowMessage(message));

			Button ShowScreenshotButton = ShowScreenshot.GetComponent<Button>();
			ShowScreenshotButton.onClick.AddListener (() => this.ShowScreenshot(reportId));

			Button DeleteButton = Remove.GetComponent<Button>();
			DeleteButton.onClick.AddListener (() => this.SetRemoveFlag(reportId, reportRow.GetComponent<Image>()));

			// Affect values to it
			ReportId.transform.GetChild(0).GetComponent<Text> ().text = reportId;
			ReportDate.transform.GetChild(0).GetComponent<Text> ().text = reportDate;
			Reporter.transform.GetChild(0).GetComponent<Text> ().text = reporterUsername;
			Toggle isDoneCheckbox = IsDone.GetComponent<Toggle>();
			isDoneCheckbox.isOn = isDone=="True";

			// Insert it in the report list
			Report report = new Report(reportId, isDoneCheckbox);
			reportRow.transform.SetParent(reportsGrid.transform);
			reports.Add(report);
		}
	}
	private void handleAdministrationError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}


	///////////////////////////////// - Get screenshot - ///////////////////////////////////////////
	private void GetScreenshot(string reportId)
	{
		string[] datas = new string[1];
		datas[0] = reportId;
		Send("GetScreenshot", handleGetScreenshotSuccess, handleGetScreenshotError, datas);
	}
	private void handleGetScreenshotSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];		// Show server message
		DisplayScreenshot(serverDatas[1]);		// Display the screenshot on screen
	}
	private void handleGetScreenshotError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}



	///////////////////////////////// - Save reports - ///////////////////////////////////////////
	private void SaveReports()
	{
		string[] datas = new string[reports.Count * 3];

		int index = 0;
		foreach(Report r in reports)
		{
			datas [index] = r.Id;
			datas [index+1] = r.IsDoneCheckbox.isOn.ToString();
			datas [index+2] = r.ToBeRemoved.ToString();
			index += 3;
		}

		Send("SaveAdministration", handleSaveSuccess, handleSaveError, datas);
	}
	private void handleSaveSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];		// Show server message
		RefreshReports();						// Refresh report list
	}
	private void handleSaveError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; 		// Show the server's message
	}




	///////////////////////////////// - Ban user - ///////////////////////////////////////////
	private void BanUser()
	{
		string[] datas = new string[1];
		datas [0] = usernameToBanField.text;
		Send("BanUser", handleBanSuccess, handleBanError, datas);
	}
	private void handleBanSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];		// Show server message
	}
	private void handleBanError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; 		// Show the server's message
	}
}
