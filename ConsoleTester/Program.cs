using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTester
{
  class Program
  {
    static int Main(string[] args)
    {
      for (int i = 0; i < 10; i++)
      {
        Console.Out.WriteLine(String.Format("Output: {0}", i+1));
        Console.Error.WriteLine(String.Format("Error: {0}", i + 1));
        System.Threading.Thread.Sleep(1000);
      }
      Console.WriteLine("done.");
      return 123;
    }
  }
}
