// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //获取Http客户端工厂服务
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var services = serviceCollection.BuildServiceProvider();
            var clientFactory = services.GetService<IHttpClientFactory>();

            Console.WriteLine("ClientCredentials模式演示");
            // discover endpoints from metadata
            var client = clientFactory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadKey();
                return;
            }

            // request token
            var clientCredentialsToken = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (clientCredentialsToken.IsError)
            {
                Console.WriteLine(clientCredentialsToken.Error);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(clientCredentialsToken.Json);

            Console.WriteLine("\n\n");

            // call api
            client = clientFactory.CreateClient();
            client.SetBearerToken(clientCredentialsToken.AccessToken);

            var response = await client.GetAsync("https://localhost:5003/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.ReadKey();
                return;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            //使用Password模式获取授权
            Console.WriteLine("Password模式演示");

            // request token 使用用户名密码
            var passwordToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "ro.client",
                ClientSecret = "secret",
                Scope = "api1",

                UserName = "bob",
                Password = "Pass123$"
            });

            if (passwordToken.IsError)
            {
                Console.WriteLine(passwordToken.Error);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(passwordToken.Json);
            Console.WriteLine("\n\n");

            // call api
            client = clientFactory.CreateClient();
            client.SetBearerToken(passwordToken.AccessToken);

            response = await client.GetAsync("https://localhost:5003/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.ReadKey();
                return;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadKey();
        }
    }
}