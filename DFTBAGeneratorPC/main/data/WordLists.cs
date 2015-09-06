using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.zuoanqh.open.zut.FileIO.Text;
using System.IO;

namespace cn.zuoanqh.open.DFTBAGen.main.data
{
	public class WordLists : IEnumerable<string>
	{
		/// <summary>
		/// Default set of lists to load. Setting file overrides this.
		/// </summary>
		public static readonly string DEFAULT_WORDSET_NAME = "default_POS";
		/// <summary>
		/// Key for SimpleSettings Class
		/// </summary>
		public static readonly string SETTINGKEY_WORDSET_NAME = "Current set of word lists";
		/// <summary>
		/// 
		/// There's only one! I want to use enumerator and index operator so it can't be static, but C# doesn't allow Scala-like "object" to be created.
		/// </summary>
		public static WordLists Instance { get; private set; }

		static WordLists()
		{
			Instance = new WordLists();
			SimpleSettings.UseDefaultValue(SETTINGKEY_WORDSET_NAME, DEFAULT_WORDSET_NAME);
			Instance.LoadLists(SimpleSettings.GetString(SETTINGKEY_WORDSET_NAME));
		}

		public static void ChangeWordSet(string SetName)
		{
			Instance.LoadLists(SetName);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////
		///Non-static starts from here
		////////////////////////////////////////////////////////////////////////////////////////////// 
		
		/// <summary>
		/// Why would you want to access this directly?
		/// </summary>
		public Dictionary<string, SortedSet<string>> Lists { get; private set; }

		private WordLists()
		{
			Lists = new Dictionary<string, SortedSet<string>>();
		}
		/// <summary>
		/// Load lists from a word set.
		/// </summary>
		/// <param name="SetName"></param>
		private void LoadLists(string SetName)
		{
			Lists = new Dictionary<string, SortedSet<string>>();
			foreach (var s in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "words", SetName)))
			{
				Lists.Add(Path.GetFileNameWithoutExtension( s), new SortedSet<string>(ByLineFileIO.readFileNoWhitespace(s)));
			}
		}

		public SortedSet<string> this[string key]
		{
			get { return Lists[key]; }
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (var l in Lists)
				foreach (var s in l.Value)
					yield return s;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
