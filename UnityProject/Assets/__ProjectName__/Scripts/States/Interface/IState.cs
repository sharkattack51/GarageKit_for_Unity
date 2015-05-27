using UnityEngine;
using System.Collections;

/*
 * State用のインターフェース
 */
public interface IState
{
	void StateStart();
	void StateUpdate();
	void StateExit();
}