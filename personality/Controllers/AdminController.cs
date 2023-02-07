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
public class AdminController : ControllerBase
{   
    private readonly ILogger<AdminController> _logger;    
    private readonly TrillianAdmin.TrillianAdminClient _adminClient;
    private readonly TrillianLog.TrillianLogClient _logClient;

    public AdminController(ILogger<AdminController> logger)
    {        
        _logger = logger;
        string? address = Environment.GetEnvironmentVariable("trillian_url");
        if(address == null) throw new HttpRequestException();                
        GrpcChannel channel = GrpcChannel.ForAddress(address);
        _adminClient = new TrillianAdmin.TrillianAdminClient(channel);        
        _logClient = new TrillianLog.TrillianLogClient(channel);        
    }

    [HttpPost(Name = "CreateTree")]
    [Authorize(Policy = "onlyTreeManager")]
    public ActionResult CreateTree() {      
        Tree tree = new Tree();
        tree.TreeType = TreeType.Log;        
        tree.TreeState = TreeState.Active;
        tree.MaxRootDuration = new Google.Protobuf.WellKnownTypes.Duration();

        CreateTreeRequest createTreeRequest = new CreateTreeRequest();                
        createTreeRequest.Tree = tree;        

        Tree newTree = _adminClient.CreateTree(createTreeRequest);             
        InitLogRequest initRequest = new InitLogRequest();       
        initRequest.LogId = newTree.TreeId;
        InitLogResponse initResponse = _logClient.InitLog(initRequest);   
        return Ok(newTree.TreeId);    
    }

    [HttpPost(Name = "DeleteTree")]
    [Authorize(Policy = "onlyTreeManager")]
    public void DeleteTree(long treeId) {
        DeleteTreeRequest deleteRequest = new DeleteTreeRequest();
        deleteRequest.TreeId = treeId;
        _adminClient.DeleteTree(deleteRequest);
    }
}

