using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace FrmBKRAT7
{
    public partial class Form1 : Form
    {
        private byte[] buffer = new byte[256]; // Chứa data của frame truyền
        private int index = 0; //vị trí của mảng (trong buffer)
        private bool isReceiving = false; //Biến để kiểm tra xem là có đang nhận data hay không, nếu = 0 đã nhận xong hoặc chưa nhận

        public Form1()
        {
            InitializeComponent();
            this.thermometerPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.thermometerPictureBox_Paint);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Đọc các port khả dụng trên PC
            liscom.DataSource = SerialPort.GetPortNames();
            //string[] lisnamecom = SerialPort.GetPortNames(); ;
            // liscom.Items.AddRange(lisnamecom);
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            cbBaudRate.Items.AddRange(BaudRate);
            // Cài đặt cho DataBits
            string[] Databits = { "6", "7", "8" };
            cbDataBits.Items.AddRange(Databits);
            //Cho Parity
            string[] Parity = { "None", "Odd", "Even" };
            cbParity.Items.AddRange(Parity);
            //Cho Stop bit
            string[] stopbit = { "1", "1.5", "2" };
            cbStopBits.Items.AddRange(stopbit);
            cbBaudRate.SelectedItem = "9600";
            cbDataBits.SelectedItem = "8";
            cbParity.SelectedItem = "None";
            cbStopBits.SelectedItem = "1";
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbBaudRate.Text == "") MessageBox.Show("Bạn chưa chọn tốc độ BaudRate!", "Cảnh báo");

                serialPort.PortName = liscom.Text;
                serialPort.BaudRate = int.Parse(cbBaudRate.Text);
                serialPort.DataBits = int.Parse(cbDataBits.Text);
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbParity.Text);
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text, true);
                serialPort.Open();
                progressBar2.Value = 100;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort.Close();
                btnDisconnect.Enabled = false;
                btnConnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        string data = "";
        int giatri = 0;
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data += serialPort.ReadExisting();
            string dataa = data;
            if (data.Length > 0 && int.TryParse(dataa, out int temperature))
            {
                txtReceivedMessages.Invoke((MethodInvoker)delegate { txtReceivedMessages.AppendText(data + Environment.NewLine); });
                data = "";
            

                txtTemperature.Invoke((MethodInvoker)delegate {
                    if (string.IsNullOrEmpty(dataa) || dataa.Length < 2)
                    {
                        txtTemperature.Text = "Invalid data";
                    }
                    else
                    {
                        txtTemperature.Text = dataa.Substring(0, 2) + "°C";
                    }
                });
            }



        }

        private void liscom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtReceivedMessages_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            serialPort.Write(txtSendData.Text);
        }



        private void cbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DrawTemperatureLevel(int temperature)
        {
            // Assuming you have a PictureBox named "thermometerPictureBox" on your form
            Graphics g = thermometerPictureBox.CreateGraphics();
            g.Clear(thermometerPictureBox.BackColor); // Clear previous drawings

            // Define the dimensions and position of the thermometer bar
            int barWidth = thermometerPictureBox.Width / 2;
            int maxBarHeight = thermometerPictureBox.Height;

            // Calculate the height of the bar based on the temperature
            // Assuming the temperature range is 0 to 100 for demonstration purposes
            int barHeight = (temperature * maxBarHeight) / 100;

            // Define the brush to fill the bar (you can customize the color)
            Brush barBrush = Brushes.Red;

            // Draw the temperature bar
            g.FillRectangle(barBrush,
                            (thermometerPictureBox.Width - barWidth) / 2,  // Center the bar horizontally
                            maxBarHeight - barHeight,                      // Bottom position of the bar
                            barWidth,
                            barHeight);

            // Optionally, draw a border around the thermometer (customize the pen as needed)
            Pen borderPen = new Pen(Color.Black);
            g.DrawRectangle(borderPen,
                            (thermometerPictureBox.Width - barWidth) / 2,
                            0,
                            barWidth,
                            maxBarHeight);
        }

        // Ensure you handle the Paint event to redraw the thermometer when needed
        private void thermometerPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (int.TryParse(txtTemperature.Text, out int temperature))
            {
                DrawTemperatureLevel(temperature);
            }
        }

        private void btnShowTemperature_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    //get Temperature from serial port sent by Arduino

                    int temperature = (int)(Math.Round(Convert.ToDecimal(
                                       serialPort.ReadLine()), 0));
                    txtTemperature.Text = temperature.ToString();
                    //draw temperature in the thermometer
                    DrawTemperatureLevel(temperature);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSendData_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

