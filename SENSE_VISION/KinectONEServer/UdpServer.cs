//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System;
//using System;
//using System.Collections;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;

//namespace PersonalRobotics.Kinect2Server
//{
//    internal class PacketState
//    {
//        internal PacketState(Socket socket, int bufferSize)
//        {
//            this.UdpSocket = socket;
//            this.Buffer = new byte[bufferSize];
//            this.CreatedTime = DateTime.Now;
//        }

//        public byte[] Buffer { get; set; }

//        public EndPoint Destination { get; set; }

//        public Socket UdpSocket { get; }

//        public bool IsComplete { get; }

//        public DateTime CreatedTime { get; }
//    }
//    public class UdpServer
//    {
//        private readonly DatagramFactory datagramFactory;

//        private Socket udpServerSocket;
//        private IPEndPoint serverEndPoint;
//        private bool isRunning;

//        public UdpServer(DatagramFactory datagramFactory)
//        {
//            this.datagramFactory = datagramFactory;
//            this.ServerPort = 11000;
//            this.PacketBufferSize = 256;
//        }

//        public int ServerPort { get; set; }

//        public int PacketBufferSize { get; set; }

//        public bool IsEventMessagingEnabled { get; set; }

//        public ServerSpecification ServerPolicy { get; set; }

//        public int ClientTimeoutPeriod { get; set; }

//        public void Start()
//        {
//            // Setup our socket for use
//            this.udpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//            this.serverEndPoint = new IPEndPoint(IPAddress.Any, ServerPort);

//            // Bind and configure the socket so we are always given the client end point packet information, such as their IP, when data is received.
//            this.udpServerSocket.Bind(serverEndPoint);
//            this.isRunning = true;
//            this.ListenForData(this.udpServerSocket);
//        }

//        public bool IsRunning()
//        {
//            return this.isRunning;
//        }

//        public void Shutdown()
//        {
//            this.isRunning = false;

//            this.udpServerSocket.Shutdown(SocketShutdown.Both);
//            this.udpServerSocket.Dispose();
//        }

//        public void SendMessage(IServerDatagram message, EndPoint destination)
//        {
//            if (!this.isRunning)
//            {
//                return;
//            }

//            if (message == null)
//            {
//                throw new ArgumentNullException();
//            }
//            if (!message.IsMessageValid())
//            {
//                // TODO: Determine how to handle invalid messages. Exception throwing would be bad, we don't want to crash the server.
//            }

//            var memoryStream = new MemoryStream();
//            byte[] data = null;
//            using (var binaryWriter = new BinaryWriter(memoryStream))
//            {
//                message.Serialize(binaryWriter);

//                // Fetch the serialized bytes from the stream
//                data = memoryStream.GetBuffer();

//                // Send the datagram packet.
//                this.udpServerSocket.SendTo(data, destination);
//            }
//        }

//        private void ListenForData(Socket socket)
//        {
//            if (!this.isRunning)
//            {
//                return;
//            }

//            // The BeginReceiveFrom requires us to give it an endpoint, even though we don't use it.
//            var state = new PacketState(socket, PacketBufferSize) { Destination = (EndPoint)new IPEndPoint(IPAddress.Any, ServerPort) };
//            byte[] buffer = state.Buffer;
//            EndPoint destination = state.Destination;

//            socket.BeginReceiveFrom(
//                state.Buffer,
//                0,
//                PacketBufferSize,
//                SocketFlags.None,
//                ref destination,
//                new AsyncCallback(this.ReceiveClientData),
//                state);
//        }

//        private void ReceiveClientData(IAsyncResult result)
//        {
//            PacketState state = (PacketState)result.AsyncState;
//            Socket socket = state.UdpSocket;
//            EndPoint endPoint = state.Destination;

//            int receivedData = socket.EndReceiveFrom(result, ref endPoint);
//            if (receivedData == 0)
//            {
//                this.ListenForData(socket);
//                return;
//            }

//            // Create a binary reader so we can deserialize the bytes delivered into an IMessage implementation
//            using (var reader = new BinaryReader(new MemoryStream(state.Buffer)))
//            {
//                reader.BaseStream.Seek(0, SeekOrigin.Begin);
//                // Read the header in from the buffer first so we know what kind of message and how to route.
//                IClientHeader header = new ClientHeader();
//                header.Deserialize(reader);
//                if (!header.IsMessageValid())
//                {
//                    throw new InvalidDataException("The header being returned was malformed.");
//                }

//                // Acknowledge that we received the packet if it is required.
//                if (header.Policy.HasFlag(DatagramPolicy.AcknowledgementRequired) ||
//                    this.ServerPolicy.HasFlag(ServerSpecification.RequireAcknowledgement))
//                {
//                    this.SendMessage(new Acknowledge(), endPoint);
//                }

//                IClientDatagram datagram = this.datagramFactory.CreateDatagramFromClientHeader(header);
//                if (datagram == null)
//                {
//                    // TODO: handle null
//                }

//                datagram.Deserialize(reader);
//            }

//            this.ListenForData(socket);
//        }
//    }
//}

