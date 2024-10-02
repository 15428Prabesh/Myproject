using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using User.Identity.Authorization;
using User.Identity.Entities;
using User.Identity.Model;
using User.Identity.Services;
using Core;
using User.Identity.Helpers;
using System.ComponentModel.DataAnnotations;

namespace User.Identity.Controllers;
[ApiController]
[Route("api/[controller]")]
//[Authorize]
[TypeFilter(typeof(AuthorizeAccount))]
public class AccountsController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
        var response = await _accountService.Authenticate(model, await ipAddress(), Request.Headers.Origin.ToString());
        await setTokenCookie(response.RefreshToken);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "", response);
    }
    [AllowAnonymous]
    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var accountId = await _accountService.ValidateToken(token);
        if (accountId != null)
        {
            var account = await _accountService.GetById(accountId.Value);
            return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "", account);
        }
        return JsonResponse.CreateJsonResponse(HttpStatusCode.Unauthorized, "Token is unauthorized");
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<AuthenticateResponse> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = new AuthenticateResponse();
        if (refreshToken != null)
        {
            response = await _accountService.RefreshToken(refreshToken, await ipAddress());
            await setTokenCookie(response.RefreshToken);
        }
        return response;
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
    {
        // accept token from request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return JsonResponse.CreateJsonResponse(HttpStatusCode.BadRequest, "Token is required");
        // users can revoke their own tokens and admins can revoke any tokens
        if (!Account.OwnsToken(token))
            return JsonResponse.CreateJsonResponse(HttpStatusCode.Unauthorized, "Token is unauthorized");
        await _accountService.RevokeToken(token, await ipAddress());
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Token revoked");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        await _accountService.Register(model, Request.Headers.Origin.ToString());
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Registration successful, please check your email for verification instructions");
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([Required] string token)
    {
        await _accountService.VerifyEmail(token);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Verification successful, you can now login");
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await _accountService.ForgotPassword(model, Request.Headers.Origin.ToString());
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Please check your email for password reset instructions");
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
    {
        await _accountService.ValidateResetToken(model);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Token is valid");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        bool resetPassword = await _accountService.ResetPassword(model);
        return JsonResponse.CreateJsonResponse(resetPassword ? HttpStatusCode.OK : HttpStatusCode.BadRequest, resetPassword ? "Password reset successful, you can now login" : "Invalid Token");
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string keyword = "")
    {
        var accounts = await _accountService.GetAll(pageNumber, pageSize, keyword, Request.Headers.Origin.ToString());
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "", accounts);
    }

    [AllowAnonymous]
    [HttpGet("{idx}")]
    public async Task<IActionResult> GetById(string idx)
    {
        // users can get their own account and admins can get any account
        //if (idx != Account.Idx)
        //    return JsonResponse.CreateJsonResponse(HttpStatusCode.Unauthorized, "Account is unathorized");

        var account = await _accountService.GetByIdx(idx);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "", account);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRequest model)
    {
        var account = await _accountService.Create(model, Request.Headers.Origin.ToString());
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "User created successfully", account.Idx);
    }

    [AllowAnonymous]
    [HttpPut("{idx}")]
    public async Task<IActionResult> Update(string idx, Application.Models.UpdateUserDTO model)
    {
        // users can update their own account and admins can update any account
        //if (idx != Account.Idx)
        //    return JsonResponse.CreateJsonResponse(HttpStatusCode.Unauthorized, "Account is unathorized");

        var account = await _accountService.Update(idx, model);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Account updated successfully", account.Idx);
    }

    [HttpDelete("{idx}")]
    public async Task<IActionResult> Delete(string idx)
    {
        // users can delete their own account
        if (idx != Account.Idx)
            return JsonResponse.CreateJsonResponse(HttpStatusCode.Unauthorized, "Account is unathorized");
        await _accountService.Delete(idx);
        return JsonResponse.CreateJsonResponse(HttpStatusCode.OK, "Account deleted successfully");
    }

    // helper methods

    private async Task setTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private async Task<string> ipAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}