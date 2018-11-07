using UnityEngine;
using System.Collections;

/*
 * VRSceneState用のインターフェース
 */
namespace GarageKit
{
	public interface IVRSceneState
	{
		void ToNextState();
		void ToPrevState();
		void ResetCurrentState();
		void SetStagingObjects();
	}
}
