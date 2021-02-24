using System;
using System.Threading;

namespace ConsoleForAPI
{
    public class QuestionResolver
    {
		public static void Main(string[] args)
		{
			APIMethods methods = new APIMethods();

            //Look all Compilers
			//methods.GetCompilerList();
            //Console.ReadKey();

            //Send source code to a compiler
			int id = methods.PostRequest("116");
			if(id == 0)
            {
				Console.Write("Error Occured");
				Console.WriteLine();
			}
            else
            {
                Console.Write("Wait Please.");
                //wait 5 seconds for compiling
                //it can be changed with while loop with using status of compiler
                for (int i = 0; i<=5; i++)
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                }               
                Console.WriteLine("DONE!");

                //Result of Submission
                methods.GetResult(id); 
            }
        }
	}
}
