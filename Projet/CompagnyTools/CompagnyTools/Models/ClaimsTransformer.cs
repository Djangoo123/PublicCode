﻿using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AuthSamples.ClaimsTransformer;

public class ClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // This will run every time Authenticate is called so its better to create a new Principal
        var transformed = new ClaimsPrincipal();
        transformed.AddIdentities(principal.Identities);
        transformed.AddIdentity(new ClaimsIdentity(new Claim[]
        {
                new Claim("Transformed", DateTime.Now.ToString(CultureInfo.InvariantCulture))
        }));
        return Task.FromResult(transformed);
    }
}