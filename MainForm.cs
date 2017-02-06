using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ViberSender2017
{
    public class MainForm : Form
    {
        private Random r = new Random();
        private Thread _th = (Thread)null;
        private int start = 0;
        private bool pause = false;
        private IContainer components = (IContainer)null;
        private TabControl tabControl_general;
        private TabPage tabPage_settings;
        private GroupBox groupBox1;
        private Button button_new_acc;
        private DataGridView dataGridView_all_accs;
        private TabPage tabPage_numbers;
        private TabPage tabPage_subjects;
        private GroupBox groupBox2;
        private CheckBox checkBox_toFirst;
        private NumericUpDown numericUpDown_kol_unvalid;
        private CheckBox checkBox_unvalid_accs;
        private NumericUpDown numericUpDown_kol_swap;
        private CheckBox checkBox_swap_acc;
        private ProgressBar progressBar1;
        private Button button_start;
        private Button button_stop;
        private GroupBox groupBox3;
        private TextBox textBox_path_numbers;
        private Button button_browse_numbers;
        private Button button_load_numbers;
        private DataGridView dataGridView_all_numbers;
        private GroupBox groupBox4;
        private RichTextBox richTextBox_text;
        private Label label1;
        private TextBox textBox_file;
        private Button button_picture;
        private TextBox textBox_link;
        private CheckBox checkBox_link;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem toolStripMenuItem1;
        private Button button1;
        private CheckBox checkBox_data;
        private ToolStripMenuItem toolStripMenuItem2;
        private Label label2;
        private NumericUpDown numericUpDown_pause;
        private DataGridViewTextBoxColumn NumberSerial;
        private DataGridViewCheckBoxColumn UseSend;
        private DataGridViewTextBoxColumn Login;
        private DataGridViewTextBoxColumn Number;
        private DataGridViewTextBoxColumn NumberSerial1;
        private DataGridViewTextBoxColumn Numbers;
        private DataGridViewTextBoxColumn Sended;
        private CheckBox checkBox_clear_history;
        private Label label3;
        private NumericUpDown numericUpDown_pause_off;

        public MainForm()
        {
            this.InitializeComponent();
            this.toolStripMenuItem1.Click += (EventHandler)((a, b) =>
           {
               for (int index = 0; index < this.dataGridView_all_accs.Rows.Count; ++index)
               {
                   if (this.dataGridView_all_accs.SelectedRows.Count > 0 && this.dataGridView_all_accs.SelectedRows[0].Index != index)
                       this.dataGridView_all_accs.Rows[index].Cells[1].Value = (object)"0";
                   if (this.dataGridView_all_accs.SelectedRows.Count > 0 && this.dataGridView_all_accs.SelectedRows[0].Index == index)
                       this.dataGridView_all_accs.Rows[index].Cells[1].Value = (object)"1";
                   this.dataGridView_all_accs.Update();
               }
           });
            this.toolStripMenuItem2.Click += (EventHandler)((a, b) => this.start = this.dataGridView_all_accs.SelectedRows[0].Index);
            try
            {
                StartSettings startSettings1 = new StartSettings();
                if (!File.Exists("settings.xml"))
                    return;
                using (Stream stream = (Stream)new FileStream("settings.xml", FileMode.Open))
                {
                    StartSettings startSettings2 = (StartSettings)new XmlSerializer(typeof(StartSettings)).Deserialize(stream);
                    this.checkBox_data.Checked = startSettings2.add_date;
                    this.checkBox_link.Checked = startSettings2.link;
                    this.checkBox_swap_acc.Checked = startSettings2.swap_accs;
                    this.checkBox_toFirst.Checked = startSettings2.first_accs;
                    this.checkBox_unvalid_accs.Checked = startSettings2.unvalid;
                    this.numericUpDown_kol_swap.Value = startSettings2.swap_accs_kol;
                    this.numericUpDown_kol_unvalid.Value = startSettings2.unvalid_kol;
                    this.textBox_file.Text = startSettings2.folder_files;
                    this.textBox_link.Text = startSettings2.link_str;
                    this.richTextBox_text.Text = startSettings2.richtextbox;
                    this.numericUpDown_pause.Value = startSettings2.pause;
                    this.checkBox_clear_history.Checked = startSettings2.clear_histiry;
                    this.numericUpDown_pause_off.Value = startSettings2.pause_off;
                    this.textBox_path_numbers.Text = startSettings2.file_numbers;
                    if (string.IsNullOrEmpty(startSettings2.file_numbers))
                        return;
                    this.LoadNumbers();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        private void checkBox_swap_acc_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_kol_swap.Enabled = this.checkBox_swap_acc.Checked;
        }

        private void checkBox_unvalid_accs_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_kol_unvalid.Enabled = this.checkBox_unvalid_accs.Checked;
        }

        private void button_browse_numbers_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string str = "TXT|*.txt";
            openFileDialog1.Filter = str;
            OpenFileDialog openFileDialog2 = openFileDialog1;
            if (openFileDialog2.ShowDialog() != DialogResult.OK)
                return;
            this.textBox_path_numbers.Text = openFileDialog2.FileName;
        }

        private void checkBox_link_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_link.Enabled = this.checkBox_link.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Activate activate = new Activate();
            if (!File.Exists("key.it") || File.ReadAllText("key.it") != activate.GetKey())
            {
                int num = (int)activate.ShowDialog();
                if (!activate.flag)
                    this.Close();
                else
                    File.WriteAllText("key.it", activate.GetKey());
            }
            this.dataGridView_all_accs.AutoGenerateColumns = false;
            this.dataGridView_all_accs.AllowUserToAddRows = false;
            this.dataGridView_all_accs.DataSource = (object)WorkBD.DownloadBD();
        }

        private async void button_new_acc_Click(object sender, EventArgs e)
        {
            IEnumerable<Process> proc = ((IEnumerable<Process>)Process.GetProcesses()).Where<Process>((Func<Process, bool>)(f => f.ProcessName == "Viber"));
            if (proc.Any<Process>())
                proc.First<Process>().Kill();
            this.button_new_acc.Enabled = false;
            WorkBD.OffAccs();
            Process.Start(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + "\\Users\\" + Environment.UserName + "\\AppData\\Local\\Viber\\Viber.exe");
            DataGridView dataGridView = this.dataGridView_all_accs;
            BindingSource bindingSource = await WorkBD.WaitNewAcc();
            object obj = (object)bindingSource;
            dataGridView.DataSource = obj;
            dataGridView = (DataGridView)null;
            bindingSource = (BindingSource)null;
            obj = (object)null;
            this.button_new_acc.Enabled = true;
        }

        private void button_load_numbers_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox_path_numbers.Text))
            {
                int num = (int)MessageBox.Show("Укажите файл с номерами!");
            }
            else
                this.LoadNumbers();
        }

        private void LoadNumbers()
        {
            string[] a = File.ReadAllLines(this.textBox_path_numbers.Text);
            this.button_load_numbers.Enabled = false;
            new Thread((ThreadStart)(() =>
           {
               DataTable dt = new DataTable();
               DataColumnCollection columns1 = dt.Columns;
               DataColumn[] columns2 = new DataColumn[3]
          {
          new DataColumn() { ColumnName = "Номера" },
          new DataColumn() { ColumnName = "Отправлено" },
          null
             };
               int index1 = 2;
               DataColumn dataColumn = new DataColumn();
               dataColumn.ColumnName = "№";
               dataColumn.AutoIncrement = true;
               long num = 1;
               dataColumn.AutoIncrementSeed = num;
               columns2[index1] = dataColumn;
               columns1.AddRange(columns2);
               for (int index2 = 0; index2 < a.Length; ++index2)
                   dt.Rows.Add((object)a[index2], (object)false);
               this.dataGridView_all_numbers.Invoke((Delegate)(new Action(() => this.dataGridView_all_numbers.DataSource = (object)dt)));
               this.button_load_numbers.Invoke((Delegate)(new Action(() => this.button_load_numbers.Enabled = true)));
               Thread.CurrentThread.Abort();
           }))
            {
                IsBackground = true
            }.Start();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_all_numbers.Rows.Count < 2)
            {
                int num1 = (int)MessageBox.Show("Загрузите номера!");
            }
            else
            {
                StartSettings startSettings1 = new StartSettings();
                int num2 = this.checkBox_data.Checked ? 1 : 0;
                startSettings1.add_date = num2 != 0;
                Decimal num3 = this.numericUpDown_pause_off.Value;
                startSettings1.pause_off = num3;
                string text1 = this.textBox_path_numbers.Text;
                startSettings1.file_numbers = text1;
                int num4 = this.checkBox_toFirst.Checked ? 1 : 0;
                startSettings1.first_accs = num4 != 0;
                string text2 = this.textBox_file.Text;
                startSettings1.folder_files = text2;
                int num5 = this.checkBox_link.Checked ? 1 : 0;
                startSettings1.link = num5 != 0;
                string text3 = this.textBox_link.Text;
                startSettings1.link_str = text3;
                string text4 = this.richTextBox_text.Text;
                startSettings1.richtextbox = text4;
                int num6 = this.checkBox_swap_acc.Checked ? 1 : 0;
                startSettings1.swap_accs = num6 != 0;
                Decimal num7 = this.numericUpDown_kol_swap.Value;
                startSettings1.swap_accs_kol = num7;
                int num8 = this.checkBox_unvalid_accs.Checked ? 1 : 0;
                startSettings1.unvalid = num8 != 0;
                Decimal num9 = this.numericUpDown_kol_unvalid.Value;
                startSettings1.unvalid_kol = num9;
                Decimal num10 = this.numericUpDown_pause.Value;
                startSettings1.pause = num10;
                int num11 = this.checkBox_clear_history.Checked ? 1 : 0;
                startSettings1.clear_histiry = num11 != 0;
                StartSettings startSettings2 = startSettings1;
                using (Stream stream = (Stream)new FileStream("settings.xml", FileMode.Create))
                    new XmlSerializer(typeof(StartSettings)).Serialize(stream, (object)startSettings2);
                this.button_start.Enabled = false;
                this._th = new Thread(new ThreadStart(this.Solution));
                this._th.SetApartmentState(ApartmentState.STA);
                this._th.IsBackground = true;
                this._th.Start();
            }
        }

        private void Solution()
        {
            this.progressBar1.Invoke((Delegate)(new Action(() =>
            {
               this.progressBar1.Value = 0;
               this.progressBar1.Maximum = this.dataGridView_all_numbers.Rows.Count - 1;
            })));
            MakeSends makeSends = new MakeSends();
            int num1 = 0;
            int num2 = 0;
            int num3;
            for (int i = this.start; i < this.dataGridView_all_accs.Rows.Count; i = num3 + 1)
            {
                try
                {
                    bool first = true;
                    for (int index = 0; index < this.dataGridView_all_accs.SelectedRows.Count; ++index)
                        this.dataGridView_all_accs.SelectedRows[index].Selected = false;
                    this.dataGridView_all_accs.Rows[i].Selected = true;
                    this.dataGridView_all_accs.Invoke((Delegate)(new Action(() => this.dataGridView_all_accs.FirstDisplayedScrollingRowIndex = i)));
                    if (!(this.dataGridView_all_accs.Rows[i].Cells[1].Value.ToString() == "0"))
                    {
                        IEnumerable<Process> source = ((IEnumerable<Process>)Process.GetProcesses()).Where<Process>((Func<Process, bool>)(f => f.ProcessName == "Viber"));
                        if (source.Any<Process>())
                        {
                            source.First<Process>().Kill();
                            source.First<Process>().WaitForExit(5000);
                        }
                        string phone = this.dataGridView_all_accs.Rows[i].Cells[3].Value.ToString();
                        WorkBD.SetAcc(phone);
                        WorkBD.SetLanguage(phone);
                        if (this.checkBox_clear_history.Checked)
                            WorkBD.ClearHistory(phone);
                        Process.Start(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + "\\Users\\" + Environment.UserName + "\\AppData\\Local\\Viber\\Viber.exe");
                        while (WinApi.FindWindow("Qt5QWindowOwnDCIcon", (string)null) == IntPtr.Zero)
                            Thread.Sleep(1000);
                        Thread.Sleep(3000);
                        Thread.Sleep((int)this.numericUpDown_pause.Value * 1000);
                        if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero || WinApi.FindWindow("Qt5QWindowOwnDCIcon", "Viber") != IntPtr.Zero)
                        {
                            WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            Thread.Sleep(2000);
                            goto label_43;
                        }
                        else
                        {
                            int num4;
                            for (int j = num1; j < this.dataGridView_all_numbers.Rows.Count - 1; j = num4 + 1)
                            {
                                for (int index = 0; index < this.dataGridView_all_numbers.SelectedRows.Count; ++index)
                                    this.dataGridView_all_numbers.SelectedRows[index].Selected = false;
                                this.dataGridView_all_numbers.Rows[j].Selected = true;
                                this.dataGridView_all_numbers.Invoke((Delegate)(new Action(() => this.dataGridView_all_numbers.FirstDisplayedScrollingRowIndex = j)));
                                num1 = j;
                                string input = this.richTextBox_text.Text; //(string) this.richTextBox_text.Invoke((Delegate) (() => this.richTextBox_text.Text)));
                                foreach (Match match in Regex.Matches(input, "{.*?}"))
                                {
                                    string[] strArray = match.Value.Split('|');
                                    input = input.Replace(match.Value, strArray[this.r.Next(0, strArray.Length)].Replace("{", "").Replace("}", ""));
                                }
                                if (this.checkBox_data.Checked)
                                    input = input + "\n" + (object)DateTime.Now;
                                string text = input.Replace("<time>", string.Concat((object)DateTime.Now));
                                bool flag = string.IsNullOrEmpty(this.textBox_file.Text) ? makeSends.SendSms(this.dataGridView_all_numbers.Rows[j].Cells[0].Value.ToString(), text, first, (string)null) : makeSends.SendSms(this.dataGridView_all_numbers.Rows[j].Cells[0].Value.ToString(), text, first, this.textBox_file.Text);
                                first = false;
                                if (!flag)
                                {
                                    ++num2;
                                    this.dataGridView_all_numbers.Rows[j].Cells[1].Value = (object)false;
                                }
                                else
                                    this.dataGridView_all_numbers.Rows[j].Cells[1].Value = (object)true;
                                this.progressBar1.Invoke((Delegate)(new Action(() =>
                               {
                                   ++this.progressBar1.Value;
                                   if (this.progressBar1.Value != this.progressBar1.Maximum)
                                       return;
                                   this.button_start.Enabled = true;
                                   this._th.Abort();
                               })));
                                if (this.checkBox_link.Checked && !string.IsNullOrEmpty(this.textBox_link.Text))
                                {
                                    Thread.Sleep(500);
                                    WinApi.SendMsg(this.textBox_link.Text, (string)null, false);
                                }
                                if (this.checkBox_unvalid_accs.Checked && (Decimal)num2 >= this.numericUpDown_kol_unvalid.Value)
                                {
                                    num2 = 0;
                                    break;
                                }
                                if (this.checkBox_swap_acc.Checked && (Decimal)(j + 1) % this.numericUpDown_kol_swap.Value == Decimal.Zero)
                                {
                                    ++num1;
                                    break;
                                }
                                num4 = j;
                            }
                            if (this.checkBox_toFirst.Checked && i + 2 > this.dataGridView_all_accs.Rows.Count)
                                i = -1;
                            Thread.Sleep((int)this.numericUpDown_pause_off.Value * 1000);
                        }
                    }
                    else
                        goto label_43;
                }
                catch
                {
                }
                label_43:
                num3 = i;
            }
            this.button_start.Invoke((Delegate)(new Action(() => this.button_start.Enabled = true)));
        }

        private void button_picture_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            this.textBox_file.Text = folderBrowserDialog.SelectedPath;
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            try
            {
                this._th.Resume();
            }
            catch
            {
            }
            this._th.Abort();
            this.button1.Text = "Пауза";
            this.button_start.Enabled = true;
        }

        private void dataGridView_all_accs_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            try
            {
                DataGridView.HitTestInfo hitTestInfo = this.dataGridView_all_accs.HitTest(e.X, e.Y);
                this.dataGridView_all_accs.ClearSelection();
                this.dataGridView_all_accs.Rows[hitTestInfo.RowIndex].Selected = true;
            }
            catch
            {
            }
            this.contextMenu.Show((Control)this.dataGridView_all_accs, e.X, e.Y);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.pause = !this.pause;
                if (this.pause)
                {
                    this._th.Suspend();
                    this.button1.Text = "Продолжить";
                }
                else
                {
                    this.button1.Text = "Пауза";
                    this.button1.Enabled = true;
                    this._th.Resume();
                }
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
            this.tabControl_general = new TabControl();
            this.tabPage_settings = new TabPage();
            this.groupBox2 = new GroupBox();
            this.checkBox_clear_history = new CheckBox();
            this.label2 = new Label();
            this.numericUpDown_pause = new NumericUpDown();
            this.checkBox_data = new CheckBox();
            this.checkBox_toFirst = new CheckBox();
            this.numericUpDown_kol_unvalid = new NumericUpDown();
            this.checkBox_unvalid_accs = new CheckBox();
            this.numericUpDown_kol_swap = new NumericUpDown();
            this.checkBox_swap_acc = new CheckBox();
            this.groupBox1 = new GroupBox();
            this.button_new_acc = new Button();
            this.dataGridView_all_accs = new DataGridView();
            this.NumberSerial = new DataGridViewTextBoxColumn();
            this.UseSend = new DataGridViewCheckBoxColumn();
            this.Login = new DataGridViewTextBoxColumn();
            this.Number = new DataGridViewTextBoxColumn();
            this.tabPage_numbers = new TabPage();
            this.dataGridView_all_numbers = new DataGridView();
            this.NumberSerial1 = new DataGridViewTextBoxColumn();
            this.Numbers = new DataGridViewTextBoxColumn();
            this.Sended = new DataGridViewTextBoxColumn();
            this.groupBox3 = new GroupBox();
            this.button_load_numbers = new Button();
            this.textBox_path_numbers = new TextBox();
            this.button_browse_numbers = new Button();
            this.tabPage_subjects = new TabPage();
            this.textBox_link = new TextBox();
            this.checkBox_link = new CheckBox();
            this.button_picture = new Button();
            this.label1 = new Label();
            this.textBox_file = new TextBox();
            this.groupBox4 = new GroupBox();
            this.richTextBox_text = new RichTextBox();
            this.progressBar1 = new ProgressBar();
            this.button_start = new Button();
            this.button_stop = new Button();
            this.contextMenu = new ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.button1 = new Button();
            this.numericUpDown_pause_off = new NumericUpDown();
            this.label3 = new Label();
            this.tabControl_general.SuspendLayout();
            this.tabPage_settings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.numericUpDown_pause.BeginInit();
            this.numericUpDown_kol_unvalid.BeginInit();
            this.numericUpDown_kol_swap.BeginInit();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)this.dataGridView_all_accs).BeginInit();
            this.tabPage_numbers.SuspendLayout();
            ((ISupportInitialize)this.dataGridView_all_numbers).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabPage_subjects.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.numericUpDown_pause_off.BeginInit();
            this.SuspendLayout();
            this.tabControl_general.Controls.Add((Control)this.tabPage_settings);
            this.tabControl_general.Controls.Add((Control)this.tabPage_numbers);
            this.tabControl_general.Controls.Add((Control)this.tabPage_subjects);
            this.tabControl_general.Dock = DockStyle.Top;
            this.tabControl_general.Location = new Point(0, 0);
            this.tabControl_general.Name = "tabControl_general";
            this.tabControl_general.SelectedIndex = 0;
            this.tabControl_general.Size = new Size(652, 382);
            this.tabControl_general.TabIndex = 0;
            this.tabPage_settings.Controls.Add((Control)this.groupBox2);
            this.tabPage_settings.Controls.Add((Control)this.groupBox1);
            this.tabPage_settings.Location = new Point(4, 22);
            this.tabPage_settings.Name = "tabPage_settings";
            this.tabPage_settings.Padding = new Padding(3);
            this.tabPage_settings.Size = new Size(644, 356);
            this.tabPage_settings.TabIndex = 0;
            this.tabPage_settings.Text = "Настройки рассылки";
            this.tabPage_settings.UseVisualStyleBackColor = true;
            this.groupBox2.Controls.Add((Control)this.label3);
            this.groupBox2.Controls.Add((Control)this.numericUpDown_pause_off);
            this.groupBox2.Controls.Add((Control)this.checkBox_clear_history);
            this.groupBox2.Controls.Add((Control)this.label2);
            this.groupBox2.Controls.Add((Control)this.numericUpDown_pause);
            this.groupBox2.Controls.Add((Control)this.checkBox_data);
            this.groupBox2.Controls.Add((Control)this.checkBox_toFirst);
            this.groupBox2.Controls.Add((Control)this.numericUpDown_kol_unvalid);
            this.groupBox2.Controls.Add((Control)this.checkBox_unvalid_accs);
            this.groupBox2.Controls.Add((Control)this.numericUpDown_kol_swap);
            this.groupBox2.Controls.Add((Control)this.checkBox_swap_acc);
            this.groupBox2.Location = new Point(9, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(627, 119);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Настройки анти- бан системы";
            this.checkBox_clear_history.AutoSize = true;
            this.checkBox_clear_history.Location = new Point(354, 73);
            this.checkBox_clear_history.Name = "checkBox_clear_history";
            this.checkBox_clear_history.Size = new Size(122, 17);
            this.checkBox_clear_history.TabIndex = 8;
            this.checkBox_clear_history.Text = "Отчищать историю";
            this.checkBox_clear_history.UseVisualStyleBackColor = true;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(351, 19);
            this.label2.Name = "label2";
            this.label2.Size = new Size(164, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Пауза между сменой аккаунта";
            this.numericUpDown_pause.Location = new Point(539, 17);
            this.numericUpDown_pause.Name = "numericUpDown_pause";
            this.numericUpDown_pause.Size = new Size(73, 20);
            this.numericUpDown_pause.TabIndex = 6;
            this.checkBox_data.AutoSize = true;
            this.checkBox_data.Location = new Point(7, 73);
            this.checkBox_data.Name = "checkBox_data";
            this.checkBox_data.Size = new Size(178, 17);
            this.checkBox_data.TabIndex = 5;
            this.checkBox_data.Text = "Добавлять дату к сообщению";
            this.checkBox_data.UseVisualStyleBackColor = true;
            this.checkBox_toFirst.AutoSize = true;
            this.checkBox_toFirst.Location = new Point(7, 96);
            this.checkBox_toFirst.Name = "checkBox_toFirst";
            this.checkBox_toFirst.Size = new Size(285, 17);
            this.checkBox_toFirst.TabIndex = 4;
            this.checkBox_toFirst.Text = "После последнего аккаунта переходить к первому";
            this.checkBox_toFirst.UseVisualStyleBackColor = true;
            this.numericUpDown_kol_unvalid.Enabled = false;
            this.numericUpDown_kol_unvalid.Location = new Point(277, 44);
            this.numericUpDown_kol_unvalid.Maximum = new Decimal(new int[4]
            {
        100000000,
        0,
        0,
        0
            });
            this.numericUpDown_kol_unvalid.Minimum = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.numericUpDown_kol_unvalid.Name = "numericUpDown_kol_unvalid";
            this.numericUpDown_kol_unvalid.Size = new Size(67, 20);
            this.numericUpDown_kol_unvalid.TabIndex = 3;
            this.numericUpDown_kol_unvalid.Value = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.checkBox_unvalid_accs.AutoSize = true;
            this.checkBox_unvalid_accs.Location = new Point(7, 45);
            this.checkBox_unvalid_accs.Name = "checkBox_unvalid_accs";
            this.checkBox_unvalid_accs.Size = new Size(264, 17);
            this.checkBox_unvalid_accs.TabIndex = 2;
            this.checkBox_unvalid_accs.Text = "Число не валидных отправок для перезапуска";
            this.checkBox_unvalid_accs.UseVisualStyleBackColor = true;
            this.checkBox_unvalid_accs.CheckedChanged += new EventHandler(this.checkBox_unvalid_accs_CheckedChanged);
            this.numericUpDown_kol_swap.Enabled = false;
            this.numericUpDown_kol_swap.Location = new Point(153, 19);
            this.numericUpDown_kol_swap.Maximum = new Decimal(new int[4]
            {
        100000000,
        0,
        0,
        0
            });
            this.numericUpDown_kol_swap.Minimum = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.numericUpDown_kol_swap.Name = "numericUpDown_kol_swap";
            this.numericUpDown_kol_swap.Size = new Size(67, 20);
            this.numericUpDown_kol_swap.TabIndex = 1;
            this.numericUpDown_kol_swap.Value = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.checkBox_swap_acc.AutoSize = true;
            this.checkBox_swap_acc.Location = new Point(7, 20);
            this.checkBox_swap_acc.Name = "checkBox_swap_acc";
            this.checkBox_swap_acc.Size = new Size(140, 17);
            this.checkBox_swap_acc.TabIndex = 0;
            this.checkBox_swap_acc.Text = "Менять аккаунт после";
            this.checkBox_swap_acc.UseVisualStyleBackColor = true;
            this.checkBox_swap_acc.CheckedChanged += new EventHandler(this.checkBox_swap_acc_CheckedChanged);
            this.groupBox1.Controls.Add((Control)this.button_new_acc);
            this.groupBox1.Controls.Add((Control)this.dataGridView_all_accs);
            this.groupBox1.Dock = DockStyle.Bottom;
            this.groupBox1.Location = new Point(3, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(638, 221);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настроки аккаунтов Viber";
            this.button_new_acc.Location = new Point(6, 20);
            this.button_new_acc.Name = "button_new_acc";
            this.button_new_acc.Size = new Size(195, 37);
            this.button_new_acc.TabIndex = 1;
            this.button_new_acc.Text = "Зарегистрировать новый Viber";
            this.button_new_acc.UseVisualStyleBackColor = true;
            this.button_new_acc.Click += new EventHandler(this.button_new_acc_Click);
            this.dataGridView_all_accs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_all_accs.Columns.AddRange((DataGridViewColumn)this.NumberSerial, (DataGridViewColumn)this.UseSend, (DataGridViewColumn)this.Login, (DataGridViewColumn)this.Number);
            this.dataGridView_all_accs.Location = new Point(3, 63);
            this.dataGridView_all_accs.Name = "dataGridView_all_accs";
            this.dataGridView_all_accs.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_all_accs.RowHeadersVisible = false;
            this.dataGridView_all_accs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_all_accs.Size = new Size(630, 143);
            this.dataGridView_all_accs.TabIndex = 0;
            this.dataGridView_all_accs.MouseClick += new MouseEventHandler(this.dataGridView_all_accs_MouseClick);
            this.NumberSerial.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.NumberSerial.DataPropertyName = "№";
            this.NumberSerial.HeaderText = "№";
            this.NumberSerial.Name = "NumberSerial";
            this.NumberSerial.ReadOnly = true;
            this.NumberSerial.Width = 43;
            this.UseSend.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.UseSend.DataPropertyName = "IsValid";
            this.UseSend.FalseValue = (object)"0";
            this.UseSend.HeaderText = "Использовать для рассылки";
            this.UseSend.Name = "UseSend";
            this.UseSend.TrueValue = (object)"1";
            this.Login.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Login.DataPropertyName = "NickName";
            this.Login.HeaderText = "Логин";
            this.Login.Name = "Login";
            this.Login.ReadOnly = true;
            this.Number.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Number.DataPropertyName = "ID";
            this.Number.HeaderText = "Номер";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            this.tabPage_numbers.Controls.Add((Control)this.dataGridView_all_numbers);
            this.tabPage_numbers.Controls.Add((Control)this.groupBox3);
            this.tabPage_numbers.Location = new Point(4, 22);
            this.tabPage_numbers.Name = "tabPage_numbers";
            this.tabPage_numbers.Padding = new Padding(3);
            this.tabPage_numbers.Size = new Size(644, 356);
            this.tabPage_numbers.TabIndex = 1;
            this.tabPage_numbers.Text = "Номера";
            this.tabPage_numbers.UseVisualStyleBackColor = true;
            this.dataGridView_all_numbers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_all_numbers.Columns.AddRange((DataGridViewColumn)this.NumberSerial1, (DataGridViewColumn)this.Numbers, (DataGridViewColumn)this.Sended);
            this.dataGridView_all_numbers.Location = new Point(317, 7);
            this.dataGridView_all_numbers.Name = "dataGridView_all_numbers";
            this.dataGridView_all_numbers.RowHeadersVisible = false;
            this.dataGridView_all_numbers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_all_numbers.Size = new Size(319, 323);
            this.dataGridView_all_numbers.TabIndex = 1;
            this.NumberSerial1.DataPropertyName = "№";
            this.NumberSerial1.HeaderText = "№";
            this.NumberSerial1.Name = "NumberSerial1";
            this.NumberSerial1.ReadOnly = true;
            this.NumberSerial1.Width = 43;
            this.Numbers.DataPropertyName = "Номера";
            this.Numbers.HeaderText = "Номера";
            this.Numbers.Name = "Numbers";
            this.Numbers.ReadOnly = true;
            this.Numbers.Width = 137;
            this.Sended.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Sended.DataPropertyName = "Отправлено";
            this.Sended.HeaderText = "Отправлено";
            this.Sended.Name = "Sended";
            this.Sended.ReadOnly = true;
            this.groupBox3.Controls.Add((Control)this.button_load_numbers);
            this.groupBox3.Controls.Add((Control)this.textBox_path_numbers);
            this.groupBox3.Controls.Add((Control)this.button_browse_numbers);
            this.groupBox3.Location = new Point(9, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(293, 141);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Файл с номерами";
            this.button_load_numbers.Location = new Point(7, 68);
            this.button_load_numbers.Name = "button_load_numbers";
            this.button_load_numbers.Size = new Size(85, 28);
            this.button_load_numbers.TabIndex = 2;
            this.button_load_numbers.Text = "Загрузить";
            this.button_load_numbers.UseVisualStyleBackColor = true;
            this.button_load_numbers.Click += new EventHandler(this.button_load_numbers_Click);
            this.textBox_path_numbers.Location = new Point(47, 23);
            this.textBox_path_numbers.Name = "textBox_path_numbers";
            this.textBox_path_numbers.Size = new Size(240, 20);
            this.textBox_path_numbers.TabIndex = 1;
            this.button_browse_numbers.Location = new Point(7, 20);
            this.button_browse_numbers.Name = "button_browse_numbers";
            this.button_browse_numbers.Size = new Size(34, 24);
            this.button_browse_numbers.TabIndex = 0;
            this.button_browse_numbers.Text = "...";
            this.button_browse_numbers.UseVisualStyleBackColor = true;
            this.button_browse_numbers.Click += new EventHandler(this.button_browse_numbers_Click);
            this.tabPage_subjects.Controls.Add((Control)this.textBox_link);
            this.tabPage_subjects.Controls.Add((Control)this.checkBox_link);
            this.tabPage_subjects.Controls.Add((Control)this.button_picture);
            this.tabPage_subjects.Controls.Add((Control)this.label1);
            this.tabPage_subjects.Controls.Add((Control)this.textBox_file);
            this.tabPage_subjects.Controls.Add((Control)this.groupBox4);
            this.tabPage_subjects.Location = new Point(4, 22);
            this.tabPage_subjects.Name = "tabPage_subjects";
            this.tabPage_subjects.Padding = new Padding(3);
            this.tabPage_subjects.Size = new Size(644, 356);
            this.tabPage_subjects.TabIndex = 2;
            this.tabPage_subjects.Text = "Сообщения";
            this.tabPage_subjects.UseVisualStyleBackColor = true;
            this.textBox_link.Enabled = false;
            this.textBox_link.Location = new Point(357, 289);
            this.textBox_link.Name = "textBox_link";
            this.textBox_link.Size = new Size(276, 20);
            this.textBox_link.TabIndex = 5;
            this.checkBox_link.AutoSize = true;
            this.checkBox_link.Location = new Point(357, 266);
            this.checkBox_link.Name = "checkBox_link";
            this.checkBox_link.Size = new Size(231, 17);
            this.checkBox_link.TabIndex = 4;
            this.checkBox_link.Text = "Прикреплять ссылку (доп. сообщением)";
            this.checkBox_link.UseVisualStyleBackColor = true;
            this.checkBox_link.CheckedChanged += new EventHandler(this.checkBox_link_CheckedChanged);
            this.button_picture.Location = new Point(4, 289);
            this.button_picture.Name = "button_picture";
            this.button_picture.Size = new Size(32, 20);
            this.button_picture.TabIndex = 3;
            this.button_picture.Text = "...";
            this.button_picture.UseVisualStyleBackColor = true;
            this.button_picture.Click += new EventHandler(this.button_picture_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(42, 270);
            this.label1.Name = "label1";
            this.label1.Size = new Size(231, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Папка с файлами (отправляется случайная)";
            this.textBox_file.Location = new Point(42, 289);
            this.textBox_file.Name = "textBox_file";
            this.textBox_file.Size = new Size(266, 20);
            this.textBox_file.TabIndex = 1;
            this.groupBox4.Controls.Add((Control)this.richTextBox_text);
            this.groupBox4.Location = new Point(9, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new Size(627, 249);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Текст для рассылки (рандомизация: {var1|var2|var3})";
            this.richTextBox_text.Dock = DockStyle.Fill;
            this.richTextBox_text.Location = new Point(3, 16);
            this.richTextBox_text.Name = "richTextBox_text";
            this.richTextBox_text.Size = new Size(621, 230);
            this.richTextBox_text.TabIndex = 0;
            this.richTextBox_text.Text = "";
            this.progressBar1.Location = new Point(7, 389);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(641, 23);
            this.progressBar1.TabIndex = 1;
            this.button_start.BackgroundImageLayout = ImageLayout.Stretch;
            this.button_start.Location = new Point(7, 419);
            this.button_start.Name = "button_start";
            this.button_start.Size = new Size(98, 32);
            this.button_start.TabIndex = 2;
            this.button_start.Text = "Старт";
            this.button_start.TextImageRelation = TextImageRelation.TextBeforeImage;
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new EventHandler(this.button_start_Click);
            this.button_stop.Location = new Point(234, 419);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new Size(98, 32);
            this.button_stop.TabIndex = 3;
            this.button_stop.Text = "Стоп";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new EventHandler(this.button_stop_Click);
            this.contextMenu.Items.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.toolStripMenuItem1
            });
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new Size(264, 48);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(263, 22);
            this.toolStripMenuItem2.Text = "Начать рассылку с этого аккаунта";
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(263, 22);
            this.toolStripMenuItem1.Text = "Использовать только этот аккаунт";
            this.button1.Location = new Point(120, 419);
            this.button1.Name = "button1";
            this.button1.Size = new Size(98, 32);
            this.button1.TabIndex = 5;
            this.button1.Text = "Пауза";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.numericUpDown_pause_off.Location = new Point(539, 45);
            this.numericUpDown_pause_off.Name = "numericUpDown_pause_off";
            this.numericUpDown_pause_off.Size = new Size(73, 20);
            this.numericUpDown_pause_off.TabIndex = 9;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(351, 47);
            this.label3.Name = "label3";
            this.label3.Size = new Size(182, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Пауза после рассылки с аккаунта";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(652, 454);
            this.Controls.Add((Control)this.button1);
            this.Controls.Add((Control)this.button_stop);
            this.Controls.Add((Control)this.button_start);
            this.Controls.Add((Control)this.progressBar1);
            this.Controls.Add((Control)this.tabControl_general);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "ViberSender2017 by ITLabs";
            this.Load += new EventHandler(this.Form1_Load);
            this.tabControl_general.ResumeLayout(false);
            this.tabPage_settings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.numericUpDown_pause.EndInit();
            this.numericUpDown_kol_unvalid.EndInit();
            this.numericUpDown_kol_swap.EndInit();
            this.groupBox1.ResumeLayout(false);
            ((ISupportInitialize)this.dataGridView_all_accs).EndInit();
            this.tabPage_numbers.ResumeLayout(false);
            ((ISupportInitialize)this.dataGridView_all_numbers).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage_subjects.ResumeLayout(false);
            this.tabPage_subjects.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.numericUpDown_pause_off.EndInit();
            this.ResumeLayout(false);
        }
    }
}
