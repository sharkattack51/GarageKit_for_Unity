using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * Manage scene state transitions
 */
namespace GarageKit
{
	[Serializable]
	public class SceneStateData
	{
		[SerializeField] private string stateName = "";
		public string StateName { get{ return stateName; } }

		[SerializeField] private StateBase stateObj = null;
		public StateBase StateObj { get{ return stateObj; } }

		[SerializeField] private bool asInitial = false;
		public bool AsInitial { get{ return asInitial; } }

		public SceneStateData(string stateName, StateBase stateObj, bool asInitial)
		{
			this.stateName = StateName;
			this.stateObj = stateObj;
			this.asInitial = asInitial;
		}
	}

	public class SceneStateManager : ManagerBase
	{
		[Header("Scnene States")]
		public List<SceneStateData> sceneStateTable;

		private SceneStateData currentState;
		public SceneStateData CurrentState { get{ return currentState; } }

		private string fromStateName = "";
		public string FromStateName { get{ return fromStateName; } }

		private bool stateChanging = false;
		public bool StateChanging { get{ return stateChanging; } }

		private bool isAsync = false;


		protected override void Awake()
		{
			base.Awake();

			foreach(SceneStateData state in sceneStateTable)
			{
				if(state.StateName == "" || state.StateObj == null)
					Debug.LogError("SceneStateManager :: StateName or StateObj is empty.");
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();

			// Display current state name
			this.gameObject.name = "SceneStateManager [" + currentState.StateName + "]";

			// Update curent state
			if(currentState.StateObj != null)
				currentState.StateObj.StateUpdate();
		}
		
		
#region Management for State
		public void InitState()
		{
			// Initial state
			SceneStateData state = sceneStateTable.Find((s) => { return s.AsInitial; });
			if(state == null)
				state = sceneStateTable[0];
			
			ChangeState(state.StateName);
		}

		public void ChangeState(string stateName, object context = null)
		{
			isAsync = false;

			if(currentState != null)
				fromStateName = currentState.StateName;

			StartCoroutine(ChangeStateCoroutine(stateName, context));
		}

		public void ChangeAsyncState(string stateName, object context = null)
		{
			if(currentState.StateObj is AsyncStateBase)
			{
				if(!stateChanging)
				{
					isAsync = true;
					stateChanging = true;
					
					if(currentState != null)
						fromStateName = currentState.StateName;

					StartCoroutine(ChangeStateCoroutine(stateName, context));
				}
			}
			else
			{
				Debug.LogWarning("SceneStateManager :: target state class is not AsyncStateBase. to transition using ChangeState().");
				ChangeState(stateName, context);
			}
		}

		private IEnumerator ChangeStateCoroutine(string stateName, object context)
		{
			// Exit previous state
			if(currentState != null)
				currentState.StateObj.StateExit();
			
			while(isAsync)
				yield return null;
			
			// Set new state
			currentState = sceneStateTable.Find((s) => { return s.StateName == stateName; });
			if(currentState == null)
			{
				Debug.LogError("SceneStateManager :: not found StateName.");
				yield break;
			}
			
			// Start new state
			currentState.StateObj.StateStart(context);

			stateChanging = false;
		}	
#endregion

		// for Async func
		public void SyncState()
		{
			isAsync = false;
		}
	}
}
