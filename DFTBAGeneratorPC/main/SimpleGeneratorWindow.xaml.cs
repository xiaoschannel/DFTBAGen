using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using cn.zuoanqh.open.zut.FileIO.Text;
using cn.zuoanqh.open.zut;

namespace cn.zuoanqh.open.DFTBAGen.main
{
	/// <summary>
	/// Interaction logic for SimpleGeneratorWindow.xaml
	/// </summary>
	public partial class SimpleGeneratorWindow : Window
	{
		Dictionary<string, List<string>> posLists;
		List<string> verbB, nounA;
		Random r = new Random();
		public SimpleGeneratorWindow()
		{
			InitializeComponent();
			posLists = new Dictionary<string, List<string>>();
			posLists.Add("noun", new List<string>());
			posLists.Add("verb", new List<string>());
			posLists.Add("adjective", new List<string>());
			posLists.Add("adverb", new List<string>());

			foreach (var s in posLists.Keys)
			{
				posLists[s].AddRange(ByLineFileIO.readFileNoWhitespace("words\\"+s + ".txt"));
			}
			posLists["verb"] = posLists["verb"].FindAll((s) => (!s.EndsWith("ed") && !s.EndsWith("ing")));

			verbB = posLists["verb"].FindAll((s) => s.StartsWith("b"));
			nounA = posLists["noun"].FindAll((s) => s.StartsWith("a"));
		}

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			this.Height = 210;
		}

		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			this.Height = 299;
		}

		private void btnOne_Click(object sender, RoutedEventArgs e)
		{
			txtOutput.Clear();

			string verb = verbB[r.Next(verbB.Count)];
			string noun = nounA[r.Next(nounA.Count)];
			txtOutput.Text += "Don't forget to " + verb + " " + noun;

			Clipboard.SetText(txtOutput.Text);
		}

		private void btnMany_Click(object sender, RoutedEventArgs e)
		{
			txtOutput.Clear();

			for (long i = 0; i < Convert.ToInt64(txtNumbers.Text); i++)
			{
				string verb = verbB[r.Next(verbB.Count)];
				string noun = nounA[r.Next(nounA.Count)];
				txtOutput.Text += "Don't forget to " + verb + " " + noun+"\n";
			}

			Clipboard.SetText(txtOutput.Text);
		}
	}
}
