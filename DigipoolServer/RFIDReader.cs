using asyncClient;
using Impinj.OctaneSdk;
using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;

class RFIDReader {
    public ImpinjReader impinjReader = new ImpinjReader();
    private readonly string settingsFilePath = "readerSettings.xml";
    private string readerAddress;  //Test Table: SpeedwayR-12-4D-B5.local
    private double signalVariance; //variance in signal strength before tag updates
    public ulong TimeBetweenUpdates { get; set; }//microseconds between tag updates

    public RFIDReader() {
        XDocument doc;
        if (!File.Exists(settingsFilePath))
        {
            //open UI to configure settings
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        doc = XDocument.Load(settingsFilePath);
        readerAddress = doc.Root.Element("ReaderAddress").Value;
        if (readerAddress == "")
        {
            ReaderSettingsUI rs = new ReaderSettingsUI();
            rs.ShowDialog();
        }
        TimeBetweenUpdates = UInt64.Parse(doc.Root.Element("TimeBetweenUpdates").Value);
        signalVariance = Double.Parse(doc.Root.Element("SignalVariance").Value);
        Console.WriteLine("Reader Address: " + readerAddress);
        Console.WriteLine("Time Between Updates: " + TimeBetweenUpdates);
        Console.WriteLine("Signal Variance: " + signalVariance);
    }

    /// <exception cref="OctaneSdkException">Thrown when we can't connect to the reader</exception>
    public void Start() {
        try {
            impinjReader.Connect(readerAddress);
            Console.WriteLine("Reader Connected");
        } catch (OctaneSdkException e) {
            Console.WriteLine(e.ToString());
            Console.WriteLine("Attempting to reconnect in 3 seconds...");
            Thread.Sleep(3000);
            Start();
        }
        Settings settings = impinjReader.QueryDefaultSettings();
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

        settings.Keepalives.Enabled = true;
        settings.Keepalives.PeriodInMs = 2000;
        settings.Keepalives.EnableLinkMonitorMode = true;
        settings.Keepalives.LinkDownThreshold = 5;
        
        impinjReader.ApplySettings(settings);
        impinjReader.Start();
    }

    public void Stop()
    {
        impinjReader.Stop();
    }
}
