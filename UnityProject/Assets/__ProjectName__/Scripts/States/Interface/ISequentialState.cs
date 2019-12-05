using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public interface ISequentialState
	{
		void ToNextState();
		void ToPrevState();
	}
}
