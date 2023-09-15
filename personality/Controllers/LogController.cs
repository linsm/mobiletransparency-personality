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

namespace personality.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly TrillianLog.TrillianLogClient _logClient;
    private readonly TrillianAdmin.TrillianAdminClient _adminClient;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
         string? address = Environment.GetEnvironmentVariable("trillian_url");
        if(address == null) throw new HttpRequestException();                
        GrpcChannel channel = GrpcChannel.ForAddress(address);
        _logClient = new TrillianLog.TrillianLogClient(channel);
        _adminClient = new TrillianAdmin.TrillianAdminClient(channel);        
    }

    [HttpGet(Name = "ListTrees")]
    public string ListTrees() {
        ListTreesResponse accessibleTrees = _adminClient.ListTrees(new ListTreesRequest());
        return accessibleTrees.ToString();
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
    public string LatestSignedLogRoot(long treeId) {
        GetLatestSignedLogRootRequest request = new GetLatestSignedLogRootRequest();
        request.LogId = treeId;        
        GetLatestSignedLogRootResponse response = _logClient.GetLatestSignedLogRoot(request);
        return response.SignedLogRoot.ToString();
    }
}
