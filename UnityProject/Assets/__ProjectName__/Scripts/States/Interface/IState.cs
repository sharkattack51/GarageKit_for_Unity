using UnityEngine;
using System.Collections;

/*
 * State用のインターフェース
 */
public interface IState
{
	void StateStart(object context);
	void StateUpdate();
	void StateExit();
}