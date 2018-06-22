﻿using asyncClient;
using Impinj.OctaneSdk;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;

class RFIDReader {
    public ImpinjReader rfidReader = new ImpinjReader();
    private readonly string settingsFilePath = "readerSettings.xml";
    private string readerAddress;  //Test Table: SpeedwayR-12-4D-B5.local
    private double signalVariance; //variance in signal strength before tag updates
    public ulong TimeBetweenUpdates { get; set; }//microseconds between tag updates

    /// <exception cref="OctaneSdkException">Thrown when we can't connect to the reader</exception>
    public void StartReader() {
        XDocument doc;
        if (!File.Exists(settingsFilePath)) {
            //open UI to configure settings
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        doc = XDocument.Load(settingsFilePath);
        readerAddress = doc.Root.Element("ReaderAddress").Value;
        if (readerAddress == "") {
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        TimeBetweenUpdates = UInt64.Parse(doc.Root.Element("TimeBetweenUpdates").Value);
        signalVariance = Double.Parse(doc.Root.Element("SignalVariance").Value);
        Console.WriteLine("Reader Address: "+readerAddress);
        Console.WriteLine("Time Between Updates: " + TimeBetweenUpdates);
        Console.WriteLine("Signal Variance: " + signalVariance);
        try {
            rfidReader.Connect(readerAddress);
        } catch (OctaneSdkException e) {
            throw e;
        }
        Settings settings = rfidReader.QueryDefaultSettings();
        settings.SearchMode = SearchMode.DualTarget;
        settings.LowDutyCycle = new LowDutyCycleSettings {
            IsEnabled = true,
            EmptyFieldTimeoutInMs = 10000,
            FieldPingIntervalInMs = 300
        };
        settings.Session = 2;
        settings.Report.Mode = ReportMode.Individual;
        settings.Report.IncludePeakRssi = true;
        settings.Report.IncludeLastSeenTime = true;
        settings.Report.IncludeAntennaPortNumber = true;
        rfidReader.ApplySettings(settings);
        rfidReader.Start();
    }

    //TODO  modify and use for reader disconnect event in future
    private bool ReaderIsAvailable(string address)
    {
        Ping pingSender = new Ping();
        PingOptions options = new PingOptions
        {
            DontFragment = true
        };
        string data = "1";
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        int timeout = 120;
        PingReply reply = pingSender.Send(address, timeout, buffer, options);
        if (reply.Status == IPStatus.Success)
            return true;
        else
            return false;
    }
}