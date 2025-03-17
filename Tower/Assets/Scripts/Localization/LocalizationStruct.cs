using System;
using System.Collections.Generic;

[Serializable]
public struct LocalizationStruct
{
    public string Key;
    public List<LanguageStruct> Value;
}
