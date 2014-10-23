#coding:utf-8

import sys, os, shutil
import codecs

import Tkinter
from tkCommonDialog import Dialog

###デバッグ用
import tkMessageBox
#tkMessageBox.showinfo(message = "")

###Tkinter
tk = Tkinter.Tk()
tk.withdraw()

###フォルダ選択ダイアログ
class DirSelectDlg(Dialog):
	command = "tk_chooseDirectory"

###テキストエンコードを変換して上書き保存
def ConvertUTF8(filePath):
	
	encoding = ['shift_jis','utf-8','euc_jp','cp932',
                    'euc_jis_2004','euc_jisx0213','iso2022_jp','iso2022_jp_1',
                    'iso2022_jp_2','iso2022_jp_2004','iso2022_jp_3','iso2022_jp_ext',
                    'shift_jis_2004','shift_jisx0213','utf_16','utf_16_be',
                    'utf_16_le','utf_7','utf_8_sig']
	
	#エンコードを判定してファイルを開く
	for encode in encoding:
		try:
			file = codecs.open(filePath, "r", encode)
			
			#utf-8に変換する
			data = ""
			for line in file:
				for char in line:
					if isinstance(char, unicode):
						char = char.encode("utf_8_sig")
				data += line
			
			file.close()
			break
			
		except:
			file.close()
			continue
	
	#改行コードを置き換え
	data = data.replace("\r\n", "\n")
	
	#ファイルを上書きして閉じる
	file = codecs.open(filePath, "w", "utf_8_sig")
	file.write(data)
	file.close()

###Main
def Main():
	
	#フォルダを選択する
	selectDir = os.path.abspath(DirSelectDlg().show())
	
	#指定フォルダ以下のスクリプトファイルを対象にする
	ct = 0
	for root,dirs,files in os.walk(selectDir):
		for file in files:
			name,ext = os.path.splitext(file)
			if(ext == ".cs" or ext == ".js" or ext == ".boo"):
				filePath = os.path.join(root, file)
				ConvertUTF8(filePath)
				ct += 1
	
	#変換終了ダイアログ
	tkMessageBox.showinfo(message = str(ct) + "ファイルを変換しました")

if __name__ == "__main__":
	Main()
