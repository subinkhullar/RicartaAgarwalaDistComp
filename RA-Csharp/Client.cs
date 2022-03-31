using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client : Node
{

	internal Socket client_socket=null;
	//internal DataOutputStream client_dout;

	internal int port_no;

	public Client(int port_no)
	{
		this.port_no = port_no;
	}

	//~Client()
	//{
	//	disconnect();
	//}

	public virtual void connect(int port_no)
	{

		//Socket clisocket = null;
		try
		{
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port_no);

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port_no);

			client_socket = new Socket(ipAddr.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

			try
            {
				client_socket.Connect(localEndPoint);
				Console.WriteLine("Client socket connected to -> {0} ",
						  client_socket.RemoteEndPoint.ToString());

			}


			//client_socket = new Socket("localhost", port_no);
			//client_dout = new DataOutputStream(client_socket.getOutputStream());

				// Manage of Socket's Exceptions
			catch (ArgumentNullException ane)
			{

				Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
			}

			catch (SocketException se)
			{

				Console.WriteLine("SocketException : {0}", se.ToString());
			}

			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception : {0}", e.ToString());
			}

		}

		catch (Exception i)
		{
			Console.WriteLine(i);
		}

	}

	public virtual void disconnect()
	{
		client_socket.Shutdown(SocketShutdown.Both);
		client_socket.Close();
	}


	public virtual void send_request(int node_no, int req_no)
	{

		lock (this)
		{
			try
			{
				//NetworkStream ns = new NetworkStream(client_socket);
				
				//Console.WriteLine("Sending request: " + Convert.ToString(req_no) + " to Node- " + node_no.ToString());
				//String request_msg = "REQUEST:" + Convert.ToString(node_no) + ":" + Convert.ToString(req_no);

				//if (ns.CanWrite)
    //            {
				//	ns.Write(Encoding.ASCII.GetBytes(request_msg), 0, request_msg.Length);
				//	ns.Flush();
    //            }

				//ns.Close();

                Console.WriteLine("Sending request: " + Convert.ToString(req_no) + " from Node- "+ Node.node_id.ToString()+ " to Node- " + node_no.ToString());
                String request_msg = "REQUEST:" + Convert.ToString(Node.node_id) + ":" + Convert.ToString(req_no);
                byte[] messageSent = Encoding.ASCII.GetBytes(request_msg);
				int byteSent;
				if(client_socket!=null)
					byteSent = client_socket.Send(messageSent);

            }
			catch (Exception e)
			{
				Console.WriteLine("Exception:" + e);
			}
		}

	}

	public virtual void send_reply(int node_no, string msg)
	{
		lock (this)
		{
			try
			{
                //NetworkStream ns = new NetworkStream(client_socket);

                //Console.WriteLine("Sending reply to Node-" + node_id);
                //String reply_msg = "REPLY:" + Convert.ToString(node_id) + ":OK";

                //if (ns.CanWrite)
                //{
                //	ns.Write(Encoding.ASCII.GetBytes(reply_msg), 0, reply_msg.Length);
                //	ns.Flush();
                //}

                //ns.Close();

                Console.WriteLine("Sending reply from Node- "+ Node.node_id.ToString()+ " to Node - " + node_no.ToString());

				String reply_msg = "REPLY:" + Convert.ToString(Node.node_id) + ":OK";
                byte[] messageSent = Encoding.ASCII.GetBytes(reply_msg);
				int byteSent;
				if(client_socket!=null)
					byteSent = client_socket.Send(messageSent);

                //disconnect();

			}
			catch (Exception e)
			{
				Console.WriteLine("Exception:" + e);
			}
		}
	}

}