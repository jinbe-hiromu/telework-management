using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WorkScheduleServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace WorkScheduleServer.Controllers
{
	[Route("[controller]")]
	public class AuthController : Controller
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly IWebHostEnvironment env;
		private readonly IConfiguration configuration;

		public AuthController(
			IWebHostEnvironment env,
			IConfiguration configuration,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			this.env = env;
			this.configuration = configuration;
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		private async Task<string> CreateJWT(ApplicationUser user)
		{
			var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
			var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

			//var userClaims = await userManager.GetClaimsAsync(user);
			//var roleClaims = (await userManager.GetRolesAsync(user)).Select(x => new Claim("role", x));

			var claims = new[]
			{
				new Claim("uid", user.Id),
				new Claim(ClaimTypes.Name, user.Name), // NOTE: this will be the "User.Identity.Name" value
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // NOTE: this could a unique ID assigned to the user by a database
			}
			//.Union(userClaims)
			//.Union(roleClaims)
			;

			var token = new JwtSecurityToken(
				issuer: configuration["JWT:Issuer"],
				audience: configuration["JWT:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(configuration.GetValue<int>("JWT:DurationInMinutes")),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		/// <summary>
		/// POST <host>/auth/token/request
		/// ex. http://127.0.0.1:5000/auth/token/request
		/// [REQUEST]
		/// Header {
		///   Content-Type: application/json
		/// }
		/// Body {
		///    "UserName": "<UserName>",
		///    "Password": "<Password>"
		/// }
		/// [RESPONSE]
		/// Header {
		///   Content-Type: application/json
		/// }
		/// Body {
		///    "UserName": "<UserName>",
		///    "AccessToken": <Token>
		/// }
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("token/request")]
		[AllowAnonymous]
		public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
		{
			if (request != null && !string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
			{
				var user = await userManager.FindByNameAsync(request.UserName);
				if (user == null)
				{
					return NotFound("Invalid user");
				}

				var result = await signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
				// NOTE:Cookie認証として整合はとれている
				if (result.Succeeded)
				{
					var response = new TokenResponse()
					{
						AccessToken = await CreateJWT(user)
					};

					// TODO: Microsoft.AspNetCore.Identityに対してTokenを登録する方法が不明;

					return Ok(response);
				}
				else
				{
					return Unauthorized("Invalid password");
				}
			}

			return BadRequest();
		}

		/// <summary>
		/// POST <host>/auth/token/release
		/// ex. http://127.0.0.1:5000/auth/token/release
		/// [REQUEST]
		/// Header {
		///   Content-Type: application/json,
		///   Authorization: Bearer <AccessToken>
		/// }
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("token/release")]
		[Authorize]
		public async Task<IActionResult> ReleaseToken([FromHeader] string authorization)
		{
			var username = User.Identity.Name;

			await signInManager.SignOutAsync();
			// NOTE:Cookie認証として整合はとれている

			// TODO: Microsoft.AspNetCore.Identityに対して登録したTokenを破棄する方法が不明;
			// ※：Authorization: Bearer <AccessToken> は使用されていない。Cookie認証で整合はとれていいる。無くても動作する。

			return Ok();
		}
	}
}
