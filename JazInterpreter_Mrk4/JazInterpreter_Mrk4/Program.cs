using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace JazInterpreter_Mrk4
{
    class Program
    {

        public static void Main(string[] args)
        {
            FileOperations fo = new FileOperations();
            List<Node> JazTree = new List<Node>();
            SymbolTable sTable = new SymbolTable();
            bool canRun = true;
            do
            {
                Console.WriteLine("Please enter name of file you wish to run");
                string filePath = Console.ReadLine();
                try
                {
                    string pathString = Directory.GetCurrentDirectory();
                    string outputFile = filePath.TrimEnd('.', 'j', 'a', 'z');
                    outputFile = outputFile + ".out";
                    pathString = Path.Combine(pathString, outputFile);
                    JazTree = fo.ReadFile(filePath);
                    sTable.getFullName = pathString;
                    sTable.RunCode(JazTree);
                    Console.ReadLine();
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("File Not Found" + "\n" + "Try again");
                    canRun = false;
                }

            } while (!canRun);


        }

    }
}
