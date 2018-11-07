using UnityEngine;
using System.Collections;

/*
 * State用のインターフェース
 */
namespace GarageKit
{
	public interface IAsyncState
	{
		void StateExitAsync();
	}
}