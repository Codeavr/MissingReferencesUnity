using System;
using System.Collections.Generic;

public interface IUIListItem
{
	/// <summary>>
	/// Draw this list item to the GUI.
	/// </summary>
	void OnDrawGUI();
}

/// <summary>
/// An IMGUI implementation of a generic List.
/// </summary>
public class UIList<T> where T : IUIListItem
{
	public event Action<T> OnSelected;

	// TODO: events: Clicked, DoubleClicked, KeyDown, KeyUp, KeyPress, KeyHold

	private List<T> itemList;

	/// <summary>
	/// Draws the list's GUI to the screen.
	/// </summary>
	public void DrawGUI()
	{
	}
}