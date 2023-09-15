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
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace personality.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly IConfiguration _configuration;

    public LoginController(ILogger<LogController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost(Name = "RequestAccessToken")]
    [AllowAnonymous]
    public ActionResult RequestAccessToken([FromBody] User user)
    {
        if(user != null) {
            if((user.Username == "admin" && user.Password == Environment.GetEnvironmentVariable("admin_password")) || (user.Username == "buildserver" && user.Password == Environment.GetEnvironmentVariable("buildserver_password"))) {
                string jwtToken = GenerateJwtToken(user);
                return Ok(new {token = jwtToken});
            }
        }
        return Unauthorized();
    }

    private string GenerateJwtToken(User user) {
        string? securityKey = Environment.GetEnvironmentVariable("security_key");
        string? issuer = Environment.GetEnvironmentVariable("issuer");
        string? audience = Environment.GetEnvironmentVariable("audience");
        if(securityKey != null && issuer != null && audience != null)
        {
             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Username));

            if(user.Username == "admin") {
                claims.Add(new Claim(ClaimTypes.Role, "treeManager"));
            }

            var jwtToken = new JwtSecurityToken(issuer, 
                audience, 
                claims, 
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        return "";       
    }
}