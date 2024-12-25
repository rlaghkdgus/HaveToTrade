using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogData : ScriptableObject
{
    // public List<TextData> 부분까지는 고정
    // 뒤에 변수 이름만 각 시트별 이름과 동일하게 설정
    public List<TextData> TextEX; 
	public List<TextData> TextEX2;
}
