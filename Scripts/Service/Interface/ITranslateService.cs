namespace CardPrototype.Service;

public interface ITranslateService
{
    string Translate(string key, params object[] args);
}
