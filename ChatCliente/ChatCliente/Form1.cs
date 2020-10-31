using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; // Para hacer referncial tipo de app que es 
using System.Threading; //Ya que se trabajaran con hilo
using System.Net.Sockets; //Canal de Comunicacion entre Cliente y Servidor a traves de una red
using System.IO; //*Para los flujos de Informacion

namespace ChatCliente
{
    public partial class Form1 : Form
    {
        //Declaramos nuestra variables que permitirán el envio de los datos a través de la red 
        static private NetworkStream FlujoDatos; //será el medio o canal que permitira el flujo de datos mediante la red  
        static private StreamWriter DatosEscritos; //los datos que o peticiones escritas que se envien en la red
        static private StreamReader DatosLeidos;  // los datos que deban de mostarse o leerse 
        static private TcpClient Clientes = new TcpClient(); // un protocolo de comunicación del cliente 
        static private string NomUsua = "unknown";

        // creamos un delegado e sirve para mandar informacion entre procesos en este
        //caso los mensajes que se vayan enviando para que aparezcan en cada uno de 
        //los chat conectados mediante una cadena de texto (que contendrá el mensaje).
        private delegate void DelAddItem(String s);

        //creamos un metodo llammado AddItem(Añadir elemento) que añadira a
        //nuestro listbox los mensajes que el cliente vaya enviendo  
        private void AddItem(String s)
        {
            listBox1.Items.Add(s);
        }
        public Form1()
        {
            InitializeComponent();
        }

        //Creación del metodo conectar 
        public void Conectar()
        {
            //mediante un try catch para que en caso de presentar un problema este lo atrape y devuelva una cierta accion 
            try
            {
                //el cliente se conectara a traves de la direccion host y del puerto asignado por el servidor
                Clientes.Connect("127.0.0.1", 8000);
                if (Clientes.Connected) //si existe una conexion entre el cliente 
                {
                    // se crearan cada una de las sesiones para cada cliente mediante el uso de hilos
                    //cada sesion sera un proceso idependiente y dentro de este se extareran los datos del cliente mediante 
                    // la uso del metodo "Escucha como parametro"
                    Thread Sesiones = new Thread(Escucha);


                    //En base a nuestras variables para el envio de datos en la red
                    //el cliente podra visualizar mensajes y enviar mensajes 
                    FlujoDatos = Clientes.GetStream();
                    DatosEscritos = new StreamWriter(FlujoDatos);
                    DatosLeidos = new StreamReader(FlujoDatos);

                    //estos se mostraran con el nombre de del cliente que lo hizo 
                    label3.Text = NomUsua;
                    DatosEscritos.WriteLine(NomUsua);
                    DatosEscritos.Flush(); //luego de que se envien se leimpiaran los datos contenido en el buffer
                    Sesiones.Start(); //y finalmente cada una de nuestras sesiones inciara.
                }

                //En caso de no existir conexión con el servidor se mostrará un mnsaje de servidor no dipobible
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

        //creamos un metodo llamado Escuchar 
        public void Escucha()
        {
            while (Clientes.Connected)  //dentro de un ciclo while preguntara si miestras exista  
                                         //la solicitud de conexion por parte de un cliente 
            {
                try
                {
                    //el cliente se conectará y se invocará al delegado que enviará los datos del cliente, es decir su inf.
                    //del usuario al servidor
                    this.Invoke(new DelAddItem(AddItem), DatosLeidos.ReadLine());

                }
                catch //en caso de que el servidor no este activo se mostrará un mensaje de servidor no disponible
                      //y se saldra de la app
                {
                    MessageBox.Show("Servidor No disponible");
                    Application.Exit();
                }
            }
        }

        //En lo refernte al diseño contara con dos botones a traves de los cuales se llamara a los metodos de conexion y 
        //peticiones cliente servidor 
        private void button2_Click_1(object sender, EventArgs e)
        {
            label1.Visible = false;
            textBox2.Visible = false;
            pictureBox2.Visible = false;
            button2.Visible = false;
            textBox1.Visible = true;
            listBox1.Visible = true;
            pictureBox1.Visible = true;
            pictureBox3.Visible = true;
            NomUsua = textBox2.Text;
            Conectar();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DatosEscritos.WriteLine(textBox1.Text);
            DatosEscritos.Flush();
            textBox1.Clear();
        }

        
    }
}
