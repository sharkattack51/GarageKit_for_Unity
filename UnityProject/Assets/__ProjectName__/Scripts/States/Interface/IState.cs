using UnityEngine;
using System.Collections;

/*
 * State用のインターフェース
 */
namespace GarageKit
{
	public interface IState
	{
		void StateStart(object context);
		void StateUpdate();
		void StateExit();
	}
}