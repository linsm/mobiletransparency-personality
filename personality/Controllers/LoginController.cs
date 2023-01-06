// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

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

    [HttpPost(Name = "Login")]
    [AllowAnonymous]
    public ActionResult Login([FromBody] User user)
    {
        if(user != null) {
            if((user.Username == "admin" && user.Password == "asdf123") || (user.Username == "buildserver" && user.Password == "asdf456")) {
                string jwtToken = GenerateJwtToken(user);
                return Ok(new {token = jwtToken});
            }
        }
        return Unauthorized();
    }

    private string GenerateJwtToken(User user) {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTAuthentication:SecurityKey"]));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Username));

        if(user.Username == "admin") {
            claims.Add(new Claim(ClaimTypes.Role, "treeManager"));
        }

        var jwtToken = new JwtSecurityToken(_configuration["JWTAuthentication:Issuer"], 
            _configuration["JWTAuthentication:Audience"], 
            claims, 
            expires: DateTime.Now.AddDays(1), 
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}