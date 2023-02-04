using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkScheduleServer.Models
{
	public class TokenRequest
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}

	public class TokenResponse
	{
		public string AccessToken { get; set; }
	}

}

