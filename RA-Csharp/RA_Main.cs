public class RA_Main
{

  public static void Main(string[] args)
  {
		Node nodeObj;
		if (args.Length!=0)
			nodeObj = new Node(int.Parse(args[0]),int.Parse(args[1]));
		else
			nodeObj = new Node(0, 1);
		
		nodeObj.Node_main();
  }

}