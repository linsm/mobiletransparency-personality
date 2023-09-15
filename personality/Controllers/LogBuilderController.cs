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
