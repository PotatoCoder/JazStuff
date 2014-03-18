using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace JazInterpreter_Mrk4
{
    public class SymbolTable
    {
        /*
        * Dictionaries
        */
        Dictionary<string, Delegate> jazCommands = new Dictionary<string, Delegate>();
        /*
         */

        /*
         * Lists
         */
        List<Dictionary<string, int>> tables = new List<Dictionary<string, int>>();
        List<object> printvalues = new List<object>();

        /*
         * Trees
         */
        List<Node> nTree = new List<Node>();

        /*
         * Stacks
         */

        Stack<int> symbolStack = new Stack<int>();
        Stack<int> locationStack = new Stack<int>();
        /*
         * Properties
         */
        Dictionary<string, int> grabDictionary { get; set; }
        int tableIndex { get; set; }
        public string getFullName { get; set; }

        /*
         * Variables
         */
        Boolean beforeCall = true;
        Boolean returning = false;

        /*
         * Class Calling
         */

        public SymbolTable()
        {

            jazCommands.Add("push", new Func<Node, Node>((Push)));
            jazCommands.Add("rvalue", new Func<Node, Node>((RValue)));
            jazCommands.Add("lvalue", new Func<Node, Node>((LValue)));
            jazCommands.Add("pop", new Func<Node, Node>((Pop)));
            jazCommands.Add(":=", new Func<Node, Node>((DoublePop)));
            jazCommands.Add("goto", new Func<Node, Node>(Jump));
            jazCommands.Add("gofalse", new Func<Node, Node>(JumpFalse));
            jazCommands.Add("gotrue", new Func<Node, Node>(JumpTrue));
            jazCommands.Add("halt", new Func<Node, Node>((HaltProgram)));
            jazCommands.Add("-", new Func<Node, Node>((Subtraction)));
            jazCommands.Add("+", new Func<Node, Node>((Addition)));
            jazCommands.Add("*", new Func<Node, Node>((Multiplication)));
            jazCommands.Add("/", new Func<Node, Node>((Division)));
            jazCommands.Add("div", new Func<Node, Node>((Remainder)));
            jazCommands.Add("&", new Func<Node, Node>((And)));
            jazCommands.Add("!", new Func<Node, Node>((Negate)));
            jazCommands.Add("|", new Func<Node, Node>((Or)));
            jazCommands.Add("<>", new Func<Node, Node>((DoubleEqual)));
            jazCommands.Add("<=", new Func<Node, Node>((LessThanEqual)));
            jazCommands.Add(">=", new Func<Node, Node>((GreaterThanEqual)));
            jazCommands.Add("<", new Func<Node, Node>((LessThan)));
            jazCommands.Add(">", new Func<Node, Node>((GreaterThan)));
            jazCommands.Add("=", new Func<Node, Node>((Equal)));
            jazCommands.Add("print", new Func<Node, Node>((PrintAnswer)));
            jazCommands.Add("show", new Func<Node, Node>(Show));
            jazCommands.Add("begin", new Func<Node, Node>((ParameterPass)));
            jazCommands.Add("end", new Func<Node, Node>((EndParameterPass)));
            jazCommands.Add("call", new Func<Node, Node>(CallProcedure));
            jazCommands.Add("return", new Func<Node, Node>((Return)));
        }

        public void RunCode(List<Node> nodeTree)
        {
            nTree = nodeTree;
            tables.Add(new Dictionary<string, int>());

            for (int t = 0; t < nodeTree.Count(); t++)
            {
                Node treeNode = nodeTree[t];
                t = ExecuteNode(treeNode, tables[0]).location;
            }

        }

        private Node ExecuteNode(Node n, Dictionary<string, int> runningDictionary)
        {
            Node w = new Node();
            grabDictionary = runningDictionary;
            w = (Node)jazCommands[n.key].DynamicInvoke(n);
            return w;

        }

        private Node Show(Node tNode)
        {
            printvalues.Add(tNode.value);
            Console.WriteLine(tNode.value);
            return tNode;
        }

        private Node Push(Node tNode)
        {
            symbolStack.Push(int.Parse(tNode.value));
            return tNode;
        }

        private Node RValue(Node tNode)
        {
            if (!grabDictionary.ContainsKey(tNode.value))
            {
                grabDictionary.Add(tNode.value, 0);
            }
            symbolStack.Push(grabDictionary[tNode.value]);
            return tNode;

        }

        private Node LValue(Node tNode)
        {
            if (grabDictionary.ContainsKey(tNode.value))
            {
                int counter = 0;
                foreach (string k in grabDictionary.Keys)
                {
                    if (k != tNode.value)
                    {
                        counter++;
                    }
                    else
                    {
                        break;
                    }
                }
                symbolStack.Push(counter);
            }
            else
            {
                grabDictionary.Add(tNode.value, grabDictionary.Count);
                symbolStack.Push(grabDictionary[tNode.value]);
            }
            return tNode;
        }

        private Node Pop(Node tNode)
        {
            symbolStack.Pop();
            return tNode;
        }

        private Node DoublePop(Node tNode)
        {
            int poppedvalue = symbolStack.Pop();
            int location = symbolStack.Pop();
            string foundValue = grabDictionary.ElementAt(location).Key;
            grabDictionary[foundValue] = poppedvalue;
            return tNode;
        }

        private Node HaltProgram(Node tNode)
        {
            CreateOutputFile(getFullName);
            Console.ReadLine();
            Environment.Exit(0);
            return tNode;
        }

        private Node Addition(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            symbolStack.Push(y + x);
            return tNode;
        }

        private Node Subtraction(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            symbolStack.Push(y - x);
            return tNode;
        }

        private Node Multiplication(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            symbolStack.Push((y * x));
            return tNode;
        }

        private Node Division(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            symbolStack.Push((y / x));
            return tNode;
        }

        private Node Remainder(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            symbolStack.Push((y % x));
            return tNode;
        }

        private Node And(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x != 0 && y != 0)
            {
                symbolStack.Push(1);
            }
            else
            {
                symbolStack.Push(0);
            }
            return tNode;
        }

        private Node Negate(Node tNode)
        {
            string binaryNum = (symbolStack.Pop().ToString());

            char[] binAr = binaryNum.ToCharArray();
            for (int a = 0; a < binAr.Length; a++)
            {
                if (binAr[a].Equals('0'))
                {
                    binAr[a] = '1';
                }
                else
                { 
                    binAr[a] = '0';
                }
            }
            binaryNum = new string(binAr);
            symbolStack.Push(int.Parse(binaryNum));
            return tNode;
        }

        private Node Or(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x != 0 || y != 0)
            {
                symbolStack.Push(1);
            }
            else
            {
                symbolStack.Push(0);
            }
            return tNode;
        }


        private Node DoubleEqual(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x == y)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;

        }

        private Node GreaterThan(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x > y - 1)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;

        }
        private Node LessThan(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x < y - 1)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;
        }

        private Node Equal(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x == y - 1)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;
        }

        private Node GreaterThanEqual(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x >= y - 1)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;

        }
        private Node LessThanEqual(Node tNode)
        {
            int x = symbolStack.Pop();
            int y = symbolStack.Pop();
            if (x <= y - 1)
            {
                symbolStack.Push(0);
            }
            else
            {
                symbolStack.Push(1);
            }
            return tNode;

        }

        private Node PrintAnswer(Node tNode)
        {
            printvalues.Add(symbolStack.Peek());
            Console.WriteLine(symbolStack.Peek());
            return tNode;
        }

        private Node ParameterPass(Node tNode)
        {
            beforeCall = true;
            returning = false;
            tables.Add(new Dictionary<string, int>());
            tableIndex++;
            for (int t = tNode.location + 1; t < nTree.Count(); t++)
            {
                Node fNode = nTree[t];
                if (beforeCall && fNode.key.Equals("rvalue"))
                {
                    t = ExecuteNode(fNode, tables[tableIndex - 1]).location;
                }
                else if (!beforeCall && returning && (fNode.key.Equals("lvalue") || fNode.key.Equals(":=")))
                {
                    t = ExecuteNode(fNode, tables[tableIndex - 1]).location;
                }
                else
                {
                    t = ExecuteNode(fNode, tables[tableIndex]).location;
                }
            }
            tables.RemoveAt(tableIndex);
            tableIndex--;
            return tNode;
        }

        private Node EndParameterPass(Node tNode)
        {
            tables.RemoveAt(tableIndex);
            tableIndex--;
            returning = false;
            return tNode;
        }

        private Node Jump(Node tNode)
        {
            return nTree.Find(t => t.key.Equals("label") && t.value.Equals(tNode.value));
        }

        private Node JumpFalse(Node tNode)
        {
            if (symbolStack.Pop() == 0)
            {
                return nTree.Find(t => t.key.Equals("label") && t.value.Equals(tNode.value));
            }
            return tNode;
        }

        private Node JumpTrue(Node tNode)
        {
            if (symbolStack.Pop() > 0)
            {
                return nTree.Find(t => t.key.Equals("label") && t.value.Equals(tNode.value));
            }
            return tNode;
        }


        private Node CallProcedure(Node tNode)
        {
            beforeCall = false;
            locationStack.Push(tNode.location);
            return nTree.Find(t => t.key.Equals("label") && t.value.Equals(tNode.value));

        }

        private Node Return(Node tNode)
        {
            int returnLocation = locationStack.Pop();
            returning = true;
            return nTree.Find(t => t.location.Equals(returnLocation));
        }

        public void CreateOutputFile(string filePath)
        {
            using (System.IO.FileStream fs = System.IO.File.Create(filePath))
            {
                using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(fs))
                {
                    foreach (object o in printvalues)
                    {
                        fileWriter.WriteLine(o);
                    }
                }
            }

        }
    }
}
