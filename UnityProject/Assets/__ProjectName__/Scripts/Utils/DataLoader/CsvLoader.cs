using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using System;

/*
 * Csvの読み込みクラス
 */
 
public class CsvLoader : MonoBehaviour 
{
	//singleton
	private static CsvLoader instance;
	public static CsvLoader Instance { get{ return instance; } }
	
	private bool isVaild = false;
	public bool IsVaild { get{ return isVaild; } }
	
	//csvファイルパス
	public string csvUrl;
	
	//自動読み込み
	public bool autoLoad = true;
	
	//csvデータを2次元配列で展開
	private string[,] csvGrid = new string[0, 0];
	public string[,] CsvGrid { get{ return csvGrid; } }
	
	private string csvString = "";
	
	
	void Awake()
	{
		//データ取得用のインスタンス
		instance = this;
		
		//自動読み込み
		if(autoLoad)
			LoadCSV();
	}
	
	void Start()
	{
		//DebugLogGrid(csvGrid);
	}
	
	
#region Load
	
	public void LoadCSV(string csvUrl)
	{
		this.csvUrl = csvUrl;
		
		LoadCSV();
	}
	
	public void LoadCSV()
	{
		//クリア
		csvGrid = new string[0, 0];
		
		try
		{
			//csv読み込み			
			StreamReader strem = new StreamReader(csvUrl, Encoding.UTF8);
			csvString = strem.ReadToEnd();
			strem.Close();
			
			//csvをパース
			ParseCSV();
			
			isVaild = true;
		}
		catch(Exception err)
		{
			Debug.Log(err.Message);
			
			isVaild = true;
		}
	}
	
#endregion
	
	private void ParseCSV()
	{
		string[] lines = csvString.Split("\n"[0]);
 
		//行数設定
		int width = 0; 
		for(int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine(lines[i]); 
			width = Mathf.Max(width, row.Length); 
		}
 
		//2次元配列を作成
		csvGrid = new string[width + 1, lines.Length + 1]; 
		for(int y = 0; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine(lines[y]); 
			for(int x = 0; x < row.Length; x++) 
			{
				csvGrid[x,y] = row[x]; 
 				
				//置き換え処理
				csvGrid[x,y] = csvGrid[x,y].Replace("\"\"", "\"");
			}
		}
	}
 	
	//csvライン分割
	private string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
			line,
			@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
			System.Text.RegularExpressions.RegexOptions.ExplicitCapture) select m.Groups[1].Value).ToArray();
	}
	
	//デバッグ用出力
	static public void DebugLogGrid(string[,] grid)
	{
		string outputText = ""; 
		for (int y = 0; y < grid.GetUpperBound(1); y++)
		{
			for (int x = 0; x < grid.GetUpperBound(0); x++)
			{
				outputText += grid[x, y];
				outputText += "|";
			}
			
			outputText += "\n";
		}
		
		Debug.Log(outputText);
	}
}
