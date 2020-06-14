using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class MatchingFiles
    {
        public Dictionary<int, int> IdMatchingDictionary;

        public MatchingFiles()
        {
            IdMatchingDictionary = new Dictionary<int, int>();
        }

        public void AddFile(int fileID, int matchingID)
        {
            IdMatchingDictionary.Add(fileID, matchingID);
        }

        public void RemoveAllFiles()
        {
            IdMatchingDictionary.Clear();
        }

        public int GetFileIDByMatchingID(int matchingID)
        { 
             return IdMatchingDictionary.FirstOrDefault(x => x.Value == matchingID).Key;
        }
    }
}
