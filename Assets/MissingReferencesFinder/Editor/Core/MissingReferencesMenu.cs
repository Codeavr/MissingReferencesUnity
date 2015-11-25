using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu items for "Find missing references"
/// </summary>
public static class MissingReferencesMenu
{
	private const string MENU_ROOT = "Tools/Missing References/";

	/// <summary>
	/// Finds all missing references to objects in the currently loaded scene.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene()
	{
		MissingReferencesFinder missingRefs = new MissingReferencesFinder();
		var results = missingRefs.FindMissingReferences(EditorApplication.currentScene, MissingReferencesFinder.GetSceneObjects());

		FindMissingReferencesWindow.InitWithResults(results);
	}

	/// <summary>
	/// Finds all missing references to objects in all enabled scenes in the project.
	/// This works by loading the scenes one by one and checking for missing object references.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in all scenes", false, 51)]
	public static void MissingSpritesInAllScenes()
	{
		foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
		{
			EditorApplication.OpenScene(scene.path);
			FindMissingReferencesInCurrentScene();
		}
	}

	/// <summary>
	/// Finds all missing references to objects in assets (objects from the project window).
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in assets", false, 52)]
	public static void MissingSpritesInAssets()
	{
		MissingReferencesFinder missingRefs = new MissingReferencesFinder();

		var allAssetPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
		var objects = allAssetPaths.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

		var results = missingRefs.FindMissingReferences("Project", objects);
	}
}