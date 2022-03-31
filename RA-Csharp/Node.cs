using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

public class Node
{
	internal static int node_id;
	internal static bool enter_cs;
	internal static int max_nodes;
	internal static int max_req_no;
	internal static int curr_req_no;
	public static int no_of_pending_req;
	internal static bool req_cs_entry;
	internal int no_of_times;
	internal IList<string> ip_addr = new List<string> (); //can be changed to static type
	internal IList<int> port_no = new List<int> (); //can be changed to static type

	internal static IList<int> def_list = new List<int> ();
	internal static IList<Client> cliObj = new List<Client>();
	internal static IList<Server> serObj = new List<Server>();
	internal static IList<Thread> serThreads = new List<Thread>();

	internal DateTime timeStamp = DateTime.Now;

	public Node(int id, int ctr)
	{
		Node.node_id = id;
		Node.enter_cs = false;
		Node.max_req_no = 0;
		Node.curr_req_no = 0;
		Node.no_of_pending_req = 0;
		Node.req_cs_entry = false;
		this.no_of_times = ctr;
		Console.WriteLine("Entered values are: Process id: " + id.ToString() + "\nNo. of times CS execution is required: "+ctr.ToString());

	}
	public Node()
	{

	}

	public virtual void read_config_file()
	{

		try
		{
			//File file = new File("config.txt");
			//Create an object of FileInfo for specified path            
			//FileInfo fi = new FileInfo(@"C:\Users\Subin Khullar\source\repos\RA-Csharp\config.txt");

			FileInfo fi = new FileInfo(@"..\config.txt");

			//Open a file for Read\Write
			//FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
			//StreamReader br = new StreamReader(fs);

			StreamReader br = fi.OpenText();
			string str;
			string[] words;
			int node_no;
			bool firstentry = true;

			while (!string.ReferenceEquals((str = br.ReadLine()), null))
			{

				if (firstentry)
				{
					Console.WriteLine("Configuration file");
					max_nodes = int.Parse(str);
					
					//Node.no_of_pending_req = max_nodes;
			    	
					Console.WriteLine(str);
					firstentry = false;

				}
				else
				{
			    	Console.WriteLine(str);
					words = str.Split(' ');
					node_no = int.Parse(words[0]);
					ip_addr.Insert(node_no,words[1]);
					port_no.Insert(node_no,int.Parse(words[2]));
					def_list.Insert(node_no,-1);
				}

			}
			br.Close();
			Console.WriteLine("Reached end of configuration file");
		}

		catch (Exception e)
		{
			Console.WriteLine("Exception:" + e);
		}

	}

	public virtual void cs_pre_requisite()
	{
		Node.curr_req_no = Node.max_req_no + 1;
		Node.req_cs_entry = true;
		Node.no_of_pending_req = Node.max_nodes - 1;

		for (int i = 0;i < Node.max_nodes;i++)
		{
			if (i == Node.node_id)
			{
				continue;
			}
				Node.cliObj[i].send_request(i, curr_req_no); // sending messages to the other nodes

		}

	}

	

	public virtual void process_deferred_requests()
	{
			for (int i = 0;i < def_list.Count;i++)
			{
				if (def_list[i] != -1)
				{

					string node_no_str = Convert.ToString(i);
					string req_no_str = Convert.ToString(def_list[i]);
					Node.serObj[i].process_request(node_no_str, req_no_str);
					def_list[i]=-1;
				}
			}
	}
	public virtual void Node_main()
	{
		try
		{
			Node obj = new Node();
//		String timeStamp = new SimpleDateFormat("yyyy.MM.dd.HH.mm.ss").format(new Date());
		read_config_file();
		Node.serThreads= new Thread[Node.max_nodes];
		for (int i = 0;i < Node.max_nodes;i++)
		{

			if (i == Node.node_id)
			{

				Node.serObj.Add(new Server(0, i)); //changed from 0,i 
					//	ParameterizedThreadStart pts = new ParameterizedThreadStart(obj.Node_main);
				//Server svr= Node.serObj[i];
				Thread t1 = new Thread(Node.serObj[i].run);
				Node.serThreads[i]=t1;
				//t1.Start();
			}
			else
			{

                int temp_port_no = port_no[i];
                Node.serObj.Add(new Server(temp_port_no + Node.node_id, i)); //server object changed from port_no[i]+Node.node_id
                Thread t1 = new Thread(Node.serObj[i].run);
                Node.serThreads[i] = t1;
                t1.Start();
             }

		}
		Thread.Sleep(10000);
		for (int i = 0;i < Node.max_nodes;i++)
		{

			if (i == Node.node_id)
			{
				int temp_port_no = port_no[i];
				for (int j = 0;j < Node.max_nodes;j++)
				{
					if (j == Node.node_id)
					{
						cliObj.Add(new Client(temp_port_no + j)); //client object
					}
					else
					{
						cliObj.Add(new Client(temp_port_no + j));
						cliObj[j].connect(temp_port_no + j);

					}

				}

			}

		}
		for (int i = 0;i < no_of_times;i++)
		{
				Thread.Sleep(10000);

				cs_pre_requisite();

				while (Node.enter_cs == false)
				{
					Thread.Sleep(10);
				}
				//enter critical section
				timeStamp = (DateTime.Now);
				Console.WriteLine("Node-" + node_id + " entered Critical Section @ " + timeStamp);
				Thread.Sleep(2000);

				//reset variables
				Node.enter_cs = false;
				Node.req_cs_entry = false;
				timeStamp = (DateTime.Now);
				Console.WriteLine("Node-" + node_id + " exited Critical Section @ " + timeStamp);

				process_deferred_requests();
		}

		Thread.Sleep(10000);



            //for (int i = 0; i < Node.max_nodes; i++)
            //{
            //    if (i != Node.node_id)
            //    {
            //        serObj[i].disconnect();
            //        //cliObj[i].disconnect();

            //    }
            //}

            Console.WriteLine("Completed");
            //Environment.Exit(0);
        }
			catch (Exception e)
			{
				Console.WriteLine("Exception:" + e);
			}

	}

}

