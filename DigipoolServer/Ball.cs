using System;

public class Ball {
    public String Epc { get; set; }
    public int Antenna { get; set; }
    public ulong LastSeenTime { get; set; }
    public ulong LastPocketedTime { get; set; }
    public double[] TableRssi { get; set; }
}
