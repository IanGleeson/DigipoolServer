using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private StreamWriter streamWriter;
    private NetworkStream networkStream;
    public bool ClientConnected { get; private set; } = false;

    /// <summary>
    /// Starts the TCPListener at the specified socket. Does not yet accept a client.
    /// </summary>
    public void Listen(IPAddress ipAddress, int port)
    {
        tcpListener = new TcpListener(ipAddress, port);
        try
        {
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            tcpListener.Start();
            Console.WriteLine("Listener Started");
        } catch (SocketException se)
        {
            Console.WriteLine(se);
        } catch (ObjectDisposedException ode)
        {
            Console.WriteLine(ode);
        }
    }
    public void Stop()
    {
        tcpListener.Stop();
        networkStream.Close();
    }
    /// <summary>
    /// Asynchronously accepts a TCPClient and sets up streams to write to
    /// </summary>
    public async void AcceptTcpClientAsync()
    {
        Console.WriteLine("Waiting for client");
        try
        {
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");
            ClientConnected = true;
            networkStream = tcpClient.GetStream();
            streamWriter = new StreamWriter(networkStream, Encoding.UTF8, 1024, true)
            {
                AutoFlush = true
            };
        } catch (SocketException se) {
            Console.WriteLine(se);
        } catch (InvalidOperationException ioe) {
            Console.WriteLine(ioe);
        }
    }
    
    /// <summary>
    /// Writes a message to an open stream
    /// </summary>
    /// <param name="message"></param>
    public void SendMsg(string message)
    {
        try
        {
            streamWriter.WriteLine(message);
        } catch (Exception)
        {
            Console.WriteLine("Client has been disconnected, reconnecting...");
            ResetClient();
            AcceptTcpClientAsync();
        }
    }

    public void ResetClient()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
            networkStream.Close();
            networkStream = null;
            streamWriter.Close();
            streamWriter = null;
            ClientConnected = false;
        }
    }
}
