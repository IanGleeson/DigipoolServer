using Impinj.OctaneSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public class Program {
    private static readonly int PORT = 11000;
    private static readonly IPAddress IPADDRESS = IPAddress.Loopback;
    private static Server server;
    private static RFIDController rfidController;
    private static List<Ball> balls = new List<Ball>();

    [STAThread]
    static void Main(string[] args) {
        server = new Server();
        rfidController = new RFIDController();

        server.Listen(IPADDRESS, PORT);
        server.AcceptTcpClientAsync();

        try {
            rfidController.StartReader();
            rfidController.rfidReader.TagsReported += OnTagsReported;
        } catch (OctaneSdkException ode) {
            Console.WriteLine("Could not connect to reader. Malformed .xml or connection timeout");
            Console.WriteLine(ode.Message);
            Console.ReadLine();
        }

        String helpText = "Type x to end server, c to connect to the reader, s to send a group of test messages to the client or d to drop and reaquire a client";
        Console.WriteLine(helpText);
        bool shutdown = false;
        while (shutdown != true) {
            String choice = Console.ReadLine();
            switch (choice) {
                case "x":
                    shutdown = true;
                    break;
                case "s":
                    if (server.ClientConnected) {
                        SendTestMessages();
                    } else {
                        Console.WriteLine("Client is not connected to send messages to");
                    }
                    break;
                case "d":
                    server.ResetClient();
                    server.AcceptTcpClientAsync();
                    Console.WriteLine(helpText);
                    break;
                case "c":
                    try {
                        rfidController.StartReader();
                        rfidController.rfidReader.TagsReported += OnTagsReported;
                    } catch (OctaneSdkException ode) {
                        Console.WriteLine("Could not connect to reader. Malformed .xml or connection timeout");
                        Console.WriteLine(ode.Message);
                        Console.ReadLine();
                    }
                    break;
                default:
                    Console.WriteLine(helpText);
                    break;
            }
        }
    }
    
    private async static void OnTagsReported(ImpinjReader sender, TagReport report) {
        Tag currentBall = report.Tags[0];
        Ball pastSighting = new Ball();

        //The if statements below checks if the tag is different enough from the previous time we've seen it to bother sending
        if (balls.Any(b => b.Epc.ToString() == currentBall.Epc.ToString())) {
            pastSighting = balls.Where(t => t.Epc.ToString() == currentBall.Epc.ToString()).First();
            ulong prevTagLastPocketedTime = Convert.ToUInt64(pastSighting.LastPocketedTime.ToString());
            ulong tagPocketedTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
            if (tagPocketedTime < (prevTagLastPocketedTime + rfidController.TimeBetweenUpdates)) {
                Console.WriteLine("return from pocketedTime");
                return;
            }
            pastSighting.LastPocketedTime = tagPocketedTime;
            Console.WriteLine("Pocketed: " + currentBall.Epc + " on Antenna: " + currentBall.AntennaPortNumber);
            
            //update ball list with current ball
            balls[balls.FindIndex(el => el.Epc.ToString() == currentBall.Epc.ToString())] = pastSighting;
        } else {
            pastSighting.Epc = currentBall.Epc.ToString();
            pastSighting.Antenna = currentBall.AntennaPortNumber;
            pastSighting.LastSeenTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
            pastSighting.LastPocketedTime = 0;
            pastSighting.TableRssi = new double[17];
            pastSighting.TableRssi[currentBall.AntennaPortNumber] = currentBall.PeakRssiInDbm;

            balls.Add(pastSighting);
        }
        if (server.ClientConnected) {
            Console.WriteLine(currentBall.Epc + " sent");
            Console.WriteLine("");
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(pastSighting);
            await server.SendMsgAsync(json);
            pastSighting.Epc = currentBall.Epc.ToString();
            pastSighting.Antenna = currentBall.AntennaPortNumber;
            pastSighting.TableRssi[currentBall.AntennaPortNumber] = currentBall.PeakRssiInDbm;
            pastSighting.LastSeenTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
        }
    }

    public static async void SendTestMessages() {
        await server.SendMsgAsync("test1");
        await server.SendMsgAsync("test2");
    }
}