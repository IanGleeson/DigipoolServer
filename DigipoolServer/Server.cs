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
    //private StreamWriter streamWriter;
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
            //streamWriter = new StreamWriter(networkStream)
            //{
            //    AutoFlush = true
            //};
        } catch (SocketException se) {
            Console.WriteLine(se);
        } catch (InvalidOperationException ioe) {
            Console.WriteLine(ioe);
        }
    }

    ///// <summary>
    ///// Overload for AcceptTcpClientAsync. Used on reconnects to send dropped data to the client
    ///// </summary>
    ///// <param name="unsentMessage">The dropped data caught in an exception</param>
    //private async void AcceptTcpClientAsync(string unsentMessage)
    //{
    //    Console.WriteLine("Waiting for client");
    //    try
    //    {
    //        tcpClient = await tcpListener.AcceptTcpClientAsync();
    //    } catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //    }
    //    Console.WriteLine("Client connected");
    //    ClientConnected = true;
    //    networkStream = tcpClient.GetStream();
    //    streamWriter = new StreamWriter(networkStream)
    //    {
    //        AutoFlush = true
    //    };
    //    await SendMsgAsync(unsentMessage);
    //}

    /// <summary>
    /// Writes a message to an open stream. This WILL write to a closed connection but on client reconnect the stream will
    /// be disposed, triggering the server to reset our TCPClient and streams before resending our dropped message
    /// </summary>
    /// <param name="message"></param>
    public async Task SendMsgAsync(string message)
    {
        try
        {
            using (var streamWriter = new StreamWriter(networkStream, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteLineAsync(message);
            }
            
        } catch (ObjectDisposedException od)
        {
            Console.WriteLine(od);
            //ResetClient();
            //AcceptTcpClientAsync();
        } catch (IOException io)
        {
            Console.WriteLine(io);
            //ResetClient();
            //AcceptTcpClientAsync();
        } catch (InvalidOperationException ioe)
        {
            Console.WriteLine(ioe);
            //ResetClient();
            //AcceptTcpClientAsync();
        }
    }

    public void ResetClient()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
            ClientConnected = false;
            Console.WriteLine("Client has been disconnected");
        }
    }
}
