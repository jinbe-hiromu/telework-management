using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkScheduleServer.Models;

namespace WorkScheduleServer.Controllers
{
    public static class AccessTokenManager
    {
        public static string CreateAccessToken(string userName)
        {
            string accessToken = string.Empty;

            accessToken = AESCryption.Encrypt(userName);

            return accessToken;
        }

        public static string GetAccessToken(string userName)
        {
            string accessToken = string.Empty;

            accessToken = AESCryption.Encrypt(userName);

            return accessToken;
        }

        public static void ReleaseAccessToken(string accessToken)
        {

        }

        public static async Task<bool> IsAccessTokenValid(
            string accessToken, 
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            if(string.IsNullOrEmpty(accessToken)) { return false; }

            var user = GetUserFormAccessToken(accessToken, signInManager, userManager, roleManager);

            return (user == null ? false : true);
        }

        public static async Task<ApplicationUser> GetUserFormAccessToken(
            string accessToken, 
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            if (string.IsNullOrEmpty(accessToken)) { return null; }

            var username = AESCryption.Decrypt(accessToken);

            ApplicationUser user = await userManager.FindByEmailAsync(username);

            return user;
        }
    }
}
