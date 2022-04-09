using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoServerSide.DTOs;
using TodoServerSide.Models;
using TodoServerSide.Repositories;

namespace TodoServerSide.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private IConfiguration _configuration;
    private IUserRepository _user;
    private readonly ILogger<UserController> _logger;

    public UserController(IConfiguration configuration, IUserRepository user, ILogger<UserController> logger)
    {
        _configuration = configuration;
        _user = user;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromBody] UserDTO data)
    {
        var toCreateUser = new User
        {
            UserName = data.UserName,
            Password = data.Password,
        };
        var createdUser = await _user.Create(toCreateUser);
        return StatusCode(StatusCodes.Status201Created, createdUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDTO userLogin)
    {

        var user = await _user.GetByUserName(userLogin.UserName);
        if (user == null)
            return NotFound("User not found");
        if (!user.Password.Equals(userLogin.Password))
            return Unauthorized("Invalid password");
        var token = Generate(user);
        return Ok(token);
    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),

        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(35),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }




}
