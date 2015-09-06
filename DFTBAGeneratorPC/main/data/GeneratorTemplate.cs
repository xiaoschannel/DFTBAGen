using cn.zuoanqh.open.zut;
using cn.zuoanqh.open.zut.FileIO.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.zuoanqh.open.DFTBAGen.main.data
{
	public class GeneratorTemplate : IEnumerable<string>
	{
		public static readonly string FILE_WORDS_SEPARATOR = "[Word]";
		public static readonly string FILE_ATTRIBUTE_SEPARATOR = ": ";

		public static readonly string FILEKEY_TEMPLATE_NAME = "Template Name";
		public static readonly string FILEKEY_TEMPLATE_DESCRIPTION = "Template Description";

		public string TemplateName;
		public string TemplateDescription;
		public List<TemplateItem> Items;
		public long TotalItems { get; private set; }
		private Random r = new Random();

		public GeneratorTemplate(List<string> input)
		{
			var ls = zusp.ListSplit(input, FILE_WORDS_SEPARATOR);
			var dic = zusp.ListToDictionary(ls[0], FILE_ATTRIBUTE_SEPARATOR);
			Items = new List<TemplateItem>();
			TemplateName = dic[FILEKEY_TEMPLATE_NAME];
			TemplateDescription = dic[FILEKEY_TEMPLATE_DESCRIPTION];

			double totald = 1;
			long total = 1;
			bool overflowflag = false;
			for (int i = 1; i < ls.Count; i++)
			{
				Items.Add(new TemplateItem(ls[i]));
				var item = Items[i - 1];
				item.FindAllMatching();
				totald *= item.Matches.Count;
				if (totald > UInt64.MaxValue)
				{
					overflowflag = true;
				}
				total *= (int)item.Matches.Count;
			}
			TotalItems = total;
		}

		public GeneratorTemplate(string file)
			: this(ByLineFileIO.readFileVerbatim(file))
		{ }

		public override string ToString()
		{
			string ans = "";

			ans += FILEKEY_TEMPLATE_NAME + FILE_ATTRIBUTE_SEPARATOR + TemplateName + "\n";
			ans += FILEKEY_TEMPLATE_DESCRIPTION + FILE_ATTRIBUTE_SEPARATOR + TemplateDescription + "\n";

			foreach (var item in Items) ans += FILE_WORDS_SEPARATOR + "\n" + item.ToString();

			return ans;
		}

		public string GetRandom()
		{
			byte[] b = new byte[8];
			r.NextBytes(b);
			return GetNth(Math.Abs(BitConverter.ToInt64(b, 0) % TotalItems));
		}

		public string GetNth(long index)
		{
			long j = index;
			StringBuilder sb = new StringBuilder();
			int k = (int)j % Items[0].Matches.Count;
			j /= (long)Items[0].Matches.Count;
			sb.Append(Items[0].Matches[k]);
			for (int l = 1; l < Items.Count; l++)
			{
				k = (int)j % Items[l].Matches.Count;
				j /= (long)Items[l].Matches.Count;
				sb.Append(" " + Items[l].Matches[k]);
			}
			//sb.Append(".");
			return sb.ToString();
		}

		public IEnumerator<string> GetEnumerator()
		{

			//if (overflowflag)
			//{
			//	yield return "Too many possible sentences, but here's first few petabytes: ";
			//}
			//if (total==0)
			//{
			//	List<int> unmatchables = new List<int>();
			//	for (int i = 0; i < items.Count; i++)
			//		if (items[i].Matches.Count == 0) unmatchables.Add(i);
			//		yield return "We can't find a match for the following places: "  + zusp.List(unmatchables.ToArray());
			//}
			//else if (items.Count == 0)
			//{
			//	yield return "We can't find any instructions to generate a sentence. Did you do something bad?";
			//}
			//else
			//{
			//yield return "Total: " + total;
			for (long i = 0; i < TotalItems; i++)
			{
				yield return GetNth(i);
			}
			//Random r = new Random();

			//}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
