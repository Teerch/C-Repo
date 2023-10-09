using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using UserAuthAPI.Data;
using UserAuthAPI.Models;
using UserAuthAPI.Models.DTO;

namespace UserAuthAPI.Services;
public class UserAuthService
{
    private readonly UserAuthContext _context;
    private readonly BackgroundJobClient _backgroundjob;

    public UserAuthService(UserAuthContext context, BackgroundJobClient backgroundJob)
    {
        _context = context;
        _backgroundjob = backgroundJob;
    }

    public async Task<IEnumerable<UserResponseDTO>?> GetUsersAsync()
    {
        try
        {
            var user = await _context.Users.ToListAsync();
            TinyMapper.Bind<List<User>, List<UserResponseDTO>>();

            var response = TinyMapper.Map<List<UserResponseDTO>>(user);
            return response;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<UserResponseDTO?> GetUserByEmailAsync(string email)
    {
        try
        {
            var response = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            TinyMapper.Bind<User, UserResponseDTO>();
            var result = TinyMapper.Map<UserResponseDTO>(response);
            return result;
        }
        catch (Exception)
        {
            return null;
        }

    }

    public async Task<User?> CreateUserAsync([FromBody] string firstname, string lastname, string username, string password, string email)
    {
        try
        {
            byte[] passwordHash, passwordKey;
            using (var hmc = new HMACSHA512())
            {
                passwordKey = hmc.Key;
                passwordHash = hmc.ComputeHash(Encoding.Unicode.GetBytes(password));
            }

            User user = new()
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username,
                Password = passwordHash,
                Passwordkey = passwordKey,
                Email = email
            };


            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var subject = $"Welcoome to our platform";
            var body = $"Welcome {username}";
            _backgroundjob.Schedule(() => SmtpSender(email, subject, body), TimeSpan.FromSeconds(5));

            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UserAlreadyExists(string userName)
    {
        try
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> EmailAlreadyExists(string email)
    {
        try
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
    {
        try
        {
            using var hmac = new HMACSHA512(passwordKey);
            var passwordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(passwordText));
            for (int i = 0; i < password.Length; i++)
            {
                if (passwordHash[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserResponseDTO?> AutenticateUserAsync(string username, string password)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.UserName == username);

            if (user == null || user.Password == null)
            {
                return null;
            }
            if (!MatchPasswordHash(password, user.Password, user.Passwordkey))
            {
                return null;
            }
            TinyMapper.Bind<User, UserResponseDTO>();
            var result = TinyMapper.Map<UserResponseDTO>(user);
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<SecurityQandA?> CreateSecurityQandAAsync([FromBody] string email, string Question, string Answer)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            byte[] securityAnswerHash, securityAnswerKey;
            using (var hmac = new HMACSHA512())
            {
                securityAnswerKey = hmac.Key;
                securityAnswerHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(Answer));
            }
            SecurityQandA securityQandA = new()
            {
                Question = Question,
                Answer = securityAnswerHash,
                Answerkey = securityAnswerKey,
                UserId = user.Id,
                UserEmail = email
            };

            await _context.AddAsync(securityQandA);
            await _context.SaveChangesAsync();

            return securityQandA;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string PhraseGenerator()
    {
        List<string> myList = new() { "dog", "rat", "cat", "bat"
                                    , "cow", "mouse", "pig", "fish"
                                    , "car", "house", "man", "woman"
                                    , "person", "thing", "make", "break"
                                    , "show", "celeb", "influence" };

        Random random = new();

        // Shuffle the list using the Fisher-Yates algorithm
        for (int i = myList.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            string temp = myList[i];
            myList[i] = myList[randomIndex];
            myList[randomIndex] = temp;
        }

        // Take the first 4 elements from the shuffled list
        var phrase = myList.GetRange(0, 10);
        var phraseString = string.Join(" ", phrase);
        return phraseString;
    }

    public byte[] EncryptString(string plainText, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using StreamWriter swEncrypt = new(csEncrypt);
            swEncrypt.Write(plainText);
            // byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
            // csEncrypt.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
            // csEncrypt.FlushFinalBlock();
        }

        return msEncrypt.ToArray();
    }

    public async Task<string> DecryptString(byte[] cipherText, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(cipherText);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        string plainText = await srDecrypt.ReadToEndAsync();
        return plainText;
    }

    public async Task<RecoveryPhrase?> CreateRecoveryPhraseAsync(string email)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.Email == email);

            if (user == null)
            {
                return null;
            }

            var phrase = PhraseGenerator();
            // var recoverystring = phrase.Replace(" ", "").ToUpper();
            byte[] encryptionKey = new byte[32]; // 256 bits
            byte[] iv = new byte[16]; // 128 bits


            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(encryptionKey);
                rng.GetBytes(iv);
            }

            // Encrypt the string
            var encryptedPhrase = EncryptString(phrase, encryptionKey, iv);

            RecoveryPhrase recovery = new()
            {
                Phrase = encryptedPhrase,
                PhraseKey = encryptionKey,
                PhraseIV = iv,
                UserId = user.Id,
                Useremail = user.Email
            };

            await _context.AddAsync(recovery);
            await _context.SaveChangesAsync();

            return recovery;
        }
        catch (Exception)
        {
            return null;
        }

    }

    public async Task<string?> GetRecoveryPhraseAsync(string email)
    {
        try
        {
            var recovery = await CreateRecoveryPhraseAsync(email);

            var phrase = await DecryptString(recovery.Phrase, recovery.PhraseKey, recovery.PhraseIV);
            return phrase;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool MatchAnswerHash(string answertext, byte[] answer, byte[] answerKey)
    {
        using var hmac = new HMACSHA512(answerKey);
        var answerHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(answertext));
        for (int i = 0; i < answer.Length; i++)
        {
            if (answerHash[i] != answer[i])
            {
                return false;
            }
        }
        return true;
    }

    public bool GetSecurityQuestionByEmailAsync(string userEmail)
    {
        try
        {
            var response = _context.SecurityQandAs.Where(x => x.UserEmail == userEmail);
            if (response == null)
            {
                return false;
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<User?> ResetPasswordUsingSecurityQandAAsync(string email, string question, string answer, string newpassword)
    {
        try
        {
            var existingUser = await _context.Users.FirstAsync(x => x.Email == email);

            if (!GetSecurityQuestionByEmailAsync(email))
            {
                return null;
            }
            var securityResponse = await _context.SecurityQandAs.FirstAsync(x => x.Question == question);

            if (securityResponse == null)
            {
                return null;
            }

            if (!MatchAnswerHash(answer, securityResponse.Answer, securityResponse.Answerkey))
            {
                return null;
            }

            byte[] newPasswordKey, newPasswordHash;
            using (var hmac = new HMACSHA512())
            {
                newPasswordKey = hmac.Key;
                newPasswordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(newpassword));
            }

            existingUser.Password = newPasswordHash;
            existingUser.Passwordkey = newPasswordKey;

            await _context.SaveChangesAsync();

            return existingUser;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<User?> ResetPasswordUsingPhraseAsync(string email, string phrase, string newpassword)
    {
        try
        {
            var existingUser = await _context.Users.FirstAsync(x => x.Email == email);
            var recoveryPhrase = await _context.RecoveryPhrases.FirstAsync(x => x.Useremail == email);

            var recoverystring = phrase.Replace(" ", "").ToUpper();

            if (recoveryPhrase.Phrase == null)
            {
                return null;
            }

            var stringPhrase = await DecryptString(recoveryPhrase.Phrase, recoveryPhrase.PhraseKey, recoveryPhrase.PhraseIV);
            var Phrasestring = stringPhrase.Replace(" ", "").ToUpper();
            if (recoverystring != Phrasestring)
            {
                return null;
            }

            byte[] newPasswordKey, newPasswordHash;
            using (var hmac = new HMACSHA512())
            {
                newPasswordKey = hmac.Key;
                newPasswordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(newpassword));
            }

            existingUser.Password = newPasswordHash;
            existingUser.Passwordkey = newPasswordKey;

            await _context.SaveChangesAsync();

            return existingUser;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public int GenerateRandomOTP()
    {
        Random random = new();
        return random.Next(100000, 999999);
    }

    public bool ElaspedOTP(DateTime dateTime)
    {
        TimeSpan elasped = DateTime.UtcNow - dateTime;

        if (elasped.TotalMinutes > 3)
        {
            return true;
        }

        return false;
    }

    public async Task DeleteOtp(string email)
    {
        var response = await _context.OTPs.Where(x => x.Useremail == email).ToListAsync();
        if (response == null)
        {
            return;
        }
        response.ForEach(item => _context.Remove(item));
        await _context.SaveChangesAsync();
        return;

    }

    public async Task<int> CreateOTPAsync(string email)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.Email == email);

            if (user == null)
            {
                return 0;
            }

            var otp = GenerateRandomOTP();

            OTP NewOtp = new()
            {
                Code = otp,
                UserId = user.Id,
                Useremail = user.Email
            };

            await _context.AddAsync(NewOtp);
            await _context.SaveChangesAsync();

            return otp;
        }
        catch (Exception)
        {
            return 0;
        }

    }

    public async Task SmtpSender(string email, string subject, string body)
    {

        string mailsubject = subject;
        string mailbody = body;
        var mail = "oyundoyintemitayo51@gmail.com";
        var emailPass = "stlpuzwhoyunzprl";



        using SmtpClient smtpClient = new("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(mail, emailPass),
            EnableSsl = true,
        };

        using MailMessage mailMessage = new(mail, email)
        {
            Subject = mailsubject,
            Body = mailbody
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception)
        {
            throw new Exception(null);
        }

        await _context.SaveChangesAsync();

    }

    public async Task<int> SendOtpToUserEmail(string email)
    {
        try
        {
            var otp = await CreateOTPAsync(email);

            if (otp == 0)
            {
                return 0;
            }

            string subject = "Your OTP Code";
            string body = $"Your OTP code is: {otp}";

            _backgroundjob.Enqueue(() => SmtpSender(email, subject, body));
            _backgroundjob.Schedule(() => DeleteOtp(email), TimeSpan.FromMinutes(5));

            return otp;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<User?> ResetPasswordUsingOTPAsync(int otp, string newpassword)
    {
        try
        {
            var Otp = await _context.OTPs.FirstAsync(x => x.Code == otp);

            var existingUser = await _context.Users.FirstAsync(x => x.Email == Otp.Useremail);
            var elaspedOTPTime = ElaspedOTP(Otp.CreatedOn);

            if (elaspedOTPTime == true)
            {
                throw new Exception("otp has expired");
            }

            if (Otp.Code != otp)
            {
                return null;
            }

            byte[] newPasswordKey, newPasswordHash;
            using (var hmac = new HMACSHA512())
            {
                newPasswordKey = hmac.Key;
                newPasswordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(newpassword));
            }

            existingUser.Password = newPasswordHash;
            existingUser.Passwordkey = newPasswordKey;
            _backgroundjob.Schedule(() => DeleteOtp(Otp.Useremail), TimeSpan.FromMinutes(1));
            await _context.SaveChangesAsync();

            return existingUser;
        }
        catch (Exception)
        {
            return null;
        }
    }

}
// cow mouse pig cat bat dog fish