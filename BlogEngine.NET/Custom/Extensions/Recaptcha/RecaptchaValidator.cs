// Copyright (c) 2007 Adrian Godong, Ben Maurer
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted for dotnetblogengine by Filip Stanek ( http://www.bloodforge.com )

namespace Recaptcha
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Calls the reCAPTCHA server to validate the answer to a reCAPTCHA challenge. Normally,
    ///     you will use the RecaptchaControl class to insert a web control on your page. However
    /// </summary>
    public class RecaptchaValidator
    {
        #region Constants and Fields

        /// <summary>
        /// The verify url.
        /// </summary>
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        /// <summary>
        /// The remote ip.
        /// </summary>
        private string remoteIp;

        private static readonly Regex RegexIPv4Address = new Regex(@"^(?<address>\d+\.\d+\.\d+\.\d+):\d+$");

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets PrivateKey.
        /// </summary>
        public string PrivateKey { get; set; }

        private static string StripPortFromIPv4Address(string address) {
            var m = RegexIPv4Address.Match(address);
            return m.Success ? m.Groups["address"].Value : address;
        }

        /// <summary>
        /// Gets or sets RemoteIP.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        public string RemoteIP {
            get {
                return this.remoteIp;
            }

            set {
                var ip = IPAddress.Parse(StripPortFromIPv4Address(value));

                if (ip.AddressFamily != AddressFamily.InterNetwork && ip.AddressFamily != AddressFamily.InterNetworkV6) {
                    throw new ArgumentException($"Expecting an IP address, got {ip}");
                }

                this.remoteIp = ip.ToString();
            }
        }

        /// <summary>
        /// Gets or sets Response.
        /// </summary>
        public string Response { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>The response.</returns>
        public RecaptchaResponse Validate() {
            CheckNotNull(this.PrivateKey, "PrivateKey");
            CheckNotNull(this.RemoteIP, "RemoteIp");
            CheckNotNull(this.Response, "Response");

            if (this.Response == string.Empty) {
                return RecaptchaResponse.InvalidSolution;
            }

            ;

            //try {

            RecaptchaResult results;

            using (var client = new HttpClient()) {
                var data = new Dictionary<string, string>() {
                    { "secret", this.PrivateKey },
                    { "response", this.Response },
                    { "remoteip", this.RemoteIP }
                };
                var result = client.PutAsync(VerifyUrl, new FormUrlEncodedContent(data)).Result;

                if (result.IsSuccessStatusCode) {
                    string json = result.Content.ReadAsStringAsync().Result;
                    results = JsonConvert.DeserializeObject<RecaptchaResult>(json);
                } else {
                    return RecaptchaResponse.RecaptchaNotReachable;
                }
            }

            if (results.Success) {
                return RecaptchaResponse.Valid;
            } else {
                return new RecaptchaResponse(false, results.ErrorCodes[0]);
            }
            //} catch (WebException) {
            //    return RecaptchaResponse.RecaptchaNotReachable;
            //}
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks the not null.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="name">The object name.</param>
        private static void CheckNotNull(object obj, string name) {
            if (obj == null) {
                throw new ArgumentNullException(name);
            }
        }

        #endregion
    }
}