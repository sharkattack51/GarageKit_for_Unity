using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public class StateBase : IStateBehaviour
	{
		public override void StateStart(object context)
		{
			base.StateStart(context);
		}

		public override void StateUpdate()
		{
			base.StateUpdate();
		}

		public override void StateExit()
		{
			base.StateExit();
		}
	}
}
