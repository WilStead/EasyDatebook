using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EasyDatebook
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.App_Startup;
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			this.StartupUri = new Uri(@"pack://application:,,,/EasyDatebook.Screens;Component/Main_Window.xaml");
		}
	}
}