
using AutomationTestUI.Forms;
using MATSys.Automation;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AutomationTestUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var appsettings = JsonSerializer.Deserialize<AppSetting>(File.ReadAllText("appsettings.json"));

            var services = new ServiceCollection();
            services.AddSingleton<TestPackage>();

            if (appsettings.UseBlazor)
            {
                services.AddWindowsFormsBlazorWebView();
                var blazorWebView1 = new BlazorWebView();
                blazorWebView1.HostPage = "wwwroot\\index.html";
                blazorWebView1.Services = services.BuildServiceProvider();
                blazorWebView1.RootComponents.Add<Components.Routes>("#app");
                blazorWebView1.Dock = DockStyle.Fill;
                this.Controls.Add(blazorWebView1);
            }
            else
            {
                var f = new MainFormView();
                f.TopLevel = false;
                f.Dock = DockStyle.Fill;
                f.ServiceProvider = services.BuildServiceProvider();
                this.Controls.Add(f);                
                f.Show();


            }
        }
    }
    internal struct AppSetting
    {
        public bool UseBlazor { get; set; }
    }

}
