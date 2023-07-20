using System.Text;

namespace NetEti.ObjectSerializer
{

    internal class ItemHeader
    {
        public const int ItemHeaderLength = 256;

        public Int32 TypeLength { get; set; }

        public string TypeName
        {
            get
            {
                return this._typeName;
            }
            set
            {
                this._typeName = FixStringLength(value, this.TypeLength);
            }
        }

        public byte[] GetFixedLengthItemHeaderByteArray()
        {
            // WriteInt32BigEndian(Span<byte> destination, int value);

            byte[] first = BitConverter.GetBytes(this.TypeLength);
            byte[] second = Encoding.ASCII.GetBytes(this.TypeName);
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public ItemHeader(Int32 typeLength, string typeName)
        {
            this.TypeLength = typeLength;
            this._typeName = FixStringLength(typeName, _typeNameLength);
        }

        /// <summary>
        /// Konstruktor - deserialisiert die Daten für ItemHeader aus einem übergebenen Bytearray.
        /// </summary>
        public ItemHeader(byte[] inBytes)
        {
            if (inBytes == null || inBytes.Length < ItemHeaderLength)
                throw new ArgumentException(nameof(inBytes) + " nicht ausreichend groß.");

            byte[] lengthBytes = new byte[sizeof(Int32)];
            byte[] typeNameBytes = new byte[_typeNameLength];
            Buffer.BlockCopy(inBytes, 0, lengthBytes, 0, lengthBytes.Length);
            Buffer.BlockCopy(inBytes, lengthBytes.Length, typeNameBytes, 0, typeNameBytes.Length);

            this.TypeLength = BitConverter.ToInt32(lengthBytes, 0);
            this._typeName = Encoding.ASCII.GetString(typeNameBytes);
        }

        public override string ToString()
        {
            return String.Format($"Length: {this.TypeLength},  Type: {this.TypeName.Trim()}");
        }

        private string _typeName;
        private static int _typeNameLength;

        static ItemHeader()
        {
            _typeNameLength = ItemHeaderLength - sizeof(Int32);
        }

        private static string FixStringLength(string? inString, int outLength)
        {
            if (String.IsNullOrEmpty(inString))
            {
                throw new ArgumentNullException(nameof(inString));
            }
            return inString.PadRight(outLength, ' ').Substring(0, outLength);
        }
    }
}
