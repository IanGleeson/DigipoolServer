using Impinj.OctaneSdk;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

public class Controller
{
    private static readonly int PORT = 56341;
    private static readonly IPAddress IPADDRESS = IPAddress.Loopback;
    private static Server server;
    private static RFIDReader rfidReader;
    public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    //Used for service methods. Do not alter method signature
    public bool Start()
    {
        server = new Server();
        rfidReader = new RFIDReader();
        Console.WriteLine("Service has started");
        server.Listen(IPADDRESS, PORT);
        server.AcceptTcpClientAsync();
        while (!rfidReader.impinjReader.IsConnected)
        {
            try
            {
                rfidReader.Start();
                rfidReader.impinjReader.TagsReported += OnTagsReported;
                rfidReader.impinjReader.ConnectionLost += OnConnectionLost;
                Console.WriteLine("Reader started");
            } catch (OctaneSdkException ose)
            {
                Console.WriteLine(ose);
            }
            Thread.Sleep(3000);
        }
        return true;
    }
    public void Stop()
    {
        server.Stop();
        rfidReader.Stop();
    }

    private async static void OnTagsReported(ImpinjReader sender, TagReport report)
    {
        Tag currentTag = report.Tags[0];
        Ball ball = new Ball
        {
            Epc = currentTag.Epc.ToString()
        };

        Console.WriteLine(currentTag.Epc + " picked up");

        if (server.ClientConnected)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ball);
            Console.WriteLine(json);
            await server.SendMsgAsync(json);
            Console.WriteLine(ball.Epc + " sent");
        }
    }

    void OnConnectionLost(ImpinjReader reader)
    {
        Console.WriteLine("Reader disconnected");
        bool connected = false;
        while (!connected)
        {
            try
            {
                rfidReader.Start();
                rfidReader.impinjReader.TagsReported += OnTagsReported;
                rfidReader.impinjReader.ConnectionLost += OnConnectionLost;
                Console.WriteLine("Reader reconnected");
                connected = true;
            } catch (OctaneSdkException ose)
            {
                Console.WriteLine(ose);
            }
            Thread.Sleep(3000);
        }
    }
}