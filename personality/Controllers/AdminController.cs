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

