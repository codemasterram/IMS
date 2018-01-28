using System;
using System.Text;
using System.Web;
using System.Web.Security;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels.BackOffice.User;

namespace IMS.Logic
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly TimeSpan _expirationTimeSpan;
        private AppUserViewModel _cachedUser;

        private IAuthService _authService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="customerSettings">Customer settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext, IAuthService authService)
        {
            this._httpContext = httpContext;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            this._authService = authService;
        }


        public virtual void SignIn(string username, string sessionDataCsv, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                username,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = _authService.GetUser(username);
        }

        public virtual void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
            HttpContext.Current.Response.Redirect("/user/login");
        }

        public virtual AppUserViewModel GetAuthenticatedCustomer()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null || _httpContext.Request == null || !_httpContext.Request.IsAuthenticated || _httpContext.User == null)
                return null;

            AppUserViewModel user = null;
            FormsIdentity formsIdentity = null;

            if ((formsIdentity = _httpContext.User.Identity as FormsIdentity) != null)
            {
                user = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            }

            if (user != null)
            {
                _cachedUser = user;
            }

            return _cachedUser;
        }

        public virtual AppUserViewModel GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var user = _authService.GetUser(usernameOrEmail);
            return user;
        }
    }
}