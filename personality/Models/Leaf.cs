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

using System.Security.Cryptography;
using Trillian;
using pb = global::Google.Protobuf;

public class Leaf {

    public Leaf(long treeId, LeafValue value) {     
        this.treeId = treeId;
        this.value = value;       
    }
    public long treeId {get; private set;}
    public LeafValue value {get; private set;}             
    public pb::ByteString hashValue { get { 
        return calculateHashValue();
    }}  
    
    public LogLeaf buildLogLeaf() {
        var logLeaf = new LogLeaf();                
        logLeaf.LeafValue = pb::ByteString.CopyFrom(value.toByteArray());
        return logLeaf;
    }
    private pb::ByteString calculateHashValue() {
        byte[] prefix = { 0 };
        byte[] leafValueBytes = value.toByteArray();
        byte[] result = new byte[prefix.Length + leafValueBytes.Length];
        System.Buffer.BlockCopy(prefix, 0, result, 0, prefix.Length);
        System.Buffer.BlockCopy(leafValueBytes, 0, result, prefix.Length, leafValueBytes.Length);
        return pb::ByteString.CopyFrom(SHA256.HashData(result));
    }
}