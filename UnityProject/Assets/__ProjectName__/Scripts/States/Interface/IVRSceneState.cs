using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public interface IVRSceneState
	{
		void ResetCurrentState();
		void SetStagingObjects();
	}
}
