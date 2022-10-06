// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

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
        byte[] applicationIdBytes = System.Text.Encoding.UTF8.GetBytes(applicationId);
        byte[] versionBytes = System.Text.Encoding.UTF8.GetBytes(version);
        byte[] signatureBytes = System.Text.Encoding.UTF8.GetBytes(signatureData);
        byte[] result = new byte[applicationIdBytes.Length + versionBytes.Length + signatureBytes.Length];
        System.Buffer.BlockCopy(applicationIdBytes, 0, result, 0, applicationIdBytes.Length);
        System.Buffer.BlockCopy(versionBytes, 0, result, applicationIdBytes.Length, versionBytes.Length);
        System.Buffer.BlockCopy(signatureBytes, 0, result, applicationIdBytes.Length + versionBytes.Length, signatureBytes.Length);
        return result;
    }
}