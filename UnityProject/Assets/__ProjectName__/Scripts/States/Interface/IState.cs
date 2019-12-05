using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public interface IState
	{
		void StateStart(object context);
		void StateUpdate();
		void StateExit();
	}
}