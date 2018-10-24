using DigipoolServer;
using Impinj.OctaneSdk;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;

class RFIDReader {
    public ImpinjReader impinjReader = new ImpinjReader();
    private const string settingsFilePath = "Reader Settings.xml";
    private readonly string readerAddress;  //Test Table: SpeedwayR-12-4D-B5.local
    private double txPowerInDbm;

    public RFIDReader() {
        if (!File.Exists(settingsFilePath))
        {
            //open UI to configure settings
            ReaderSettings rs = new ReaderSettings();
            rs.ShowDialog();
        }

        XDocument doc = XDocument.Load(settingsFilePath);

        readerAddress = doc.Root.Element("ReaderAddress").Value;
        if (string.IsNullOrWhiteSpace(readerAddress))
        {
            ReaderSettings rs = new ReaderSettings();
            rs.ShowDialog();
        }

        double.TryParse(doc.Root.Element("TxPowerInDbm").Value, out txPowerInDbm);

        Console.WriteLine("Reader Address: " + readerAddress);
        Console.WriteLine("Tx Power In Dbm: " + txPowerInDbm);
    }

    /// <exception cref="OctaneSdkException">Thrown when we can't connect to the reader</exception>
    public void Start() {
        try
        {
            impinjReader.Connect(readerAddress);
            Console.WriteLine("Reader Connected");
        } catch (OctaneSdkException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Attempting to reconnect in 3 seconds...");
            Thread.Sleep(3000);
            Start();
        } catch (SocketException se)
        {
            Console.WriteLine(se);
            Console.WriteLine("Attempting to reconnect in 3 seconds...");
            Thread.Sleep(3000);
            Start();
        }

        Settings settings = impinjReader.QueryDefaultSettings();
        settings.SearchMode = SearchMode.TagFocus;
        settings.Session = 1;
        settings.Antennas.TxPowerInDbm = txPowerInDbm;

        settings.LowDutyCycle = new LowDutyCycleSettings {
            IsEnabled = true,
            EmptyFieldTimeoutInMs = 10000,
            FieldPingIntervalInMs = 300
        };
        
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
