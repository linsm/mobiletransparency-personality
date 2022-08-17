public class LeafValue
{
    public LeafValue()
    {
        this.applicationId = "";
        this.version = "";
        this.signatureData = "";
    }

    public LeafValue(string applicationId,
                     string version,
                     string signatureData)
    {
        this.applicationId = applicationId;
        this.version = version;
        this.signatureData = signatureData;
    }
    public LeafValue(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.applicationId = reader.ReadString();
                this.version = reader.ReadString();
                this.signatureData = reader.ReadString();
            }
        }
    }
    public string applicationId { get; set; }
    public string version { get; set; }
    public string signatureData { get; set; }

    public byte[] toByteArray()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(applicationId);
                writer.Write(version);
                writer.Write(signatureData);
            }
            return stream.ToArray();
        }
    }
}