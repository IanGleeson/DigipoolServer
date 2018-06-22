using Impinj.OctaneSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Topshelf;

public class Program {

    [STAThread]
    static void Main(string[] args)
    {

        var rc = HostFactory.Run(x =>
        {
            x.Service<Controller>(s =>
            {
                s.ConstructUsing(name => new Controller());
                s.WhenStarted(st => st.Start());
                s.WhenStopped(st => st.Stop());
            });
            x.RunAsLocalSystem();

            x.SetDisplayName("DigiPool Server");
            x.SetServiceName("DigiPool Server");
            x.SetDescription("Reads RFID's from an Impinj Reader and sends them to a specified socket");
        });

        var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
        Environment.ExitCode = exitCode;
    }
}