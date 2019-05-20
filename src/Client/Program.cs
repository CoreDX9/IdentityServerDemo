// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
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
            Console.WriteLine("ClientCredentials模式演示");
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("https://localhost:5001");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:5003/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            //使用ResourceOwnerPassword模式获取授权
            Console.WriteLine("ResourceOwnerPassword模式演示");
            // discover endpoints from metadata
            //var disco = await DiscoveryClient.GetAsync("https://localhost:5001");

            // request token
            tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("bob", "Pass123$", "api1");//使用用户名密码

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            response = await client.GetAsync("https://localhost:5003/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}