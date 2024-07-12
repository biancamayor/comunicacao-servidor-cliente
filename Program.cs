using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Principal
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        
            Console.WriteLine("""
            Bem vindo! Deseja iniciar: 
            [1]Servidor 
            [2]Cliente
            """);

            string? input = Console.ReadLine();

            if (input == "1")
            {
                Server();
            }
            else if (input == "2")
            {
                Client();
            }
            else
            {
                Console.WriteLine("Opção inválida. Por favor, selecione 1 ou 2.");
            }
        }

        static void Server()
        {
            TcpListener? server = null;
            try{
                int port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                byte[] bytes = new byte[256];
                string? data = null;

                while(true){
                    Console.WriteLine("Aguardando por uma conexão...");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Conexão estabelecida!");

                    data = null;


                    Thread clientThread = new Thread(() =>
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();

                            int i;

                            
                            while((i = stream.Read(bytes, 0, bytes.Length)) != 0){
                                {
                                    data = Encoding.UTF8.GetString(bytes, 0, i);
                                    Console.WriteLine("Received: {0}", data);

                                    data = data.ToUpper();

                                    byte[] msg = Encoding.UTF8.GetBytes(data);

                                    stream.Write(msg, 0, msg.Length);
                                    Console.WriteLine("Sent: {0}", data);
                                }
                            }
                        }
                        catch(Exception e){
                            Console.WriteLine("Exception: " + e.Message);
                        }
                        finally{
                            Console.WriteLine("Conexão encerrada...");
                            client.Close(); 
                        }
                    });
                    clientThread.Start();
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally{
                server?.Stop();
            }
        }

        static void Client()
        {
            try{
            int port = 13000;
            string serverIpAddress = "127.0.0.1";

            TcpClient client = new TcpClient(serverIpAddress, port);
            Console.WriteLine("Conectado ao servidor {0}:{1}", serverIpAddress, port);

            while(true)
            {
                Console.WriteLine("Digite uma mensagem: ('exit' para encerrar conexão)");
                string? message = Console.ReadLine();

                if (message == "exit")
                    break;
                
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);

                stream.Write(data, 0, data.Length);
                Console.WriteLine("Mensagem enviada: {0}", message);

                data = new byte[256];
                string? serverResponse = null;
                int bytes = stream.Read(data, 0, data.Length);
                serverResponse = Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine("Resposta ao servidor: {0}", serverResponse);
            }

            client.Close();
          } 
          catch(Exception e)
          {
            Console.WriteLine("Erro: " + e.Message);
          } 

        }
    }
}