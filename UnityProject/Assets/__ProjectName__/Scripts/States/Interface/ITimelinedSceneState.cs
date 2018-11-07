using UnityEngine;
using System.Collections;

/*
 * VRSceneState用のインターフェース
 */
namespace GarageKit
{
	public interface ITimelinedSceneState
	{
		void Pause();
		void Resume();
		void OnStateTimer(GameObject sender);
	}
}