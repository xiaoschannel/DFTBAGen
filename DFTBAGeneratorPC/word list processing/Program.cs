using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.zuoanqh.open.zut.FileIO.Text;
using cn.zuoanqh.open.zut;

namespace cn.zuoanqh.open.DFTBAGen.word_list_processing
{
	class Program
	{
		static void Main(string[] args)
		{
			StopWatch w = new StopWatch();

			List<string> mobypos;
			SortedSet<string> scowl35 = new SortedSet<string>();
			w.Click();
			Console.WriteLine("Reading files...");
			mobypos = ByLineFileIO.readFileNoWhitespace("mobyposi.i");
			foreach (var s in ByLineFileIO.readFileNoWhitespace("english-words.35"))
				scowl35.Add(s);
			Console.WriteLine("Finished! Took {0:F} ms.", w.Click());
			Console.WriteLine("Now parsing...");
			Dictionary<string, string> mobyParsed = new Dictionary<string, string>();
			foreach(string s in mobypos)
			{
				try
				{
					var pts = zusp.CutFirst(s, "*");
					if (!mobyParsed.ContainsKey(pts.First))
						mobyParsed.Add(pts.First, pts.Second);
					else
						mobyParsed[pts.First] += pts.Second;
				}
				catch (Exception e)//I've had enough with encoding
				{ continue; }

			}
			Console.WriteLine("Finished! Took {0:F} ms.", w.Click());
			Console.WriteLine("Filtering the list with SCOWL and remove uncommon words...");
			Dictionary<string, string> filtered = new Dictionary<string, string>();
			foreach (string s in mobyParsed.Keys)
			{
				if (scowl35.Contains(s)) filtered.Add(s, mobyParsed[s]);
			}
			Console.WriteLine("Finished! Took {0:F} ms.", w.Click());
			Console.WriteLine("Filtered list contains {0:D} words with part-of-speech.", filtered.Count);
			Console.WriteLine("Generating part-of-speech lists...");
			Dictionary<string, List<string>> posLists = new Dictionary<string, List<string>>();
			posLists.Add("noun", new List<string>());
			posLists.Add("verb", new List<string>());
			posLists.Add("adjective", new List<string>());
			posLists.Add("adverb", new List<string>());
			foreach (var s in filtered.Keys)
			{ 
				foreach(char c in filtered[s].ToCharArray())
				{
					if (c == 'N' || c == 'r') posLists["noun"].Add(s);
					if (c == 'V' || c == 't' || c == 'i') posLists["verb"].Add(s);
					if (c == 'A') posLists["adjective"].Add(s);
					if (c == 'v') posLists["adverb"].Add(s);
				}
			}
			Console.WriteLine("Finished! Took {0:F} ms.", w.Click());
			Console.WriteLine("Writing lists into files...");
			foreach (var s in posLists.Keys)
			{
				Console.WriteLine("There were {0:D} in list {1}", posLists[s].Count,s);
				ByLineFileIO.writeFile(posLists[s], s + ".txt");
			}
			Console.WriteLine("Finished! Took {0:F} ms. The end :)", w.Click());
			Console.ReadLine();
		}
	}
}
