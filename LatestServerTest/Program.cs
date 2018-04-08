using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class LatestServerTest {
    static Client client = new Client();

    static void Main(string[] args) {
        Server server = new Server();
        server.Listen();
        server.AcceptTcpClientAsync();
        
        Client client = new Client();
        client.ConnectToServer();

        server.SendMsg("test");

        Console.ReadLine();
    }
    //public static async void GameController() {
    //    Client client = new Client();
    //    //Client client2 = new Client();
    //    await client.ConnectToServer();
    //}

    public class Server {
        private static int PORT = 11000;
        private static IPAddress IPADDRESS = IPAddress.Loopback;
        private static TcpListener tcpListener;
        private static TcpClient tcpClient;
        private static StreamWriter streamWriter;
        private static bool clientConnected;

        public static bool ClientConnected { get => clientConnected; set => clientConnected = value; }

        public void Listen() {
            Console.WriteLine("Starting Listener");
            tcpListener = new TcpListener(IPADDRESS, PORT);
            tcpListener.Start();
            Console.WriteLine("Listener Started");
        }

        public void AcceptTcpClientAsync() {
            //Task.Run()
            tcpClient = tcpListener.AcceptTcpClientAsync().Result;
            ClientConnected = true;
        }

        public async void SendMsg(String message) {
            try {
                using (NetworkStream networkStream = tcpClient.GetStream())
                using (streamWriter = new StreamWriter(networkStream)) {
                    await streamWriter.WriteLineAsync(message);
                }
            } catch (Exception) {
                tcpClient.Close();
                tcpClient = null;
                ClientConnected = false;
                //AcceptTcpClientAsync();
                throw;
            }
        }

    }

    public class Client {
        private static int PORT = 11000;
        private static IPAddress IPADDRESS = IPAddress.Loopback;
        private TcpClient tcpClient;
        private static StreamReader streamReader;
        private bool connected = false;

        public bool Connected { get => connected; private set => connected = value; }

        public void ConnectToServer() {
            tcpClient = new TcpClient();
            Console.WriteLine("Trying to connect to Server...");
            tcpClient.ConnectAsync(IPADDRESS, PORT);
            Connected = true;
            Console.WriteLine("Connected to Server");
        }

        public void ReceiveMsgs() {
            try {
                using (NetworkStream networkStream = tcpClient.GetStream())
                using (streamReader = new StreamReader(networkStream)) {
                    while (Connected) {
                        string dataFromClient = streamReader.ReadLineAsync().Result;
                        Console.WriteLine("Client: " + dataFromClient);

                        
                    }
                };
            } catch (Exception e) {
                //if (e is SocketException || e is IOException || e is InvalidOperationException) {}
                Console.WriteLine("HERE :::" + e);
            }
        }
        public void Close() {
            if (tcpClient != null) {
                tcpClient.Close();
                tcpClient = null;
                Connected = false;
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