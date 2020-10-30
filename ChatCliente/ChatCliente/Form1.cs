using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace ChatCliente
{
    public partial class Form1 : Form
    {
        static private NetworkStream FlujoDatos;
        static private StreamWriter DatosEscritos;
        static private StreamReader DatosLeidos;
        static private TcpClient Clientes = new TcpClient();
        static private string NomUsua = "unknown";

        //el delegado sirve para mandar informacion entre procesos
        private delegate void DelAddItem(String s);

        private void AddItem(String s)
        {
            listBox1.Items.Add(s);
        }
        public Form1()
        {
            InitializeComponent();
        }


        public void Conectar()
        {

            try
            {
                Clientes.Connect("127.0.0.1", 8000);
                if (Clientes.Connected)
                {
                    Thread Sesiones = new Thread(Escucha);


                    FlujoDatos = Clientes.GetStream();
                    DatosEscritos = new StreamWriter(FlujoDatos);
                    DatosLeidos = new StreamReader(FlujoDatos);

                    DatosEscritos.WriteLine(NomUsua);
                    DatosEscritos.Flush();
                    Sesiones.Start();
                }


                else
                {
                    MessageBox.Show("Servidor No disponible");
                    Application.Exit();
                }
            }

            catch
            {
                MessageBox.Show("Servidor No disponible");
                Application.Exit();
            }

        }

        public void Escucha()
        {
            while (Clientes.Connected)
            {
                try
                {
                    this.Invoke(new DelAddItem(AddItem), DatosLeidos.ReadLine());

                }
                catch
                {
                    MessageBox.Show("Servidor No disponible");
                    Application.Exit();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            label1.Visible = false;
            textBox2.Visible = false;
            button2.Visible = false;
            textBox1.Visible = true;
            listBox1.Visible = true;
            button1.Visible = true;
            NomUsua = textBox2.Text;
            Conectar();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DatosEscritos.WriteLine(textBox1.Text);
            DatosEscritos.Flush();
            textBox1.Clear();
        }
    }
}
