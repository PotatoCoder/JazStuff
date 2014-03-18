using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazInterpreter_Mrk4
{
    public class Node
    {
        
        public string key,value;
        public int location;
        
        public Node()
        {
            
        }

        public Node(string keyword,string val,int spot)
        {
            this.key = keyword;
            this.value = val;
            this.location = spot;
        }

    }
}
