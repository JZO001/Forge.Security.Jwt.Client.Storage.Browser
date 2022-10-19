using Forge.Security.Jwt.Shared.Client.Models;

namespace Forge.Security.Jwt.Client.Storage.Browser.Models
{

    /// <summary>
    ///   <para>Represents the result data for the callback</para>
    /// </summary>
    public class ReceiveAuthenticationResult
    {

        /// <summary>Gets or sets the parsed token data.</summary>
        /// <value>The parsed token data.</value>
        public ParsedTokenData ParsedTokenData { get; set; }

        /// <summary>Gets or sets a value indicating whether the JS service need to start or not</summary>
        /// <value>
        ///   <c>true</c> if need to start; otherwise, <c>false</c>.</value>
        public bool StartService { get; set; }

        /// <summary>Gets or sets the service due time.</summary>
        /// <value>The service wait time.</value>
        public int ServiceDueTime { get; set; }

    }

}
