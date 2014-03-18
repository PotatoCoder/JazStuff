using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace JazInterpreter_Mrk4
{
    public class FileOperations
    {
        public List<Node> ReadFile(string filePath)
        {
            List<Node> nodeTree = new List<Node>();
            List<string> fileLines = new List<string>();
            foreach(string s in File.ReadAllLines(filePath))
            {
                if(!s.Equals(""))
                    fileLines.Add(s);
            }
            string sKey, sValue = "";

            foreach(string line in fileLines)
            {
                sKey = "";
                sValue = "";
                int nullspace = line.Trim().IndexOf(" ");
                if(nullspace < 0)
                {
                    sKey = line.Trim();
                }
                else
                {
                    sKey = line.Trim().Substring(0, nullspace);
                    sValue = line.Trim().Substring(nullspace);
                    
                }
                nodeTree.Add(new Node(sKey, sValue, nodeTree.Count()));
              
            }

            return nodeTree;
        }
    }
}
