using Forge.Security.Jwt.Shared.Client.Models;
using System;

namespace Forge.Security.Jwt.Client.Storage.Browser.Models
{

    /// <summary>Represents the option(s) for the storage</summary>
    public class BrowserStorageOptions
    {

        /// <summary>Gets or sets the type of the authentication response.
        /// Change this type, if you are using a different type to receive response at authentication.</summary>
        /// <value>The type of the authentication response.</value>
        public Type AuthenticationResponseType { get; set; } = typeof(AuthenticationResponse);

    }

}
