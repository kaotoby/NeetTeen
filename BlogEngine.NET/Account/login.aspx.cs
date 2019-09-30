namespace Account
{
    using System;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;

    using Resources;
    using System.Threading;

    /// <summary>
    /// The login.
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        private const string RECAPTCHA_SITE_KEY = "6LffTxoUAAAAALTcYVIhNYM0C8Pz8FZMl99SgZtt";
        private const string RECAPTCHA_SECRET_KEY = "6LffTxoUAAAAANxPGHqeAs-2pLswk1F0azvLzkZt";

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e) {
            #region Recaptcha

            Button button = (Button)LoginUser.FindControl("LoginButtonReal");
            button.CssClass += " g-recaptcha";
            button.Attributes.Add("data-sitekey", RECAPTCHA_SITE_KEY);
            button.Attributes.Add("data-callback", "ValidateLogin");

            #endregion

            HyperLink linkForgotPassword = (HyperLink)LoginUser.FindControl("linkForgotPassword");
            if (linkForgotPassword != null) {
                linkForgotPassword.NavigateUrl = Utils.RelativeWebRoot + "Account/password-retrieval.aspx";
            }

            this.RegisterHyperLink.NavigateUrl = Utils.RelativeWebRoot + "Account/register.aspx?ReturnUrl=" +
                                                 HttpUtility.UrlEncode(this.Request.QueryString["ReturnUrl"]);
            this.RegisterHyperLink.Text = labels.createNow;
            ((PlaceHolder)LoginUser.FindControl("phResetPassword")).Visible = BlogSettings.Instance.EnablePasswordReset;

            if (this.Request.QueryString.ToString() == "logoff") {
                Security.SignOut();
                if (this.Request.UrlReferrer != null && this.Request.UrlReferrer != this.Request.Url && this.Request.UrlReferrer.LocalPath.IndexOf("/neet/", StringComparison.OrdinalIgnoreCase) == -1) {
                    this.Response.Redirect(this.Request.UrlReferrer.ToString(), true);
                } else {
                    this.Response.Redirect(BlogEngine.Core.Utils.RelativeWebRoot);
                }

                return;
            }

            if (!this.Page.IsPostBack || Security.IsAuthenticated) {
                return;
            }

            this.Master.SetStatus("warning", Resources.labels.loginFailed);
        }

        protected void LoginUser_OnAuthenticate(object sender, AuthenticateEventArgs e) {
            // always set to false
            e.Authenticated = false;

            try {
                var validator = new Recaptcha.RecaptchaValidator
                {
                    PrivateKey = RECAPTCHA_SECRET_KEY,
                    RemoteIP = Utils.GetClientIP(),
                    Response = Context.Request.Form["g-recaptcha-response"]
                };

                var result = validator.Validate();
                if (result.IsValid) {
                    Security.AuthenticateUser(LoginUser.UserName, LoginUser.Password, LoginUser.RememberMeSet);
                }
            } catch (ThreadAbortException) {
            } catch (ArgumentNullException) {
            } catch (Exception ex) {
                Utils.Log("Login User Authenticate", ex);
            }

        }

        #endregion
    }
}