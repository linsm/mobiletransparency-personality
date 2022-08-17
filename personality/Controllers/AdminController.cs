using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Trillian;

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
        //@TODO create connection service
        GrpcChannel channel = GrpcChannel.ForAddress("INSERTIP");
        _adminClient = new TrillianAdmin.TrillianAdminClient(channel);        
        _logClient = new TrillianLog.TrillianLogClient(channel);
    }

    [HttpGet(Name = "GetTreeDetails")]
    public string ListTrees() {
        ListTreesResponse accessibleTrees = _adminClient.ListTrees(new ListTreesRequest());
        return accessibleTrees.ToString();
    }

    [HttpPost(Name = "CreateTree")]
    public long CreateTree() {
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
        return newTree.TreeId;
    }

    [HttpPost(Name = "DeleteTree")]
    public void DeleteTree(long treeId) {
        DeleteTreeRequest deleteRequest = new DeleteTreeRequest();
        deleteRequest.TreeId = treeId;
        _adminClient.DeleteTree(deleteRequest);
    }
}

