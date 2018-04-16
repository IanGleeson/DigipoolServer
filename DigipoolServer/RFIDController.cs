using asyncClient;
using Impinj.OctaneSdk;
using System;
using System.IO;
using System.Xml.Linq;

class RFIDController {
    public ImpinjReader rfidReader = new ImpinjReader();
    private string settingsFilePath = "readerSettings.xml";
    private string readerAddress; // "SpeedwayR-11-AD-98.local" //TEST: SpeedwayR-12-32-8B.local //TEST: SpeedwayR-12-4D-5B.local
    private double signalVariance; //variance in signal strength before tag updates
    private ulong timeBetweenUpdates; //microseconds between tag updates

    public ulong TimeBetweenUpdates { get => timeBetweenUpdates; set => timeBetweenUpdates = value; }

    public void StartReader() {
        if (!File.Exists(settingsFilePath)) {
            //open UI to configure settings
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        XDocument doc = XDocument.Load(settingsFilePath);
        readerAddress = doc.Root.Element("ReaderAddress").Value;
        if (readerAddress == "") {
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        TimeBetweenUpdates = UInt64.Parse(doc.Root.Element("TimeBetweenUpdates").Value);
        signalVariance = Double.Parse(doc.Root.Element("SignalVariance").Value);
        try {
            rfidReader.Connect(readerAddress);
        } catch (OctaneSdkException e) {
            Console.WriteLine(readerAddress);
            Console.WriteLine("Could not connect to reader. Malformed .xml or connection timeout");
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
        Settings settings = rfidReader.QueryDefaultSettings();
        settings.SearchMode = SearchMode.DualTarget;
        settings.Session = 2;
        settings.Report.Mode = ReportMode.Individual;
        settings.Report.IncludePeakRssi = true;
        settings.Report.IncludeLastSeenTime = true;
        settings.Report.IncludeAntennaPortNumber = true;
        rfidReader.ApplySettings(settings);
        rfidReader.Start();
    }
}
