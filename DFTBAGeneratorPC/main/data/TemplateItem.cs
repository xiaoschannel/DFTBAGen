using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.zuoanqh.open.zut.Processing;
using System.Text.RegularExpressions;

namespace cn.zuoanqh.open.DFTBAGen.main.data
{
	/// <summary>
	/// The type of an item in the template, decides how it will be matched.
	/// </summary>
	public enum TemplateItemType
	{ EXACT, DESCRIBED_POS , DESCRIBED_CFG , DESCRIBED_ANY};
	public class TemplateItem
	{
		public static readonly string FILEKEY_ITEM_TYPE = "Type";
		public static readonly string FILEKEY_EXACT_TEXT_STRING = "Exact Text";
		public static readonly string FILEKEY_PART_OF_SPEECH = "POS Type";
		public static readonly string FILEKEY_CFG_PHRASE_TYPE = "CFG Phrase Type";
		public static readonly string FILEKEY_CFG_PHRASE_LENGTH = "CFG Phrase Length";
		public static readonly string FILEKEY_CHECK_STARTS_WITH = "Check Starts With";
		public static readonly string FILEKEY_STARTS_WITH_STRING = "Starts With String";
		public static readonly string FILEKEY_CHECK_ENDS_WITH = "Check Ends With";
		public static readonly string FILEKEY_ENDS_WITH_STRING = "Ends With String";
		public static readonly string FILEKEY_CHECK_REGEX = "Check REGEX";
		public static readonly string FILEKEY_REGEX_STRING = "REGEX String";

		public static readonly string FILE_BOOLEAN_TRUE = "Yes";
		public static readonly string FILE_BOOLEAN_FALSE = "No";


		public TemplateItemType ItemType;
		public string PartOfSpeech;
		/// <summary>
		/// Not implemented yet :)
		/// </summary>
		public string CFGPhraseType, CFGPhraseLength;
		public bool CheckStartsWith, CheckEndsWith, CheckREGEX;
		public string ExactTextString, StartsWithString, EndsWithString, REGEXString;
		private Predicate<string> SERMatcher;

		private string EmptyIfNoKey(Dictionary<string,string> dic, string key)
		{
			return dic.ContainsKey(key) ? dic[key] : "";
		}
		public TemplateItem(List<string> input)
		{
			var dic = zut.zusp.ListToDictionary(input, GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR);
			ItemType = (TemplateItemType)Enum.Parse(typeof(TemplateItemType), dic[FILEKEY_ITEM_TYPE]);
			PartOfSpeech = EmptyIfNoKey(dic, FILEKEY_PART_OF_SPEECH);
			CFGPhraseType = EmptyIfNoKey(dic,FILEKEY_CFG_PHRASE_TYPE);
			CFGPhraseLength = EmptyIfNoKey(dic,FILEKEY_CFG_PHRASE_LENGTH);
			CheckStartsWith = dic[FILEKEY_CHECK_STARTS_WITH].Equals(FILE_BOOLEAN_TRUE);
			CheckEndsWith = dic[FILEKEY_CHECK_ENDS_WITH].Equals(FILE_BOOLEAN_TRUE);
			CheckREGEX = dic[FILEKEY_CHECK_REGEX].Equals(FILE_BOOLEAN_TRUE);
			ExactTextString = EmptyIfNoKey(dic,FILEKEY_EXACT_TEXT_STRING);
			StartsWithString = EmptyIfNoKey(dic,FILEKEY_STARTS_WITH_STRING);
			EndsWithString = EmptyIfNoKey(dic,FILEKEY_ENDS_WITH_STRING);
			REGEXString = EmptyIfNoKey(dic,FILEKEY_REGEX_STRING);
			UpdateSERMatcher();
		}

		/// <summary>
		/// A huge mess for optimization. If you have a better solution definitely shoot.
		/// </summary>
		private void UpdateSERMatcher()
		{
			if (CheckStartsWith)
			{
				if (CheckEndsWith)
				{
					if (CheckREGEX)
						SERMatcher = (s) => s.StartsWith(StartsWithString) && s.EndsWith(EndsWithString) && Regex.Match(s, REGEXString).Success;
					else
						SERMatcher = (s) => s.StartsWith(StartsWithString) && s.EndsWith(EndsWithString);
				}
				else
				{
					if (CheckREGEX)
						SERMatcher = (s) => s.StartsWith(StartsWithString) && Regex.Match(s, REGEXString).Success;
					else
						SERMatcher = (s) => s.StartsWith(StartsWithString);
				}
			}
			else
			{
				if (CheckEndsWith)
				{
					if (CheckREGEX)
						SERMatcher = (s) => s.EndsWith(EndsWithString) && Regex.Match(s, REGEXString).Success;
					else
						SERMatcher = (s) => s.EndsWith(EndsWithString);
				}
				else
				{
					if (CheckREGEX)
						SERMatcher = (s) => Regex.Match(s, REGEXString).Success;
					else
						SERMatcher = (s) => true;
				}
			}
		}

		/// <summary>
		/// The strings this item could match.
		/// The list is generated the first time it's accessed, which might take quite some time.
		/// </summary>
		public List<string> Matches
		{
			get
			{
				if (_Matches == null) FindAllMatching();
				return _Matches;
			}
			private set { _Matches = value; }
		} private List<string> _Matches;


		public void FindAllMatching()
		{
			_Matches = new List<string>();
			switch (ItemType)
			{
				case TemplateItemType.EXACT:
					{
						_Matches.Add(ExactTextString);
					} break;
				case TemplateItemType.DESCRIBED_ANY:
					{
						foreach (var s in WordLists.Instance)
						{
							if (SERMatcher.Invoke(s)) _Matches.Add(s);
						}
					} break;
				case TemplateItemType.DESCRIBED_POS:
					{
						foreach (var s in WordLists.Instance[PartOfSpeech])
						{
							if (SERMatcher.Invoke(s)) _Matches.Add(s);
						}
					} break;
				case TemplateItemType.DESCRIBED_CFG:
					{
						//Not implemented. Oh dear, this is gonna get huge!
					} break;
			}
		}

		private string BoolToText(bool b)
		{ return b ? FILE_BOOLEAN_TRUE : FILE_BOOLEAN_FALSE; }

		public override string ToString()
		{
			string ans = "";

			ans += FILEKEY_ITEM_TYPE + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + ItemType + "\n";
			ans += FILEKEY_EXACT_TEXT_STRING + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + ExactTextString+ "\n";
			ans += FILEKEY_PART_OF_SPEECH + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + PartOfSpeech + "\n";
			ans += FILEKEY_CFG_PHRASE_TYPE + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + CFGPhraseType + "\n";
			ans += FILEKEY_CFG_PHRASE_LENGTH + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + CFGPhraseLength + "\n";
			ans += FILEKEY_CHECK_STARTS_WITH + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + BoolToText(CheckStartsWith) + "\n";
			ans += FILEKEY_STARTS_WITH_STRING + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + StartsWithString + "\n";
			ans += FILEKEY_CHECK_ENDS_WITH + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + BoolToText(CheckEndsWith) + "\n";
			ans += FILEKEY_ENDS_WITH_STRING + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + EndsWithString + "\n";
			ans += FILEKEY_CHECK_REGEX + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + BoolToText(CheckREGEX) + "\n";
			ans += FILEKEY_REGEX_STRING + GeneratorTemplate.FILE_ATTRIBUTE_SEPARATOR + REGEXString + "\n";

			return ans;
		}

	}
}
