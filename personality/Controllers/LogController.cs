// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Trillian;
using Microsoft.AspNetCore.Authorization;

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
         string? address = Environment.GetEnvironmentVariable("trillian_url");
        if(address == null) throw new HttpRequestException();                
        GrpcChannel channel = GrpcChannel.ForAddress(address);
        _logClient = new TrillianLog.TrillianLogClient(channel);
    }

    [HttpPost(Name = "AddLogEntry")]
    [Authorize]
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

    [HttpGet(Name="LatestSignedLogRoot")]
    public string GetLatestSignedLogRoot(long treeId) {
        GetLatestSignedLogRootRequest request = new GetLatestSignedLogRootRequest();
        request.LogId = treeId;
        GetLatestSignedLogRootResponse response = _logClient.GetLatestSignedLogRoot(request);
        return response.Proof.Hashes.ToString();
    }
}
