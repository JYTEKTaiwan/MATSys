using AutomationTestUI.Dialogs;
using MATSys;
using MATSys.Automation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace AutomationTestUI.Forms
{
    public partial class PluginEditor : SubForm
    {
        private static PluginEditor page = null;
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        public static event EventHandler PluginChanged;

        public static PluginEditor Form
        {
            get
            {
                if (page == null || page.IsDisposed)
                {
                    page = new PluginEditor();
                }
                return page;
            }
        }

        public PluginInfo Info { get; private set; }

        private PluginEditor()
        {
            InitializeComponent();

        }

        public void Load(PluginInfo pi)
        {
            Info = pi;
            comboBox_mods.SelectedIndexChanged -= ComboBox_mods_SelectedIndexChanged;

            BindToUI();
            comboBox_mods.SelectedIndexChanged += ComboBox_mods_SelectedIndexChanged;
            this.ActiveControl = null;

        }
        private void BindToUI()
        {
            //ID must exist, and path and qualifiedName might be empty
            textBox_id.Text = Info.Alias;

            if (string.IsNullOrEmpty(Info.Path))
            {
                //if path is empty, just put an empty string in it
                textBox_path.Text = textBox_fullname.Text = comboBox_mods.Text = "";
                comboBox_mods.Items.Clear();

            }
            else
            {
                //validate the path info
                var assem = ValidateDllFile<IModule>(Info.Path);
                if (assem == null)
                {
                    //validation fail, path and qualifiedname should be empty string
                    MessageBox.Show($"Dll does not contains any class inherited from ({typeof(IModule).Name}), please choose another dll");
                    textBox_path.Text = "";
                    textBox_fullname.Text = "";
                    return;
                }
                else
                {
                    //validation OK, update the path 
                    textBox_path.Text = Info.Path;
                    //update the dictionary
                    dict = ListAllSupportedModules<IModule>(assem);

                    //update combobox items
                    comboBox_mods.Items.Clear();
                    comboBox_mods.Items.AddRange(dict.Keys.ToArray());

                    if (string.IsNullOrEmpty(Info.QualifiedName))
                    {
                        //qualifiedname is empty, combobox do not update
                        textBox_fullname.Text = "";
                    }
                    else
                    {
                        //qualifiedname exists, check if it also exists in dictionary
                        if (dict.Values.Any(x => x == Info.QualifiedName))
                        {
                            //exists
                            textBox_fullname.Text = Info.QualifiedName;
                            var key = dict.First(x => x.Value == Info.QualifiedName).Key;
                            comboBox_mods.Text = key;
                        }
                        else
                        {
                            //not exists, do nothing
                        }
                    }

                }
            }


        }

        private void ComboBox_mods_SelectedIndexChanged(object? sender, EventArgs e)
        {
            textBox_fullname.Text = dict[comboBox_mods.Text];
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            Info.Alias = textBox_id.Text;
            Info.Path = textBox_path.Text;
            Info.QualifiedName = textBox_fullname.Text;
            PluginChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button_browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog1.FileName;
                var assem = ValidateDllFile<IModule>(filePath);
                if (assem == null)
                {
                    MessageBox.Show($"Dll does not contains any class inherited from ({typeof(IModule).Name}), please choose another dll");
                    return;
                }
                else
                {
                    textBox_path.Text = filePath;
                    dict = ListAllSupportedModules<IModule>(assem);
                    comboBox_mods.Items.Clear();
                    comboBox_mods.Items.AddRange(dict.Keys.ToArray());
                    comboBox_mods.SelectedIndex = 0;
                }

            }

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
