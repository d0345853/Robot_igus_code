using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TPM;                 // 軸卡
using System.IO;           // path
using System.IO.Ports;
using System.Threading;

using System.Net;          // TCP                                       
using System.Net.Sockets;


namespace PCI_L122DSF_M1x1a
{
    public partial class Form1 : Form
    {
        #region  ///////////  控制器宣告   ///////////

        //////////////////////  軸卡  //////////////////////  
        short ret = 0;                          // 軸卡
        ushort m_Baudrate0 = 0;                 // 通訊 Baudrate (2) = 10M
        ushort m_CardNo = 0;                    // 卡號
        ushort m_RingNo = 0;                    // only one ring(網路線 通常插0號)        
        uint[] lDevTable = new uint[2];

        //////////////////////  警示狀態  //////////////////////  
        string[] IoStsName = new string[] {     // 警示字串        
            "RDY", "ALM", "+EL", "-EL", "ORG", "DIR", "EMG", "PCS",
            "ERC", "EZ", "CLR", "LTC", "SD", "INP", "SVON", "RALM"       
        };

        Label[,] lblIoSts = new Label[5, 16];       // 文字
        Label[,] lb_sts = new Label[5, 16];         // 顏色


        //////////////////////  感測狀態  //////////////////////
        Label[,] m_Label;
        Button[] btn_Reset;
        CheckBox[] cb_SvOn;
        Button[] btn_Move, btn_Move2, btn_Move3, btn_Move4, btn_Move5;
        TextBox[] tb_Set;
        ComboBox[] cmb_Mode;                // 下拉選單
        ushort[] SlaveIP = new ushort[6];   // 馬達 ip X 5 (0號不算)

        string path = string.Empty;         // ini路徑
        int sin_n = 0;
        double sin_ans = 0;
        #endregion

        #region  ///////////  運動學宣告   ///////////
        // 初始角度 afa(UR 手臂的 afa不用變，是固定的)
        int ap1 = 90, ap2 = 0, ap3 = 0, ap4 = 90, ap5 = 0, ap6 = 0;

        // 自製手臂a d長度參數(每個手臂長度不同)
        int a1 = 0, a2 = 350, a3 = 270, a4 = 0, a5 = 0, a6 = 0;
        int d1 = 142, d2 = 0, d3 = 0, d4 = 0, d5 = 170, d6 = 0;

        // 運動學參數
        private double ZERO_THRESH = 0.00000001;
        private double[] P5 = new double[4];
        private double pi = Math.PI;
        private double div = 0;

        // 運動學選解
        private int num_sols = 0;                       // 當前運動學，總共有幾種解
        private double[] q_ans    = new double[100];    // 所有答案之存放矩陣(6的倍數)

        private double[] motor_pulse = new double[5] { 9000 / 90, 12000 / 90, 9000 / 90, 8000 / 90, 5500 / 90 };     // 馬達pulse規格(除90度)
        private double[] Ans_temp  = new double[6] {  90, 90,   90, 180, 90, -90 };     // 最佳解(暫存)
        private double[] Min_limit = new double[6] { -60,  50,  0,   90, -90, -90 };    //-40 ~ 90 range
        private double[] Max_limit = new double[6] { 240, 180, 130, 220, 270, 270 };
        private int[]     Ans_output = new int[5];
        private int[] Ans_output_p2p = new int[5];

        // 末端點(mm)
        private double[,] T06 = new double[4, 4] { {  0, 1,  0,    0 }, 
                                                   {  0, 0, -1, -270 }, 
                                                   { -1, 0,  0,  322 }, 
                                                   {  0, 1,  0,    1 } };
        private double point_x = 0, point_y = -270, point_z = 322, point_r = 100;

        //cal vector normalize
        private double normal_x = 0, normal_y = 0, normal_z = 0;   

        // 運動 Mode
        private int Kina_mode = 0;
        int timer_cnt = 0;
        int timer_total = 0;

        // S_curve
        private int S_curve_flexible = 5;
        private double line_length = 0;
        private int timer2_resolution = 50;

        // Step
        private int step_cnt = 0;

        //路徑部分
        private StreamWriter sw = new StreamWriter("step.txt");

        #endregion

        #region  ///////////  TCP宣告   ///////////

        public Socket TCP_socket;
        public Thread tt;
        public int tt_flag = 0;
        public IPAddress TCP_IP = IPAddress.Parse("192.168.1.1");//對方電腦ip
        public int TCP_port = 6000;

        public int serial_flags = 0;
        #endregion

        #region  ///////////  初始化手臂   ////////////////////////////////////////////
        //////////////////////  關閉視窗  //////////////////////
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 釋放軸卡
            for (int i = 0; i < 5; i++)
            {
                MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[i], 0);
            }
            ret = MNet.Basic._mnet_close();
            ret = Master.PCI_L122_DSF._l122_dsf_close(m_CardNo);


            sw.Close();

            //TCP close
            if(tt_flag == 1) { 
                //thread close
                TCP_socket.Close();
            }
        }
        
        //////////////////////  初始化  //////////////////////
        public Form1()
        {

            //////////////////////////////////////////////////////////////////////////////////
            // 介面初始化
            InitializeComponent();



            //////////////////////////////////////////////////////////////////////////////////
            // encoder 顯示           1.I/O     2.motion    3.speed   4.Command  5.Position     6.Error
            m_Label = new Label[5, 6] {{ label1,     label2,    label3,    label4,     label5,     label6 },
                                       { label2_1, label2_2,  label2_3,  label2_4,   label2_5,   label2_6 },
                                       { label3_1, label3_2,  label3_3,  label3_4,   label3_5,   label3_6 },
                                       { label4_1, label4_2,  label4_3,  label4_4,   label4_5,   label4_6 },
                                       { label5_1, label5_2,  label5_3,  label5_4,   label5_5,   label5_6 }};

            btn_Reset = new Button[1] { btnReset0 };
            cb_SvOn = new CheckBox[5] { ChkSVON0, ChkSVON2, ChkSVON3, ChkSVON4, ChkSVON5 };

            // 下拉選單                1. move mode  2. mode    3.原點      4.EZ       5.EZ_count   6.Erc
            cmb_Mode = new ComboBox[6] { comboBox1, comboBox5, comboBox6, comboBox7, comboBox8, comboBox9 };

            // 馬達控制選單
            btn_Move = new Button[3] { btnMoveP0,       // 1. 左移動
                                        btnStop0,       // 2. stop
                                        btnMoveM0 };    // 3. 右移動
            btn_Move2 = new Button[3] { btnMoveP2,
                                        btnStop2,
                                        btnMoveM2 };
            btn_Move3 = new Button[3] { btnMoveP3,      // 1. 左移動
                                        btnStop3,       // 2. stop
                                        btnMoveM3 };    // 3. 右移動
            btn_Move4 = new Button[3] { btnMoveP4,
                                        btnStop4,
                                        btnMoveM4 };
            btn_Move5 = new Button[3] { btnMoveP5,
                                        btnStop5,
                                        btnMoveM5 };

            // 輸入數值             1. 移動距離    2. 初始速度    3.最大速度  4.加速時間   5.減速時間 
            tb_Set = new TextBox[5] { textBox1, textBox2, textBox3, textBox4, textBox5 };



            //////////////////////////////////////////////////////////////////////////////////
            // 下拉選單 初始化
            for (int i = 0; i < 6; i++)
            {
                if (i < 1)
                {
                    cmb_Mode[i].SelectedIndex = 0;

                    // 按鈕都初始化
                    cmb_Mode[i].Tag = btn_Reset[i].Tag = btn_Move[i].Tag = tb_Set[i].Tag = i;
                }
                else if (i >= 1 && i < 3)
                    btn_Move[i].Tag = tb_Set[i].Tag = cmb_Mode[i].Tag = i;
                else if (i >= 3 && i < 4)
                    tb_Set[i].Tag = cmb_Mode[i].Tag = i;
                else
                    cmb_Mode[i].Tag = i;

                if (i < 5)
                    cb_SvOn[i].Tag = 0;
            }
            int x, y;

            // 狀態指示燈 初始化
            for (int i = 0; i < 2; i++)
            {
                // 位於小區塊內
                x = grpIoSts.Width / 80 + 10;
                y = grpIoSts.Height / 8 + i * grpIoSts.Width / 20 + 10;
                int k = 0;      //馬達編號

                // 新增16個狀態指示燈
                for (int j = 0; j < 16; j++)
                {
                    if (j == 7 || j == 10 || j == 11)
                        continue;

                    else if (i == 0)
                    {
                        ///////////////////
                        // 狀態label  文字 //
                        lblIoSts[k, j] = new Label();
                        lblIoSts[k, j].Text = IoStsName[j];
                        lblIoSts[k, j].TextAlign = ContentAlignment.MiddleCenter;
                        lblIoSts[k, j].Location = new System.Drawing.Point(x, y);
                        lblIoSts[k, j].Size = new System.Drawing.Size(grpIoSts.Width / 14, 20);

                        x += grpIoSts.Width / 15 + grpIoSts.Width / 160;
                        grpIoSts.Controls.Add(lblIoSts[k, j]);
                    }
                    else
                    {
                        ///////////////////
                        // 狀態label 顏色外框 //
                        lb_sts[k, j] = new Label();
                        lb_sts[k, j].BackColor = Color.Black;          // 初始黑色
                        lb_sts[k, j].Location = new System.Drawing.Point(x, y);
                        lb_sts[k, j].Size = new System.Drawing.Size(grpIoSts.Width / 15, grpIoSts.Width / 15);

                        x += grpIoSts.Width / 15 + grpIoSts.Width / 160;
                        grpIoSts.Controls.Add(lb_sts[k, j]);
                    }
                }

                //02 end of for i 
                // 位於小區塊內
                x = grpIoSts2.Width / 80 + 10;
                y = grpIoSts2.Height / 8 + i * grpIoSts2.Width / 20 + 10;
                k++;

                // 新增16個狀態指示燈
                for (int j = 0; j < 16; j++)
                {
                    if (j == 7 || j == 10 || j == 11)
                        continue;

                    else if (i == 0)
                    {
                        ///////////////////
                        // 狀態label  文字 //
                        lblIoSts[k, j] = new Label();
                        lblIoSts[k, j].Text = IoStsName[j];
                        lblIoSts[k, j].TextAlign = ContentAlignment.MiddleCenter;
                        lblIoSts[k, j].Location = new System.Drawing.Point(x, y);
                        lblIoSts[k, j].Size = new System.Drawing.Size(grpIoSts2.Width / 14, 20);

                        x += grpIoSts2.Width / 15 + grpIoSts2.Width / 160;
                        grpIoSts2.Controls.Add(lblIoSts[k, j]);
                    }
                    else
                    {
                        ///////////////////
                        // 狀態label 顏色外框 //
                        lb_sts[k, j] = new Label();
                        lb_sts[k, j].BackColor = Color.Black;          // 初始黑色
                        lb_sts[k, j].Location = new System.Drawing.Point(x, y);
                        lb_sts[k, j].Size = new System.Drawing.Size(grpIoSts2.Width / 15, grpIoSts2.Width / 15);

                        x += grpIoSts2.Width / 15 + grpIoSts2.Width / 160;
                        grpIoSts2.Controls.Add(lb_sts[k, j]);
                    }
                }
                //03 end of for i
                // 位於小區塊內
                x = grpIoSts3.Width / 80 + 10;
                y = grpIoSts3.Height / 8 + i * grpIoSts3.Width / 20 + 10;
                k++;

                // 新增16個狀態指示燈
                for (int j = 0; j < 16; j++)
                {
                    if (j == 7 || j == 10 || j == 11)
                        continue;

                    else if (i == 0)
                    {
                        ///////////////////
                        // 狀態label  文字 //
                        lblIoSts[k, j] = new Label();
                        lblIoSts[k, j].Text = IoStsName[j];
                        lblIoSts[k, j].TextAlign = ContentAlignment.MiddleCenter;
                        lblIoSts[k, j].Location = new System.Drawing.Point(x, y);
                        lblIoSts[k, j].Size = new System.Drawing.Size(grpIoSts3.Width / 14, 20);

                        x += grpIoSts3.Width / 15 + grpIoSts3.Width / 160;
                        grpIoSts3.Controls.Add(lblIoSts[k, j]);
                    }
                    else
                    {
                        ///////////////////
                        // 狀態label 顏色外框 //
                        lb_sts[k, j] = new Label();
                        lb_sts[k, j].BackColor = Color.Black;          // 初始黑色
                        lb_sts[k, j].Location = new System.Drawing.Point(x, y);
                        lb_sts[k, j].Size = new System.Drawing.Size(grpIoSts3.Width / 15, grpIoSts3.Width / 15);

                        x += grpIoSts3.Width / 15 + grpIoSts3.Width / 160;
                        grpIoSts3.Controls.Add(lb_sts[k, j]);
                    }
                }
                // 04 end of for i
                // 位於小區塊內
                x = grpIoSts4.Width / 80 + 10;
                y = grpIoSts4.Height / 8 + i * grpIoSts4.Width / 20 + 10;
                k++;
                // 新增16個狀態指示燈
                for (int j = 0; j < 16; j++)
                {
                    if (j == 7 || j == 10 || j == 11)
                        continue;

                    else if (i == 0)
                    {
                        ///////////////////
                        // 狀態label  文字 //
                        lblIoSts[k, j] = new Label();
                        lblIoSts[k, j].Text = IoStsName[j];
                        lblIoSts[k, j].TextAlign = ContentAlignment.MiddleCenter;
                        lblIoSts[k, j].Location = new System.Drawing.Point(x, y);
                        lblIoSts[k, j].Size = new System.Drawing.Size(grpIoSts4.Width / 14, 20);

                        x += grpIoSts4.Width / 15 + grpIoSts4.Width / 160;
                        grpIoSts4.Controls.Add(lblIoSts[k, j]);
                    }
                    else
                    {
                        ///////////////////
                        // 狀態label 顏色外框 //
                        lb_sts[k, j] = new Label();
                        lb_sts[k, j].BackColor = Color.Black;          // 初始黑色
                        lb_sts[k, j].Location = new System.Drawing.Point(x, y);
                        lb_sts[k, j].Size = new System.Drawing.Size(grpIoSts4.Width / 15, grpIoSts4.Width / 15);

                        x += grpIoSts4.Width / 15 + grpIoSts4.Width / 160;
                        grpIoSts4.Controls.Add(lb_sts[k, j]);
                    }
                }
                //05 end of for i
                // 位於小區塊內
                x = grpIoSts5.Width / 80 + 10;
                y = grpIoSts5.Height / 8 + i * grpIoSts5.Width / 20 + 10;
                k++;

                // 新增16個狀態指示燈
                for (int j = 0; j < 16; j++)
                {
                    if (j == 7 || j == 10 || j == 11)
                        continue;

                    else if (i == 0)
                    {
                        ///////////////////
                        // 狀態label  文字 //
                        lblIoSts[k, j] = new Label();
                        lblIoSts[k, j].Text = IoStsName[j];
                        lblIoSts[k, j].TextAlign = ContentAlignment.MiddleCenter;
                        lblIoSts[k, j].Location = new System.Drawing.Point(x, y);
                        lblIoSts[k, j].Size = new System.Drawing.Size(grpIoSts5.Width / 14, 20);

                        x += grpIoSts5.Width / 15 + grpIoSts5.Width / 160;
                        grpIoSts5.Controls.Add(lblIoSts[k, j]);
                    }
                    else
                    {
                        ///////////////////
                        // 狀態label 顏色外框 //
                        lb_sts[k, j] = new Label();
                        lb_sts[k, j].BackColor = Color.Black;          // 初始黑色
                        lb_sts[k, j].Location = new System.Drawing.Point(x, y);
                        lb_sts[k, j].Size = new System.Drawing.Size(grpIoSts5.Width / 15, grpIoSts5.Width / 15);

                        x += grpIoSts5.Width / 15 + grpIoSts.Width / 160;
                        grpIoSts5.Controls.Add(lb_sts[k, j]);
                    }
                }
            }

            // comboBox初始化
            cBox_cardno.SelectedIndex = comboBox2.SelectedIndex = 0;    //軸卡選號 選0
            cBox_baudrate.SelectedIndex = 2;                            //baudrate 選10Mk

            //timer 更新速度為0.1s
            timer1.Interval = 100;

            //////////////////////////////////////////////////////////////////////////////////
            // serial
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;

             
            //////////////////////////////////////////////////////////////////////////////////
            // thread
            tt = new Thread(new ThreadStart(ReadData));     //利用 t1 執行續去接收訊息

        }

        /////////////////////   初始按鈕  ////////////////////
        //初始化軸卡
        private void btn_Initial_Click(object sender, EventArgs e)
        {
            //宣告
            ret = 0;
            ushort existcards = 0;

            // 1.啟動軸卡(或多個軸卡)
            ret += Master.PCI_L122_DSF._l122_dsf_open(ref existcards);
            if (existcards == 0)
            {
                MessageBox.Show("No any PCI_L122_DSF!!!");
                return;
            }
            else
                ret = Master.PCI_L122_DSF._l122_dsf_get_switch_card_num(0, ref m_CardNo);    // 2.選擇軸卡

            // 3.更新指示燈
            if (ret == 0)
            {
                btn_Initial.BackColor = Color.Lime;     // 綠色
                btn_CheckSlave.Enabled = true;
                btn_Initial.Enabled = false;
            }
            else
                btn_Initial.BackColor = Color.Red;
        }

        //關閉
        private void btn_exit_Click(object sender, EventArgs e)
        {
            //svon off
            for (int i = 0; i < 5; i++)
            {
                MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[i], 0);
            }

            // 釋放軸卡
            ret = MNet.Basic._mnet_close();
            ret = Master.PCI_L122_DSF._l122_dsf_close(m_CardNo);    

            //Thread stop
                tt = null;



            this.Close();
        }



        ////////////////////   軸卡初始化   /////////////////////
        // 設備搜尋(重新確認軸卡與驅動器)    步驟如圖片
        private void Check_Slave()
        {
            // 1.設定網路線ring
            if (Master.PCI_L122_DSF._l122_dsf_set_ring_config(m_CardNo, m_RingNo, m_Baudrate0) != 0)
            {
                MessageBox.Show("Set ring config fail !!");
                return;
            }

            // 2.重製
            if (MNet.Basic._mnet_reset_ring(m_RingNo) != 0)
            {
                MessageBox.Show("Reset Ring fail!!");
                return;
            }

            // 3.啟動		
            ret = MNet.Basic._mnet_start_ring(m_RingNo);

            // 4.啟動active table
            ret = MNet.Basic._mnet_get_ring_active_table(m_RingNo, lDevTable);
            if (ret == -74)
            {
                MessageBox.Show("No Device!");
                return;
            }

            ////////////////////////////////
            MNet.SlaveType slavetype = 0;
            int count_m1a = 0;

            // 5.掃描ip
            for (ushort ip = 0; ip < 64; ip++)
            {
                if ((lDevTable[ip / 32] & (0x01 << (ip % 32))) != 0)
                {
                    ret += MNet.Basic._mnet_get_slave_type(m_RingNo, ip, ref slavetype);

                    // 6.確認機器軸是否符合 M1x1a axis module
                    if (slavetype == MNet.SlaveType.AXIS_M1x1a)
                    {
                        // 如果有軸卡
                        if ((ret = MNet.M1A._mnet_m1a_initial(m_RingNo, ip)) == 0)
                        {
                            count_m1a++;

                            //掃描馬達IP
                            if (count_m1a == 1)
                            {
                                // 7.取得ip 並顯示
                                SlaveIP[0] = ip;
                                //groupBox44.Text += string.Format(" IP={0}", ip);  

                                // initial settings from EEPROM or by API//
                                if (comboBox2.SelectedIndex == 0)
                                    ret = MNet.M1A._mnet_m1a_recovery_from_EEPROM(m_RingNo, ip);
                                else
                                {
                                    // Recovery settings form file(*.ini) saved by MyLink//
                                    // char[] bArray = Encoding.UTF8.GetBytes(path);
                                    // ret = MNet.M1A._mnet_m1a_load_motion_file(m_RingNo, ip, bArray);
                                }
                                if (ret != 0)
                                    MessageBox.Show("IP01  Initial fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                //////////////////////
                                //啟用部分介面
                                groupBox1.Enabled = grpIoSts.Enabled = true;
                            }
                            else if (count_m1a == 2)
                            {
                                // 7.取得ip 並顯示
                                SlaveIP[1] = ip;

                                // initial settings from EEPROM or by API//
                                if (comboBox2.SelectedIndex == 0)
                                    ret = MNet.M1A._mnet_m1a_recovery_from_EEPROM(m_RingNo, ip);
                                else
                                {
                                    // Recovery settings form file(*.ini) saved by MyLink//
                                    // char[] bArray = Encoding.UTF8.GetBytes(path);
                                    // ret = MNet.M1A._mnet_m1a_load_motion_file(m_RingNo, ip, bArray);
                                }
                                if (ret != 0)
                                    MessageBox.Show("IP02  Initial fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                //////////////////////
                                //啟用部分介面
                                groupBox2.Enabled = grpIoSts.Enabled = true;
                            }
                            else if (count_m1a == 3)
                            {
                                // 7.取得ip 並顯示
                                SlaveIP[2] = ip;

                                // initial settings from EEPROM or by API//
                                if (comboBox2.SelectedIndex == 0)
                                    ret = MNet.M1A._mnet_m1a_recovery_from_EEPROM(m_RingNo, ip);
                                else
                                {
                                    // Recovery settings form file(*.ini) saved by MyLink//
                                    // char[] bArray = Encoding.UTF8.GetBytes(path);
                                    // ret = MNet.M1A._mnet_m1a_load_motion_file(m_RingNo, ip, bArray);
                                }
                                if (ret != 0)
                                    MessageBox.Show("IP03  Initial fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                //////////////////////
                                //啟用部分介面
                                groupBox3.Enabled = grpIoSts.Enabled = true;
                            }
                            else if (count_m1a == 4)
                            {
                                // 7.取得ip 並顯示
                                SlaveIP[3] = ip;

                                // initial settings from EEPROM or by API//
                                if (comboBox2.SelectedIndex == 0)
                                    ret = MNet.M1A._mnet_m1a_recovery_from_EEPROM(m_RingNo, ip);
                                else
                                {
                                    // Recovery settings form file(*.ini) saved by MyLink//
                                    // char[] bArray = Encoding.UTF8.GetBytes(path);
                                    // ret = MNet.M1A._mnet_m1a_load_motion_file(m_RingNo, ip, bArray);
                                }
                                if (ret != 0)
                                    MessageBox.Show("IP04  Initial fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


                                //////////////////////
                                //啟用部分介面
                                groupBox4.Enabled = grpIoSts.Enabled = true;
                            }
                            else if (count_m1a == 5)
                            {
                                // 7.取得ip 並顯示
                                SlaveIP[4] = ip;

                                // initial settings from EEPROM or by API//
                                if (comboBox2.SelectedIndex == 0)
                                    ret = MNet.M1A._mnet_m1a_recovery_from_EEPROM(m_RingNo, ip);
                                else
                                {
                                    // Recovery settings form file(*.ini) saved by MyLink//
                                    // char[] bArray = Encoding.UTF8.GetBytes(path);
                                    // ret = MNet.M1A._mnet_m1a_load_motion_file(m_RingNo, ip, bArray);
                                }
                                if (ret != 0)
                                    MessageBox.Show("IP05  Initial fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                //////////////////////
                                for (int i = 1; i < 6; i++)
                                    cmb_Mode[i].SelectedIndex = 0;

                                //按鈕便綠色
                                btn_CheckSlave.BackColor = Color.Lime;  //綠色

                                //啟用部分介面
                                groupBox5.Enabled = grpIoSts.Enabled = true;
                                btn_CheckSlave.Enabled = false;
                                timer1.Enabled = true;
                            }
                        }
                        else
                            btn_CheckSlave.BackColor = Color.Red;
                    }
                }
            }

            // 6.設備確認
            if (btn_CheckSlave.BackColor != Color.Lime)
            {
                btn_CheckSlave.BackColor = Color.Red;
                MessageBox.Show("Cann't find M1x1a!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_CheckSlave_Click(object sender, EventArgs e)
        {
            Check_Slave();
            TCP_Setting();
        }

        ////////////////////////////////  timer  狀態  ////////////////////////////////
        // timer function
        private void GetAllStatus()
        {
            for (int k = 0; k < 5; k++) //五個馬達
            {
                int Cmd = 0, Pos = 0, Error = 0;
                uint Speed = 0, IoSts = 0;
                ushort McSts = 0;

                // 4.取得指令  Get the value of command counter.
                if ((ret = MNet.M1A._mnet_m1a_get_command(m_RingNo, SlaveIP[k], ref Cmd)) == 0)
                    m_Label[k, 3].Text = Cmd.ToString();

                // 5.取得位置
                if ((ret = MNet.M1A._mnet_m1a_get_position(m_RingNo, SlaveIP[k], ref Pos)) == 0)
                    m_Label[k, 4].Text = Pos.ToString();

                // 3.取得速度  
                if ((ret = MNet.M1A._mnet_m1a_get_current_speed(m_RingNo, SlaveIP[k], ref Speed)) == 0)
                    m_Label[k, 2].Text = Speed.ToString();

                // 1.取得I/O狀態 (16個警示燈)
                if ((ret = MNet.M1A._mnet_m1a_get_io_status(m_RingNo, SlaveIP[k], ref IoSts)) == 0)
                {
                    m_Label[k, 0].Text = IoSts.ToString("X");
                    for (int i = 0; i < 16; i++)
                    {
                        if (i == 7 || i == 10 || i == 11)
                            continue;
                        else if ((IoSts & (0x01 << i)) != 0)
                            lb_sts[k, i].BackColor = Color.Red;    // 紅色
                        else
                            lb_sts[k, i].BackColor = Color.Lime;   // 綠色
                    }
                }

                // 2.取得運動狀態  Return the motion status of Motionnet motion slave.
                if ((ret = MNet.M1A._mnet_m1a_motion_done(m_RingNo, SlaveIP[0], ref McSts)) == 0)
                    m_Label[k, 1].Text = McSts.ToString("X");

                // 6.取得error
                if ((ret = MNet.M1A._mnet_m1a_get_error_counter(m_RingNo, SlaveIP[0], ref Error)) == 0)
                    m_Label[k, 5].Text = Error.ToString();
            }

        }
        // timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetAllStatus();
        }




        //////////////////   基礎設定   ///////////////////////
        // baudrate選擇
        private void cBox_baudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_Baudrate0 = (byte)cBox_baudrate.SelectedIndex;
        }
        
        // error  ComboBox9
        private void ComboBox_Selected_Changed(object sender, EventArgs e)
        {
            int index = (int)(sender as ComboBox).Tag;

            if (index != 0) //axis 0
            {
                //set home config before home move 進行原點移動之前設置原點配置
                ret = MNet.M1A._mnet_m1a_set_home_config(m_RingNo, SlaveIP[0], (ushort)cmb_Mode[1].SelectedIndex, (ushort)cmb_Mode[2].SelectedIndex, (ushort)cmb_Mode[3].SelectedIndex, (ushort)cmb_Mode[4].SelectedIndex, (ushort)cmb_Mode[5].SelectedIndex);
            }

            // 設定error
            if (ret != 0)
                MessageBox.Show("Set Home configuration fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        ////////////////////   初始位置   /////////////////////
        // 初始化檔案
        private void btn_Getfile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "initial file(*.ini)|*.ini";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName; //get the initial file(PCI_M114GH.ini) saved by MyLink
            }
        }
        //初始化來源選擇 comboBox2
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 1)
                btn_Getfile.Enabled = true;     // EEPROM
            else
                btn_Getfile.Enabled = false;    // local
        }
        #endregion

        #region  ///////////  控制手臂   ////////////////////////////////////////////
        /////////////////////////////   SVON   /////////////////
        // 啟動馬達 01
        private void ServerOn_Checked(object sender, EventArgs e)
        {
            // int no = (int)(sender as CheckBox).Tag;     // 得到當前 On/off 狀態
            bool on_off = false;

            // 更新
            if (cb_SvOn[0].Checked)
                on_off = true;
            else
                on_off = false;

            // driver ON.                   Motionnet Ring number, Slave IP 0 ~ 63
            if (MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[0], (ushort)(on_off ? 1 : 0)) != 0)
                MessageBox.Show(string.Format("Set ServerON fail at axis:{0}", SlaveIP[0]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        // 啟動馬達 02
        private void SVON2_CheckedChanged(object sender, EventArgs e)
        {
            // int no = (int)(sender as CheckBox).Tag;     // 得到當前 On/off 狀態
            bool on_off = false;

            // 更新
            if (cb_SvOn[1].Checked)
                on_off = true;
            else
                on_off = false;

            // driver ON.                   Motionnet Ring number, Slave IP 0 ~ 63
            if (MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[1], (ushort)(on_off ? 1 : 0)) != 0)
                MessageBox.Show(string.Format("Set ServerON fail at axis:{0}", SlaveIP[1]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        // 啟動馬達 03
        private void SVON3_CheckedChanged(object sender, EventArgs e)
        {
            // int no = (int)(sender as CheckBox).Tag;     // 得到當前 On/off 狀態
            bool on_off = false;

            // 更新
            if (cb_SvOn[2].Checked)
                on_off = true;
            else
                on_off = false;

            // driver ON.                   Motionnet Ring number, Slave IP 0 ~ 63
            if (MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[2], (ushort)(on_off ? 1 : 0)) != 0)
                MessageBox.Show(string.Format("Set ServerON fail at axis:{0}", SlaveIP[2]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        // 啟動馬達 04
        private void SVON4_CheckedChanged(object sender, EventArgs e)
        {
            // int no = (int)(sender as CheckBox).Tag;     // 得到當前 On/off 狀態
            bool on_off = false;

            // 更新
            if (cb_SvOn[3].Checked)
                on_off = true;
            else
                on_off = false;

            // driver ON.                   Motionnet Ring number, Slave IP 0 ~ 63
            if (MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[3], (ushort)(on_off ? 1 : 0)) != 0)
                MessageBox.Show(string.Format("Set ServerON fail at axis:{0}", SlaveIP[3]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        // 啟動馬達 05
        private void SVON5_CheckedChanged(object sender, EventArgs e)
        {
            // int no = (int)(sender as CheckBox).Tag;     // 得到當前 On/off 狀態
            bool on_off = false;

            // 更新
            if (cb_SvOn[4].Checked)
                on_off = true;
            else
                on_off = false;

            // driver ON.                   Motionnet Ring number, Slave IP 0 ~ 63
            if (MNet.M1A._mnet_m1a_set_svon(m_RingNo, SlaveIP[4], (ushort)(on_off ? 1 : 0)) != 0)
                MessageBox.Show(string.Format("Set ServerON fail at axis:{0}", SlaveIP[4]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }



        ////////////////////////////////  馬達控制  ////////////////////////////////
        // control
        private void Move_Click(object sender, EventArgs e)
        {
            // 0.取得按鈕的模式
            String nn = (sender as Button).Name;    // 按鈕名稱
            int k = 0;                              // 馬達編號

            // 1.positive
            if (nn == "btnMoveP0")
            {
                // 設定移動參數                          接線ring     馬達              初始速度                   最大速度                   加速度                       減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));

                //確認動作模式
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0: // 相對   relative                                             相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 1: // 絕對   abs                                                  絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 2: // 持續   conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 1);
                        break;
                    case 3: // 原點  home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 1);
                        break;
                }

            }
            // 2.stop
            else if (nn == "btnStop0")
            {
                ret = MNet.M1A._mnet_m1a_emg_stop(m_RingNo, SlaveIP[k]);
            }
            // 3.negative
            else if (nn == "btnMoveM0")
            {
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0://relative                                                         反向相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 1://abs                                                               反向絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 2://conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 0);
                        break;
                    case 3://home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 0);
                        break;
                }
            }

        }
        private void Move_Click2(object sender, EventArgs e)
        {
            // 0.取得按鈕的模式
            String nn = (sender as Button).Name;    // 按鈕名稱
            int k = 1;                              // 馬達編號

            // 1.positive
            if (nn == "btnMoveP2")
            {
                // 設定移動參數                          接線ring     馬達              初始速度                   最大速度                   加速度                       減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));

                //確認動作模式
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0: // 相對   relative                                             相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 1: // 絕對   abs                                                  絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 2: // 持續   conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 1);
                        break;
                    case 3: // 原點  home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 1);
                        break;
                }

            }
            // 2.stop
            else if (nn == "btnStop2")
            {
                ret = MNet.M1A._mnet_m1a_emg_stop(m_RingNo, SlaveIP[k]);
            }
            // 3.negative
            else if (nn == "btnMoveM2")
            {
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0://relative                                                         反向相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 1://abs                                                               反向絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 2://conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 0);
                        break;
                    case 3://home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 0);
                        break;
                }
            }

        }
        private void Move_Click3(object sender, EventArgs e)
        {
            // 0.取得按鈕的模式
            String nn = (sender as Button).Name;    // 按鈕名稱
            int k = 2;                              // 馬達編號

            // 1.positive
            if (nn == "btnMoveP3")
            {
                // 設定移動參數                          接線ring     馬達              初始速度                   最大速度                   加速度                       減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));

                //確認動作模式
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0: // 相對   relative                                             相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 1: // 絕對   abs                                                  絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 2: // 持續   conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 1);
                        break;
                    case 3: // 原點  home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 1);
                        break;
                }

            }
            // 2.stop
            else if (nn == "btnStop3")
            {
                ret = MNet.M1A._mnet_m1a_emg_stop(m_RingNo, SlaveIP[k]);
            }
            // 3.negative
            else if (nn == "btnMoveM3")
            {
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0://relative                                                         反向相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 1://abs                                                               反向絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 2://conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 0);
                        break;
                    case 3://home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 0);
                        break;
                }
            }

        }
        private void Move_Click4(object sender, EventArgs e)
        {
            // 0.取得按鈕的模式
            String nn = (sender as Button).Name;    // 按鈕名稱
            int k = 3;                              // 馬達編號

            // 1.positive
            if (nn == "btnMoveP4")
            {
                // 設定移動參數                          接線ring     馬達              初始速度                   最大速度                   加速度                       減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));

                //確認動作模式
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0: // 相對   relative                                             相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 1: // 絕對   abs                                                  絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 2: // 持續   conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 1);
                        break;
                    case 3: // 原點  home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 1);
                        break;
                }

            }
            // 2.stop
            else if (nn == "btnStop4")
            {
                ret = MNet.M1A._mnet_m1a_emg_stop(m_RingNo, SlaveIP[k]);
            }
            // 3.negative
            else if (nn == "btnMoveM4")
            {
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0://relative                                                         反向相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 1://abs                                                               反向絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 2://conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 0);
                        break;
                    case 3://home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 0);
                        break;
                }
            }

        }
        private void Move_Click5(object sender, EventArgs e)
        {
            // 0.取得按鈕的模式
            String nn = (sender as Button).Name;    // 按鈕名稱
            int k = 4;                              // 馬達編號

            // 1.positive
            if (nn == "btnMoveP5")
            {
                // 設定移動參數                          接線ring     馬達              初始速度                   最大速度                   加速度                       減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));

                //確認動作模式
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0: // 相對   relative                                             相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 1: // 絕對   abs                                                  絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text));
                        break;
                    case 2: // 持續   conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 1);
                        break;
                    case 3: // 原點  home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 1);
                        break;
                }

            }
            // 2.stop
            else if (nn == "btnStop5")
            {
                ret = MNet.M1A._mnet_m1a_emg_stop(m_RingNo, SlaveIP[k]);
            }
            // 3.negative
            else if (nn == "btnMoveM5")
            {
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[k], uint.Parse(tb_Set[1].Text), uint.Parse(tb_Set[2].Text), float.Parse(tb_Set[3].Text), float.Parse(tb_Set[4].Text));
                switch (cmb_Mode[0].SelectedIndex)
                {
                    case 0://relative                                                         反向相對距離
                        ret = MNet.M1A._mnet_m1a_start_r_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 1://abs                                                               反向絕對距離
                        ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[k], int.Parse(tb_Set[0].Text) * -1);
                        break;
                    case 2://conti
                        ret = MNet.M1A._mnet_m1a_v_move(m_RingNo, SlaveIP[k], 0);
                        break;
                    case 3://home
                        ret = MNet.M1A._mnet_m1a_start_home_move(m_RingNo, SlaveIP[k], 0);
                        break;
                }
            }

        }

        //////////////////////////////////////////////
        // reset
        private void Reset_Click(object sender, EventArgs e)
        {
            //int no = (int)(sender as Button).Tag;       // 得到當前Checked狀態
            int no = 0;
            m_Label[no, 3].Text = m_Label[no, 4].Text = m_Label[no, 5].Text = "0";   // 把string 歸零
            MNet.M1A._mnet_m1a_set_command(m_RingNo, SlaveIP[no], 0);                // 設定command  = 0
            MNet.M1A._mnet_m1a_set_position(m_RingNo, SlaveIP[no], 0);               // 設定position = 0
            MNet.M1A._mnet_m1a_reset_error_counter(m_RingNo, SlaveIP[no]);           // 設定error    = 0
        }
        private void Reset2_Click(object sender, EventArgs e)
        {
            int no = 1;
            m_Label[no, 3].Text = m_Label[no, 4].Text = m_Label[no, 5].Text = "0";   // 把string 歸零
            MNet.M1A._mnet_m1a_set_command(m_RingNo, SlaveIP[no], 0);                // 設定command  = 0
            MNet.M1A._mnet_m1a_set_position(m_RingNo, SlaveIP[no], 0);               // 設定position = 0
            MNet.M1A._mnet_m1a_reset_error_counter(m_RingNo, SlaveIP[no]);           // 設定error    = 0
        }
        private void Reset3_Click(object sender, EventArgs e)
        {
            int no = 2;
            m_Label[no, 3].Text = m_Label[no, 4].Text = m_Label[no, 5].Text = "0";   // 把string 歸零
            MNet.M1A._mnet_m1a_set_command(m_RingNo, SlaveIP[no], 0);                // 設定command  = 0
            MNet.M1A._mnet_m1a_set_position(m_RingNo, SlaveIP[no], 0);               // 設定position = 0
            MNet.M1A._mnet_m1a_reset_error_counter(m_RingNo, SlaveIP[no]);           // 設定error    = 0
        }
        private void Reset4_Click(object sender, EventArgs e)
        {
            int no = 3;
            m_Label[no, 3].Text = m_Label[no, 4].Text = m_Label[no, 5].Text = "0";   // 把string 歸零
            MNet.M1A._mnet_m1a_set_command(m_RingNo, SlaveIP[no], 0);                // 設定command  = 0
            MNet.M1A._mnet_m1a_set_position(m_RingNo, SlaveIP[no], 0);               // 設定position = 0
            MNet.M1A._mnet_m1a_reset_error_counter(m_RingNo, SlaveIP[no]);           // 設定error    = 0
        }
        private void Reset5_Click(object sender, EventArgs e)
        {
            int no = 4;
            m_Label[no, 3].Text = m_Label[no, 4].Text = m_Label[no, 5].Text = "0";   // 把string 歸零
            MNet.M1A._mnet_m1a_set_command(m_RingNo, SlaveIP[no], 0);                // 設定command  = 0
            MNet.M1A._mnet_m1a_set_position(m_RingNo, SlaveIP[no], 0);               // 設定position = 0
            MNet.M1A._mnet_m1a_reset_error_counter(m_RingNo, SlaveIP[no]);           // 設定error    = 0
        }
        #endregion

        // 同控模式(timer2 主要運動學)
        private void timer2_Tick(object sender, EventArgs e)
        {
            // 0. Sin 波
            if (Kina_mode == 0)
            {

                //Sin 波
                sin_ans = Math.Sin((Math.PI * sin_n) / 180) * 3000 + 3000;
                Console.WriteLine("The password = " + sin_ans);

                //馬達01 移動
                ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], Convert.ToInt32(sin_ans));

                sin_n = sin_n + 2;

            }
            
            // 2. Line
            else if (Kina_mode == 2)
            {

                double dis = s_curve(timer_cnt, timer_total, line_length);

                double Pos_x_now = point_x + dis * normal_x;
                double Pos_y_now = point_y + dis * normal_y;
                double Pos_z_now = point_z + dis * normal_z;
                //Console.Write(Pos_x_now + "\t");
                //Console.Write(Pos_y_now + "\t");
                //Console.Write(Pos_z_now + "\t");

                // 運動學
                double[] Ans = inv_kinematics(Pos_x_now, Pos_y_now, Pos_z_now);


                for (int i = 0; i < 5; i++)
                {

                    // 轉pulse
                    if (Ans[i] != 0)
                        Ans_output[i] = Convert.ToInt32(Ans[i] * motor_pulse[i]);  //標準化

                    // 設定 馬達 移動參數                   接線ring   馬達            初始速度             最大速度           加速度               減速度
                    ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("2000"), uint.Parse("3000"), float.Parse("0.2"), float.Parse("0"));

                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[i], Ans_output[i]);

                    // print
                    Console.Write((i + 1) + " = " + Ans[i] + "\t");
                    //Console.Write((i + 1) + " = " + Ans_output[i] + "\t");
                }
                Console.WriteLine(" ");

                //計算
                timer_cnt++;

                //停止
                if (timer_cnt == timer_total + 1)
                {
                    timer_cnt = 0;
                    timer_total = 0;
                    Kina_mode = 1;

                    //更新
                    point_x = Pos_x_now;
                    point_y = Pos_y_now;
                    point_z = Pos_z_now;
                }
            }

            // 3. Circle  往Y軸畫圓
            else if (Kina_mode == 3)
            {

                // 畫圓
                double cir_angle = 0;
                if (timer_cnt!=0)
                    cir_angle = (2 *timer_cnt * pi / timer_total) ;
                double Cx = point_r * Math.Sin(cir_angle) + point_x ;      // 變化圓X
                double Cy = point_r * Math.Cos(cir_angle) + point_y - point_r;      // 變化圓Y

                //Console.Write(timer_cnt + "\t");
                //Console.Write(Cx + "\t");
                //Console.Write(Cy + "\t");

                // 運動學
                double[] Ans = inv_kinematics(Cx, Cy, point_z);


                for (int i = 0; i < 5; i++)
                {

                    // 轉pulse
                    if (Ans[i] != 0)
                        Ans_output[i] = Convert.ToInt32(Ans[i] * motor_pulse[i]);  //標準化

                    // 設定 馬達 移動參數                   接線ring   馬達            初始速度             最大速度           加速度               減速度
                    ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("2000"), uint.Parse("3000"), float.Parse("0.1"), float.Parse("0"));
                    
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[i], Ans_output[i]);

                    // print
                    //Console.Write((i + 1) + " = " + Ans[i] + "\t");
                    //Console.Write((i + 1) + " = " + Ans_output[i] + "\t");
                }
                //Console.WriteLine(" ");

                //計算
                timer_cnt++;
                
                //停止
                if (timer_cnt == timer_total + 1)
                {
                    timer_cnt = 0;
                    timer_total = 0;
                    Kina_mode = 1;
                }
            }

            // 4. Cup put
            else if (Kina_mode == 4)
            {
                if (step_cnt == 0)     // 桌上
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 8500);

                }
                else if (step_cnt == 1)     // 桌下
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 400);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -2700);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 9000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;
                }
                else if (step_cnt == 2)     // 杯下
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 3000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 9000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;
                }
                else if (step_cnt == 3)      // 杯上
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 1500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;
                }
                else if (step_cnt == 4)      // 鍋上 
                {
                    
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 1, 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 2, 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 3, 8000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                }
                else if (step_cnt == 5)     // 過下
                {
                    // 倒廖
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, -9500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 3000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 9000);
                    timer_cnt++;
                }
                else if (step_cnt == 6)     // 倒料
                {
                    // 倒廖
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, -9500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 1, 3000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 2, 2000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 3, 7000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], -2600);
                }
                else if (step_cnt == 7)     // 倒料(回)
                {
                    // 倒廖
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, -9500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 1, 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 2, 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 3, 8000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 8500);
                }
                else if (step_cnt == 8)     // 鍋上
                {
                    ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[1], uint.Parse("1000"), uint.Parse("4000"), float.Parse("1"), float.Parse("1"));

                    // 倒廖
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 1500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;
                }
                else if (step_cnt ==9)    //原點  
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -3000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1500);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], -2000);

                    // TCP
                    if (timer_cnt >= timer_total)
                    {
                        WriteData();
                    }
                }
                else if (step_cnt == 10)     // 杯下
                {

                    ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[1], uint.Parse("2000"), uint.Parse("5000"), float.Parse("1"), float.Parse("1"));

                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 3000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -500);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 9000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;

                }
                else if (step_cnt == 11)    //桌下 
                {

                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 400);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -2400);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 9000);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], -2000);
                    timer_cnt++;
                }
                else if (step_cnt == 12)     // 桌上
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8200);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt++;
                }
                else if (step_cnt == 13)     // 桌上
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 8500);
                    timer_cnt++;
                }
                else if (step_cnt == 14)
                {
                    // 控制
                    timer_cnt = 0;
                    timer_total = 0;
                    Kina_mode = 1;

                    // timer2
                    timer2.Enabled = false;
                }

                //計算
                timer_cnt++;

                //停止
                if (timer_cnt >= timer_total + 1)
                {
                    timer_cnt = 0;
                    step_cnt++;
                    Console.WriteLine(step_cnt);
                }


            }

            // 5. Trip
            else if (Kina_mode == 5)
            {
                if (step_cnt == 0)    //鍋鏟原點  
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], 2000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -3000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], -1850);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8100);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 8500);

                }
                else if (step_cnt == 1)     // 鍋鏟桌下
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], 2000);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 400);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8100);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[4], 8500);

                }
                else if (step_cnt == 2)     // 桌上
                {
                    // 控制
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 0, 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 2700);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 6400);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                }
                else if (step_cnt == 3)     // 鍋上
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9700);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 3700);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8100);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);

                }
                else if (step_cnt == 4)     // 斜角1
                {
                    // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9700);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -1200);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 0);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8400);
                    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                    timer_cnt = timer_cnt;

                }
                //else if (step_cnt == 5)     // 斜角2
                //{
                //    // 控制
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -8900);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 1200);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1800);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8600);
                //    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                //    timer_cnt = timer_cnt + 2;
                //}
                //else if (step_cnt == 6)     // 斜角3
                //{
                //    // 控制
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9700);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1],  2800);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 3000);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 9000);
                //    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                //    timer_cnt = timer_cnt + 2;
                //}
                //else if (step_cnt == 7)     // 斜角4
                //{
                //    // 控制
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -10500);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], 1200);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 1800);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8600);
                //    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                //    timer_cnt = timer_cnt + 2;
                //}
                //else if (step_cnt == 8)     // 斜角1
                //{
                //    // 控制
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], -9700);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], -1200);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], 0);
                //    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], 8400);
                //    //ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, 4, -2000);
                //    timer_cnt = timer_cnt + 2;
                //}
                else if (step_cnt == 5)      // 杯上
                {       
                    // 控制
                    timer_cnt = 0;
                    //timer_total = 0;
                    Kina_mode = 6;
                    //WriteData();
                    
                    // timer2
                    //timer2.Enabled = false;

                }

                //計算
                timer_cnt++;

                //停止
                if (timer_cnt >= timer_total + 1)
                {
                    timer_cnt = 0;
                    step_cnt++;
                    Console.WriteLine(step_cnt);
                }


            }

            // 6. Circle  往Y軸畫圓
            else if (Kina_mode == 6)
            {

                // 畫圓
                double cir_angle = 0;
                if (timer_cnt != 0)
                    cir_angle = (timer_cnt  * pi /( timer_total));
                double Cx = Math.Sin(cir_angle);           // 變化圓X
                double Cy = Math.Cos(cir_angle);      // 變化圓Y
                
                //每軸變化
                int C_1 = Convert.ToInt32(800 * Cx) - 9700;
                int C_2 = Convert.ToInt32(-2100 * Cy) + 900;
                int C_3 = Convert.ToInt32(-1600 * Cy) + 1600;
                int C_4 = Convert.ToInt32(-100 * Cy) + 8500;

                for (int i = 0; i < 5; i++)
                {
                    // 設定 馬達 移動參數                   接線ring   馬達            初始速度             最大速度           加速度               減速度
                    ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("1000"), uint.Parse("2000"), float.Parse("0.1"), float.Parse("0"));
                }


                // 控制
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[0], C_1);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[1], C_2);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[2], C_3);
                    ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[3], C_4);

                // print
                Console.Write("1" + " = " + C_1 + "\t\t");
                Console.Write("2" + " = " + C_2 + "\t\t");
                Console.Write("3" + " = " + C_3 + "\t\t");
                Console.Write("4" + " = " + C_4 + "\t\t");
                Console.WriteLine(" ");


                //計算
                timer_cnt++;

                ////停止
                //if (timer_cnt >= timer_total * 4 + 1)
                //{
                //    timer_cnt = 0;
                //    timer_total = 0;
                //    Kina_mode = 1;
                //}



            }
        }

        #region  ///////////  逆運動學   ////////////////////////////////////////////
        ////////////////////   逆運動學   /////////////////////
        // 取整數
        private int SIGN(double x)
        {
            // 如果值等於零,返回  0
            // 如果值大於零,返回  1
            // 如果值小於零,返回 -1 
            return (Convert.ToInt32(x > 0)) - (Convert.ToInt32(x < 0));
        }

        // 取最小值
        private int find_min(int num,  double[] data) {
            int minimum = 0;
                double min_temp = data[0];

            for (int i = 1; i<num; ++i) {
                if (data[i] < min_temp) {
                  minimum = i;
                  min_temp = data[i];
                }
            }
            return minimum;
        }

        // 選最佳解
        private void Choose_Ans(int num_ans, double[] data_ans, double[] data_motor)
        {

            //當前位置
            double m1_pre = data_motor[0];
            double m2_pre = data_motor[1];
            double m3_pre = data_motor[2];
            double m4_pre = data_motor[3];
            double m5_pre = data_motor[4];
            double m6_pre = data_motor[5];

            double[] error = new double[num_ans];

            for (int i = 0; i < num_ans; ++i)
            {
                error[i] =  Math.Abs(data_ans[i * 6] - m1_pre);
                error[i] += Math.Abs(data_ans[i * 6 + 1] - m2_pre);
                error[i] += Math.Abs(data_ans[i * 6 + 2] - m3_pre);
                error[i] += Math.Abs(data_ans[i * 6 + 3] - m4_pre);
                error[i] += Math.Abs(data_ans[i * 6 + 4] - m5_pre);
                error[i] += Math.Abs(data_ans[i * 6 + 5] - m6_pre);
                //Console.WriteLine("The password = " + error[i]);
            }

            int ans_index = find_min(num_ans, error);


            data_motor[0] = data_ans[ans_index * 6];
            data_motor[1] = data_ans[ans_index * 6 + 1];
            data_motor[2] = data_ans[ans_index * 6 + 2];
            data_motor[3] = data_ans[ans_index * 6 + 3];
            data_motor[4] = data_ans[ans_index * 6 + 4];
            data_motor[5] = data_ans[ans_index * 6 + 5];

            //for (int i = 0; i < 6; ++i)
            //{
            //    Console.WriteLine("The password = " + data_motor[i]);
            //}

        }

        //  逆運動學  
        private double[] inv_kinematics(double X_now, double Y_now, double Z_now)
        {
            double[] Ans = new double[6] { 0, 0, 0, 0, 0, 0 };    // 最佳解
            
            // 指定XYZ
            T06[0, 3] = X_now;
            T06[1, 3] = Y_now;
            T06[2, 3] = Z_now;

            #region  ///////////  1.  P5長度 前處理   ///////////
            double[] temp_01 = new double[4] { 0, 0, -(d6), 1 };
            double[] temp_02 = new double[4] { 0, 0, 0, 1 };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {                      
                     P5[i] += T06[i, j] * temp_01[j];    // 最後一根的長度
                }
                P5[i] = P5[i] - temp_02[i];    // 最後一根的長度
            }

            P5[0] = d6 * T06[1, 2] - T06[1, 3];                             // A  代數
            P5[1] = d6 * T06[0, 2] - T06[0, 3];                             // B  代數

            double P5_lxy = Math.Sqrt(P5[0] * P5[0] + P5[1] * P5[1]);       // 絕對距離 R = A * A + B * B;

            #endregion

            #region  ///////////  2.  逆運動學  th1   ///////////

            double[] q1 = new double[] { 0, 0};

            // A.  如果A=0  "arcsin"  簡化公式  
            if (Math.Abs(P5[0]) < ZERO_THRESH)                            
            {
                div = 0;

                // P5 與 實際距離d4 比較，是否>0
                if (Math.Abs(Math.Abs(d4) - Math.Abs(P5[1])) < ZERO_THRESH)
                    div = -SIGN(d4) * SIGN(P5[1]);
                else
                    div = -(d4 / P5[1]);

                // Asin
                double arcsin = Math.Asin(div);
                if (Math.Abs(arcsin) < ZERO_THRESH)
                    arcsin = 0.0;

                // th1 第一種解
                if (arcsin < 0.0)
                    q1[0] = arcsin + 2.0 * pi;
                else
                    q1[0] = arcsin;

                // th2 第二種解             
                q1[1] = pi - arcsin;

            }

            // B.  如果B=0  "arccos"  簡化公式  
            else if ( Math.Abs(P5[1]) < ZERO_THRESH ) { 

                div = 0;

                if ( Math.Abs(  Math.Abs(d4) - Math.Abs(P5[1]) )   < ZERO_THRESH)
                    div = SIGN(d4) * SIGN(P5[1]);
                else
                    div = d4 / P5[0];

                // Acos
                double arccos = Math.Acos(div);

                // th1 第一種解
                q1[0] = arccos;

                // th2 第二種解  
                q1[1] = 2.0 * pi - arccos;


            }
            
            // C.  如果都不是
            else
            {
                double arccos = Math.Acos(d4 / Math.Sqrt(P5_lxy));  // 前半部分
                double arctan = Math.Atan2(-P5[1], P5[0]);          // 後半部分
                double pos = arccos + arctan;                       // 正解
                double neg = -arccos + arctan;                      // 反解

                // 確認 大小 != 0
                if (Math.Abs(pos) < ZERO_THRESH)
                      pos = 0.0;
                if (Math.Abs(neg) < ZERO_THRESH)
                      neg = 0.0;

                // th1 第一種解(確認 -180~180 )
                if (pos >= 0.0)
                      q1[0] = pos;
                else
                      q1[0] = 2.0 * pi + pos;

                // th1 第二種解(確認 -180~180 )
                if (neg >= 0.0)
                      q1[1] = neg;
                else
                      q1[1] = 2.0 * pi + neg;

            }

            #endregion

            #region  ///////////  3.  逆運動學  th5   ///////////
            double[,] q5 = new double[,] { { 0, 0 }, { 0, 0 } };

            // th5 有兩種解
            for (int i = 0; i < 2; i++) {

                // 從th1 來解聯立
                double numer = (T06[0, 3] * Math.Sin(q1[i])) - (T06[1, 3] * Math.Cos(q1[i]) - d4);
                div = 0;

                // 確任 div範圍
                if ( Math.Abs(  Math.Abs(numer) - Math.Abs(d6)  ) < ZERO_THRESH )    // Math.Abs(Math.Abs(numer) - Math.Abs(d6)) < ZERO_THRESH)           
                    div = SIGN(numer) * SIGN(d6);
                else
                    div = numer / d6;

                // Acos
                double arccos = Math.Acos(div);
                q5[i, 0] = arccos;
                q5[i, 1] = 2.0 * pi - arccos;


                //// th5 第一種解
                //if (Math.Imag(q5[i, 1]) != 0)                                         // 虛數檢測
                //    q5[i, 1] = 0;                                                     // 歸零

                //// th5 第二種解
                //if (imag(q5[i, 2]) != 0)
                //    q5[i, 2] = 0;

            }


            #endregion

            #region  ///////////  4.  逆運動學  th6 -> th3 -> th2 -> th4  ///////////
            num_sols = 0;   //選解歸零
            double q6 = 0;

            //透過之前  兩個th1 和  兩個th5 不同的答案來求解th6
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //////////////////////////  A. th6  ///////////////////////////////
                    // 取之前th1與th5的值
                    double c1 = Math.Cos(q1[i]);
                    double s1 = Math.Sin(q1[i]);
                    double c5 = Math.Cos(q5[i, j]);
                    double s5 = Math.Sin(q5[i, j]);
                    

                    // 計算th6 馬達角度(解聯立)
                    if (Math.Abs(s5) < ZERO_THRESH )        // 如果 (Math.Stheta5) != 0)
                        q6 = 0;                             // q6 = q6_des;
                    else 
        	            q6 = Math.Atan2( SIGN(s5) * -(T06[0, 1] * s1 - T06[1, 1] * c1),    SIGN(s5) * (T06[0, 0] * s1 - T06[1, 0] * c1));


                    // 歸零保護
                    if (Math.Abs(q6) < ZERO_THRESH)
                        q6 = 0;

                    // 如果theta6為( -180~0 )
                    if (q6 < 0.0)
                        q6 = q6 + 2.0 * pi;


                    //////////////////////////  A. th3  ///////////////////////////////
                    double[] q3 = new double[] { 0, 0 };


                    // 取之前th6的值
                    double c6 = Math.Cos(q6);
                    double s6 = Math.Sin(q6);


                    // T06[0, 4] 解聯立
                    double x04x = -s5 * (T06[0, 2] * c1 + T06[1, 2] * s1) - c5 * (s6 * (T06[0, 1] * c1 + T06[1, 1] * s1) - c6 * (T06[0, 0] * c1 + T06[1, 0] * s1));
                    double x04y =  c5 * (T06[2, 0] * c6 - T06[2, 1] * s6) - T06[2, 2] * s5;

                    // T06[1, 3] 解聯立
                    double p13x =  d5 * (s6 * (T06[0, 0] * c1 + T06[1, 0] * s1) + c6 * (T06[0, 1] * c1 + T06[1, 1] * s1))    - d6 * (T06[0, 2] * c1 + T06[1, 2] * s1)    + T06[0, 3] * c1    + T06[1, 3] * s1;
                    double p13y =  T06[2, 3] - d1 - d6 * T06[2, 2]                                                           + d5 * (T06[2, 1] * c6 + T06[2, 0] * s6);

                    // cos th3
                    double c3 = (p13x * p13x + p13y * p13y - a2 * a2 - a3 * a3) / (2.0 * a2 * a3);      // 解聯立
                    if (Math.Abs(Math.Abs(c3) - 1.0) < ZERO_THRESH )                                    // 確認cos是否<0
                        c3 = SIGN(c3);
                    else if(Math.Abs(c3) > 1.0)
                        continue;

                    // 利用Acos 求th3 兩個不同的解
                    double arccos = Math.Acos(c3);
                    q3[0] = arccos;
                    q3[1] = 2.0 * pi - arccos;


                    //////////////////////////  B. th2  ///////////////////////////////
                    double[] q2 = new double[] { 0, 0 };

                    double denom = a2 * a2 + a3 * a3 + 2 * a2 * a3 * c3;
                    double s3 = Math.Sin(arccos);
                    double A_3 = (a2 + a3 * c3);
                    double B_3 = a3 * s3;

                    // th2 兩個不同的解(解聯立)
                    q2[0] = Math.Atan2((A_3 * p13y - B_3 * p13x) / denom, (A_3 * p13x + B_3 * p13y) / denom);
                    q2[1] = Math.Atan2((A_3 * p13y + B_3 * p13x) / denom, (A_3 * p13x - B_3 * p13y) / denom);


                    //////////////////////////  C. th4  ///////////////////////////////
                    double[] q4 = new double[] { 0, 0 };

                    // 由前面所求得的角度求出 sin 與 cos 合成角度
                    double c23_0 = Math.Cos(q2[0] + q3[0]);    // 最常那一根的角度 cos
                    double s23_0 = Math.Sin(q2[0] + q3[0]);    // 最常那一根的角度 cos

                    double c23_1 = Math.Cos(q2[1] + q3[1]);
                    double s23_1 = Math.Sin(q2[1] + q3[1]);

                    // th4 兩個不同的解(解聯立)
                    q4[0] = Math.Atan2((c23_0 * x04y) - (s23_0 * x04x), (x04x * c23_0) + (x04y * s23_0));
                    q4[1] = Math.Atan2((c23_1 * x04y) - (s23_1 * x04x), (x04x * c23_1) + (x04y * s23_1));


                    for (int k = 0; k < 2; k++)
                    {
                        // 確認 th2  範圍
                        if (Math.Abs(q2[k]) < ZERO_THRESH)       // 太小則歸零
                            q2[k] = 0.0;
                        else if (q2[k] < 0.0)                    // 確保 0~180
                            q2[k] = q2[k] + 2.0 * pi;

                        // 確認 th4  範圍
                        if (Math.Abs(q4[k]) < ZERO_THRESH)       // 太小則歸零
                            q4[k] = 0.0;
                        else if (q4[k] < 0.0)                    // 確保 0~180
                            q4[k] = q4[k] + 2.0 * pi;

                        //////////////////////////  D. 儲存每個軸的角度  ///////////////////////////////
                        q_ans[num_sols * 6 ]    = q1[i];        //兩種解
                        q_ans[num_sols * 6 + 1] = q2[k];        //8種解
                        q_ans[num_sols * 6 + 2] = q3[k];        //8種解
                        q_ans[num_sols * 6 + 3] = q4[k];        //8種解
                        q_ans[num_sols * 6 + 4] = q5[i, j];     //4種解
                        q_ans[num_sols * 6 + 5] = q6;


                        num_sols = num_sols + 1;
                    }
                }
            }
            #endregion

            #region  ///////////  5.  最佳解   ///////////      
            for (int i = 0; i < 7; i++)         // 八種不同解
            {
                for (int j = 0; j < 6; j++)     // 馬達數量
                {
                    // 1.角度轉換
                    q_ans[i * 6 + j] = q_ans[i * 6 + j] * 180 / pi;

                    // 2.角度保護  (-180 ~ 180)
                    if (q_ans[i * 6 + j] < Min_limit[j])
                        q_ans[i * 6 + j] = Min_limit[j];
                    else if(q_ans[i * 6 + j] > Max_limit[j])
                        q_ans[i * 6 + j] = Max_limit[j];

                    // 3.print
                    // Console.WriteLine("The password = " + q_ans[i * 6 + j]);

                }
                // Console.WriteLine("  ");
            }


            // 3.選最佳解
            Choose_Ans(num_sols, q_ans, Ans_temp);


            // 4.角度矯正
            if(  Ans_temp[0]==0 && Ans_temp[1] == 0 && Ans_temp[2] == 0 && Ans_temp[3] == 0 && Ans_temp[4] == 0) {
                // 圓點保護
                Ans[0] = Ans_temp[0] ;
                Ans[1] = Ans_temp[1] ;
                Ans[2] = Ans_temp[2] ;
                Ans[3] = Ans_temp[3] ;
                Ans[4] = Ans_temp[0] ;
            }
            else {
                Ans[0] = Ans_temp[0] - 90;
                Ans[1] = Ans_temp[1] - 90;
                Ans[2] =-Ans_temp[2] + 90;
                Ans[3] =-Ans_temp[3] + 180;
                Ans[4] =-Ans_temp[0] + 90;
            }

            // 5.print
            //Console.WriteLine("01 = " + Ans[0]);
            //Console.WriteLine("02 = " + Ans[1]);
            //Console.WriteLine("03 = " + Ans[2]);
            //Console.WriteLine("04 = " + Ans[3]);
            //Console.WriteLine("05 = " + Ans[4]);
            #endregion

            return Ans;
        }


        ////////////////////   mode   /////////////////////
        //s_curve
        double s_curve(double t, double tf, double L)
        {

            double melo = S_curve_flexible * (t - tf / 2) / (tf / 2);
            double deno = 1.0 / (1 + Math.Exp(-melo));
            double pos = L * deno;
            if (t == tf)
                pos = L;
            
            return pos;
        }

        // Line
        double cal_3D_dis(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            double z = z2 - z1;
            double dis = Math.Sqrt(x * x + y * y + z * z);
            if (dis == 0)
            {
                normal_x = 0;   //cal vector normalize
                normal_y = 0;   //cal vector normalize
                normal_z = 0;   //cal vector normalize              
            }
            else { 
                normal_x = x / dis;   //cal vector normalize
                normal_y = y / dis;   //cal vector normalize
                normal_z = z / dis;   //cal vector normalize
            }
            return dis;
        }
        #endregion

        #region  ///////////  function   ////////////////////////////////////////////
        // Stop
        private void stop_test_Click(object sender, EventArgs e)
        {

            // timer2
            timer2.Enabled = false;

            for (int i = 0; i < 5; i++)
            {
                // 設定
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("1000"), uint.Parse("2000"), float.Parse("1"), float.Parse("1"));

                // move
                ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[i], Convert.ToInt32(0));
            }

            X_input.Text =   "0";
            Y_input.Text = "270";
            Z_input.Text = "322";
            R_input.Text = "100";
            Ans_temp[0] =  90;
            Ans_temp[1] =  90;
            Ans_temp[2] =  90;
            Ans_temp[3] = 180;
            Ans_temp[4] =  90;
        }

        // 0.Sin 波測試
        private void sin_Click(object sender, EventArgs e)
        {
            // 設定 01馬達 移動參數                  接線ring    馬達        初始速度          最大速度           加速度               減速度
            ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[0], uint.Parse("500"), uint.Parse("2000"), float.Parse("0.5"), float.Parse("0"));

            // mode
            Kina_mode = 0;

            // timer2
            timer2.Enabled = true;
        }

        // 1.P2P
        private void P2P_test_Click(object sender, EventArgs e)
        {
        
            double X_now =  Convert.ToDouble(X_input.Text);    //左右
            double Y_now = -Convert.ToDouble(Y_input.Text);    //前後
            double Z_now =  Convert.ToDouble(Z_input.Text);    //上下
            double time_now = Convert.ToDouble(time2_input.Text);     //timer
            

            // 運動學
            double[] Ans = inv_kinematics(X_now, Y_now, Z_now);

            // mode
            Kina_mode = 1;
            
            for (int i = 0; i < 5; i++)
            {

                // 轉pulse
                if (Ans[i] != 0)
                    Ans_output[i] = Convert.ToInt32(Ans[i] * motor_pulse[i]);  //標準化;
                else
                    Ans_output[i] = 0;

                // 速度換算
                int speed = 0;
                if (Ans_output_p2p[i] - Ans_output[i] != 0)
                    speed = Convert.ToInt32(Math.Abs((Ans_output_p2p[i] - Ans_output[i])) / time_now);
                string speed_txt = Convert.ToString(speed);
                string speed_max = Convert.ToString(speed * 2);
                Ans_output_p2p[i] = Ans_output[i];  //暫存上一次位移

                 Console.Write(speed + "\t");

                // 設定
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse(speed_txt), uint.Parse(speed_max), float.Parse("0.2"), float.Parse("0.2"));

                // 控制
                ret = MNet.M1A._mnet_m1a_start_a_move(m_RingNo, SlaveIP[i], Ans_output[i]);

                // print
                //Console.Write((i + 1) + " = " + Ans[i] + "\t");
                //Console.Write((i+1) + " = " + Ans_output[i] + "\t");
            }
           // Console.WriteLine(" ");
        }

        // 2.Line測試
        private void line_test_Click(object sender, EventArgs e)
        {

            double X_now =  Convert.ToDouble(X_input.Text);           //左右
            double Y_now = -Convert.ToDouble(Y_input.Text);           //前後
            double Z_now =  Convert.ToDouble(Z_input.Text);           //上下
            double time_now = Convert.ToDouble(time2_input.Text);     //timer

            // mode
            Kina_mode = 2;

            // 位移
            line_length = cal_3D_dis(point_x, point_y, point_z, X_now, Y_now, Z_now);

            // timer2
            timer_total = Convert.ToInt32(time_now * timer2_resolution);
            timer_cnt = 0;
            timer2.Enabled = true;
        }
        
        // 3.Circle
        private void Cir_test_Click(object sender, EventArgs e)
        {

            point_x =  Convert.ToDouble(X_input.Text);           //左右
            point_y = -Convert.ToDouble(Y_input.Text);           //前後
            point_z =  Convert.ToDouble(Z_input.Text);           //上下
            point_r =  Convert.ToDouble(R_input.Text);           //半徑

            //更新
            double time_now = Convert.ToDouble(time2_input.Text);     //timer

            // mode
            Kina_mode = 3;

            // timer2
            timer_total = Convert.ToInt32(time_now * timer2_resolution);
            timer_cnt = 0;
            timer2.Enabled = true;
        }

        // Save
        private void Save_Click(object sender, EventArgs e)
        {
            sw.WriteLine(label5.Text + "," + label5_2.Text + "," + label5_3.Text + "," + label5_4.Text + "," + label5_5.Text + "," + time2_input.Text);

        }

        // 4.5.Step
        private void auto_test_Click(object sender, EventArgs e)
        {

            double time_now = Convert.ToDouble(time2_input.Text);     //timer

            for (int i = 0; i < 5; i++)
            {

                // 設定 馬達 移動參數                   接線ring   馬達            初始速度             最大速度           加速度               減速度
                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("2500"), uint.Parse("6000"), float.Parse("1"), float.Parse("1"));
            }

            // 倒料速度
            ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[4], uint.Parse("2000"), uint.Parse("9000"), float.Parse("1"), float.Parse("1"));

            // mode
            Kina_mode = 4;

            // timer2
            timer2.Enabled = true;
            step_cnt = 0;
            timer_cnt = 0;
            timer_total = Convert.ToInt32(time_now * timer2_resolution);
        }

        // gripper
        private void grip_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                if (serial_flags == 0)
                {
                    serialPort.PortName = "COM3";
                    serialPort.Open();
                    serial_flags = 1;

                } 
            }
            else
            {
                if (serial_flags == 1)
                {
                    serialPort.Write("1");
                    serial_flags = 2;

                }
                else if (serial_flags == 2)
                {
                    serialPort.Write("0");
                    serial_flags = 1;
                }

            }
        }

        #endregion

        #region  ///////////  TCP   ////////////////////////////////////////////

        // Read 
        private void ReadData()
        {
            while (true)
            {
                try {
                    int dataLength;
                    byte[] myBufferBytes = new byte[2];
                    dataLength = TCP_socket.Receive(myBufferBytes);

                    // 利用委派來更新
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        //Get
                        string receive_txt = Encoding.ASCII.GetString(myBufferBytes, 0, dataLength);

                        // Print
                        if (receive_txt != "") {
                            Console.WriteLine("[" + System.DateTime.Now + ", Server] :" + receive_txt);
                        }

                        //杯子位置
                        if (receive_txt == "01")
                        {
                            double time_now = Convert.ToDouble(time2_input.Text);     //timer

                            // setting
                            for (int i = 0; i < 4; i++)
                            {

                                // 設定 馬達 移動參數                   接線ring   馬達            初始速度             最大速度           加速度               減速度
                                ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[i], uint.Parse("2000"), uint.Parse("5000"), float.Parse("1"), float.Parse("1"));
                            }

                            // 倒料速度
                            ret = MNet.M1A._mnet_m1a_set_tmove_speed(m_RingNo, SlaveIP[4], uint.Parse("2000"), uint.Parse("7000"), float.Parse("1"), float.Parse("1"));

                            // mode
                            Kina_mode = 4;

                            // timer2
                            timer2.Enabled = true;
                            step_cnt = 0;
                            timer_cnt = 0;
                            timer_total = Convert.ToInt32(time_now * timer2_resolution);
                        }
                        else if (receive_txt == "02")
                        {

                        }
                    }));
                }
                catch(SocketException ex)
                {
                    MessageBox.Show(  ex.ToString()  );
                    break;

                }
            }
        }

        // Write
        private void WriteData()
        {            
            // print 動作完成
            string strTest = "P";
            Console.WriteLine(strTest);

            //將字串轉回 byte array，使用ASCII 編碼
            Byte[] writeByte = Encoding.ASCII.GetBytes(strTest);    // 將srting 轉成 ASCII
            TCP_socket.Send(writeByte, writeByte.Length, 0);        // Send
        }

        // TCP Setting
        private void TCP_Setting()
        {
            try
            {
                // Setting
                TCP_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TCP_socket.Connect(new IPEndPoint(TCP_IP, TCP_port));

                // print
                Console.WriteLine("Connect sucessful !!\n");
                if (tt_flag == 0) { 
                    // thread
                    tt.Start();
                    tt_flag = 1;
                }
                // 3.更新指示燈
                btn_CheckSlave.BackColor = Color.Lime;     // 綠色
                btn_CheckSlave.Enabled = false;

            }
            catch (SocketException ex)
            {
                MessageBox.Show("Server is not opened (connected) !!\n");
                btn_CheckSlave.BackColor = Color.Red;
            }
        }

        // TCP Write
        private void TCP_Click(object sender, EventArgs e)
        {
            WriteData();
        }

        #endregion
    }
}