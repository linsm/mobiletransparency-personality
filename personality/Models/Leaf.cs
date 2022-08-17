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
        prefix.CopyTo(result,0);
        leafValueBytes.CopyTo(result,prefix.Length);        
        return pb::ByteString.CopyFrom(SHA256.HashData(result));
    }

}