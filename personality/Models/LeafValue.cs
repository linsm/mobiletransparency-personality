/*
** Copyright (C) 2023  Johannes Kepler University Linz, Institute of Networks and Security
** Copyright (C) 2023  CDL Digidow <https://www.digidow.eu/>
**
** Licensed under the EUPL, Version 1.2 or – as soon they will be approved by
** the European Commission - subsequent versions of the EUPL (the "Licence").
** You may not use this work except in compliance with the Licence.
** 
** You should have received a copy of the European Union Public License along
** with this program.  If not, you may obtain a copy of the Licence at:
** <https://joinup.ec.europa.eu/software/page/eupl>
** 
** Unless required by applicable law or agreed to in writing, software
** distributed under the Licence is distributed on an "AS IS" basis,
** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
** See the Licence for the specific language governing permissions and
** limitations under the Licence.
**
*/

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