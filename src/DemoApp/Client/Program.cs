// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    interface Inter<T, TT>
    {

    }

    class Cin<T,TT> : Inter<T, TT> { }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //获取Http客户端工厂服务
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddTransient(typeof(IList<>), typeof(List<>));
            serviceCollection.AddTransient(typeof(Inter<,>), typeof(Cin<,>));
            serviceCollection.AddTransient(typeof(IDictionary<,>), typeof(Dictionary<,>));
            var services = serviceCollection.BuildServiceProvider();
            var clientFactory = services.GetService<IHttpClientFactory>();

            var l = services.GetService<IList<int>>();
            //var d = services.GetService<IDictionary<int, int>>();
            var cc = services.GetService<Inter<int, int>>();

            Console.WriteLine("ClientCredentials模式演示");
            // discover endpoints from metadata
            var client = clientFactory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            Console.WriteLine("\n\n");

            // call api
            client = clientFactory.CreateClient();
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
            //var tokenClient = new TokenClient(disco.TokenEndpoint, new TokenClientOptions().) "ro.client", "secret");
            //tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("bob", "Pass123$", "api1");//使用用户名密码

            //if (tokenResponse.IsError)
            //{
            //    Console.WriteLine(tokenResponse.Error);
            //    return;
            //}

            //Console.WriteLine(tokenResponse.Json);
            //Console.WriteLine("\n\n");

            //// call api
            //client = new HttpClient();
            //client.SetBearerToken(tokenResponse.AccessToken);

            //response = await client.GetAsync("https://localhost:5003/identity");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(JArray.Parse(content));
            //}
            Console.ReadKey();
        }
    }
}