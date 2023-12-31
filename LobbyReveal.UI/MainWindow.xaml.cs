﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LobbyReveal.Core.Interfaces;
using LobbyReveal.Infrastructure.Clients;
using LobbyReveal.Infrastructure.Services;
using LobbyReveal.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LobbyReveal.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		public IConfigurationRoot Configuration { get; set; }

        public MainWindow()
        {
			var builder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			Configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();
            Startup.ConfigureServices(services, Configuration);

			var builtServiceProvider = services.BuildServiceProvider();

            Resources.Add("services", builtServiceProvider);

            InitializeComponent();
			TrySetVersionNumber();
        }

        private void TrySetVersionNumber()
		{
            try
            {
				var version = Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
				this.Dispatcher.Invoke(() => {
					versionNum.Text = $"v{version}";
				});
            }
            catch
            {
                this.Dispatcher.Invoke(() => {
                    versionNum.Text = "";
                });
            }
        }

		private void Close(object sender, RoutedEventArgs e)
        {
			this.Close();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
			SystemCommands.MinimizeWindow(this);
		}

        private void Maximize(object sender, RoutedEventArgs e)
        {
			if (this.WindowState != WindowState.Maximized)
				SystemCommands.MaximizeWindow(this);
			else
                SystemCommands.RestoreWindow(this);
        }
    }
}
