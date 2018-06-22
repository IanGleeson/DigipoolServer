﻿using Impinj.OctaneSdk;
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
    private static RFIDReader rfidController;
    private static List<Ball> balls = new List<Ball>();
    public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private async static void OnTagsReported(ImpinjReader sender, TagReport report)
    {
        Tag currentBall = report.Tags[0];
        Ball pastSighting = new Ball();

        //The if statements below checks if the tag is different enough from the previous time we've seen it to bother sending
        if (balls.Any(b => b.Epc.ToString() == currentBall.Epc.ToString()))
        {
            pastSighting = balls.Where(t => t.Epc.ToString() == currentBall.Epc.ToString()).First();
            ulong prevTagLastPocketedTime = Convert.ToUInt64(pastSighting.LastPocketedTime.ToString());
            ulong tagPocketedTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
            if (tagPocketedTime < (prevTagLastPocketedTime + rfidController.TimeBetweenUpdates))
            {
                //Console.WriteLine("return from pocketedTime");
                return;
            }
            pastSighting.LastPocketedTime = tagPocketedTime;
            //Console.WriteLine("Pocketed: " + currentBall.Epc + " on Antenna: " + currentBall.AntennaPortNumber);

            //update ball list with current ball
            balls[balls.FindIndex(el => el.Epc.ToString() == currentBall.Epc.ToString())] = pastSighting;
        } else
        {
            pastSighting.Epc = currentBall.Epc.ToString();
            pastSighting.Antenna = currentBall.AntennaPortNumber;
            pastSighting.LastSeenTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
            pastSighting.LastPocketedTime = 0;
            pastSighting.TableRssi = new double[17];
            pastSighting.TableRssi[currentBall.AntennaPortNumber] = currentBall.PeakRssiInDbm;

            balls.Add(pastSighting);
        }
        Console.WriteLine(currentBall.Epc + " picked up");
        if (server.ClientConnected)
        {
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(pastSighting);
            await server.SendMsgAsync(json);
            Console.WriteLine(currentBall.Epc + " sent");
            pastSighting.Epc = currentBall.Epc.ToString();
            pastSighting.Antenna = currentBall.AntennaPortNumber;
            pastSighting.TableRssi[currentBall.AntennaPortNumber] = currentBall.PeakRssiInDbm;
            pastSighting.LastSeenTime = Convert.ToUInt64(currentBall.LastSeenTime.ToString());
        }
    }
    
    //Used for service methods // do not delete
    public bool Start()
    {
        server = new Server();
        rfidController = new RFIDReader();

        server.Listen(IPADDRESS, PORT);
        server.AcceptTcpClientAsync();

        while (!rfidController.rfidReader.IsConnected)
        {
            try
            {
                rfidController.StartReader();
                rfidController.rfidReader.TagsReported += OnTagsReported;
            } catch (OctaneSdkException ose)
            {
                log.Info(ose.Message);
            }
            Thread.Sleep(3000);
        }
        Console.WriteLine("started");
        return true;
    }
    public void Stop()
    {
        Console.WriteLine("stopped");
    }
}