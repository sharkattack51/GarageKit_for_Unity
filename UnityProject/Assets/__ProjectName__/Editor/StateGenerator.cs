using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class StateInfo
{
	public string className = "";
	public string sourcePath = "";
	public StateInfo.BaseClass baseClass = StateInfo.BaseClass.StateBase;

	public enum BaseClass
	{
		StateBase = 0,
		AsyncStateBase
	}

	public StateInfo() {}
}

public class StateGenerator : EditorWindow
{ 
	private static StateGenerator window;
	private static bool modified = false;

	private static Dictionary<string, string> managedPath;
	private static List<StateInfo> currentStateInfos;
	private static StateInfo newStateInfo;

	private enum GENERATE_MODE
	{
		ADD = 0,
		DELETE
	}

	public const string SOURCE_STATECLASS_ORG
= @"using UnityEngine;
using System.Collections;

public class {0} : {1}
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
";

	[MenuItem("EditorScript/StateGenerator")]
	public static void WindowOpen()
	{
		Init();

		window = EditorWindow.GetWindow<StateGenerator>("SateGen");
	}

	void OnGUI()
	{
		if(modified)
			return;

		if(currentStateInfos == null)
			return;
		
		EditorWindow.GetWindow<StateGenerator>().maximized = false;
		EditorWindow.GetWindow<StateGenerator>().minSize = new Vector2(430, 250);

		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.LabelField("Manage scene state. Add and delete source files.");
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical();
			{
				// Colum名
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Enum", GUILayout.Width(50.0f));
					EditorGUILayout.LabelField("Class Name", GUILayout.Width(150.0f));
					EditorGUILayout.LabelField("Base Class", GUILayout.Width(150.0f));
					EditorGUILayout.LabelField("", GUILayout.Width(50.0f));
				}
				EditorGUILayout.EndHorizontal();

				// 各State表示
				int ct = 0;
				List<StateInfo> infos = new List<StateInfo>(currentStateInfos.ToArray());
				foreach(StateInfo state in infos)
				{
					if(state == null)
						break;

					EditorGUILayout.BeginHorizontal(GUI.skin.box);
					{
						EditorGUILayout.LabelField(ct.ToString(), GUILayout.Width(50.0f));
						EditorGUILayout.LabelField(state.className, GUILayout.Width(150.0f));
						EditorGUILayout.LabelField(state.baseClass.ToString(), GUILayout.Width(150.0f));

						if(ct == 0)
							EditorGUILayout.LabelField("", GUILayout.Width(50.0f));
						else
						{
							if(GUILayout.Button("Del", GUILayout.Width(50.0f)))
							{
								modified = true;
								DeleteSource(state);
							}
						}
					}
					EditorGUILayout.EndHorizontal();

					ct++;
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("New Class");

			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			{
				EditorGUILayout.LabelField("", GUILayout.Width(50.0f));
				newStateInfo.className = EditorGUILayout.TextField(newStateInfo.className, GUILayout.Width(150.0f));
				newStateInfo.baseClass = (StateInfo.BaseClass)EditorGUILayout.EnumPopup(newStateInfo.baseClass, GUILayout.Width(150.0f));

				if(GUILayout.Button("Add", GUILayout.Width(50.0f)))
				{
					modified = true;
					AddNewSource(newStateInfo);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private static void Init()
	{
		managedPath = new Dictionary<string, string>();
		managedPath.Add("Scripts/Manager/SceneStateManager.cs", Path.GetFullPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SceneStateManager")[0])));
		managedPath.Add("Scripts/States", managedPath["Scripts/Manager/SceneStateManager.cs"].Split(new string[]{"Scripts"}, StringSplitOptions.None)[0] + "Scripts/States");

		// 既存Stateクラスファイルの確認
		string[] state_files = Directory.GetFiles(managedPath["Scripts/States"], "*State.cs");
		currentStateInfos = new List<StateInfo>();

		// enum順でリストを作成
		foreach(string state_name in Enum.GetNames(typeof(SceneStateManager.SceneState)))
		{
			foreach (string path in state_files)
			{
				if(path.ToLower().Contains(state_name.ToLower()))
				{
					StateInfo state = new StateInfo();
					state.className = Path.GetFileName(path).Split(new string[]{ ".cs" }, StringSplitOptions.None)[0];
					state.sourcePath = path;
					state.baseClass = StateInfo.BaseClass.StateBase;
					if(File.ReadAllText(path).Contains("AsyncStateBase"))
						state.baseClass = StateInfo.BaseClass.AsyncStateBase;

					currentStateInfos.Add(state);

					break;
				}
			}
		}

		newStateInfo = new StateInfo();
	}

	private static void AddNewSource(StateInfo state)
	{
		if(state.className == "")
			return;

		// 新規Stateファイル作成
		state.className = state.className.Substring(0, 1).ToUpper() + state.className.Substring(1, state.className.Length - 1);
		if(state.className.Length > 5)
		{
			if(state.className.Substring(state.className.Length - 5, 5) != "State")
				state.className += "State";
		}
		else
			state.className += "State";
		
		state.sourcePath = Path.Combine(managedPath["Scripts/States"], state.className + ".cs");

		currentStateInfos.Add(state);
		UpdateManagerSource(GENERATE_MODE.ADD);

		using(FileStream fs = File.Create(state.sourcePath))
		{
			using(StreamWriter sw = new StreamWriter(fs))
			{
				string src = SOURCE_STATECLASS_ORG.Replace("{0}", state.className).Replace("{1}", state.baseClass.ToString());
				sw.Write(src);
			}
		}
		AssetDatabase.Refresh();

		window.Close();
	}

	private static void DeleteSource(StateInfo state)
	{
		currentStateInfos.Remove(state);
		UpdateManagerSource(GENERATE_MODE.DELETE);

		File.Delete(state.sourcePath);
		AssetDatabase.Refresh();

		window.Close();
	}

	// マネージャーのソース更新
	private static void UpdateManagerSource(GENERATE_MODE mode)
	{
		string src = File.ReadAllText(managedPath["Scripts/Manager/SceneStateManager.cs"]);
		StringSplitOptions none = StringSplitOptions.None;

		string src_pre = src.Split(new string[]{ "\t{" }, 2, none)[0];
		string src_enm = src.Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t}" }, 2, none)[0];
		string src_mid = src.Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t}" }, 2, none)[1].Split(new string[]{ "\t{" }, 2, none)[0];
		string src_tbl = src.Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t}" }, 2, none)[1].Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t};" }, 2, none)[0];
		string src_pst = src.Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t}" }, 2, none)[1].Split(new string[]{ "\t{" }, 2, none)[1].Split(new string[]{ "\t};" }, 2, none)[1];
		
		// enum行の配列
		List<string> list_enm = new List<string>(src_enm.Trim().Split(new string[]{ "\n" }, none));
		if(list_enm.Count == 1)
			list_enm = new List<string>(src_enm.Trim().Split(new string[]{ "\r\n" }, none));
		for(int i = 0; i < list_enm.Count; i++)
			list_enm[i] = list_enm[i].Trim();
		
		if(mode == GENERATE_MODE.ADD)
		{
			foreach(StateInfo state in currentStateInfos)
			{
				string name = state.className.ToUpper().Replace("STATE", "");

				string contain = "";
				foreach(string s in list_enm)
				{
					if(s.Contains(name))
					{
						contain = s;
						break;
					}
				}

				if(contain == "")
					list_enm.Add(name + ",");
			}
		}
		else if(mode == GENERATE_MODE.DELETE)
		{
			List<string> tmp_list_enm = new List<string>();
			foreach(StateInfo state in currentStateInfos)
			{
				string name = state.className.ToUpper().Replace("STATE", "");

				string contain = "";
				foreach(string s in list_enm)
				{
					if(s.Contains(name))
					{
						contain = s;
						break;
					}
				}

				if(contain != "")
					tmp_list_enm.Add(contain);
			}

			list_enm = new List<string>();
			foreach(string e in tmp_list_enm)
				list_enm.Add(e);
		}

		// table行の配列
		List<string> list_tbl = new List<string>(src_tbl.Trim().Split(new string[]{ "\r\n" }, none));
		if(list_tbl.Count == 1)
			list_tbl = new List<string>(src_tbl.Trim().Split(new string[]{ "\n" }, none));
		for(int i = 0; i < list_tbl.Count; i++)
			list_tbl[i] = list_tbl[i].Trim();

		if(mode == GENERATE_MODE.ADD)
		{
			foreach(StateInfo state in currentStateInfos)
			{
				string name = state.className.ToUpper().Replace("STATE", "");

				string contain = "";
				foreach(string s in list_tbl)
				{
					if(s.Contains(name))
					{
						contain = s;
						break;
					}
				}

				if(contain == "")
					list_tbl.Add("{ SceneState." + name + ", typeof(" + state.className + ") },");
			}
		}
		else if(mode == GENERATE_MODE.DELETE)
		{
			List<string> tmp_list_tbl = new List<string>();
			foreach(StateInfo state in currentStateInfos)
			{
				string name = state.className.ToUpper().Replace("STATE", "");

				string contain = "";
				foreach(string s in list_tbl)
				{
					if(s.Contains(name))
					{
						contain = s;
						break;
					}
				}

				if(contain != "")
					tmp_list_tbl.Add(contain);
			}

			list_tbl = new List<string>();
			foreach(string t in tmp_list_tbl)
				list_tbl.Add(t);
		}
		
		// 内容更新
		string src_out = "";
		src_out += src_pre;
		src_out += "\t{\n";
		foreach(string e in list_enm)
			src_out += "\t\t" + e + "\n";
		src_out += "\t}";
		src_out += src_mid;
		src_out += "\t{\n";
		foreach(string t in list_tbl)
			src_out += "\t\t" + t + "\n";
		src_out += "\t};";
		src_out += src_pst;

		// 書き込み
		using(FileStream fs = File.Open(managedPath["Scripts/Manager/SceneStateManager.cs"], FileMode.Create))
		{
			using(StreamWriter sw = new StreamWriter(fs))
			{
				sw.Write("");
				sw.Write(src_out);
			}
		}
	}
}
