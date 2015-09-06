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
using System.IO;
using cn.zuoanqh.open.zut.FileIO.Text;
using cn.zuoanqh.open.zut;
using cn.zuoanqh.open.DFTBAGen.main.data;

namespace cn.zuoanqh.open.DFTBAGen.main
{
	/// <summary>
	/// Interaction logic for SimpleGeneratorWindow.xaml
	/// </summary>
	public partial class SimpleGeneratorWindow : Window
	{
		GeneratorTemplate currentTemplate
		{
			get { return _currentTemplate; }
			set
			{
				_currentTemplate = value;
				lblCurrentTemplateName.Content = value.TemplateName;
				rbtAll.Content = String.Format("Generate ALL combinations: {0} Possible", value.TotalItems);
			}
		} GeneratorTemplate _currentTemplate;

		public SimpleGeneratorWindow()
		{
			InitializeComponent();
			currentTemplate = new GeneratorTemplate(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Templates", "test1.txt"));

		}

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			this.Height = 250;
		}

		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			this.Height = 250 + 89;
		}

		private void btnOne_Click(object sender, RoutedEventArgs e)
		{
			txtOutput.Clear();

			txtOutput.Text = currentTemplate.GetRandom();

			Clipboard.SetText(txtOutput.Text);
		}

		private void btnMany_Click(object sender, RoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			if (rbtThisMany.IsChecked == true)//WTF do you mean by null? it's checked or not checked!
				for (long i = 0; i < Convert.ToInt64(txtNumbers.Text); i++)
					sb.Append(currentTemplate.GetRandom()).Append("\n");
			else
				foreach (var s in currentTemplate)
					sb.Append(s).Append("\n");

			txtOutput.Text = sb.ToString();
			Clipboard.SetText(txtOutput.Text);
		}

		private void btnTemplateFromClipboard_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				currentTemplate = new GeneratorTemplate(zusp.Split(Clipboard.GetText(), "\n").Select((s) => s.Trim()).ToList());
			}
			catch (Exception ex)
			{
				//TODO: bake some toast!
			}
		}
	}
}
