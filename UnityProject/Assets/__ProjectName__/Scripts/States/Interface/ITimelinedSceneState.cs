using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public interface ITimelinedSceneState
	{
		void Pause();
		void Resume();
		void OnStateTimer(GameObject sender);
	}
}