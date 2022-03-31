using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Server : Node
{
	 internal Socket server_socket = null;
	 //internal ServerSocket server = null;
	 //internal DataInputStream @in = null;
	 internal int port_no;
	 internal int corr_cli_index;
	 
	public Server()
	{

	}
	public Server(int port_no, int cli_index)
	{


		this.port_no = port_no;
		this.corr_cli_index = cli_index;


	}
	//~Server()
 //   {
	//	disconnect();
 //   }
	
	

	public virtual void read_msg(Socket client_socket)
	{
		//IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
		//IPAddress ipAddr = ipHost.AddressList[0];
		//IPEndPoint localEndPoint = new IPEndPoint(ipAddr, this.port_no_server);

		//// Creation TCP/IP Socket using
		//// Socket Class Constructor
		//server_socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		//string res = "";
		//lock (this)
		//{
			try
			{
				while (true)
				{



					string data = "";

					//while (true)
					//{
					int numBytes = 0;
					byte[] bytes = new byte[1024];
					if (client_socket != null)
					{
						numBytes = client_socket.Receive(bytes); //changed from client_socket

						//Console.WriteLine("Num bytes:" + numBytes);

						//if (numBytes == 0)
						//break;
						data += Encoding.ASCII.GetString(bytes, 0, numBytes);
					}

					//Console.WriteLine("Data:" + data);
					//if (data.IndexOf("<EOF>") > -1)
					//break;
					//}

					//Console.WriteLine("Text received -> {0} ", data);
					//byte[] message = Encoding.ASCII.GetBytes(data);

					//client_socket.Send(message);

					// Close client Socket using the
					// Close() method. After closing,
					// we can use the closed Socket
					// for a new Client Connection

					//clientSocket.Shutdown(SocketShutdown.Both);
					//clientSocket.Close();
					//}

					//if (@in.available() > 0)
					//{

					//string line = (string)@in.readUTF();

					//     while (true)
					//{ 
					//byte[] byte_data = new byte[1024];

					//int recd_data_length = 0;
					//if (networkStream.Read(byte_data) > 0)
					//	recd_data_length = networkStream.Read(byte_data, 0, byte_data.Length);
					//else
					//	continue;

					//string data = Encoding.ASCII.GetString(byte_data, 0, recd_data_length);

					if (data == "")
						continue;
					Console.WriteLine("Msg received:" + data);

					string[] res_arr = data.Split(":", 3); // req has three parts: request:node_no:req_no or reply:node_no:ok

					if (res_arr[0].Equals("REQUEST"))
					{
						process_request(res_arr[1], res_arr[2]);
					}
					else
					{
						process_reply(res_arr[1]);
					}


					//numBytes = client_socket.Receive(bytes);
					//if (numBytes == 0)
					//{
					//	Thread.Sleep(5000);
					//	//continue;
					//}

					Thread.Sleep(5000);

				}
				//}
				//else
				//{

			}

			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}

			//return res;

		//}
	}



			
	

public virtual void disconnect()
{
	server_socket.Shutdown(SocketShutdown.Both);
	server_socket.Close();
}

public virtual void process_request(string node_no_str, string req_no_str)
	{
		//lock (this)
		//{
			int node_no = int.Parse(node_no_str);
			int req_no = int.Parse(req_no_str);

			if (Node.enter_cs == true)
			{
				Node.def_list[node_no] = req_no;
				Console.WriteLine("Request from Node-" + node_no_str + " is deferred. Request Value: " + req_no_str);
				if (Node.max_req_no < req_no)
				{
					Node.max_req_no = req_no;
				}
				return;
			}

			if (Node.req_cs_entry)
            {
                if (req_no < Node.curr_req_no)
                {
                    //send reply
                    Node.cliObj[corr_cli_index].send_reply(node_no, "OK");//corr_cli_index
                    if (Node.max_req_no < req_no)
                    {
                        Node.max_req_no = req_no;
                    }
                }
                else if (req_no > Node.curr_req_no)
                {
                    //defer reply
                    Node.def_list[node_no] = req_no;
                    Console.WriteLine("Request from Node-" + node_no_str + " is deferred. Request Value: " + req_no_str);
                    if (Node.max_req_no < req_no)
                    {
                        Node.max_req_no = req_no;
                    }
                }
                else
                {
                    if (node_no < Node.node_id)
                    {
                        //send reply
                        Node.cliObj[corr_cli_index].send_reply(node_no, "OK");
                        if (Node.max_req_no < req_no)
                        {
                            Node.max_req_no = req_no;
                        }
                    }
                    else
                    {
                        //defer reply
                        Node.def_list[node_no] = req_no;
                        Console.WriteLine("Request from Node-" + node_no_str + " is deferred. Request Value:" + req_no_str);
                        if (Node.max_req_no < req_no)
                        {
                            Node.max_req_no = req_no;
                        }
                    }
                }

            }
            else
            {
                //send reply
                Node.cliObj[corr_cli_index].send_reply(node_no, "OK");
                if (Node.max_req_no < req_no)
                {
                    Node.max_req_no = req_no;
                }
            }

            						

		//}
	}

	

	public virtual void process_reply(string msg)
	{
		lock (this)
		{
				try
				{
				no_of_pending_req = no_of_pending_req - 1;
		
				if (no_of_pending_req == 0)
				{
					enter_cs = true;
		
					Thread.Sleep(10);
				}
		
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception:" + e);
				}
		}
	}

    public virtual void run()
    {

		Socket client_socket = null;
		//NetworkStream ns = null;

		try
        {
            Console.WriteLine("Server Listening @" + port_no);
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];

            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port_no);

            // Creation TCP/IP Socket using
            // Socket Class Constructor
            server_socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port_no);

			//server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			Thread.Sleep(10);
			//Console.WriteLine("Server Listening @" + port_no_server);
			
			Console.WriteLine("Waiting connection at server...");

			server_socket.Bind(localEndPoint);
			server_socket.Listen(10);
			//client_socket = server_socket.Accept();

			client_socket=server_socket.Accept();
			//ns = new NetworkStream(client_socket);

			Console.WriteLine("Connection Accepted @" + port_no);
			
		}
        catch (Exception e)
        {
            Console.WriteLine("Exception:" + e);
        }


        while (true)
        {
            try
            {
                read_msg(client_socket);
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e);

            }
        }
    }
}
