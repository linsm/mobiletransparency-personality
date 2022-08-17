using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Trillian;

namespace personality.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly TrillianLog.TrillianLogClient _logClient;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
        //@TODO create connection service
        var channel = GrpcChannel.ForAddress("INSERTIP");
        _logClient = new TrillianLog.TrillianLogClient(channel);
    }

    [HttpPost(Name = "AddLogEntry")]
    public string AddLogEntry(long treeId, [FromBody] LeafValue value)
    {
        Leaf leaf = new Leaf(treeId, value);
        QueueLeafRequest leafRequest = new QueueLeafRequest();
        leafRequest.LogId = leaf.treeId;
        leafRequest.Leaf = leaf.buildLogLeaf();
        var reply = _logClient.QueueLeaf(leafRequest);
        return reply.ToString();
    }

    [HttpPost(Name = "InclusionProof")]
    public string InclusionProof(long treeId, long treeSize, [FromBody] LeafValue leafValue)
    {
        var request = new GetInclusionProofByHashRequest();
        var leaf = new Leaf(treeId, leafValue);
        request.LogId = leaf.treeId;
        request.LeafHash = leaf.hashValue;
        request.TreeSize = treeSize;
        var response = _logClient.GetInclusionProofByHash(request);
        return response.ToString();
    }

    [HttpGet(Name = "ConsistencyProof")]
    public string ConsistencyProof(long treeId, long firstTreeSize, long secondTreeSize)
    {
        var request = new GetConsistencyProofRequest();
        request.LogId = treeId;
        request.FirstTreeSize = firstTreeSize;
        request.SecondTreeSize = secondTreeSize;
        var response = _logClient.GetConsistencyProof(request);
        return response.ToString();
    }
}
