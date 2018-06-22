using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            Controller.log.Error(se.Message + " :: " + se.StackTrace);
        } catch (ObjectDisposedException ode)
        {
            Controller.log.Error(ode.Message + " :: " + ode.StackTrace);
        }
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
        } catch (SocketException se) {
            Controller.log.Info(se.Message + " :: " + se.StackTrace);
        } catch (InvalidOperationException ioe) {
            Controller.log.Info(ioe.Message + " :: " + ioe.StackTrace);
        }
        streamWriter = new StreamWriter(networkStream)
        {
            AutoFlush = true
        };
    }
    /// <summary>
    /// Overload for AcceptTcpClientAsync. Used on reconnects to send dropped data to the client
    /// </summary>
    /// <param name="unsentMessage">The dropped data caught in an exception</param>
    private async void AcceptTcpClientAsync(string unsentMessage)
    {
        Console.WriteLine("Waiting for client");
        tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine("Client connected");
        ClientConnected = true;
        networkStream = tcpClient.GetStream();
        streamWriter = new StreamWriter(networkStream)
        {
            AutoFlush = true
        };
        await SendMsgAsync(unsentMessage);
    }

    /// <summary>
    /// Writes a message to an open stream. This WILL write to a closed connection but on client reconnect the stream will
    /// be disposed, triggering the server to reset our TCPClient and streams before resending our dropped message
    /// </summary>
    /// <param name="message"></param>
    public async Task SendMsgAsync(string message)
    {
        try
        {
            await streamWriter.WriteLineAsync(message);
        } catch (ObjectDisposedException od)
        {
            Controller.log.Info(od);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (IOException io)
        {
            Controller.log.Info(io);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (InvalidOperationException ioe)
        {
            Controller.log.Info(ioe);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (Exception e)
        {
            Controller.log.Info(e);
            ResetClient();
            AcceptTcpClientAsync(message);
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
