using UnityEngine;
using System.Collections;

public class MissingReferenceDrawer : IUIListItem
{
	public MissingReferenceDrawer (MissingReferenceResult data)
	{
	}

	#region IUIListItem implementation

	public void OnDrawGUI ()
	{
		// TODO: draw the stored MissingReferenceResult

		throw new System.NotImplementedException ();
	}

	#endregion
}