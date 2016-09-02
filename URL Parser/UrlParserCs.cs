using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UrlParser
{
    class Program
    {
		Console.WriteLine("Enter the file path for the csv to be parsed.");
		string fileName = Console.Readline();
		StreamWriter input = File.OpenText(fileName);
		
		LoopToList(input);
		
		EditList();
		
		SaveToFile();
	}
	
	void LoopToList(StreamWriter input);
}