using MATSys.Configurator.Core;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Configurator.Winform
{
    public partial class ExportDialog : Form
    {
        private readonly IEnumerable<MATSysInformation> _info;

        public bool EnableNLog { get; set; }
        public JsonNode Node_Nlog { get; set; }

        public bool EnableTransceiver { get; set; }
        public JsonNode Node_Transceiver { get; set; }

        public bool EnableScript { get; set; }
        public JsonNode Node_Script { get; set; }

        public ExportDialog(IEnumerable<MATSysInformation> collection)
        {
            InitializeComponent();
            _info = collection;
            comboBox1.Items.AddRange(_info.Where(x => x.Category == "Transceiver").Select(x => x.Type).ToArray());
            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(null, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox_nlog.Visible = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Visible = checkBox2.Checked;
            propertyGrid_transceiver.Visible = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox_scripts.Visible = checkBox3.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateProperties();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = _info.FirstOrDefault(x => x.Type == comboBox1.Text);
            propertyGrid_transceiver.SelectedObject = item.Setting;
        }

        public void UpdateProperties()
        {
            if (string.IsNullOrEmpty(richTextBox_nlog.Text))
            {
                EnableNLog = false;
            }
            else
            {
                EnableNLog = checkBox1.Checked;
                Node_Nlog = JsonObject.Parse(richTextBox_nlog.Text);
            }

            EnableTransceiver = checkBox2.Checked;
            Node_Transceiver = JsonSerializer.SerializeToNode(propertyGrid_transceiver.SelectedObject);


            if (string.IsNullOrEmpty(richTextBox_scripts.Text))
            {
                EnableScript = false;
            }
            else
            {
                EnableScript = checkBox3.Checked;
                Node_Script = JsonObject.Parse(richTextBox_scripts.Text);


            }

        }

        private void ExportDialog_Shown(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }
    }
}
