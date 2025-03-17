using System.Collections.Generic;

public interface IGameConfig
{
    public List<ConfigStruct> GetConfig();

    public ELanguage GetTextLanguage();
}
