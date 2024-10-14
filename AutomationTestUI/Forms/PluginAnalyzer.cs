using AutomationTestUI.Dialogs;
using MATSys;
using MATSys.Automation;
using MATSys.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace AutomationTestUI.Forms
{
    public partial class PluginAnalyzer : SubForm
    {
        private static PluginAnalyzer page;

        private PluginAnalyzer()
        {
            InitializeComponent();

        }
        public static PluginAnalyzer OpenForm(DockPanel dp, DockState ds,ServiceProvider sp, PluginInfo pi)
        {
            if (page == null || page.IsDisposed)
            {
                page = new PluginAnalyzer();
                page.ServiceProvider = sp;
                page.Show(dp, ds);
            }
            page.Load(pi);

            return page;
        }
        public static PluginAnalyzer OpenForm(DockPane dp, DockAlignment da, float proportion, ServiceProvider sp, PluginInfo pi)
        {
            if (page == null || page.IsDisposed)
            {
                page = new PluginAnalyzer();
                page.ServiceProvider = sp;
                page.Show(dp, da,proportion);
            }
            page.Load(pi);

            return page;
        }

        public void Load(PluginInfo pi)
        {

        }




        private Assembly? ValidateDllFile<T>(string path)
        {
            var assem = Assembly.LoadFrom(path);
            if (assem.GetTypes().Any(x => typeof(T).IsAssignableFrom(x))) return assem;
            else return null;
        }

        private Dictionary<string, string> ListAllSupportedModules<T>(Assembly assem)
        {
            return assem.GetTypes().Where(x => typeof(T).IsAssignableFrom(x)).Select(x => x.AssemblyQualifiedName).ToDictionary(k =>
            {
                var str = k.Split(',');
                return $"{str[0]}, {str[1]}";
            });
        }

    }
}
