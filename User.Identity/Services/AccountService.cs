using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using User.Identity.Authorization;
using User.Identity.Data;
using User.Identity.Herlpers;
using User.Identity.Model;
using User.Identity.Entities;
using User.Identity.Helpers;
using Core;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Amazon.Runtime;

namespace User.Identity.Services;
public interface IAccountService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress, string origin);
    Task<int?> ValidateToken(string token);
    Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
    Task RevokeToken(string token, string ipAddress);
    Task Register(RegisterRequest model, string origin);
    Task VerifyEmail(string token);
    Task ForgotPassword(ForgotPasswordRequest model, string origin);
    Task ValidateResetToken(ValidateResetTokenRequest model);
    Task<bool> ResetPassword(ResetPasswordRequest model);
    Task<PaginatedResult<AccountResponse>> GetAll(int pageNumber, int pageSize, string keyword, string origin);
    Task<AccountResponse> GetByIdx(string idx);
    Task<AccountResponse> GetById(int id);
    Task<AccountResponse> Create(CreateRequest model, string origin);
    Task<AccountResponse> Update(string idx, Application.Models.UpdateUserDTO model);
    Task Delete(string idx);
}

public class AccountService : IAccountService
{
    private readonly UserIdentityContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;

    public AccountService(
        UserIdentityContext context,
        IJwtUtils jwtUtils,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IEmailService emailService)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _emailService = emailService;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress, string origin)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email && x.Origin == origin);
        if (account == null)
        {
            throw new AppException("Email doesn't exists on this origin");
        }

        // validate
        if (!BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
            throw new AppException("Email or password is incorrect");
        //else if (!account.IsVerified)
        //    throw new AppException("User is not verified");
        // authentication successful so generate jwt and refresh tokens
        var jwtToken = await _jwtUtils.GenerateJwtToken(account);
        var refreshToken = await _jwtUtils.GenerateRefreshToken(ipAddress);
        account.RefreshTokens.Add(refreshToken);

        // remove old refresh tokens from account
        await removeOldRefreshTokens(account);

        // save changes to db
        _context.Update(account);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = refreshToken.Token;
        return response;
    }
    public async Task<int?> ValidateToken(string token)
    {
        return await _jwtUtils.ValidateJwtToken(token);
        //var tokenHandler = new JwtSecurityTokenHandler();
        //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        //tokenHandler.ValidateToken(token, new TokenValidationParameters
        //{
        //    ValidateIssuerSigningKey = true,
        //    IssuerSigningKey = new SymmetricSecurityKey(key),
        //    ValidateIssuer = false,
        //    ValidateAudience = false
        //}, out SecurityToken validatedToken);


    }



    public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
    {
        var account = await getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            // revoke all descendant tokens in case this token has been compromised
            await revokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
            _context.Update(account);
            await _context.SaveChangesAsync();
        }

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = await rotateRefreshToken(refreshToken, ipAddress);
        account.RefreshTokens.Add(newRefreshToken);

        // remove old refresh tokens from account
        await removeOldRefreshTokens(account);

        // save changes to db
        _context.Update(account);
        await _context.SaveChangesAsync();

        // generate new jwt
        var jwtToken = await _jwtUtils.GenerateJwtToken(account);

        // return data in authenticate response object
        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = newRefreshToken.Token;
        return response;
    }

    public async Task RevokeToken(string token, string ipAddress)
    {
        Account account = await getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // revoke token and save
        await revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _context.Update(account);
        await _context.SaveChangesAsync();
    }
    private async Task<bool> IsEmailAndOriginUnique(string email, string origin)
    {
        // Check if the email and origin combination exists
        return !_context.Accounts.Any(u => u.Email == email && u.Origin == origin);
    }

    public async Task Register(RegisterRequest model, string origin)
    {
        // validate
        if (!await IsEmailAndOriginUnique(model.Email, origin))
        {
            // send already registered error in email to prevent account enumeration
            await sendAlreadyRegisteredEmail(model.Email, origin);
            throw new AppException("User already exists on this origin");
        }

        // map model to new account object
        var account = _mapper.Map<Account>(model);

        // first registered account is an admin
        var isFirstAccount = _context.Accounts.Count() == 0;
        account.Created = DateTime.UtcNow;
        account.Idx = UniqueIndexGenerator.GenerateUniqueAlphanumericIndex(length: 12, prefix: "acc");
        account.VerificationToken = await generateVerificationToken();
        account.Origin = origin;

        // hash password
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // save account
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // send email
        await sendVerificationEmail(account, origin, model.Password);
    }

    public async Task VerifyEmail(string token)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

        if (account == null)
            throw new AppException("Verification failed");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

        // always return ok response to prevent email enumeration
        if (account == null) return;

        // create reset token that expires after 1 day
        account.ResetToken = await generateResetToken();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        // send email
        await sendPasswordResetEmail(account, origin);
    }

    public async Task ValidateResetToken(ValidateResetTokenRequest model)
    {
        await getAccountByResetToken(model.Token);
    }

    public async Task<bool> ResetPassword(ResetPasswordRequest model)
    {
        var account = await getAccountByResetToken(model.Token);
        if (account == null) return false;
        // update password and remove reset token
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedResult<AccountResponse>> GetAll(int pageNumber, int pageSize, string keyword, string origin)
    {
        var query = _context.Accounts.AsQueryable();
        query = query.Where(p => (p.FirstName.Contains(keyword) || p.LastName.Contains(keyword) || p.Email.Contains(keyword)) && p.Origin.Equals(origin));
        int totalCount = query.Count();
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var accounts = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PaginatedResult<AccountResponse>
        {
            Data = _mapper.Map<IList<AccountResponse>>(accounts),
            TotalCount = totalCount,
            TotalPages = totalPages
        };
        return result;
    }

    public async Task<AccountResponse> GetByIdx(string idx)
    {
        var account = await getAccount(idx);
        return _mapper.Map<AccountResponse>(account);
    }
    public async Task<AccountResponse> GetById(int id)
    {
        var account = await getAccountById(id);
        return _mapper.Map<AccountResponse>(account);
    }

    public async Task<AccountResponse> Create(CreateRequest model, string origin)
    {
        // validate
        if (_context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Email '{model.Email}' is already registered");

        // map model to new account object
        var account = _mapper.Map<Account>(model);
        account.Created = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;
        account.Idx =
        // hash password
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        account.Idx = UniqueIndexGenerator.GenerateUniqueAlphanumericIndex(length: 12, prefix: "acc");
        account.VerificationToken = await generateVerificationToken();
        account.Origin = origin;

        // save account
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // send email
        await sendVerificationEmail(account, origin, model.Password);
        return _mapper.Map<AccountResponse>(account);
    }

    public async Task<AccountResponse> Update(string idx, Application.Models.UpdateUserDTO model)
    {
        var account = await getAccount(idx);

        // validate
        if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Email '{model.Email}' is already registered");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // copy model to account and save
        _mapper.Map(model, account);
        account.Updated = DateTime.UtcNow;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountResponse>(account);
    }

    public async Task Delete(string idx)
    {
        var account = await getAccount(idx);
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    // helper methods

    private async Task<Account> getAccount(string idx)
    {
        var account = _context.Accounts.SingleOrDefault(u => u.Idx == idx);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }
    private async Task<Account> getAccountById(int id)
    {
        var account = _context.Accounts.SingleOrDefault(u => u.Id == id);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }
    private async Task<Account> getAccountByRefreshToken(string token)
    {
        var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (account == null) throw new AppException("Invalid token");
        return account;
    }

    private async Task<Account> getAccountByResetToken(string token)
    {
        var account = _context.Accounts.SingleOrDefault(x =>
            x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
        return account;
    }

    private async Task<string> generateJwtToken(Account account)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("idx", account.Idx.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<string> generateResetToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var tokenIsUnique = !_context.Accounts.Any(x => x.ResetToken == token);
        if (!tokenIsUnique)
            return await generateResetToken();

        return token;
    }

    private async Task<string> generateVerificationToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var tokenIsUnique = !_context.Accounts.Any(x => x.VerificationToken == token);
        if (!tokenIsUnique)
            return await generateVerificationToken();

        return token;
    }

    private async Task<RefreshToken> rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = await _jwtUtils.GenerateRefreshToken(ipAddress);
        await revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private async Task removeOldRefreshTokens(Account account)
    {
        account.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private async Task revokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                await revokeRefreshToken(childToken, ipAddress, reason);
            else
                await revokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
        }
    }

    private async Task revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }

    private async Task sendVerificationEmail(Account account, string origin, string password = "")
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            // origin exists if request sent from browser single page app (e.g. Angular or React)
            // so send link to verify via single page app
            var verifyUrl = $"{origin}/api/auth/verify?token={account.VerificationToken}";
            var tempPassword = $@"<p>Your random password is:{password}</p>";
            message = $@"{tempPassword}</br><p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            // origin missing if request sent directly to api (e.g. from Postman)
            // so send instructions to verify directly with api
            message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
        }

        await _emailService.Send(
             to: account.Email,
             subject: "Verify Email",
             html: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
         );
    }

    private async Task sendAlreadyRegisteredEmail(string email, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
        else
            message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

        await _emailService.Send(
            to: email,
            subject: "Email Already Registered",
            html: $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}"
        );
    }

    private async Task sendPasswordResetEmail(Account account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
            message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
        }

        await _emailService.Send(
            to: account.Email,
            subject: "Reset Password",
            html: $@"<h4>Reset Password Email</h4>
                        {message}"
        );
    }
}