using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;  /*Ya que se trabajaran con hilo*/
using System.Net.Sockets; /*Canal de Comunicacion entre Cliente y Servidor a traves de una red*/
using System.IO; /*Para los flujos de Informacion*/ 
using System.Net; /* Trabajar en red */
namespace ChatServisor
{ 
    class Servidor
    {
        /* TcpListener  ------>Espera la conexión del cliente*/
        /*TcpClient ---------->Proporciona la conexión entre el cliente y el servidor*/
        /*NetWorkStream ------>Se encarga de enviar los mensajes a travesde los sockets*/


        private TcpListener MiServidor; // Esuchara y esperara las posibles conexiones que hagan los clientes 
        private TcpClient Clientes = new TcpClient(); // Nos creará la conexión entre el servidor y los clientes
        private IPEndPoint IPConexion = new IPEndPoint(IPAddress.Any, 8000); //Almacenara la direccion IP y nuestro puerto a traves del cual existirá la comunicacion
        private List<Connection> list = new List<Connection>(); //Lista de la Estructura conexion que almacenara los datos para el chat

        Connection con;

        private struct Connection
        {
            public NetworkStream FlujoDatos;
            public StreamReader DatosLeidos;
            public StreamWriter DatosEscritos;
            public string NomUsua;  
        }


        public Servidor()
        {
            Inicio(); 
        }

        public void Inicio()
        {
            Console.WriteLine("Servidor Funcionado");
            MiServidor = new TcpListener(IPConexion); //El servidor empezará a escuchar a través de IPEndPoint creado
            MiServidor.Start(); //Luego el servidor Iniciará 

            // A traves del ciclo while nos permitira estar siempre preguntado si hay posibles conexiones, es decir si existen clientes.
            while (true)
            {
                Clientes = MiServidor.AcceptTcpClient(); // Si existen clientes aceptamos la solicitude de conexion 
                con = new Connection(); //Y conectamos
                con.FlujoDatos = Clientes.GetStream();  //Obtner el flujo de datos que provienen del cliente 
                con.DatosLeidos = new StreamReader(con.FlujoDatos);  //Escribir o Leer datos a traves del canal de Flujo de datos
                con.DatosEscritos = new StreamWriter(con.FlujoDatos);

                con.NomUsua = con.DatosLeidos.ReadLine(); //Obtener el nombre de usario que esta ejecuntando las acciones y mostrar para su lectura alamecenandolo en el nombre de usario.

                list.Add(con); //Se añadira la informacion a nuestra lista de las conexiones que existan 
                Console.WriteLine(con.NomUsua + "se ha conectado"); //Se mostrara un mensaje cada con el nombre con que el usuario se ha conectado


                //Se crearán Hilos que son procesos que trabajaran en paralelo para que se ejecuten cada una de las sesiones de acuerdo al numero de clientes
                Thread Sesiones = new Thread(Escuchar_Conexiones);
                Sesiones.Start(); 
            }

            
        }

        public void Escuchar_Conexiones()
        { 
            //Creamos una conexion que permita utilizar los elementos que permitan la utilzacion de los diferentes flujos de informacion. 
            Connection hcon = con;

            //Creamos un bucle do while para que se este ejecutando infinitamente 

            do
            {
                try  //ell uso de try atrapara posibles excepciones en caso de que ocurra un error atrapandolo en el catch
                {
                    string Mensajes = hcon.DatosLeidos.ReadLine(); //Leer los datos mandados por el cliente 
                    Console.WriteLine(hcon.NomUsua + ":" + Mensajes); //En la consola mostramos el usario conectado y la informacion que mando 

                    //Est bucle recorrera todas la conexiones que existan y permitira mostrar los mensajes nuevos que se vayan madando
                    //a traves de la verificacion de la lista que tiene de todas las conexiones que se han ido creando. 
                    foreach (Connection c in list)
                    {
                        try
                        {
                            c.DatosEscritos.WriteLine(hcon.NomUsua + ":" + Mensajes); //se agregaran a la lista los mensajes que escriba el cliente con todo y su nombre de usuario
                            c.DatosEscritos.Flush(); //despues en el textboxt donde se escribiran los mensajes se limpiaran luego de que se envien. 
                        }
                        catch
                        {

                        }
                        
                    }
    
                }
                //En caso de que ocurra un problema o un error removera la conexion y mostrará que el cliente se desconecto
                catch
                {
                    list.Remove(hcon);
                    Console.WriteLine(hcon.NomUsua + "se ha desconectado");
                    break;
                }


            }

            while (true);
            {

            }
        }
    }
}

