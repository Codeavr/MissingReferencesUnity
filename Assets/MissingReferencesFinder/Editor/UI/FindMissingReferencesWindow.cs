using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindMissingReferencesWindow : EditorWindow
{
	private const string TITLE = "Missing Refs";

	private UIList<MissingReferenceDrawer> contents;

	private static List<MissingReferenceResult> results;

	public static void Init()
	{
		var window = EditorWindow.GetWindow<FindMissingReferencesWindow>(false, TITLE, true);

		// TODO: Center window on screen.

		// Show as a popup
		window.ShowPopup();
	}

	public static void InitWithResults(List<MissingReferenceResult> results)
	{
		Debug.Log("Initialized with: " + results.Count);

		FindMissingReferencesWindow.results = results;

		Init();
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("Results");
		EditorGUILayout.Space();

		foreach (var i in results)
		{
			EditorGUILayout.LabelField(string.Format("Result: {0} Path: {1}", i.Name, i.Path));
			EditorGUILayout.Space();
		}
	}
}