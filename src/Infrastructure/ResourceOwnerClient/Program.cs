// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResourceOwnerClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            //var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            //// request token
            //var options = new TokenClientOptions();
            //options.ClientSecret = "secret";
            //options.ClientId = "ro.client";
            //var tokenClient = new TokenClient(disco.TokenEndpoint, options);
            //var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "Pass123$", "api1");

            //if (tokenResponse.IsError)
            //{
            //    Console.WriteLine(tokenResponse.Error);
            //    return;
            //}

            //Console.WriteLine(tokenResponse.Json);
            //Console.WriteLine("\n\n");

            //// call api
            //var client = new HttpClient();
            //client.SetBearerToken(tokenResponse.AccessToken);

            //var response = await client.GetAsync("https://localhost:5003/identity");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = response.Content.ReadAsStringAsync().Result;
            //    Console.WriteLine(JArray.Parse(content));
            //}
        }
    }
}