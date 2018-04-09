using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class LatestServerTest {
    static void Main(string[] args) {
        Server server = new Server();
        server.Listen();
        server.AcceptTcpClientAsync();
        Client client = new Client();
        client.ConnectToServer();

        //Thread.Sleep(1000);
        //client.Close();
        //Console.WriteLine("client closed");

        Thread.Sleep(1000);
        if (server.ClientConnected) {
            client.GetDataAsync(client.ProcessData);
            Console.WriteLine("before sending test msgs");
            Thread.Sleep(1000);
            server.SendMsg("test");
            server.SendMsg("test2");
            server.SendMsg("test3");
        }
        Console.ReadLine();
    }

    public class Server {
        private static int PORT = 11000;
        private static IPAddress IPADDRESS = IPAddress.Loopback;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private Socket tcpSocket;
        private StreamWriter streamWriter;
        private bool clientConnected;

        public bool ClientConnected { get => clientConnected; private set => clientConnected = value; }

        public void Listen() {
            tcpListener = new TcpListener(IPADDRESS, PORT);
            tcpListener.Start();
            Console.WriteLine("Listener Started");
        }

        public async void AcceptTcpClientAsync() {
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            //tcpSocket = await tcpListener.AcceptSocketAsync();
            ClientConnected = true;
            Console.WriteLine("client connected");
        }

        public async void SendMsg(String message) {
            try {
                NetworkStream networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream);
                await streamWriter.WriteLineAsync(message);
                Console.WriteLine("after send msg");
            } catch (Exception e) {
                if (tcpClient != null) {
                    tcpClient.Close();
                    tcpClient = null;
                    ClientConnected = false;
                }
                Console.WriteLine(e);
            }
        }
    }

    public class Client {
        private static int PORT = 11000;
        private static IPAddress IPADDRESS = IPAddress.Loopback;
        private TcpClient tcpClient;
        //private StreamReader streamReader;
        private bool connected = false;

        public bool ConnectedToServer { get => connected; private set => connected = value; }

        public void ConnectToServer() {
            tcpClient = new TcpClient();
            Console.WriteLine("Trying to connect to Server...");
            tcpClient.ConnectAsync(IPADDRESS, PORT);
            ConnectedToServer = true;
            Console.WriteLine("Connected to Server");
        }

        public async void GetDataAsync(Action<string> callback) {
            try {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader streamReader = new StreamReader(networkStream);
                while (true) {
                    Console.WriteLine("in receive");
                    string dataFromClient = await streamReader.ReadLineAsync();
                    Console.WriteLine("in receive after read");
                    callback(dataFromClient);
                }
            } catch (Exception e) {
                //if (e is SocketException || e is IOException || e is InvalidOperationException) {}
                Console.WriteLine("HERE :::" + e);
            }
        }
        public void ProcessData(string callback) {
            Console.WriteLine("Callback: " + callback);
        }
        public void Close() {
            if (tcpClient != null) {
                Console.WriteLine("closing tcpClient");
                tcpClient.Close();
                tcpClient = null;
                ConnectedToServer = false;
            }
        }
    }





















    /// <summary>
    /// /////////////////////////////////////////
    /// </summary>

    //async public void Connect()
    //{
    //    TCPClient tcpClient = new TCPClient();
    //    try {
    //        await tcpClient.ConnectToServer();
    //    } catch (SocketException e) {
    //        if (e.ErrorCode == 10061) {
    //            Console.WriteLine("Connection Refused; Listener not started");
    //        }
    //        Console.WriteLine("Error Code: "+ e.ErrorCode + " :: could not connect");
    //        //Console.WriteLine(e.);
    //    }

    //}

    //public class TCPClient {
    //    private static int port = 11000;
    //    private static IPAddress ipAddress = IPAddress.Loopback;
    //    private static TcpClient tcpClient = new TcpClient();
    //    //public bool connected = false;
    //    public bool gameInPlay = false;

    //    /// <summary>
    //    /// replacement below
    //    /// </summary>
    //    /// <returns></returns>
    //    public async Task ConnectToServer() {
    //        while (true) {
    //            Console.WriteLine("Trying to connect to Server...");
    //            try {
    //                await tcpClient.ConnectAsync(ipAddress, port);
    //            } catch (Exception e) {
    //                Console.WriteLine("here" +  e);
    //            }
    //            await tcpClient.ConnectAsync(ipAddress, port);
    //            connected = true;
    //            Console.WriteLine("Connected to Server");

    //            using (NetworkStream networkStream = tcpClient.GetStream())
    //            using (var reader = new StreamReader(networkStream))
    //            using (var writer = new StreamWriter(networkStream)) {
    //                writer.AutoFlush = true;
    //                char keepalive = '0';
    //                while (connected) {
    //                    await writer.WriteLineAsync(keepalive);
    //                    //timer and break?
    //                    string dataFromClient = await reader.ReadLineAsync();
    //                    //how to check loop counter here?
    //                }
    //            };
    //        }     
    //    }

    //    //private static void OnTimedEvent(object source, ElapsedEventArgs e) {

    //    //}

    //    public async void GetData(Action<String> callback) {
    //        try {
    //            using (var networkStream = tcpClient.GetStream())
    //            using (var reader = new StreamReader(networkStream))
    //            using (var writer = new StreamWriter(networkStream)) {
    //                writer.AutoFlush = true;
    //                //TODO: toggle gameInPlay to off after game
    //                while (gameInPlay) {
    //                    string dataFromClient = await reader.ReadLineAsync();
    //                    //makes sure client sends below
    //                    //if is 1000 or 500 bytes then ball or keepalive

    //                    int byteSize = System.Text.Encoding.Unicode.GetByteCount(dataFromClient);

    //                    if (byteSize == 500) {


    //                    }

    //                    String returned = "return";
    //                    callback(returned);
    //                }
    //            }
    //        } catch (NullReferenceException e) {
    //            Console.WriteLine(e);

    //        } finally {
    //            tcpClient.Close();
    //            connected = false;
    //        }
    //    }
    //}
}