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
public class LogBuilderController : ControllerBase
{
    private readonly ILogger<LogBuilderController> _logger;
    private readonly TrillianLog.TrillianLogClient _logClient;

    public LogBuilderController(ILogger<LogBuilderController> logger)
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
}
