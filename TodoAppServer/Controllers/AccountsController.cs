using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoAppServer.Configuration;
using TodoAppServer.Data;
using TodoAppServer.DTOs.Requests;
using TodoAppServer.DTOs.Responses;
using TodoAppServer.Models;

namespace TodoAppServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly JwtConfig jwtConfig;
        private readonly TodoAppContext todoAppContext;

        public AccountsController(
            UserManager<AppUser> userManager,
            IOptionsMonitor<JwtConfig> JwtoptionsMonitor,
            TodoAppContext todoAppContext)
        {
            this.userManager = userManager;
            this.jwtConfig = JwtoptionsMonitor.CurrentValue;
            this.todoAppContext = todoAppContext;
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest signupRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingEmail = await userManager.FindByEmailAsync(signupRequest.Email);

                    if (existingEmail == null)
                    {
                        var existingUsername = await userManager.FindByNameAsync(signupRequest.Username);

                        if (existingUsername == null)
                        {
                            AppUser newUser = new AppUser()
                            {
                                Email = signupRequest.Email,
                                UserName = signupRequest.Username
                            };

                            var isCreated = await userManager.CreateAsync(newUser, signupRequest.Password);

                            if (isCreated.Succeeded)
                            {
                                SignupResponse signupResponse = new SignupResponse
                                {
                                    Username = newUser.UserName,
                                    Email = newUser.Email
                                };

                                return Ok(signupResponse);
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            }
                        }
                        else
                        {
                            return BadRequest("Username already in use");
                        }
                    }
                    else
                    {
                        return BadRequest("Email already in use");
                    }
                }
                else
                {
                    return BadRequest("Invalid request");
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(loginRequest.Email);

                    if (user != null)
                    {
                        var isCorrect = await userManager.CheckPasswordAsync(user, loginRequest.Password);

                        if (isCorrect)
                        {
                            var jwtToken = GenerateJwtToken(user);

                            LoginResponse loginResponse = new LoginResponse
                            {
                                Username = user.UserName,
                                Email = user.Email,
                                Token = jwtToken
                            };

                            return Ok(loginResponse);
                        }
                        else
                        {
                            return BadRequest("Invalid email and/or password");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid email and/or password");
                    }
                }
                else
                {
                    return BadRequest("Invalid request");
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string GenerateJwtToken(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        } 
    }
}
