using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public interface IResult
{
	/// <summary>
	/// Returns the display name of this instance.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Returns the full path of this instance.
	/// </summary>
	string Path { get; }

	/// <summary>
	/// Selects this instance (either in the scene or project view).
	/// NOTE: this may cause a scene loading, in case the object originates from another scene.
	/// </summary>
	void Select();
}

public class MissingReferenceResult : IResult
{
	private string origin;

	public MissingReferenceResult (string name, string path)
	{
		this.name = name;
		this.path = path;
	}

	private string name;
	private string path;

	#region IResult implementation

	public void Select ()
	{
		throw new System.NotImplementedException ();
	}

	public string Name {
		get {
			return name;
		}
	}

	public string Path {
		get {
			return path;
		}
	}

	#endregion
}

/// <summary>
/// A helper editor script for finding missing references to objects.
/// </summary>
public class MissingReferencesFinder : MonoBehaviour 
{
	/// </summary>
	private const string MENU_ROOT = "Tools/Missing References/";

	/// <summary>
	/// Finds all missing references to objects in the currently loaded scene.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene()
	{
		var sceneObjects = GetSceneObjects();
		var results = FindMissingReferences(EditorApplication.currentScene, sceneObjects);

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
		var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
		var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();
		
		var results = FindMissingReferences("Project", objs);
	}

	private static List<MissingReferenceResult> FindMissingReferences(string context, GameObject[] objects)
	{
		List<MissingReferenceResult> result = new List<MissingReferenceResult>();

		foreach (var go in objects)
		{
			var components = go.GetComponents<Component>();
			
			foreach (var c in components)
			{
				// Missing components will be null, we can't find their type, etc.
				if (!c)
				{
//					Debug.LogError("Missing Component in GO: " + GetFullPath(go), go);

					result.Add(new MissingReferenceResult(go.name, GetFullPath(go)));
					continue;
				}
				
				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();

				// Iterate over the components' properties.
				while (sp.NextVisible(true))
				{
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == null
						    && sp.objectReferenceInstanceIDValue != 0)
						{
//							ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));

							result.Add(new MissingReferenceResult(go.name, ObjectNames.NicifyVariableName(sp.name)));
						}
					}
				}
			}
		}

		return result;
	}

	private static GameObject[] GetSceneObjects()
	{
		// Use this method since GameObject.FindObjectsOfType will not return disabled objects.
		return Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
			       && go.hideFlags == HideFlags.None).ToArray();
	}
		
	private static void ShowError (string context, GameObject go, string componentName, string propertyName)
	{
		var ERROR_TEMPLATE = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

		Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
	}

	/// <summary>
	/// Return a game object's full path as a string
	/// </summary>
	private static string GetFullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}
}