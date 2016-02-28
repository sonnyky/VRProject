using System;
using System.Linq;

public static class SpeachResult
{
    // 文字列比較
    public static bool wordCmp(string[] wordLst, string str)
    {
        string wrkWord = "";

        foreach (string word in wordLst)
        {
            wrkWord = word;

            // トリム
            wrkWord = wrkWord.Trim();
            wrkWord = wrkWord.Replace(" ", "");
            wrkWord = wrkWord.Replace("　", "");
            // 大文字を小文字に変換
            wrkWord = wrkWord.ToLower();
            // カタカナをひらがなに変換
            wrkWord = ToHiragana(wrkWord);
            //TODO 半角を全角に変換

            // 比較 (含むかどうか)
            if (0 <= wrkWord.IndexOf(ToHiragana(str).ToLower()))
            //if (wrkWord == str)
            {
                return true;
            }
        }

        return false;
    }

    // ひらがなに変換
    public static string ToHiragana(this string s)
    {
        return new string(s.Select(c => (c >= 'ァ' && c <= 'ヶ') ? (char)(c + 'ぁ' - 'ァ') : c).ToArray());
    }
}