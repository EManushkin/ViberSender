using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ViberSender2017
{
    public class Activate : Form
    {
        public bool flag = false;
        private IContainer components = (IContainer)null;
        private Label label1;
        private TextBox textBox_key;
        private Button button_ok;
        private LinkLabel linkLabel;

        public Activate()
        {
            this.InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Activate_Load(object sender, EventArgs e)
        {
            this.label1.Text = this.label1.Text + CreateMD5(Workstation.GenerateWorkstationId());
        }

        public string GetKey()
        {
            string md5 = CreateMD5(Workstation.GenerateWorkstationId());
            for (int index = 0; index < 1000; ++index)
                md5 = CreateMD5(md5);
            return md5;
        }

        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < hash.Length; ++index)
                    stringBuilder.Append(hash[index].ToString("X2"));
                return stringBuilder.ToString();
            }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(this.label1.Text.Split(':')[1].Trim());
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            string text = this.textBox_key.Text;
            string input = this.label1.Text.Split(':')[1].Trim();
            for (int index = 0; index < 1000; ++index)
                input = CreateMD5(input);
            if (text == input)
            {
                this.flag = true;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                int num = (int)MessageBox.Show("Неверный ключ!");
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
            this.label1 = new Label();
            this.textBox_key = new TextBox();
            this.button_ok = new Button();
            this.linkLabel = new LinkLabel();
            this.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(141, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ИД вашего оборудования:";
            this.label1.Click += new EventHandler(this.label1_Click);
            this.textBox_key.Location = new Point(49, 68);
            this.textBox_key.Name = "textBox_key";
            this.textBox_key.Size = new Size(305, 20);
            this.textBox_key.TabIndex = 1;
            this.button_ok.Location = new Point(166, 94);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new Size(75, 23);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "Ок!";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new EventHandler(this.button_ok_Click);
            this.linkLabel.AutoSize = true;
            this.linkLabel.Location = new Point(13, 34);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new Size(93, 13);
            this.linkLabel.TabIndex = 3;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "Скопировать ИД";
            this.linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(402, 125);
            this.Controls.Add((Control)this.linkLabel);
            this.Controls.Add((Control)this.button_ok);
            this.Controls.Add((Control)this.textBox_key);
            this.Controls.Add((Control)this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Name = "Activate";
            this.Text = "Требуется активация!";
            this.Load += new EventHandler(this.Activate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
