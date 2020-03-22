using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using FbAndGoogleAutentication.Models;
//using System.Web.Services.Description;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;

namespace FbAndGoogleAutentication
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Inserire qui la parte di codice del servizio di posta elettronica per l'invio di un messaggio.
            //return Task.FromResult(0);
            return Task.Factory.StartNew(() =>
            { 
            sendMail(message);
            });
        }

        void sendMail(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
          
            //string html = "Please confirm your account by clicking this link: <a href='" + message.Body + "'>link</a><br/>";
            string html =  message.Body ;
            html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
            //impostare Less secure app access enabled se si vuole usare l'smtp di google per spedire messaggi, andare qui per impostarlo:
            //https://myaccount.google.com/lesssecureapps
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Inserire qui la parte di codice del servizio SMS per l'invio di un SMS.
            // Twilio Begin
            //var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            //var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            //var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            //TwilioClient.Init(accountSid, authToken);

            //MessageResource result = MessageResource.Create(
            //new PhoneNumber(message.Destination),
            //from: new PhoneNumber(fromNumber),
            //body: message.Body
            //);

            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            //Trace.TraceInformation(result.Status.ToString());
            ////Twilio doesn't currently have an async API, so return success.
            //return Task.FromResult(0);    
            // Twilio End

            // ASPSMS Begin 
            // var soapSms = new MvcPWx.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            // soapSms.SendSimpleTextSMS(
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"],
            //   message.Destination,
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"],
            //   message.Body);
            // soapSms.Close();
            // return Task.FromResult(0);
            // ASPSMS End
            return Task.FromResult(0);
        }
    }

    // Configurare la gestione utenti dell'applicazione utilizzata in questa applicazione. UserManager viene definito in ASP.NET Identity ed è utilizzato dall'applicazione.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configurare la logica di convalida per i nomi utente
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configurare la logica di convalida per le password
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configurare le impostazioni predefinite per il blocco dell'utente
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registrare i provider di autenticazione a due fattori. Questa applicazione usa il numero di telefono e gli indirizzi e-mail come metodi per ricevere un codice di verifica dell'utente
            // Si può scrivere un provider personalizzato e inserirlo qui.
            manager.RegisterTwoFactorProvider("Codice telefono", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Il codice di sicurezza è {0}"
            });
            manager.RegisterTwoFactorProvider("Codice e-mail", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Codice di sicurezza",
                BodyFormat = "Il codice di sicurezza è {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configurare la gestione accessi usata in questa applicazione.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
