using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Server {
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private StreamWriter streamWriter;
    private NetworkStream networkStream;
    public bool ClientConnected { get; private set; } = false;

    /// <summary>
    /// Starts the TCPListener at the specified socket. Does not yet accept a client.
    /// </summary>
    public void Listen(IPAddress ipAddress, int port) {
        tcpListener = new TcpListener(ipAddress, port);
        tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        tcpListener.Start();
        Console.WriteLine("Listener Started");
    }
    /// <summary>
    /// Asynchronously accepts a TCPClient and sets up streams to write to
    /// </summary>
    public async void AcceptTcpClientAsync() {
        Console.WriteLine("Waiting for client");
        tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine("Client connected");
        ClientConnected = true;
        networkStream = tcpClient.GetStream();
        streamWriter = new StreamWriter(networkStream) {
            AutoFlush = true
        };
    }
    /// <summary>
    /// Overload for AcceptTcpClientAsync. Used on reconnects to send dropped data to the client
    /// </summary>
    /// <param name="unsentMessage">The dropped data caught in an exception</param>
    private async void AcceptTcpClientAsync(string unsentMessage) {
        Console.WriteLine("Waiting for client");
        tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine("Client connected");
        ClientConnected = true;
        networkStream = tcpClient.GetStream();
        streamWriter = new StreamWriter(networkStream) {
            AutoFlush = true
        };
        await SendMsgAsync(unsentMessage);
    }

    /// <summary>
    /// Writes a message to an open stream. This WILL write to a closed connection but on client reconnect the stream will
    /// be disposed, triggering the server to reset our TCPClient and streams before resending our dropped message
    /// </summary>
    /// <param name="message"></param>
    public async Task SendMsgAsync(string message) {
        try
        {
            await streamWriter.WriteLineAsync(message);
        } catch (ObjectDisposedException od)
        {
            Log("Server: ObjectDisposedException" + od.Message);
            Log(od.StackTrace);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (IOException io)
        {
            Log("Server: IOException" + io.Message);
            Log(io.StackTrace);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (InvalidOperationException ioe)
        {
            Log("Server: InvalidOperationException" + ioe.Message);
            Log(ioe.StackTrace);
            ResetClient();
            AcceptTcpClientAsync(message);
        } catch (Exception e) {
            Log("Server: Exception" + e.Message);
            Log(e.StackTrace);
        }
    }

    public void ResetClient() {
        if (tcpClient != null) {
            tcpClient.Close();
            tcpClient = null;
            ClientConnected = false;
            Console.WriteLine("Client has been disconnected");
        }
    }
    public void Log(String lines) {
        Console.WriteLine(lines);
        using (StreamWriter file = new StreamWriter("logfile.txt", true)) {
            file.WriteLine(DateTime.Now.ToString() + ": " + lines);
        }
        //Console.ReadLine();
    }
}
