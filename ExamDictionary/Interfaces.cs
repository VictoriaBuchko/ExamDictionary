using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary
{
    public interface IAddable
    {
        void AddWord(string activeDictionaryType);
    }

    public interface IDeletable
    {
        void DeleteWord(string activeDictionaryType);
        void DeleteTranslation(string activeDictionaryType);
    }


    public interface ISearchable
    {
        void SearchTranslations(string word);
        void TranslatePhrase();
    }

    public interface IStorage
    {
        void Save(Dictionary<string, List<string>> dictionary, string fileName);
        Dictionary<string, List<string>> Load(string fileName);
    }

    public interface IReplaceable
    {
        void ReplaceWord(string activeDictionaryType);
        void ReplaceTranslation(string activeDictionaryType);
    }

}
