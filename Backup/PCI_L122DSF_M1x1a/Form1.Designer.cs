namespace PCI_L122DSF_M1x1a
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_CheckSlave = new System.Windows.Forms.Button();
            this.lbl_Baudrate = new System.Windows.Forms.Label();
            this.cBox_baudrate = new System.Windows.Forms.ComboBox();
            this.label45 = new System.Windows.Forms.Label();
            this.cBox_cardno = new System.Windows.Forms.ComboBox();
            this.btn_Initial = new System.Windows.Forms.Button();
            this.ChkSVON0 = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnReset0 = new System.Windows.Forms.Button();
            this.label48 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.btnMoveP0 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBox9 = new System.Windows.Forms.ComboBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpIoSts = new System.Windows.Forms.GroupBox();
            this.btnMoveM0 = new System.Windows.Forms.Button();
            this.btnStop0 = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_Getfile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.Items.AddRange(new object[] {
            "Relative",
            "Absolute",
            "Continue",
            "Home"});
            this.comboBox1.Location = new System.Drawing.Point(395, 79);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(88, 23);
            this.comboBox1.TabIndex = 7;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_Getfile);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.btn_CheckSlave);
            this.groupBox3.Controls.Add(this.lbl_Baudrate);
            this.groupBox3.Controls.Add(this.cBox_baudrate);
            this.groupBox3.Controls.Add(this.label45);
            this.groupBox3.Controls.Add(this.cBox_cardno);
            this.groupBox3.Controls.Add(this.btn_Initial);
            this.groupBox3.Controls.Add(this.btn_exit);
            this.groupBox3.Location = new System.Drawing.Point(13, 16);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(203, 461);
            this.groupBox3.TabIndex = 147;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Master";
            // 
            // btn_CheckSlave
            // 
            this.btn_CheckSlave.Enabled = false;
            this.btn_CheckSlave.Location = new System.Drawing.Point(12, 340);
            this.btn_CheckSlave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_CheckSlave.Name = "btn_CheckSlave";
            this.btn_CheckSlave.Size = new System.Drawing.Size(171, 44);
            this.btn_CheckSlave.TabIndex = 8;
            this.btn_CheckSlave.Text = "Check Slave Device";
            this.btn_CheckSlave.UseVisualStyleBackColor = true;
            this.btn_CheckSlave.Click += new System.EventHandler(this.btn_CheckSlave_Click);
            // 
            // lbl_Baudrate
            // 
            this.lbl_Baudrate.AutoSize = true;
            this.lbl_Baudrate.Location = new System.Drawing.Point(12, 294);
            this.lbl_Baudrate.Name = "lbl_Baudrate";
            this.lbl_Baudrate.Size = new System.Drawing.Size(119, 15);
            this.lbl_Baudrate.TabIndex = 4;
            this.lbl_Baudrate.Text = "Baudrate of Ring0 :";
            // 
            // cBox_baudrate
            // 
            this.cBox_baudrate.FormattingEnabled = true;
            this.cBox_baudrate.Items.AddRange(new object[] {
            "2.5HZ [11]",
            "5HZ [10]",
            "10HZ [01]",
            "20Hz [00]"});
            this.cBox_baudrate.Location = new System.Drawing.Point(12, 312);
            this.cBox_baudrate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cBox_baudrate.Name = "cBox_baudrate";
            this.cBox_baudrate.Size = new System.Drawing.Size(121, 23);
            this.cBox_baudrate.TabIndex = 3;
            this.cBox_baudrate.SelectedIndexChanged += new System.EventHandler(this.cBox_baudrate_SelectedIndexChanged);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(11, 40);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(63, 15);
            this.label45.TabIndex = 2;
            this.label45.Text = "Card No :";
            // 
            // cBox_cardno
            // 
            this.cBox_cardno.FormattingEnabled = true;
            this.cBox_cardno.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.cBox_cardno.Location = new System.Drawing.Point(12, 58);
            this.cBox_cardno.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cBox_cardno.Name = "cBox_cardno";
            this.cBox_cardno.Size = new System.Drawing.Size(121, 23);
            this.cBox_cardno.TabIndex = 1;
            // 
            // btn_Initial
            // 
            this.btn_Initial.Location = new System.Drawing.Point(12, 88);
            this.btn_Initial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Initial.Name = "btn_Initial";
            this.btn_Initial.Size = new System.Drawing.Size(171, 44);
            this.btn_Initial.TabIndex = 0;
            this.btn_Initial.Text = "Initial";
            this.btn_Initial.UseVisualStyleBackColor = true;
            this.btn_Initial.Click += new System.EventHandler(this.btn_Initial_Click);
            // 
            // ChkSVON0
            // 
            this.ChkSVON0.AutoSize = true;
            this.ChkSVON0.Location = new System.Drawing.Point(653, 62);
            this.ChkSVON0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChkSVON0.Name = "ChkSVON0";
            this.ChkSVON0.Size = new System.Drawing.Size(67, 19);
            this.ChkSVON0.TabIndex = 7;
            this.ChkSVON0.Text = "SVON";
            this.ChkSVON0.UseVisualStyleBackColor = true;
            this.ChkSVON0.CheckedChanged += new System.EventHandler(this.ServerOn_Checked);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnReset0
            // 
            this.btnReset0.Location = new System.Drawing.Point(563, 58);
            this.btnReset0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset0.Name = "btnReset0";
            this.btnReset0.Size = new System.Drawing.Size(64, 29);
            this.btnReset0.TabIndex = 6;
            this.btnReset0.Text = "Reset";
            this.btnReset0.UseVisualStyleBackColor = true;
            this.btnReset0.Click += new System.EventHandler(this.Reset_Click);
            // 
            // label48
            // 
            this.label48.BackColor = System.Drawing.Color.Transparent;
            this.label48.Location = new System.Drawing.Point(225, 120);
            this.label48.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(69, 29);
            this.label48.TabIndex = 130;
            this.label48.Text = "EZ_Logic";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.SystemColors.Control;
            this.label30.Location = new System.Drawing.Point(464, 26);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(51, 29);
            this.label30.TabIndex = 5;
            this.label30.Text = "Error";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMoveP0
            // 
            this.btnMoveP0.Location = new System.Drawing.Point(907, 171);
            this.btnMoveP0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMoveP0.Name = "btnMoveP0";
            this.btnMoveP0.Size = new System.Drawing.Size(72, 45);
            this.btnMoveP0.TabIndex = 142;
            this.btnMoveP0.Text = "+";
            this.btnMoveP0.UseVisualStyleBackColor = true;
            this.btnMoveP0.Click += new System.EventHandler(this.Move_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBox9);
            this.groupBox4.Controls.Add(this.comboBox8);
            this.groupBox4.Controls.Add(this.comboBox1);
            this.groupBox4.Controls.Add(this.comboBox7);
            this.groupBox4.Controls.Add(this.comboBox6);
            this.groupBox4.Controls.Add(this.comboBox5);
            this.groupBox4.Controls.Add(this.label39);
            this.groupBox4.Controls.Add(this.label50);
            this.groupBox4.Controls.Add(this.label46);
            this.groupBox4.Controls.Add(this.label38);
            this.groupBox4.Controls.Add(this.label49);
            this.groupBox4.Controls.Add(this.label47);
            this.groupBox4.Controls.Add(this.label37);
            this.groupBox4.Controls.Add(this.label48);
            this.groupBox4.Controls.Add(this.label36);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.label40);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.textBox2);
            this.groupBox4.Controls.Add(this.textBox4);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Location = new System.Drawing.Point(235, 16);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(517, 205);
            this.groupBox4.TabIndex = 146;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "M1x1a :";
            // 
            // comboBox9
            // 
            this.comboBox9.FormattingEnabled = true;
            this.comboBox9.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.comboBox9.Location = new System.Drawing.Point(404, 155);
            this.comboBox9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox9.Name = "comboBox9";
            this.comboBox9.Size = new System.Drawing.Size(79, 23);
            this.comboBox9.TabIndex = 137;
            this.comboBox9.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Selected_Changed);
            // 
            // comboBox8
            // 
            this.comboBox8.FormattingEnabled = true;
            this.comboBox8.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.comboBox8.Location = new System.Drawing.Point(332, 155);
            this.comboBox8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(64, 23);
            this.comboBox8.TabIndex = 136;
            this.comboBox8.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Selected_Changed);
            // 
            // comboBox7
            // 
            this.comboBox7.FormattingEnabled = true;
            this.comboBox7.Items.AddRange(new object[] {
            "Low Active",
            "High Active"});
            this.comboBox7.Location = new System.Drawing.Point(225, 155);
            this.comboBox7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(99, 23);
            this.comboBox7.TabIndex = 135;
            this.comboBox7.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Selected_Changed);
            // 
            // comboBox6
            // 
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Items.AddRange(new object[] {
            "Low Active",
            "High Active"});
            this.comboBox6.Location = new System.Drawing.Point(120, 155);
            this.comboBox6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(99, 23);
            this.comboBox6.TabIndex = 134;
            this.comboBox6.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Selected_Changed);
            // 
            // comboBox5
            // 
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "Mode 0",
            "Mode 1",
            "Mode 2",
            "Mode 3",
            "Mode 4",
            "Mode 5",
            "Mode 6",
            "Mode 7",
            "Mode 8",
            "Mode 9",
            "Mode 10",
            "Mode 11",
            "Mode 12"});
            this.comboBox5.Location = new System.Drawing.Point(35, 155);
            this.comboBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(79, 23);
            this.comboBox5.TabIndex = 133;
            this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Selected_Changed);
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.Transparent;
            this.label39.Location = new System.Drawing.Point(320, 45);
            this.label39.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(73, 29);
            this.label39.TabIndex = 4;
            this.label39.Text = "Dec. Time";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label50
            // 
            this.label50.BackColor = System.Drawing.Color.Transparent;
            this.label50.Location = new System.Drawing.Point(404, 120);
            this.label50.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(59, 29);
            this.label50.TabIndex = 131;
            this.label50.Text = "Erc_Out";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label46
            // 
            this.label46.BackColor = System.Drawing.Color.Transparent;
            this.label46.Location = new System.Drawing.Point(35, 120);
            this.label46.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(45, 29);
            this.label46.TabIndex = 128;
            this.label46.Text = "Mode ";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.Transparent;
            this.label38.Location = new System.Drawing.Point(241, 45);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(72, 29);
            this.label38.TabIndex = 3;
            this.label38.Text = "Acc. Time";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label49
            // 
            this.label49.BackColor = System.Drawing.Color.Transparent;
            this.label49.Location = new System.Drawing.Point(329, 120);
            this.label49.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(79, 29);
            this.label49.TabIndex = 132;
            this.label49.Text = "EZ_Count";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.Transparent;
            this.label47.Location = new System.Drawing.Point(120, 120);
            this.label47.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(80, 29);
            this.label47.TabIndex = 129;
            this.label47.Text = "Org_Logic";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label37
            // 
            this.label37.BackColor = System.Drawing.Color.Transparent;
            this.label37.Location = new System.Drawing.Point(172, 45);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(67, 29);
            this.label37.TabIndex = 2;
            this.label37.Text = "Max Vel.";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.Transparent;
            this.label36.Location = new System.Drawing.Point(104, 46);
            this.label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(67, 29);
            this.label36.TabIndex = 1;
            this.label36.Text = "Start Vel.";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.Transparent;
            this.label35.Location = new System.Drawing.Point(35, 45);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(61, 29);
            this.label35.TabIndex = 0;
            this.label35.Text = "Distance ";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label40
            // 
            this.label40.BackColor = System.Drawing.Color.Transparent;
            this.label40.Location = new System.Drawing.Point(392, 45);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(89, 29);
            this.label40.TabIndex = 5;
            this.label40.Text = "Move Mode";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(35, 79);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(63, 25);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "12000";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(179, 79);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(63, 25);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "5000";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(107, 79);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(63, 25);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "500";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(251, 79);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(63, 25);
            this.textBox4.TabIndex = 6;
            this.textBox4.Text = "0.2";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(323, 79);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(63, 25);
            this.textBox5.TabIndex = 6;
            this.textBox5.Text = "0.3";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ChkSVON0);
            this.groupBox1.Controls.Add(this.btnReset0);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(233, 239);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(759, 99);
            this.groupBox1.TabIndex = 144;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status && Counters";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label6.Location = new System.Drawing.Point(455, 55);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 29);
            this.label6.TabIndex = 5;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.SystemColors.Control;
            this.label29.Location = new System.Drawing.Point(387, 26);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(64, 29);
            this.label29.TabIndex = 4;
            this.label29.Text = "Position";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label5.Location = new System.Drawing.Point(385, 55);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 29);
            this.label5.TabIndex = 4;
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.SystemColors.Control;
            this.label28.Location = new System.Drawing.Point(315, 26);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(73, 29);
            this.label28.TabIndex = 3;
            this.label28.Text = "Command";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label4.Location = new System.Drawing.Point(315, 55);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 29);
            this.label4.TabIndex = 3;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.SystemColors.Control;
            this.label27.Location = new System.Drawing.Point(244, 26);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(64, 29);
            this.label27.TabIndex = 2;
            this.label27.Text = "Speed ";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label3.Location = new System.Drawing.Point(245, 55);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 29);
            this.label3.TabIndex = 2;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.SystemColors.Control;
            this.label26.Location = new System.Drawing.Point(173, 26);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(80, 29);
            this.label26.TabIndex = 1;
            this.label26.Text = "Motion Sts.";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label2.Location = new System.Drawing.Point(175, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 29);
            this.label2.TabIndex = 1;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.SystemColors.Control;
            this.label31.Location = new System.Drawing.Point(15, 58);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(64, 29);
            this.label31.TabIndex = 0;
            this.label31.Text = "Axis 0";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.SystemColors.Control;
            this.label25.Location = new System.Drawing.Point(88, 26);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(107, 29);
            this.label25.TabIndex = 0;
            this.label25.Text = "IO Status";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(105, 55);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 29);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpIoSts
            // 
            this.grpIoSts.Enabled = false;
            this.grpIoSts.Location = new System.Drawing.Point(233, 346);
            this.grpIoSts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpIoSts.Name = "grpIoSts";
            this.grpIoSts.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpIoSts.Size = new System.Drawing.Size(759, 131);
            this.grpIoSts.TabIndex = 145;
            this.grpIoSts.TabStop = false;
            this.grpIoSts.Text = "I/O Status";
            // 
            // btnMoveM0
            // 
            this.btnMoveM0.Location = new System.Drawing.Point(761, 171);
            this.btnMoveM0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMoveM0.Name = "btnMoveM0";
            this.btnMoveM0.Size = new System.Drawing.Size(72, 45);
            this.btnMoveM0.TabIndex = 143;
            this.btnMoveM0.Text = "─";
            this.btnMoveM0.UseVisualStyleBackColor = true;
            this.btnMoveM0.Click += new System.EventHandler(this.Move_Click);
            // 
            // btnStop0
            // 
            this.btnStop0.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnStop0.Location = new System.Drawing.Point(835, 171);
            this.btnStop0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop0.Name = "btnStop0";
            this.btnStop0.Size = new System.Drawing.Size(72, 45);
            this.btnStop0.TabIndex = 141;
            this.btnStop0.Text = "n";
            this.btnStop0.UseVisualStyleBackColor = true;
            this.btnStop0.Click += new System.EventHandler(this.Move_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.Location = new System.Drawing.Point(12, 403);
            this.btn_exit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(171, 44);
            this.btn_exit.TabIndex = 148;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "EEPROM",
            "INI file"});
            this.comboBox2.Location = new System.Drawing.Point(12, 177);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 23);
            this.comboBox2.TabIndex = 149;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 15);
            this.label7.TabIndex = 150;
            this.label7.Text = "Recovery settings from :";
            // 
            // btn_Getfile
            // 
            this.btn_Getfile.Location = new System.Drawing.Point(12, 208);
            this.btn_Getfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Getfile.Name = "btn_Getfile";
            this.btn_Getfile.Size = new System.Drawing.Size(171, 44);
            this.btn_Getfile.TabIndex = 151;
            this.btn_Getfile.Text = "Get ini file";
            this.btn_Getfile.UseVisualStyleBackColor = true;
            this.btn_Getfile.Click += new System.EventHandler(this.btn_Getfile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 492);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnMoveP0);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpIoSts);
            this.Controls.Add(this.btnMoveM0);
            this.Controls.Add(this.btnStop0);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_CheckSlave;
        private System.Windows.Forms.Label lbl_Baudrate;
        private System.Windows.Forms.ComboBox cBox_baudrate;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.ComboBox cBox_cardno;
        private System.Windows.Forms.Button btn_Initial;
        private System.Windows.Forms.CheckBox ChkSVON0;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnReset0;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Button btnMoveP0;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox9;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpIoSts;
        private System.Windows.Forms.Button btnMoveM0;
        private System.Windows.Forms.Button btnStop0;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_Getfile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

